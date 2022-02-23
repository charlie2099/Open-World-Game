using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private List<TerrainData> terrainDataList = new List<TerrainData>();
    
    private List<GameObject> chunksList = new List<GameObject>();
    private GameObject _chunk;

    private void Start()
    {
        LoadChunk();
    }

    private void LoadChunk()
    {
        float x = 0;
        float z = 0;
        float rowLength = 16;
        const float spacing = 112.5f;
        
        if (chunksList.Count < terrainDataList.Count)
        {
            foreach (var tData in terrainDataList)
            {
                var terrainData = Resources.Load<TerrainData>("Terrain/" + tData.name);
                _chunk = Terrain.CreateTerrainGameObject(terrainData);
                _chunk.transform.position = new Vector3(x * spacing, 0, z * spacing);
                z++;
                if (z >= rowLength)
                {
                    z = 0;
                    x++;
                }
                chunksList.Add(_chunk);
            }
        }

        if (chunksList.Count == terrainDataList.Count)
        {
            //Debug.Log("Chunks: " + chunksList.Count);
            //Debug.Log("TerrainData: " + terrainDataList.Count);
            Debug.Log("<color=lime> All Chunks successfully loaded! </color>");
        }
        else
        {
            Debug.LogError("<color=red> Some Chunks failed to load! </color>");
        }
    }

    private void UnloadChunk()
    {
        foreach (GameObject chunk in chunksList.ToList())
        {
            chunksList.Remove(chunk);
            Destroy(chunk);
        }
        Resources.UnloadUnusedAssets();

        if (chunksList.Count <= 0)
        {
            Debug.Log("<color=orange> All Chunks successfully unloaded! </color>");
        }
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
