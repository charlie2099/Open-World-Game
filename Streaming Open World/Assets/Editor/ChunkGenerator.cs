using UnityEngine;
using UnityEditor;

/// <summary>
/// Must sit inside the Editor folder.
/// Anything inside the Editor folder will not be included in the build of the game
/// Nothing critical to the game should be contained within this script
/// </summary>

public class ChunkGenerator : EditorWindow
{
    private Texture2D heightMap;
    private Material material;
    private int multiplier;
    private int chunkSize; 
    
    [MenuItem("My Tools/TerrainGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ChunkGenerator));
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn Terrain Chunks", EditorStyles.boldLabel);
        
        heightMap  = EditorGUILayout.ObjectField("Heightmap", heightMap, typeof(Texture2D), false) as Texture2D;
        material   = EditorGUILayout.ObjectField("Material", material, typeof(Material), false) as Material;
        multiplier = EditorGUILayout.IntField("Multiplier", multiplier);
        chunkSize  = EditorGUILayout.IntField("Chunk Size", chunkSize);

        if (GUILayout.Button("Generate Terrain"))
        {
            GenerateTerrain();
        }
    }

    private void GenerateTerrain()
    {
        if (heightMap == null)
        {
            Debug.LogError("Error: Please assign a heightmap to be used");
            return;
        }

        if (material == null)
        {
            Debug.LogError("Error: Please assign a material to be used");
            return;
        }
        
        // call GenerateTerrain method from MeshLoader script

        //MeshLoader.GenerateMap();
    }
}
