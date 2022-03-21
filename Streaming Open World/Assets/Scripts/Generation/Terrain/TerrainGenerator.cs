using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Generates a chunked terrain and reads chunk data from file should it exist.
/// </summary>

public class TerrainGenerator : MonoBehaviour
{
    // Terrain
    [SerializeField] private Texture2D heightmap;
    [SerializeField] private Material serialisedChunkMaterial;
    [SerializeField] private int serialisedChunkSize;
    [SerializeField] private int serialisedTerrainWidth;
    [SerializeField] private int serialisedTerrainHeight;

    // Not serialised
    public static int numberOfChunks;
    public static int totalChunks;
    public static int _chunkSize;
    public static int _terrainWidth;
    public static int _terrainHeight;
    public static GameObject container;

    private static List<GameObject> _generatedChunks = new List<GameObject>();
    private static Mesh _mesh;
    private static Vector3[] _vertices;
    private static int[] _triangles;
    
    // Terrain Objects
    private static ObjectGenerator _objectGenerator;

    private void Awake()
    {
        GenerateMap(heightmap, serialisedChunkMaterial, serialisedChunkSize, serialisedTerrainWidth, serialisedTerrainHeight);
    }

    public static void GenerateMap(Texture2D heightMap, Material terrainMaterial, int chunkSize, int terrainWidth, int terrainHeight)
    {
        // Create chunk container 
        container = new GameObject("Terrain");
        
        // Remove previously existing terrain from list
        foreach (var chunk in _generatedChunks.ToArray())
        {
            _generatedChunks.Remove(chunk);
        }
        
        // Read terrain related data from file should it exist
        string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
        if (File.Exists(terrainPath))
        {
            SaveManager.TerrainData loadedData = JsonUtility.FromJson<SaveManager.TerrainData>(File.ReadAllText(terrainPath));
            chunkSize     = loadedData.chunkSize;
            terrainWidth  = loadedData.terrainWidth;
            terrainHeight = loadedData.terrainHeight;
        }
        
        // getter data
        _chunkSize = chunkSize;
        _terrainWidth = terrainWidth;
        _terrainHeight = terrainHeight;

        numberOfChunks = terrainWidth / chunkSize;
        totalChunks = numberOfChunks * numberOfChunks;

        for (int x = 0; x < numberOfChunks; x++)
        {
            for (int z = 0; z < numberOfChunks; z++)
            {
                GameObject chunk = new GameObject("chunk " + x + " , " + z);
                chunk.tag = "TerrainChunk";
                
                _mesh = new Mesh();

                CreateMesh(heightMap, chunkSize, terrainHeight,x * chunkSize, z * chunkSize);
                
                chunk.AddComponent<MeshFilter>().sharedMesh = _mesh;
                chunk.AddComponent<MeshRenderer>().sharedMaterial = terrainMaterial;
                chunk.AddComponent<MeshCollider>().sharedMesh = _mesh;
                chunk.AddComponent<Chunk>().isLoaded = true;
                
                string chunkPath = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
                if (File.Exists(chunkPath))
                {
                    SaveManager.ChunkData loadedData = JsonUtility.FromJson<SaveManager.ChunkData>(File.ReadAllText(chunkPath));
                    chunk.GetComponent<MeshRenderer>().sharedMaterial = loadedData.material;
                    chunk.transform.position = loadedData.position;
                    
                    print("loaded chunk (" + x + " , " + z + ") obj count: " + loadedData.objects.Count);
                    
                    // Load terrain objects in each chunk 
                    for (int i = 0; i < loadedData.objects.Count; i++)
                    {
                        GameObject newObj = new GameObject(loadedData.objectNames[i] + i);
                        newObj.AddComponent<MeshFilter>().sharedMesh       = loadedData.objectMeshes[i];
                        newObj.AddComponent<MeshRenderer>().sharedMaterial = loadedData.objectMaterials[i];
                        newObj.AddComponent<MeshCollider>().sharedMesh = loadedData.objectMeshes[i];
                        newObj.transform.position = loadedData.objectPos[i];
                        newObj.transform.parent = chunk.transform;
                        chunk.GetComponent<Chunk>().chunkObjects.Add(newObj);
                    }
                }
                else
                {
                    chunk.transform.position = new Vector3(x * chunkSize, 0, z * chunkSize);
                }

                UpdateMesh();

                chunk.transform.parent = container.transform;
                container.tag = "MainTerrain";
                _generatedChunks.Add(chunk);
            }
        }
        SaveManager.SaveToFile();
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

    public static int GetChunkSize()
    {
        return _chunkSize;
    }

    public static int GetTerrainWidth()
    {
        return _terrainWidth;
    }
    
    public static int GetTerrainHeight()
    {
        return _terrainHeight;
    }
    
}



