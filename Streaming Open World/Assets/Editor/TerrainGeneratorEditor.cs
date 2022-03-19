using UnityEngine;
using UnityEditor;

/// <summary>
/// Generate a chunked terrain
/// 
/// WARNING:
/// Must sit inside the Editor folder.
/// Anything inside the Editor folder will not be included in the build of the game.
/// Nothing critical to the game should be contained within this script.
/// </summary>

public class TerrainGeneratorEditor : EditorWindow
{
    private Texture2D heightMap;
    private Material material;
    private int chunkSize        = 32;
    private int terrainWidth     = 1024;
    private int terrainHeight    = 100; 
    private string containerName = "Terrain";

    [MenuItem("My Tools/TerrainGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TerrainGeneratorEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn Terrain Chunks", EditorStyles.boldLabel);
        
        heightMap  = EditorGUILayout.ObjectField("Heightmap", heightMap, typeof(Texture2D), false) as Texture2D;
        material   = EditorGUILayout.ObjectField("Material", material, typeof(Material), false) as Material;
        terrainHeight = EditorGUILayout.IntField("Multiplier", terrainHeight);
        chunkSize  = EditorGUILayout.IntField("Chunk Size", chunkSize);
        terrainWidth  = EditorGUILayout.IntField("Terrain Width", terrainWidth);
        containerName = EditorGUILayout.TextField("Container name", containerName);
        
        if (GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrain();
            Debug.Log("Chunks list size: " + TerrainGenerator.GetChunks().Count);
        }
        
        if (GUILayout.Button("Save to file"))
        {
            //ChunkLoader.SaveToFile(heightMap, material, chunkSize, terrainWidth, terrainHeight);
            SaveToJson2.SaveToFile();
            
            // NOTE:
            // - If there are multiple calls to the TerrainGenerator.GenerateMap() method, how will the activeChunks
            // list be affected?? Also what about when saving them to file?
        }
    }

    private void GenerateTerrain()
    {
        if (heightMap == null)
        {
            Debug.LogError("<color=red> Error: Please assign a heightmap to be used! </color>");
            return;
        }

        if (material == null)
        {
            Debug.LogError("<color=red> Error: Please assign a material to be used! </color>");
            return;
        }

        // Generates the terrain 
        TerrainGenerator.GenerateMap(heightMap, material, chunkSize, terrainWidth, terrainHeight);
        
        // Creates the container for the chunks to parent to
        GameObject container = new GameObject { name = containerName };

        foreach (var chunk in FindObjectsOfType<GameObject>())
        {
            // Parents object to the container if it contains the tag AND doesn't already have a parent
            if (chunk.CompareTag("TerrainChunk") && chunk.transform.parent == null)
            {
                chunk.transform.parent = container.transform;
            }
        }
        
        Debug.Log("<color=lime> A new terrain has been generated! </color>");
    }
}
