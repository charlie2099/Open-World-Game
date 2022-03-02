using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveToJson : MonoBehaviour
{
    [SerializeField] private TerrainData _tData;
    [SerializeField] private Material material;
    private ChunkData chunkData;
    //private string saveFile;
    //private string jsonFile;
    //[SerializeField] private Shader _shader;
    //[SerializeField] private TerrainLayer[] _terrainLayer;
    
    [System.Serializable]
    public class ChunkData
    {
        //public Terrain Terrain;
        public TerrainData TerrainData;
        public Material Material;
        public Vector3 Position;
    }

    private void Start()
    {
        //string saveFile = Application.persistentDataPath + "/gamedata.json";
        
        chunkData = new ChunkData
        {
            Position = new Vector3(1, 2, 3),
            TerrainData = _tData,
            Material = material
        };
        
        string jsonFile = JsonUtility.ToJson(chunkData);

        //Write to file
        File.WriteAllText(Application.dataPath + "chunkDataFile.json", jsonFile);
        
        // Read from file
        ChunkData loadedChunkData = JsonUtility.FromJson<ChunkData>(jsonFile);
        Debug.Log("Loaded Position: " + loadedChunkData.Position);
        Debug.Log("Loaded TerrainData: " + loadedChunkData.TerrainData);

        // Instantiate a chunk
        GameObject gameObject = new GameObject("anything");
        
        gameObject.AddComponent<Terrain>();
        gameObject.AddComponent<TerrainCollider>();
        
        // Passing the data 
        gameObject.GetComponent<Terrain>().materialTemplate = loadedChunkData.Material;
        gameObject.GetComponent<Terrain>().terrainData = loadedChunkData.TerrainData;
        gameObject.GetComponent<TerrainCollider>().terrainData = loadedChunkData.TerrainData;
        //gameObject.GetComponent<Terrain>().materialTemplate.shader = _shader;
        //gameObject.GetComponent<Terrain>().terrainData.terrainLayers = _terrainLayer;

    }

    /*private void ReadFromFile()
    {
        if (File.Exists(saveFile))
        {
            Debug.Log("Reading from file");
            string fileContents = File.ReadAllText(saveFile);
            
            // Deserialize the JSON data
            chunkData = JsonUtility.FromJson<ChunkData>(saveFile);
        }
    }*/

    /*private void WriteToFile()
    {
        Debug.Log("Writing to file");
        
        // Serialize the object into JSON an save string 
        string jsonString = JsonUtility.ToJson(chunkData);
        
        // Writes contents of the JSON file to the save file
        File.WriteAllText(saveFile, jsonFile);
    }*/
}
