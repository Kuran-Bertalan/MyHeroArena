using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPlane : MonoBehaviour
{
    public int gridSize;
    public float lavaSize;

    private MeshFilter meshFilter;
    
    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GenerateMesh();
    }

    private Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        var vertices = new List<Vector3>(); // Cs�csok t�rol�sa x,y,z koordin�ta szerint     
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>(); // 2 �rt�k t�rol�s�hoz x,z

        // V�gigmegy�nk az x �s az y koordin�t�kon.
        for (int x = 0; x < gridSize + 1; x++)
        {
            for (int y = 0; y < gridSize + 1; y++)
            {
                vertices.Add(new Vector3(-lavaSize * 0.5f + lavaSize * (x / ((float)gridSize)),
                    0, - lavaSize * 0.5f + lavaSize * (y / ((float)gridSize))));
                normals.Add(Vector3.up); // Ir�ny�t �ll�tsuk be vele a norm�l�rt�keknek
                uvs.Add(new Vector2(x / (float)gridSize, y / (float)gridSize));
            }
        }

        var triangles = new List<int>();
        var verticlesCount = gridSize + 1;
        for (int i = 0; i < verticlesCount * verticlesCount - verticlesCount; i++) 
        {
            if ((i + 1) % verticlesCount == 0)
            {
                continue;
            }
            triangles.AddRange(new List<int>() {
                    i+1+verticlesCount, i+verticlesCount,
                    i, i, 
                    i+1, i+verticlesCount+1
            });
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        return mesh;
    }
}
