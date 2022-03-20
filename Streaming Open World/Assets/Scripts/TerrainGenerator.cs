using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private static List<GameObject> _generatedChunks = new List<GameObject>();
    private static Mesh _mesh;
    private static Vector3[] _vertices;
    private static int[] _triangles;
    public static int numberOfChunks;
    public static int totalChunks;
    //public static GameObject container;

    [SerializeField] private Texture2D _heightmap;
    [SerializeField] private Material _terrainMaterial;
    [SerializeField] private int _chunkSize;
    [SerializeField] private int _terrainWidth;
    [SerializeField] private int _terrainHeight;
    
    private void Awake()
    {
        //container = new GameObject("MainTerrain");
        /*if (FindObjectOfType<Transform>().name == "name")
        {
            print("goodbye");
            return;
        }*/
        
        GenerateMap(_heightmap, _terrainMaterial, _chunkSize, _terrainWidth, _terrainHeight);
    }

    public static void GenerateMap(Texture2D heightMap, Material terrainMaterial, int chunkSize, int terrainWidth, int multiplier)
    {
        foreach (var chunk in _generatedChunks.ToArray())
        {
            _generatedChunks.Remove(chunk);
        }
        
        numberOfChunks = terrainWidth / chunkSize;
        totalChunks = numberOfChunks * numberOfChunks;
        print("Num of chunks: " + numberOfChunks);

        for (int x = 0; x < numberOfChunks; x++)
        {
            for (int z = 0; z < numberOfChunks; z++)
            {
                GameObject chunk = new GameObject("chunk " + x + " , " + z);
                chunk.tag = "TerrainChunk";
                
                _mesh = new Mesh();

                CreateMesh(heightMap, chunkSize, multiplier,x * chunkSize, z * chunkSize);
                
                chunk.AddComponent<MeshFilter>().sharedMesh = _mesh;
                chunk.AddComponent<MeshRenderer>().sharedMaterial = terrainMaterial;
                chunk.AddComponent<MeshCollider>().sharedMesh = _mesh;
                chunk.AddComponent<Chunk>();
                
                string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
                SaveManager.ChunkData loadedData = JsonUtility.FromJson<SaveManager.ChunkData>(File.ReadAllText(path));
                
                //chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.mesh;
                //chunk.AddComponent<MeshRenderer>().sharedMaterial = loadedData.material;
                //chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.mesh;
                //chunk.AddComponent<Chunk>();

                /*Mesh objectMesh = new Mesh();
                chunk.GetComponent<Chunk>().chunkObjects.Add(new GameObject());
                chunk.GetComponent<Chunk>().chunkObjects[0].AddComponent<MeshFilter>().sharedMesh = loadedData.objectMeshes[0];
                chunk.GetComponent<Chunk>().chunkObjects[0].AddComponent<MeshCollider>().sharedMesh = loadedData.objectMeshes[0];*/
                
                chunk.transform.position = loadedData.position;
                
                
                // build object meshes then add to chunk list
                
                //chunk.GetComponent<Chunk>().chunkObjects = loadedData.objects;
                
                /*objectMesh.Clear();
                objectMesh.vertices = loadedData.objectMeshes[0].vertices;
                objectMesh.triangles = loadedData.objectMeshes[0].triangles;
                objectMesh.RecalculateNormals();
                objectMesh.RecalculateBounds();*/

                /*mesh.Clear();
                mesh.vertices = loadedData.mesh.vertices;
                mesh.triangles = loadedData.mesh.triangles;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();*/

                //newChunk.transform.parent = TerrainGenerator.container.transform;
                //TerrainGenerator.GetChunks().Add(newChunk);
                //ChunkLoader.loadedChunks.Add(newChunk);
                
                //chunk.transform.position = new Vector3(x * chunkSize, 0, z * chunkSize);

                UpdateMesh();

                //chunk.transform.parent = container.transform;
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
}



