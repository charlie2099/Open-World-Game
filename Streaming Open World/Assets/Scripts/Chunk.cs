using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private List<TerrainData> terrainDataList = new List<TerrainData>();
    private List<GameObject> chunksList = new List<GameObject>();
    private GameObject chunk;
    private bool[] isLoaded;

    private void Start()
    {
        isLoaded = new bool[terrainDataList.Count];
    }

    public void LoadChunk()
    {
        if (chunksList.Count < terrainDataList.Count)
        {
            /*foreach (TerrainData data in terrainDataList)
            {
                var terrainData = Resources.Load<TerrainData>("Terrain/" + data.name);
                chunk = Terrain.CreateTerrainGameObject(terrainData);
                chunksList.Add(chunk);
                isLoaded[] = true;
            }*/

            for (var i = 0; i < terrainDataList.Count; i++)
            {
                var terrainData = Resources.Load<TerrainData>("Terrain/" + terrainDataList[i].name);
                chunk = Terrain.CreateTerrainGameObject(terrainData);
                chunksList.Add(chunk);
                isLoaded[i] = true;
            }
        }
    }

    public void UnloadChunk()
    {
        foreach (var chunk in chunksList)
        {
            chunksList.Remove(chunk);
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
