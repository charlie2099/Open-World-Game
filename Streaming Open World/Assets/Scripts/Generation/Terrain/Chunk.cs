using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chunk component attached to each chunk. Maintains a list of all objects
/// assigned to that chunk. 
/// </summary>

public class Chunk : MonoBehaviour
{
    public List<GameObject> chunkObjects = new List<GameObject>();
    [HideInInspector] public bool isLoaded = false;

    private void OnEnable()
    {
        // Objects on Chunk
        /*foreach (var chunkObj in chunkObjects)
        {
            // Enable all objects found in this chunk
            if (chunkObj != null)
            {
                chunkObj.SetActive(true);
            }
        }*/
    }

    private void OnDisable()
    {
        // Objects on Chunk
        /*foreach (var chunkObj in chunkObjects)
        {
            // Disable all objects found in this chunk
            if (chunkObj != null)
            {
                chunkObj.SetActive(false);
            }
        }*/
    }

    public bool IsLoaded()
    {
        return isLoaded;
    }
}
