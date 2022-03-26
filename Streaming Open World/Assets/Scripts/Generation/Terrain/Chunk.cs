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
    private bool isLoaded = false;

    public bool IsLoaded()
    {
        return isLoaded;
    }

    public void SetLoaded(bool load)
    {
        isLoaded = load;
    }
}
