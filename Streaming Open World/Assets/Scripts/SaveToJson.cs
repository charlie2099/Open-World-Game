using System.IO;
using UnityEngine;

public class SaveToJson : MonoBehaviour
{
    [System.Serializable]
    public class ChunkData
    {
        public TerrainData[] terrainData;
        public Material material;
        public Vector3 position;
    }
    [SerializeField] private ChunkData chunkData;
    //private string saveFile;
    //private string jsonFile;

    private void Start()
    {
        //string saveFile = Application.persistentDataPath + "/gamedata.json";
        string jsonFile = JsonUtility.ToJson(chunkData);
        
        //Write to file
        File.WriteAllText(Application.dataPath + "chunkDataFile.json", jsonFile);
    
        // Read from file
        ChunkData loadedChunkData = JsonUtility.FromJson<ChunkData>(jsonFile);
        Debug.Log("Loaded Position: " + loadedChunkData.position);
        Debug.Log("Loaded TerrainData: " + loadedChunkData.terrainData);
        
        float x = 0;
        float z = 0;
        float rowLength = 16;
        const float spacing = 112.5f;

        for (int i = 0; i < chunkData.terrainData.Length; i++)
        {
            // Instantiate a chunk
            GameObject gObject = new GameObject("chunk " + i);
        
            gObject.AddComponent<Terrain>();
            gObject.AddComponent<TerrainCollider>();
        
            gObject.GetComponent<Terrain>().materialTemplate = loadedChunkData.material;
            gObject.GetComponent<Terrain>().terrainData = loadedChunkData.terrainData[i];
            gObject.GetComponent<TerrainCollider>().terrainData = loadedChunkData.terrainData[i];

            gObject.transform.position = new Vector3(loadedChunkData.position.x + (x * spacing), loadedChunkData.position.y, loadedChunkData.position.z + (z * spacing));
            z++;
            if (z >= rowLength)
            {
                z = 0;
                x++;
            }
            
            gObject.transform.parent = transform;
        }
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
