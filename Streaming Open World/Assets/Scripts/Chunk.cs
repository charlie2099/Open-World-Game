using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour
{
    public List<GameObject> chunkObjects = new List<GameObject>();
    [HideInInspector] public bool isLoaded = false;

    private void OnEnable()
    {
        // Objects on Chunk
        foreach (var chunkObj in chunkObjects)
        {
            // Enable all objects found in this chunk
            if (chunkObj != null)
            {
                chunkObj.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        // Objects on Chunk
        foreach (var chunkObj in chunkObjects)
        {
            // Disable all objects found in this chunk
            if (chunkObj != null)
            {
                chunkObj.SetActive(false);
            }
        }
    }

    public bool IsLoaded()
    {
        return isLoaded;
    }
}
