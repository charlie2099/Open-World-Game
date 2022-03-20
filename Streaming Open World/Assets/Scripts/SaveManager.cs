using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public class ChunkData
    {
        public string name;
        public Mesh mesh;
        public Material material;
        public Vector3 position;
        public List<GameObject> objects;
    }
    private static string _jsonFile;

    public static void LoadSelectedChunk(GameObject chunk)
    { 
        string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
        ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(path));
        
        //GameObject newChunk = new GameObject(loadedData.name);
        Mesh mesh = new Mesh();

        chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.mesh;
        chunk.AddComponent<MeshRenderer>().sharedMaterial = loadedData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.mesh;
        //chunk.AddComponent<Chunk>();

        chunk.transform.position = loadedData.position;

        mesh.Clear();
        mesh.vertices = loadedData.mesh.vertices;
        mesh.triangles = loadedData.mesh.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        //newChunk.transform.parent = TerrainGenerator.container.transform;
        //TerrainGenerator.GetChunks().Add(newChunk);
        //ChunkLoader.loadedChunks.Add(newChunk);
        
        chunk.GetComponent<Chunk>().isLoaded = true;
        chunk.tag = "TerrainChunk";
                
        print("Terrain List: " + TerrainGenerator.GetChunks().Count);
    }

    public static void UnloadSelectedChunk(GameObject chunk)
    {
        chunk.GetComponent<Chunk>().isLoaded = false;
        
        // Update chunk data to the state when it was unloaded
        ChunkData newChunkData = new ChunkData();
        newChunkData.material     = chunk.GetComponent<MeshRenderer>().sharedMaterial;
        newChunkData.mesh         = chunk.GetComponent<MeshFilter>().sharedMesh;
        newChunkData.position     = chunk.transform.position;
        newChunkData.name         = chunk.name;
        
        // Converts new chunk data to JSON form.
        _jsonFile = JsonUtility.ToJson(newChunkData, true);

        // Writes chunkData to json file
        string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
        File.WriteAllText(path, _jsonFile);

        // Remove it from the active chunks list and strip its components
        Destroy(chunk.GetComponent<MeshRenderer>());
        Destroy(chunk.GetComponent<MeshFilter>());
        Destroy(chunk.GetComponent<MeshCollider>());
        //Destroy(chunk.GetComponent<Chunk>());
        chunk.tag = "Untagged";
        //TerrainGenerator.GetChunks().Remove(chunk);
        //ChunkLoader.loadedChunks.Remove(chunk);
    }

    public static void SaveToFile()
    {
        print("Size: " + TerrainGenerator.GetChunks().Count);
        // Update chunk data when unloaded
        foreach (var chunk in TerrainGenerator.GetChunks())
        {
            ChunkData newChunkData = new ChunkData();
            newChunkData.material     = chunk.GetComponent<MeshRenderer>().sharedMaterial;
            newChunkData.mesh         = chunk.GetComponent<MeshFilter>().sharedMesh;
            newChunkData.position     = chunk.transform.position;
            newChunkData.name         = chunk.name;
            newChunkData.objects      = chunk.GetComponent<Chunk>().chunkObjects;

            // Converts new chunk data to JSON form.
            _jsonFile = JsonUtility.ToJson(newChunkData, true);

            // Writes chunkData to json file
            string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
            File.WriteAllText(path, _jsonFile);
        }
        print("<color=orange> Chunks unloaded </color>");
        print("<color=orange> Chunk data written to file </color>");
    }
}