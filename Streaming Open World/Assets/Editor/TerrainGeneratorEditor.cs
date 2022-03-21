using System;
using System.IO;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Generates a chunked terrain using the editor window. Chunk data is saved to json files.
/// When a new terrain is generated the previous one will be overwritten.
/// </summary>

public class TerrainGeneratorEditor : EditorWindow
{
    /// Must sit inside the Editor folder.
    /// Anything inside the Editor folder will not be included in the build of the game.
    
    private Texture2D heightMap;
    private Material material;
    private int chunkSize        = 32;
    private int terrainWidth     = 500;
    private int terrainHeight    = 100;
    private string regenButtonName;

    [MenuItem("My Tools/TerrainGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TerrainGeneratorEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn Terrain Chunks", EditorStyles.boldLabel);
        
        heightMap     = EditorGUILayout.ObjectField("Heightmap", heightMap, typeof(Texture2D), false) as Texture2D;
        material      = EditorGUILayout.ObjectField("Material", material, typeof(Material), false) as Material;
        terrainWidth  = EditorGUILayout.IntSlider("Terrain Width", terrainWidth, 0, 1024);
        terrainHeight = EditorGUILayout.IntSlider("Terrain Height", terrainHeight, 0, 1024);
        chunkSize     = EditorGUILayout.IntField("Chunk Size", chunkSize);

        string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
        if (TerrainGenerator.GetChunks().Count <= 0 || !File.Exists(terrainPath)) // if a terrain doesn't already exist
        {
            regenButtonName = "Generate Terrain";
        }
        else // if a terrain already exists
        {
            regenButtonName = "Regenerate Terrain";
        }
        if (GUILayout.Button(regenButtonName))
        {
            GenerateTerrain();
            Debug.Log("Chunks list size: " + TerrainGenerator.GetChunks().Count);
        }

        if (GUILayout.Button("Save to file"))
        {
            SaveManager.SaveToFile();
            Debug.Log("Saving!");
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
        
        if (terrainWidth < chunkSize)
        {
            Debug.LogError("<color=red> Error: Terrain width should be larger than the chunk size! </color>");
            return;
        }

        // if no terrain exists already, generate a terrain
        if (TerrainGenerator.GetChunks().Count <= 0)
        {
            TerrainGenerator.GenerateMap(heightMap, material, chunkSize, terrainWidth, terrainHeight);
            Debug.Log("<color=lime> A new terrain has been generated! </color>");
        }
        else // if a terrain exists delete it's save data, clear list and regenerate the terrain
        {
            // Delete existing terrain data
            string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
            if (File.Exists(terrainPath))
            {
                File.Delete(terrainPath);
            }

            // Delete existing chunk data
            string chunkPath = Application.dataPath + "/SaveData/ChunkData/";
            var dir = Directory.GetFiles(chunkPath);
            foreach (var chunkFile in dir)
            {
                File.Delete(chunkFile);
            }

            // if a terrain exists destroy it's container 
            foreach (var gObj in FindObjectsOfType<GameObject>())
            {
                if (gObj.CompareTag("MainTerrain"))
                {
                    Debug.Log("Main terrain found");
                    DestroyImmediate(gObj);
                }
            }

            TerrainGenerator.GetChunks().Clear();

            TerrainGenerator.GenerateMap(heightMap, material, chunkSize, terrainWidth, terrainHeight);
            Debug.Log("<color=lime> Existing terrain has been regenerated! </color>");

            Debug.Log("Size: " + TerrainGenerator.GetChunks().Count);
        }
    }
}
