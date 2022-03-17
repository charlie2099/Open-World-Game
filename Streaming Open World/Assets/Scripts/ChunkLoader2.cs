using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader2 : MonoBehaviour
{
    //[SerializeField] private List<GameObject> activeChunks = new List<GameObject>();
    //[SerializeField] private List<GameObject> activeOceanChunks = new List<GameObject>();
    [SerializeField] private Transform activeChunks;
    [SerializeField] private Transform activeOceanChunks;
    [SerializeField] private int maxChunkLoadDistance = 400;
    [SerializeField] private int checkRate;
    private Transform player;

    private void Start()
    {
        player = transform;
        //InvokeRepeating(CheckChunks(), 0.0f, checkRate);
    }
    
    private void CheckChunks()
    {
        // No matter how high up player is, it is always the same distance it is calculating
        // So playerPos.y needs to be set to 0
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


        /*foreach (var chunk in activeChunks.ToArray())
        {
            if (Vector3.Distance(player.position, chunk.transform.position) > maxChunkLoadDistance)
            {
                chunk.SetActive(false);
                //activeChunks.Remove(chunk);
                //Destroy(chunk);
            }
            else
            {
                chunk.SetActive(true);
                //LoadChunk(chunk);
            }
        }*/

        /*foreach (var oceanChunk in activeOceanChunks.ToArray())
        {
            if (Vector3.Distance(player.position, oceanChunk.transform.position) > maxChunkLoadDistance)
            {
                oceanChunk.SetActive(false);
                //activeChunks.Remove(chunk);
                //Destroy(chunk);
            }
            else
            {
                oceanChunk.SetActive(true);
                //LoadChunk(chunk);
            }
        }*/
    }
}
