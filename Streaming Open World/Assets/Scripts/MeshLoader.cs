/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshLoader : MonoBehaviour
{
    [SerializeField] private Texture2D heightMap;
    [SerializeField] private int chunkSize;
    
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    
    private int numOfChunks;

    private void Start()
    {
        numOfChunks = heightMap.width / chunkSize;
        
        print("Heightmap Width: " + heightMap.width);
        print("Heightmap Height: " + heightMap.height);
        print("Num of Chunks: " + numOfChunks);
    }

    private void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MeshLoader : MonoBehaviour
{
    [SerializeField] private int checkRate;
    
    [Header("Terrain")]
    [SerializeField] private Material terrainMaterial;
    [SerializeField] private Texture2D heightMap;
    [SerializeField] private float multiplier = 20f;
    [SerializeField] private int chunkSize = 32;
    
    [Header("Trees")]
    [SerializeField] private Material treeMaterial;
    [SerializeField] [Range(0,50)] private int spawnFrequencyPercent;

    private List<GameObject> activeChunks = new List<GameObject>();
    private Mesh mesh;
    private Vector3[] vertices;
    private int[]     triangles;
    private float minTerrainHeight = 1000;
    private float maxTerrainHeight = -1000;
    private int numberOfChunks;

    private void Start()
    {
        //GenerateMap();
        //InvokeRepeating(CheckChunks(), 0.0f, checkRate);
    }

    private void CheckChunks()
    {
        // No matter how high up player is, it is always the same distance it is calculating
        // So playerPos.y needs to be set to 0
    }
    
    [ContextMenu("GenerateTerrain")]
    public void GenerateMap()
    {
        numberOfChunks = heightMap.width / chunkSize;

        for (int x = 0; x < numberOfChunks; x++)
        {
            for (int z = 0; z < numberOfChunks; z++)
            {
                GameObject chunk = new GameObject("chunk " + x + " , " + z);
                
                mesh = new Mesh();
                
                //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                
                CreateMesh(x * chunkSize, z * chunkSize);

                //AddMeshComponents();
                chunk.AddComponent<MeshFilter>();
                chunk.AddComponent<MeshRenderer>();
                chunk.AddComponent<MeshCollider>();
                
                chunk.GetComponent<MeshFilter>().sharedMesh = mesh;
                chunk.GetComponent<MeshRenderer>().material = terrainMaterial;
                chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
                
                chunk.transform.position = new Vector3(x * chunkSize, 0, z * chunkSize);

                //chunk.AddComponent<ChunkSaveSystem>();
                //chunk.GetComponent<ChunkSaveSystem>().SetMesh(mesh, terrainMaterial);
                //chunk.GetComponent<ChunkSaveSystem>().Save();
                
                UpdateMesh();
                activeChunks.Add(chunk);
            }
        }
    }


    private void CreateMesh(int offsetX, int offsetZ)
    {
        vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];
        //colours = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= chunkSize; z++)
        {
            for (int x = 0; x <= chunkSize; x++)
            {
                vertices[i] = new Vector3(x, heightMap.GetPixel(x + offsetX, z + offsetZ).r * multiplier, z);

                if (vertices[i].y > maxTerrainHeight)
                {
                    maxTerrainHeight = vertices[i].y;
                }
                if (vertices[i].y < minTerrainHeight)
                {
                    minTerrainHeight = vertices[i].y;
                }

                i++;
            }
        }

        triangles = new int[chunkSize * chunkSize * 6];
        int vertexIndex = 0;
        int trianglesIndex = 0;

        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                triangles[trianglesIndex + 0] = vertexIndex + 0;
                triangles[trianglesIndex + 1] = vertexIndex + chunkSize + 1;
                triangles[trianglesIndex + 2] = vertexIndex + 1;
                triangles[trianglesIndex + 3] = vertexIndex + 1;
                triangles[trianglesIndex + 4] = vertexIndex + chunkSize + 1;
                triangles[trianglesIndex + 5] = vertexIndex + chunkSize + 2;

                vertexIndex++;
                trianglesIndex += 6;
            }
            vertexIndex++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}


