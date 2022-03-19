using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private Transform activeOceanChunks;
    [SerializeField] private int maxChunkLoadDistance = 400;
   //[SerializeField] private int checkRate;
    private Transform player;

    private void Start()
    {
        player = transform;
    }
    
    private void CheckChunks()
    {
        // No matter how high up player is, it is always the same distance it is calculating
        // So playerPos.y needs to be set to 0
        // check centre of chunks
    }

    private void Update()
    {
        foreach (var chunk in TerrainGenerator.GetChunks())
        {
            if (chunk != null)
            {
                if (Vector3.Distance(player.position, chunk.transform.position ) > maxChunkLoadDistance)
                {
                    //chunk.Unload();
                }
                else
                {
                    //chunk.Load();
                }
            }
        }
        
        
        /*for (int x = 0; x < TerrainGenerator._numberOfChunks; x++)
        {
            for (int z = 0; z < TerrainGenerator._numberOfChunks; z++)
            {
                if (Vector3.Distance(player.position, ) > maxChunkLoadDistance)
                {
                    //chunk.Unload();
                }
                else
                {
                    //chunk.Load();
                }
                "/chunk " + x + " , " + z + ".json";
            }
        }*/





        // Grass Terrain
        /*foreach (var chunk in TerrainGenerator.GetChunks().ToArray())
        {
            if (Vector3.Distance(player.position, chunk.transform.position) > maxChunkLoadDistance)
            {
                //chunk.Unload();
            }
            else
            {
                //chunk.Load();
            }
        }*/

        // Ocean Terrain
        for (int i = 0; i < activeOceanChunks.childCount; i++)
        {
            if (Vector3.Distance(player.position, activeOceanChunks.GetChild(i).position) > maxChunkLoadDistance)
            {
                activeOceanChunks.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                activeOceanChunks.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}








/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [Serializable]
    public class ChunkData
    {
        public Texture2D heightmap;
        public Material material;
        public Vector3 position;
        
        public Mesh mesh;
        public Vector3[] vertices;
        public int[] triangles;
        public int numberOfChunks;
        public int chunkSize;
        public int terrainWidth;
        private int multiplier;
    }

    //[SerializeField] private ChunkData chunkData;
    
    [SerializeField] private Transform activeChunks;
    [SerializeField] private Transform activeOceanChunks;
    [SerializeField] private int maxChunkLoadDistance = 400;
   //[SerializeField] private int checkRate;
    private Transform player;
    //private string jsonFile;

    private void Start()
    {
        player = transform;

        // Converts 'chunkData' to JSON form.
        //jsonFile = JsonUtility.ToJson(chunkData);
        
        //InvokeRepeating(CheckChunks(), 0.0f, checkRate);
    }
    
    private void CheckChunks()
    {
        // No matter how high up player is, it is always the same distance it is calculating
        // So playerPos.y needs to be set to 0
        // check centre of chunks
    }

    private void Update()
    {
        for (int i = 0; i < activeChunks.childCount; i++)
        {
            if (Vector3.Distance(player.position, activeChunks.GetChild(i).position) > maxChunkLoadDistance)
            {
                activeChunks.GetChild(i).gameObject.SetActive(false);
                //activeChunks.Remove(chunk);
                //Destroy(chunk);
            }
            else
            {
                activeChunks.GetChild(i).gameObject.SetActive(true);
                //LoadChunk(chunk);
            }
        }
        
        for (int i = 0; i < activeOceanChunks.childCount; i++)
        {
            if (Vector3.Distance(player.position, activeOceanChunks.GetChild(i).position) > maxChunkLoadDistance)
            {
                activeOceanChunks.GetChild(i).gameObject.SetActive(false);
                //activeChunks.Remove(chunk);
                //Destroy(chunk);
            }
            else
            {
                activeOceanChunks.GetChild(i).gameObject.SetActive(true);
                //LoadChunk(chunk);
            }
        }
    }

    public static void SaveToFile(Texture2D hMap, Material tMat, int chunkSize, int tWidth, int tHeight)
    {
        print("Heightmap: " + hMap.name);
        print("Material: " + tMat.name);
        print("Chunk Size: " + chunkSize);
        print("Terrain Width: " + tWidth);
        print("Terrain Height: " + tHeight);
    }
}*/
