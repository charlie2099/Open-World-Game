using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private Transform activeOceanChunks;
    [SerializeField] private int maxChunkLoadDistance = 400;
    private Transform player;
    public static List<GameObject> loadedChunks = new List<GameObject>();

    private void Start()
    {
        player = transform;

        foreach (var chunk in TerrainGenerator.GetChunks())
        {
            loadedChunks.Add(chunk);
            chunk.GetComponent<Chunk>().isLoaded = true;
        }
    }

    private void Update()
    {
        foreach (var chunk in TerrainGenerator.GetChunks().ToArray())
        {
            // If distance between player and chunk is more maxViewDistance AND chunk is loaded, unload it
            if (Vector3.Distance(player.position, chunk.transform.position) > maxChunkLoadDistance)
            {
                if (chunk.GetComponent<Chunk>().IsLoaded())
                {
                    SaveManager.UnloadChunk(chunk);
                }
            }
            
            // If distance between player and chunk is less than maxViewDistance AND chunk is unloaded, load it
            if (Vector3.Distance(player.position, chunk.transform.position) < maxChunkLoadDistance)
            {
                if (!chunk.GetComponent<Chunk>().IsLoaded())
                {
                    SaveManager.LoadChunk(chunk);
                }
            }
        }
        

        /*foreach (var chunk in loadedChunks)
        {
            // do this
        }*/
        
        /*if(chunk.!IsLoaded())
        {
            SaveManager.LoadSelectedChunk(chunk);
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
