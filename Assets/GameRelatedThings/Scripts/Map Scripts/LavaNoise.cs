using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaNoise : MonoBehaviour
{
    public float scale;
    public float timeScale;
    public float power;

    private float offsetX;
    private float offsetY;
    private MeshFilter filter;

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        MakeNoise();
    }

    
    void Update()
    {
        MakeNoise();
        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;
    }
    float CalculateHeight(float x, float y)
    {
        float coordinateX = x * scale + offsetX;
        float coordinateY = y * scale + offsetY;

        return Mathf.PerlinNoise(coordinateX, coordinateY);
    }
    void MakeNoise()
    {
        Vector3 [] verticies = filter.mesh.vertices;

        for (int i = 0; i < verticies.Length; i++)
        {
            verticies[i].y = CalculateHeight(verticies[i].x, verticies[i].z * power);
        }
        filter.mesh.vertices = verticies;
    }
}
