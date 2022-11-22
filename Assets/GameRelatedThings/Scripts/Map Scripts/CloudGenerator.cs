using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudDatas {
    public Vector3 position;

    public Vector3 scale;

    public Quaternion rotation;

    private bool _isActive;

    //megakadályozza, hogy más osztályok közvetlenül beállítsák isActive változót
    public bool isActive {
        get 
        {
            return _isActive; 
        } 
    }

    public int x, y;

    public float distanceFromCamera;

    //Visszaadja a felhõnk Matrix4x4-ét
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(position, rotation, scale);
        }
    }

    //A felhõnk elindítására használjuk
    public CloudDatas(Vector3 position, Vector3 scale, Quaternion rotation, int x, int y, float distanceFromCamera)
    {
        this.position = position;
        this.scale = scale;
        this.rotation = rotation;
        SetActive(true);
        this.x = x;
        this.y = y;
        this.distanceFromCamera = distanceFromCamera;
    }

    //Meghatározza a privát isActive állapotunkat, mivel más osztályok nem tudják közvetlenül beállítani.
    public void SetActive(bool basicState)
    {
        _isActive = basicState;
    }
}
public class CloudGenerator : MonoBehaviour {
    //Hálók adatai
    public Mesh cloudMesh;
    public Material cloudMaterial;

    //Felhõ adatok
    public float cloudSize = 6;
    public float maxScale = 1;

    //Zaj generálás
    public float timeScale = 1;
    public float texScale = 1;

    //Felhõ skálázás
    public float minNoiseSize = 0.8f;
    public float sizeScale = 0.35f;

    //Kiválasztási adatok
    public Camera cam;
    public int maxDist;

    //Egyszerre elõálított "termék" mennyisége
    public int batchesToCreate;

    private Vector3 prevCamPos;
    private float offsetX = 1;
    private float offsetY = 1;
    private List<List<CloudDatas>> batches = new List<List<CloudDatas>>();
    private List<List<CloudDatas>> batchesToUpdate = new List<List<CloudDatas>>();

    private void Start()
    {
        for (int batchesX = 0; batchesX < batchesToCreate; batchesX++)
        {
            for (int batchesY = 0; batchesY < batchesToCreate; batchesY++)
            {
                BuildCloudBatch(batchesX, batchesY);
            }
        }
    }

    //Az X és Y értékek átfutásával kezdjük, hogy egy 35x35 felhõ méretû termékköteget hozzunk létre.
 
    private void BuildCloudBatch(int xLoop, int yLoop)
    {
        // termékünk, a kameránk hatósugarán belül van
        bool markBatch = false;

        //Ez az aktuális felhõs termékünk, amit épp generálunk.
        List<CloudDatas> currentBatch = new List<CloudDatas>();

        for (int x = 0; x < 31; x++)
        {
            for (int y = 0; y < 31; y++)
            {
                //minden egyes ciklushoz felhõt adunk
                AddCloud(currentBatch, x + xLoop * 31, y + yLoop * 31);
            }
        }

        markBatch = CheckForActiveBatch(currentBatch);

        batches.Add(currentBatch);

        if(markBatch) batchesToUpdate.Add(currentBatch);
    }

    //Ez a módszer ellenõrzi, hogy az aktuális termékünknek van-e olyan felhõje, amely a kameránk hatótávolságán belül van.
    //true, ha a felhõ hatótávolságon belül van, false, ha nincs felhõ a hatótávolságon belül.
    private bool CheckForActiveBatch(List<CloudDatas> batch)
    {
        foreach (var cloud in batch)
        {
            cloud.distanceFromCamera = Vector3.Distance(cloud.position, cam.transform.position);
            if (cloud.distanceFromCamera < maxDist) return true;
        }
        return false;
    }

    //Ez a módszer létrehozza a felhõinket CloudData objektumként.
    private void AddCloud(List<CloudDatas> currBatch, int x, int y)
    {
        Vector3 position = new Vector3(transform.position.x + x * cloudSize, transform.position.y, 
                                        transform.position.z + y * cloudSize);

        float disToCamera = Vector3.Distance(new Vector3(x, transform.position.y, y), cam.transform.position);

        currBatch.Add(new CloudDatas(position, Vector3.zero, Quaternion.identity, x, y, disToCamera));
    }

    //Zajgenerálás
    //Frissítjük az offseteinket, hogy a zaj "gördüljön" a felhõobjektumokon keresztül.
    private void Update()
    {
        MakeNoise();
        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;
    }

    //Ez a módszer frissíti a zajokat/felhõket.
    //Elõször ellenõrizzük, hogy a kamera mozog-e.
    //Ha nem, akkor frissítjük a tételeket.
    //Ha elmozdult, akkor a prevCamPos-t vissza kell állítanunk
    private void MakeNoise()
    {
        if (cam.transform.position == prevCamPos)
        {
            UpdateBatches();
        }
        else
        {
            prevCamPos = cam.transform.position;
            UpdateBatchList();
            UpdateBatches();
        }
        RenderBatches();
        prevCamPos = cam.transform.position;
    }

    //Ez a módszer frissíti a felhõinket
    //Elõször végigmegyünk az összes tételünkön a batchesToUpdate listában.
    //Minden egyes tételhez egy újabb ciklusban le kell hívnunk minden egyes felhõt.
    private void UpdateBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            foreach (var cloud in batch)
            {
                //A zajméret meghatározása a felhõk pozíciója, a zajtextúra skálája és az eltolásunk összege alapján
                float size = Mathf.PerlinNoise(cloud.x * texScale + offsetX, cloud.y * texScale + offsetY);

                //Ha a felhõnk mérete meghaladja a látható felhõhatárt, akkor meg kell mutatnunk.
                if (size > minNoiseSize)
                {
                    //A felhõ aktuális méretarányának lekérdezése
                    float localScaleX = cloud.scale.x;

                    // aktiváljuk a felhõket
                    if(!cloud.isActive)
                    {
                        cloud.SetActive(true);
                        cloud.scale = Vector3.zero;
                    }
                    if(localScaleX < maxScale)
                    {
                        ScaleCloud(cloud, 1);

                        if(cloud.scale.x > maxScale)
                        {
                            cloud.scale = new Vector3(maxScale, maxScale, maxScale);
                        }
                    }
                }
                else if(size < minNoiseSize)
                {
                    float localScaleX = cloud.scale.x;
                    ScaleCloud(cloud, -1);

                    if(localScaleX <= 0.1)
                    {
                        cloud.SetActive(false);
                        cloud.scale = Vector3.zero;
                    }
                }
            }
        }
    }

    //Ez a módszer új méretre ad a felhõknek
    private void ScaleCloud(CloudDatas cloud, int direction)
    {
        cloud.scale += new Vector3(sizeScale * Time.deltaTime * direction,
                                   sizeScale * Time.deltaTime * direction, 
                                   sizeScale * Time.deltaTime * direction);
    }

    //Ez a módszer törli a batchesToUpdate listát, mert csak a listán belül látható tételeket akarjuk látni.
    private void UpdateBatchList()
    {
        batchesToUpdate.Clear();

        foreach (var batch in batches)
        {
            if(CheckForActiveBatch(batch))
            {
                batchesToUpdate.Add(batch);
            }
        }
    }

    //Ez a módszer végigmegy az összes frissítendõ tételen, és kirajzolja a hálóikat a képernyõre.
    private void RenderBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            Graphics.DrawMeshInstanced(cloudMesh, 0, cloudMaterial, batch.Select((a) => a.matrix).ToList());
        }
    }
}
