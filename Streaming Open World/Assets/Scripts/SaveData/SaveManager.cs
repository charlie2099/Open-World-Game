using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Saves chunk data, terrain data, and npc data to json files.
/// </summary>

public class SaveManager : MonoBehaviour
{
    public class ChunkData
    {
        public string name;
        public Mesh chunkMesh;
        public Material material;
        public Vector3 position;

        public string[] objectNames;
        public List<GameObject> objects; // don't write gameobjects to file?
        public Mesh[] objectMeshes;
        public Material[] objectMaterials;
        public Vector3[] objectPos;
    }

    public class TerrainData
    {
        public int chunkSize;
        public int terrainWidth;
        public int terrainHeight;
    }
    
    private static string _jsonFile;

    public static void LoadChunk(GameObject chunk)
    { 
        string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
        ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(path));
        //Mesh chunkMesh = new Mesh();

        chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.chunkMesh;
        chunk.AddComponent<MeshRenderer>().sharedMaterial = loadedData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.chunkMesh;
        //chunk.AddComponent<Chunk>();
        
        // Load chunk terrain objects 
        for (int i = 0; i < loadedData.objects.Count; i++)
        {
            GameObject newObj = new GameObject(loadedData.objectNames[i]);
            newObj.AddComponent<MeshFilter>().sharedMesh       = loadedData.objectMeshes[i];
            newObj.AddComponent<MeshRenderer>().sharedMaterial = loadedData.objectMaterials[i];
            newObj.AddComponent<MeshCollider>().sharedMesh = loadedData.objectMeshes[i];
            newObj.transform.position = loadedData.objectPos[i];
            newObj.transform.parent = chunk.transform;
            chunk.GetComponent<Chunk>().chunkObjects.Add(newObj);
        }
        
        chunk.transform.position = loadedData.position;

        /*chunkMesh.Clear();
        chunkMesh.vertices = loadedData.chunkMesh.vertices;
        chunkMesh.triangles = loadedData.chunkMesh.triangles;
        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();*/

        chunk.GetComponent<Chunk>().isLoaded = true;
        chunk.tag = "TerrainChunk";
                
        print("Terrain List: " + TerrainGenerator.GetChunks().Count);
    }

    public static void UnloadChunk(GameObject chunk)
    {
        chunk.GetComponent<Chunk>().isLoaded = false;
        
        // Update chunk data to the state when it was unloaded
        ChunkData newChunkData = new ChunkData();
        newChunkData.material           = chunk.GetComponent<MeshRenderer>().sharedMaterial;
        newChunkData.chunkMesh          = chunk.GetComponent<MeshFilter>().sharedMesh;
        newChunkData.position           = chunk.transform.position;
        newChunkData.name               = chunk.name;
        newChunkData.objects            = chunk.GetComponent<Chunk>().chunkObjects;
        newChunkData.objectNames        = new string[newChunkData.objects.Count];
        newChunkData.objectPos          = new Vector3[newChunkData.objects.Count];
        newChunkData.objectMeshes       = new Mesh[newChunkData.objects.Count];
        newChunkData.objectMaterials    = new Material[newChunkData.objects.Count];
        for (int i = 0; i < newChunkData.objects.Count; i++)
        {
            if (newChunkData.objectPos != null)
            {
                newChunkData.objectNames[i] = chunk.GetComponent<Chunk>().chunkObjects[i].name;
                newChunkData.objectMeshes[i] = chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<MeshFilter>().sharedMesh;
                newChunkData.objectMaterials[i] = chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<MeshRenderer>().sharedMaterial;
                newChunkData.objectPos[i] = chunk.GetComponent<Chunk>().chunkObjects[i].transform.position;
            }
        }
        
        // Converts new chunk data to JSON form.
        _jsonFile = JsonUtility.ToJson(newChunkData, true);

        // Writes chunkData to json file
        string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
        File.WriteAllText(path, _jsonFile);

        // Remove it from the active chunks list and strip its components
        Destroy(chunk.GetComponent<MeshRenderer>());
        Destroy(chunk.GetComponent<MeshFilter>());
        Destroy(chunk.GetComponent<MeshCollider>());
        foreach (var obj in chunk.GetComponent<Chunk>().chunkObjects.ToArray())
        {
            Destroy(obj);
        }
        chunk.GetComponent<Chunk>().chunkObjects.Clear();
        //Destroy(chunk.GetComponent<Chunk>());
        chunk.tag = "Untagged";
    }

    public static void SaveToFile() // Saves terrain, chunk, and other data to file
    {
        print("Size when saving to file: " + TerrainGenerator.GetChunks().Count);

        TerrainData newTerrainData = new TerrainData();
        newTerrainData.chunkSize     = TerrainGenerator.GetChunkSize();
        newTerrainData.terrainWidth  = TerrainGenerator.GetTerrainWidth();
        newTerrainData.terrainHeight = TerrainGenerator.GetTerrainHeight();

        // Converts new terrain data to JSON form.
        _jsonFile = JsonUtility.ToJson(newTerrainData, true);

        // Writes terrainData to json file
        string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
        File.WriteAllText(terrainPath, _jsonFile);

        // Update chunk data when unloaded
        foreach (var chunk in TerrainGenerator.GetChunks())
        {
            ChunkData newChunkData = new ChunkData();
            newChunkData.material           = chunk.GetComponent<MeshRenderer>().sharedMaterial;
            newChunkData.chunkMesh          = chunk.GetComponent<MeshFilter>().sharedMesh;
            newChunkData.position           = chunk.transform.position;
            newChunkData.name               = chunk.name;
            
            newChunkData.objects            = chunk.GetComponent<Chunk>().chunkObjects;
            newChunkData.objectNames        = new string[newChunkData.objects.Count];
            newChunkData.objectPos          = new Vector3[newChunkData.objects.Count];
            newChunkData.objectMeshes       = new Mesh[newChunkData.objects.Count];
            newChunkData.objectMaterials    = new Material[newChunkData.objects.Count];
            
            print("Object Size: " + newChunkData.objects.Count);

            for (int i = 0; i < newChunkData.objects.Count; i++)
            {
                if (newChunkData.objectPos != null)
                {
                    newChunkData.objectNames[i] = chunk.GetComponent<Chunk>().chunkObjects[i].name;
                    newChunkData.objectMeshes[i] = chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<MeshFilter>().sharedMesh;
                    newChunkData.objectMaterials[i] = chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<MeshRenderer>().sharedMaterial;
                    newChunkData.objectPos[i] = chunk.GetComponent<Chunk>().chunkObjects[i].transform.position;
                }
            }

            // Converts new chunk data to JSON form.
            _jsonFile = JsonUtility.ToJson(newChunkData, true);

            // Writes chunkData to json file
            string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
            File.WriteAllText(path, _jsonFile);
        }
        //print("<color=orange> Chunks unloaded </color>");
        //print("<color=orange> Chunk data written to file </color>");
        Debug.Log("Saving!");
    }
}