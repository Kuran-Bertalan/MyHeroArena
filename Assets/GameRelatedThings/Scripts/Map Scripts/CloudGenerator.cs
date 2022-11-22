using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudDatas {
    public Vector3 position;

    public Vector3 scale;

    public Quaternion rotation;

    private bool _isActive;

    //megakad�lyozza, hogy m�s oszt�lyok k�zvetlen�l be�ll�ts�k isActive v�ltoz�t
    public bool isActive {
        get 
        {
            return _isActive; 
        } 
    }

    public int x, y;

    public float distanceFromCamera;

    //Visszaadja a felh�nk Matrix4x4-�t
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(position, rotation, scale);
        }
    }

    //A felh�nk elind�t�s�ra haszn�ljuk
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

    //Meghat�rozza a priv�t isActive �llapotunkat, mivel m�s oszt�lyok nem tudj�k k�zvetlen�l be�ll�tani.
    public void SetActive(bool basicState)
    {
        _isActive = basicState;
    }
}
public class CloudGenerator : MonoBehaviour {
    //H�l�k adatai
    public Mesh cloudMesh;
    public Material cloudMaterial;

    //Felh� adatok
    public float cloudSize = 6;
    public float maxScale = 1;

    //Zaj gener�l�s
    public float timeScale = 1;
    public float texScale = 1;

    //Felh� sk�l�z�s
    public float minNoiseSize = 0.8f;
    public float sizeScale = 0.35f;

    //Kiv�laszt�si adatok
    public Camera cam;
    public int maxDist;

    //Egyszerre el��l�tott "term�k" mennyis�ge
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

    //Az X �s Y �rt�kek �tfut�s�val kezdj�k, hogy egy 35x35 felh� m�ret� term�kk�teget hozzunk l�tre.
 
    private void BuildCloudBatch(int xLoop, int yLoop)
    {
        // term�k�nk, a kamer�nk hat�sugar�n bel�l van
        bool markBatch = false;

        //Ez az aktu�lis felh�s term�k�nk, amit �pp gener�lunk.
        List<CloudDatas> currentBatch = new List<CloudDatas>();

        for (int x = 0; x < 31; x++)
        {
            for (int y = 0; y < 31; y++)
            {
                //minden egyes ciklushoz felh�t adunk
                AddCloud(currentBatch, x + xLoop * 31, y + yLoop * 31);
            }
        }

        markBatch = CheckForActiveBatch(currentBatch);

        batches.Add(currentBatch);

        if(markBatch) batchesToUpdate.Add(currentBatch);
    }

    //Ez a m�dszer ellen�rzi, hogy az aktu�lis term�k�nknek van-e olyan felh�je, amely a kamer�nk hat�t�vols�g�n bel�l van.
    //true, ha a felh� hat�t�vols�gon bel�l van, false, ha nincs felh� a hat�t�vols�gon bel�l.
    private bool CheckForActiveBatch(List<CloudDatas> batch)
    {
        foreach (var cloud in batch)
        {
            cloud.distanceFromCamera = Vector3.Distance(cloud.position, cam.transform.position);
            if (cloud.distanceFromCamera < maxDist) return true;
        }
        return false;
    }

    //Ez a m�dszer l�trehozza a felh�inket CloudData objektumk�nt.
    private void AddCloud(List<CloudDatas> currBatch, int x, int y)
    {
        Vector3 position = new Vector3(transform.position.x + x * cloudSize, transform.position.y, 
                                        transform.position.z + y * cloudSize);

        float disToCamera = Vector3.Distance(new Vector3(x, transform.position.y, y), cam.transform.position);

        currBatch.Add(new CloudDatas(position, Vector3.zero, Quaternion.identity, x, y, disToCamera));
    }

    //Zajgener�l�s
    //Friss�tj�k az offseteinket, hogy a zaj "g�rd�lj�n" a felh�objektumokon kereszt�l.
    private void Update()
    {
        MakeNoise();
        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;
    }

    //Ez a m�dszer friss�ti a zajokat/felh�ket.
    //El�sz�r ellen�rizz�k, hogy a kamera mozog-e.
    //Ha nem, akkor friss�tj�k a t�teleket.
    //Ha elmozdult, akkor a prevCamPos-t vissza kell �ll�tanunk
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

    //Ez a m�dszer friss�ti a felh�inket
    //El�sz�r v�gigmegy�nk az �sszes t�tel�nk�n a batchesToUpdate list�ban.
    //Minden egyes t�telhez egy �jabb ciklusban le kell h�vnunk minden egyes felh�t.
    private void UpdateBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            foreach (var cloud in batch)
            {
                //A zajm�ret meghat�roz�sa a felh�k poz�ci�ja, a zajtext�ra sk�l�ja �s az eltol�sunk �sszege alapj�n
                float size = Mathf.PerlinNoise(cloud.x * texScale + offsetX, cloud.y * texScale + offsetY);

                //Ha a felh�nk m�rete meghaladja a l�that� felh�hat�rt, akkor meg kell mutatnunk.
                if (size > minNoiseSize)
                {
                    //A felh� aktu�lis m�retar�ny�nak lek�rdez�se
                    float localScaleX = cloud.scale.x;

                    // aktiv�ljuk a felh�ket
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

    //Ez a m�dszer �j m�retre ad a felh�knek
    private void ScaleCloud(CloudDatas cloud, int direction)
    {
        cloud.scale += new Vector3(sizeScale * Time.deltaTime * direction,
                                   sizeScale * Time.deltaTime * direction, 
                                   sizeScale * Time.deltaTime * direction);
    }

    //Ez a m�dszer t�rli a batchesToUpdate list�t, mert csak a list�n bel�l l�that� t�teleket akarjuk l�tni.
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

    //Ez a m�dszer v�gigmegy az �sszes friss�tend� t�telen, �s kirajzolja a h�l�ikat a k�perny�re.
    private void RenderBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            Graphics.DrawMeshInstanced(cloudMesh, 0, cloudMaterial, batch.Select((a) => a.matrix).ToList());
        }
    }
}
