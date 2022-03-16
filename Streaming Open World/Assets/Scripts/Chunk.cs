using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private GameObject chunkObjects;

    private void OnEnable()
    {
        // Enable all objects found in this chunk
        if (chunkObjects != null)
        {
            chunkObjects.SetActive(true);
        }
    }

    private void OnDisable()
    {
        // Disable all objects found in this chunk
        if (chunkObjects != null)
        {
            chunkObjects.SetActive(false);
        }
    }
}
