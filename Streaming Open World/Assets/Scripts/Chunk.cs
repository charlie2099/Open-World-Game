using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private List<TerrainData> terrainDataList = new List<TerrainData>();
    private List<GameObject> chunksList = new List<GameObject>();
    private GameObject chunk;
    private bool loaded;

    public void LoadChunk()
    {
        foreach (TerrainData data in terrainDataList)
        {
            var terrainData = Resources.Load<TerrainData>("Terrain/" + data.name);
            chunk = Terrain.CreateTerrainGameObject(terrainData);
            chunksList.Add(chunk);
        }
    }

    public void UnloadChunk()
    {
        foreach (var chunk in chunksList)
        {
            Destroy(chunk);
        }
        Resources.UnloadUnusedAssets();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadChunk();
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            UnloadChunk();
        }
    }
}
