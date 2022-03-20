using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public class ChunkData
    {
        public string name;
        public Mesh chunkMesh;
        public Material material;
        public Vector3 position;
        
        public List<GameObject> objects; // don't write gameobjects to file?
        public Mesh[] objectMeshes;
        public List<Vector3> objectPositions;
    }
    private static string _jsonFile;

    public static void LoadSelectedChunk(GameObject chunk)
    { 
        string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
        ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(path));
        
        //GameObject newChunk = new GameObject(loadedData.name);
        Mesh chunkMesh = new Mesh();

        chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.chunkMesh;
        chunk.AddComponent<MeshRenderer>().sharedMaterial = loadedData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.chunkMesh;
        //chunk.AddComponent<Chunk>();

        /*Mesh objectMesh = new Mesh();
        chunk.GetComponent<Chunk>().chunkObjects.Add(new GameObject());
        chunk.GetComponent<Chunk>().chunkObjects[0].AddComponent<MeshFilter>().sharedMesh = loadedData.objectMeshes[0];
        chunk.GetComponent<Chunk>().chunkObjects[0].AddComponent<MeshCollider>().sharedMesh = loadedData.objectMeshes[0];*/
        //chunk.GetComponent<Chunk>().chunkObjects = loadedData.objects;
        chunk.transform.position = loadedData.position;
        
        /*for (int i = 0; i < chunk.GetComponent<Chunk>().chunkObjects.Count; i++)
        {
            chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<Mesh>() = loadedData.objectMeshes[i];
        }*/

        chunkMesh.Clear();
        chunkMesh.vertices = loadedData.chunkMesh.vertices;
        chunkMesh.triangles = loadedData.chunkMesh.triangles;
        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();
        
        /*objectMesh.Clear();
        objectMesh.vertices = loadedData.objectMeshes[0].vertices;
        objectMesh.triangles = loadedData.objectMeshes[0].triangles;
        objectMesh.RecalculateNormals();
        objectMesh.RecalculateBounds();*/

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
        newChunkData.chunkMesh         = chunk.GetComponent<MeshFilter>().sharedMesh;
        newChunkData.position     = chunk.transform.position;
        newChunkData.name         = chunk.name;
        //newChunkData.objects      = chunk.GetComponent<Chunk>().chunkObjects;
        
        /*for (int i = 0; i < chunk.GetComponent<Chunk>().chunkObjects.Count; i++)
        {
            newChunkData.objectMeshes[i] = chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<Mesh>();
        }*/

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
            newChunkData.chunkMesh         = chunk.GetComponent<MeshFilter>().sharedMesh;
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