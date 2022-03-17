using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public List<GameObject> chunkObjects = new List<GameObject>();

    private void OnEnable()
    {
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
        foreach (var chunkObj in chunkObjects)
        {
            // Disable all objects found in this chunk
            if (chunkObj != null)
            {
                chunkObj.SetActive(false);
            }
        }
    }
}
