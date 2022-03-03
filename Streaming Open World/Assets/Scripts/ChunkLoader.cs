using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    //[SerializeField] private Transform player;
    //[SerializeField] private GameObject playerPrefab;
    //private GameObject player;
    
    [System.Serializable]
    public class ChunkData
    {
        public TerrainData[] terrainData;
        public Material material;
        public Vector3 position;
    }
    
    [SerializeField] private ChunkData chunkData;
    private List<GameObject> chunkList = new List<GameObject>(); 
    private string jsonFile;

    private void Start()
    {
        // Converts 'chunkData' to JSON form.
        jsonFile = JsonUtility.ToJson(chunkData);
        WriteToFile();
    }

    private void Update()
    {
        /*foreach (var chunk in chunkList.ToArray())
        {
            if (Vector3.Distance(player.position, chunk.transform.position) > 500)
            {
                chunkList.Remove(chunk);
                Destroy(chunk);
            }
        }*/

        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadChunk();
            //player = Instantiate(playerPrefab, new Vector3(1038.2f,-142.32f,861.56f), Quaternion.identity);
        }
        
        if(Input.GetKeyDown(KeyCode.U))
        {
            UnloadChunk();
            //Destroy(player);
        }
    }

    private void LoadChunk()
    {
        ReadFromFile();
    }

    private void UnloadChunk()
    {
        WriteToFile();
        
        foreach (var chunk in chunkList.ToArray())
        {
            chunkList.Remove(chunk);
            Destroy(chunk);
        }
    }

    private void WriteToFile()
    {
        // Converts 'chunkData' to JSON form.
        jsonFile = JsonUtility.ToJson(chunkData); // needed here?
        
        // Writes chunkData to json file
        File.WriteAllText(Application.dataPath + "chunkDataFile.json", jsonFile);
    }

    private void ReadFromFile()
    {
        // Read contents of json file
        ChunkData loadedChunkData = JsonUtility.FromJson<ChunkData>(jsonFile);
        float x = 0;
        float z = 0;
        const float spacing = 112.5f;
        const int rowLength = 16;

        for (int i = 0; i < chunkData.terrainData.Length; i++)
        {
            GameObject chunk = new GameObject("chunk " + i);
            chunk.AddComponent<Terrain>();
            chunk.AddComponent<TerrainCollider>();

            chunk.GetComponent<Terrain>().materialTemplate = loadedChunkData.material;
            chunk.GetComponent<Terrain>().terrainData = loadedChunkData.terrainData[i];
            chunk.GetComponent<TerrainCollider>().terrainData = loadedChunkData.terrainData[i];

            var chunkPos = loadedChunkData.position;
            var xPos = chunkPos.x + (x * spacing);
            var zPos = chunkPos.z + (z * spacing);
            chunk.transform.position = new Vector3(xPos, chunkPos.y, zPos);
            chunk.transform.parent = transform;

            //chunkData.position = chunk.transform.position;
            chunkList.Add(chunk);
            
            z++;
            if (z >= rowLength)
            {
                z = 0;
                x++;
            }
        }
    }

    //private void CreateChunk(ChunkData loadedData) { }
    //Debug.Log("Loaded Position: " + loadedChunkData.position);
    //Debug.Log("Loaded TerrainData: " + loadedChunkData.terrainData);
}
