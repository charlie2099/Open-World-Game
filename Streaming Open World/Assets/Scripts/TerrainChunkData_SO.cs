using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "ScriptableObjects/TerrainDataScriptableObject", order = 1)]
public class TerrainChunkData_SO : ScriptableObject
{
    public Texture2D heightMap;
    public Material material;
    public int multiplier       = 100; // terrainHe
    public int chunkSize        = 32;
    public int terrainWidth     = 1024;
    public string containerName = "Terrain";
}
