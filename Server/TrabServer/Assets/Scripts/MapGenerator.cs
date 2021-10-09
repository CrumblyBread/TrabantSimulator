using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    public int seed;
    [Header("Map Parameters")] 

    [Tooltip("Map width in world units (meters)")]  
    public int xSize = 250;
    [Tooltip("Map lenght in world units (meters)")]
    public int zSize = 250;
    [Tooltip("How many points in one world unit (meter)")]
    public float resolution = 0.5f;
    private int xRes;
    private int zRes;
    [Header("Noise Parameters")]
    public float noiseSize = 10f; 
    public float noiseStrength = 1f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateMap();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        UpdateMesh();
    }

    private void GenerateMap(){
        xSize = Mathf.Abs(xSize);
        zSize = Mathf.Abs(zSize);
        resolution = Mathf.Abs(resolution);
        xRes = (int)(xSize / resolution);
        zRes = (int)(zSize / resolution);

        vertices = new Vector3[(xRes + 1) * (zRes + 1)];
        
        for (int i = 0, z = 0; z <= zRes; z++)
        {
            for (int x = 0; x <= xRes; x++)
            {
                float X = (x*resolution) - (xSize/2);
                float Z = (z*resolution) - (zSize/2);
                float Y = 0;
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(X,this.transform.position.y,Z),Vector3.up,out hit))
                {
                    Y = hit.point.y - this.transform.position.y;
                }else
                {
                    Y = Mathf.PerlinNoise((X / noiseSize) + seed,  (Z / noiseSize) + seed) * noiseStrength;
                }
                vertices[i] = new Vector3(X,Y,Z);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[xRes * xRes * 6];
        for (int z = 0; z < zRes; z++)
        {
            for (int x = 0; x < xRes; x++)
            {   
                triangles[tris] = vert;
                triangles[tris + 1] = vert + xRes + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xRes + 1;
                triangles[tris + 5] = vert + xRes + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void UpdateMesh(){
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    
}
