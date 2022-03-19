using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private static List<GameObject> _generatedChunks = new List<GameObject>();
    private static Mesh _mesh;
    private static Vector3[] _vertices;
    private static int[] _triangles;
    public static int _numberOfChunks;
    public static GameObject container;

    [SerializeField] private Texture2D _heightmap;
    [SerializeField] private Material _terrainMaterial;
    [SerializeField] private int _chunkSize;
    [SerializeField] private int _terrainWidth;
    [SerializeField] private int _terrainHeight;
    
    private void Awake()
    {
        container = new GameObject("MainTerrain");
        GenerateMap(_heightmap, _terrainMaterial, _chunkSize, _terrainWidth, _terrainHeight);
        print("Chunk list size: " + GetChunks().Count);
    }

    public static void GenerateMap(Texture2D heightMap, Material terrainMaterial, int chunkSize, int terrainWidth, int multiplier)
    {
        foreach (var chunk in _generatedChunks.ToArray())
        {
            _generatedChunks.Remove(chunk);
        }
        
        _numberOfChunks = terrainWidth / chunkSize;
        print("Num of chunks: " + _numberOfChunks);

        for (int x = 0; x < _numberOfChunks; x++)
        {
            for (int z = 0; z < _numberOfChunks; z++)
            {
                GameObject chunk = new GameObject("chunk " + x + " , " + z);
                chunk.tag = "TerrainChunk";
                
                _mesh = new Mesh();

                CreateMesh(heightMap, chunkSize, multiplier,x * chunkSize, z * chunkSize);
                
                chunk.AddComponent<MeshFilter>();
                chunk.AddComponent<MeshRenderer>();
                chunk.AddComponent<MeshCollider>();
                chunk.AddComponent<Chunk>();
                
                chunk.GetComponent<MeshFilter>().sharedMesh = _mesh;
                chunk.GetComponent<MeshRenderer>().material = terrainMaterial;
                chunk.GetComponent<MeshCollider>().sharedMesh = _mesh;
                
                chunk.transform.position = new Vector3(x * chunkSize, 0, z * chunkSize);

                UpdateMesh();

                chunk.transform.parent = container.transform;
                _generatedChunks.Add(chunk);
            }
        }
    }


    // Creates mesh for a single chunk
    public static void CreateMesh(Texture2D heightMap, int chunkSize, int multiplier, int offsetX, int offsetZ)
    {
        _vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];

        for (int i = 0, z = 0; z <= chunkSize; z++)
        {
            for (int x = 0; x <= chunkSize; x++)
            {
                _vertices[i] = new Vector3(x, heightMap.GetPixel(x + offsetX, z + offsetZ).r * multiplier, z);
                i++;
            }
        }

        _triangles = new int[chunkSize * chunkSize * 6];
        int vertexIndex = 0;
        int trianglesIndex = 0;

        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                _triangles[trianglesIndex + 0] = vertexIndex + 0;
                _triangles[trianglesIndex + 1] = vertexIndex + chunkSize + 1;
                _triangles[trianglesIndex + 2] = vertexIndex + 1;
                _triangles[trianglesIndex + 3] = vertexIndex + 1;
                _triangles[trianglesIndex + 4] = vertexIndex + chunkSize + 1;
                _triangles[trianglesIndex + 5] = vertexIndex + chunkSize + 2;

                vertexIndex++;
                trianglesIndex += 6;
            }
            vertexIndex++;
        }
    }

    private static void UpdateMesh()
    {
        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    public static List<GameObject> GetChunks()
    {
        return _generatedChunks;
    }
}



