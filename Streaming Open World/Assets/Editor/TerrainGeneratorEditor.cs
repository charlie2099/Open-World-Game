using System;
using System.IO;
using Codice.Client.BaseCommands;
using Generation.Terrain;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Generates a chunked terrain using the editor window. Chunk data is saved to json files.
/// When a new terrain is generated the previous one will be overwritten.
/// </summary>

public class TerrainGeneratorEditor : EditorWindow
{
    // Terrain
    private Texture2D heightMap;
    private Material material;
    private int chunkSize        = 32;
    private int terrainWidth     = 500;
    private int terrainHeight    = 100;
    
    // Terrain Objects
    private static bool _activeAssigner;
    private static bool _activeDeassigner;
    private GameObject objectToPaint;

    [MenuItem("My Tools/TerrainGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TerrainGeneratorEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Terrain Customisation", EditorStyles.boldLabel);
        heightMap     = EditorGUILayout.ObjectField("Heightmap", heightMap, typeof(Texture2D), false) as Texture2D;
        material      = EditorGUILayout.ObjectField("Material", material, typeof(Material), false) as Material;
        terrainWidth  = EditorGUILayout.IntSlider("Terrain Width", terrainWidth, 0, 1024);
        terrainHeight = EditorGUILayout.IntSlider("Terrain Height", terrainHeight, 0, 1024);
        chunkSize     = EditorGUILayout.IntField("Chunk Size", chunkSize);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.padding = new RectOffset(5, 5, 5, 5);
        buttonStyle.margin = new RectOffset(10, 10, 10, 10);
        buttonStyle.hover.textColor = Color.yellow;
        buttonStyle.fontStyle = FontStyle.Bold;

        if (GUILayout.Button("Load existing terrain", buttonStyle))
        {
            LoadExistingTerrain();
        }

        else if (GUILayout.Button("Generate a new terrain", buttonStyle))
        {
            GenerateNewTerrain();
        }

        else if (GUILayout.Button("Save to file", buttonStyle))
        {
            SaveManager.SaveToFile();
        }
        
        else if (GUILayout.Button("Destroy active terrain", buttonStyle))
        {
            DestroyActiveTerrain();
            Debug.Log("<color=orange> Active terrain has been destroyed! </color>");
        }
        
        GUILayout.Space(25);
        GUILayout.Label("Chunk Object Spawner", EditorStyles.boldLabel);
        GUILayout.Space(10);
        objectToPaint = EditorGUILayout.ObjectField("Object To Spawn", objectToPaint, typeof(GameObject), false) as GameObject;
        
        if (GUILayout.Button("Spawner: " + _activeAssigner, buttonStyle))
        {
            _activeAssigner = !_activeAssigner;
        }
        
        if (GUILayout.Button("Despawner: " + _activeDeassigner, buttonStyle))
        {
            _activeDeassigner = !_activeDeassigner;
        }
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;
    
    private void OnSceneGUI(SceneView view)
    {
        if (!_activeAssigner && !_activeDeassigner)
        {
            return;
        }

        if (Event.current.type == EventType.MouseDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit))
            {
                if (_activeAssigner)
                {
                    GameObject spawnedObj = Instantiate(objectToPaint, hit.point, Quaternion.identity);
                    hit.collider.gameObject.GetComponent<Chunk>().chunkObjects.Add(spawnedObj);
                    spawnedObj.transform.parent = hit.transform;
                }

                else if (_activeDeassigner)
                {
                    var chunkObj = hit.collider.gameObject.GetComponent<Chunk>().chunkObjects;
                    if (chunkObj != null)
                    {
                        foreach (var obj in chunkObj.ToArray())
                        {
                            DestroyImmediate(obj, true);
                            chunkObj.Clear();
                        }
                    }
                }
            }
            Event.current.Use();
        }
    }

    private void LoadExistingTerrain()
    {
        NullChecks();
        TerrainGenerator.GenerateMap(heightMap, material, chunkSize, terrainWidth, terrainHeight);
        Debug.Log("<color=cyan> A terrain has been loaded from file! </color>");
    }

    private void GenerateNewTerrain()
    {
        NullChecks();

        // if no terrain exists already, generate a terrain
        if (TerrainGenerator.GetChunks().Count <= 0)
        {
            TerrainGenerator.GenerateMap(heightMap, material, chunkSize, terrainWidth, terrainHeight);
            Debug.Log("<color=lime> A new terrain has been generated! </color>");
        }
        else // if a terrain exists delete it's save data, clear list and regenerate the terrain
        {
            DeleteSavedData();
            DestroyActiveTerrain();
            TerrainGenerator.GenerateMap(heightMap, material, chunkSize, terrainWidth, terrainHeight);
            Debug.Log("<color=green> Existing terrain has been regenerated! </color>");
            Debug.Log("Chunks list size: " + TerrainGenerator.GetChunks().Count);
        }
    }

    private void DeleteSavedData()
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
    }

    private void DestroyActiveTerrain()
    {
        // if a terrain exists destroy it's container 
        foreach (var gObj in FindObjectsOfType<GameObject>())
        {
            if (gObj.CompareTag("MainTerrain"))
            {
                //Debug.Log("Main terrain found");
                DestroyImmediate(gObj);
            }
        }

        TerrainGenerator.GetChunks().Clear();
    }

    private void NullChecks()
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
    }
}
