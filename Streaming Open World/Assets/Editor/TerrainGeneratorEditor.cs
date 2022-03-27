using System;
using System.IO;
using Codice.Client.BaseCommands;
using Chilli.Terrain;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Generates a chunked terrain using the editor window. Chunk data is saved to json files.
/// When a new terrain is generated the previous one will be overwritten.
/// </summary>

public class TerrainGeneratorEditor : EditorWindow
{
    // Terrain
    private Texture2D _heightMap;
    private Material _material;
    private int _chunkSize     = 64;
    private int _terrainWidth  = 750;
    private int _terrainHeight = 100;

    // Terrain Objects
    private static bool _activeAssigner;
    private static bool _activeDeassigner;
    private GameObject _objectToPaint;

    [MenuItem("My Tools/TerrainGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TerrainGeneratorEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Terrain Customisation", EditorStyles.boldLabel);
        _heightMap     = EditorGUILayout.ObjectField("Heightmap", _heightMap, typeof(Texture2D), false) as Texture2D;
        _material      = EditorGUILayout.ObjectField("Material", _material, typeof(Material), false) as Material;
        _terrainWidth  = EditorGUILayout.IntSlider("Terrain Width", _terrainWidth, 0, 1024);
        _terrainHeight = EditorGUILayout.IntSlider("Terrain Height", _terrainHeight, 0, 1024);
        _chunkSize     = EditorGUILayout.IntField("Chunk Size", _chunkSize);

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
            SaveToFile();
        }
        
        GUILayout.Label("Nav Mesh", EditorStyles.boldLabel);
        if (GUILayout.Button("Bake nav mesh", buttonStyle))
        {
            NavMeshGenerator.GenerateNavMesh();
        }
        
        else if (GUILayout.Button("Clear nav mesh data", buttonStyle))
        { 
            NavMeshGenerator.ClearNavMeshData();
        }

        GUILayout.Label("Destructional", EditorStyles.boldLabel);
        if (GUILayout.Button("Destroy active terrain", buttonStyle))
        {
            DestroyActiveTerrain();
        }
        
        else if (GUILayout.Button("Delete existing saved data", buttonStyle))
        {
            DeleteSavedData();
        }
        
        GUILayout.Label("Chunk Object Spawner", EditorStyles.boldLabel);
        GUILayout.Space(10);
        _objectToPaint = EditorGUILayout.ObjectField("Object To Spawn", _objectToPaint, typeof(GameObject), false) as GameObject;
        
        if (GUILayout.Button("Spawner: " + _activeAssigner, buttonStyle))
        {
            _activeAssigner = !_activeAssigner;
        }
        
        else if (GUILayout.Button("Despawner: " + _activeDeassigner, buttonStyle))
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
                    if (hit.collider.gameObject.GetComponent<Chunk>() != null)
                    {
                        GameObject spawnedObj = Instantiate(_objectToPaint, hit.point, Quaternion.identity);
                        hit.collider.gameObject.GetComponent<Chunk>().chunkObjects.Add(spawnedObj);
                        spawnedObj.transform.parent = hit.transform;
                    }
                }

                else if (_activeDeassigner)
                {
                    if (hit.collider.gameObject.GetComponent<Chunk>() != null)
                    {
                        var chunkObj = hit.collider.gameObject.GetComponent<Chunk>().chunkObjects;
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
        if (TerrainGenerator.instance == null)
        {
            if (GameObject.Find("Generator") == null)
            {
                Debug.LogError("<color=red> Error: Could not find the gameobject called 'Generator', so the TerrainGenerator component could not be found </color>");
                return;
            }
            TerrainGenerator.instance = GameObject.Find("Generator").GetComponent<TerrainGenerator>();
        }
        
        // Check that terrain data exists
        string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
        if (!File.Exists(terrainPath))
        {
            Debug.LogError("<color=red> Error: Could not find any existing terrain data from file </color>");
            return;
        }

        // Check that chunk data exists
        string chunkPath = Application.dataPath + "/SaveData/ChunkData/";
        var dir = Directory.GetFiles(chunkPath);
        foreach (var chunkFile in dir)
        {
            if (!File.Exists(chunkFile))
            {
                Debug.LogError("<color=red> Error: Could not find any chunk terrain data from file </color>");
                return;
            }
        }

        var loadedHeightMap = TerrainGenerator.instance.GetHeightMap();
        var loadedMaterial = TerrainGenerator.instance.GetMaterial();
        TerrainGenerator.instance.GenerateMap(loadedHeightMap, loadedMaterial, _chunkSize, _terrainWidth, _terrainHeight, true);
        Debug.Log("<color=cyan> A terrain has been loaded from file! </color>");
    }

    private void GenerateNewTerrain()
    {
        if (!NullChecks())
        {
            return;
        }

        // if no terrain exists in the scene already, generate a new terrain
        if (TerrainGenerator.instance.GetChunks().Count <= 0)
        {
            TerrainGenerator.instance.GenerateMap(_heightMap, _material, _chunkSize, _terrainWidth, _terrainHeight, false);
            Debug.Log("<color=lime> A new terrain has been generated! </color>");
        }
        else // if a terrain exists in the scene clear chunk list and regenerate the terrain
        {
            DestroyActiveTerrain();
            TerrainGenerator.instance.GenerateMap(_heightMap, _material, _chunkSize, _terrainWidth, _terrainHeight, false);
            Debug.Log("<color=green> Existing terrain has been regenerated! </color>");
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
        var chunkDir = Directory.GetFiles(chunkPath);
        foreach (var chunkFile in chunkDir)
        {
            File.Delete(chunkFile);
        } 
        
        string npcPath = Application.dataPath + "/SaveData/NPCData/";
        var npcDir = Directory.GetFiles(npcPath);
        foreach (var npcFile in npcDir)
        {
            File.Delete(npcFile);
        }
    }

    private void DestroyActiveTerrain()
    {
        if (GameObject.FindWithTag("MainTerrain") == null)
        {
            Debug.LogError("<color=red> Error: There is no active terrain in the scene, OR it is not tagged with 'MainTerrain'! </color>");
            return;
        }
        
        // if a terrain exists destroy it's container 
        foreach (var gObj in FindObjectsOfType<GameObject>())
        {
            if (gObj.CompareTag("MainTerrain"))
            {
                DestroyImmediate(gObj);
            }
        }

        TerrainGenerator.instance.GetChunks().Clear();
        Debug.Log("<color=orange> Active terrain has been destroyed! </color>");
    }

    private void SaveToFile()
    {
        if (TerrainGenerator.instance.GetChunks() == null)
        {
            Debug.LogError("<color=red> Error: No existing data to save");
            return;
        }
            
        SaveManager.SaveToFile();
    }

    private bool NullChecks()
    {
        if (TerrainGenerator.instance == null)
        {
            if (GameObject.Find("Generator") == null)
            {
                Debug.LogError("<color=red> Error: Could not find the gameobject called 'Generator', so the TerrainGenerator component could not be found </color>");
                return false;
            }
            TerrainGenerator.instance = GameObject.Find("Generator").GetComponent<TerrainGenerator>();
        }

        if (_heightMap == null)
        {
            Debug.LogError("<color=red> Error: Please assign a heightmap to be used! </color>");
            return false;
        }

        if (_material == null)
        {
            Debug.LogError("<color=red> Error: Please assign a material to be used! </color>");
            return false;
        }
        
        if (_terrainWidth < _chunkSize)
        {
            Debug.LogError("<color=red> Error: Terrain width should be larger than the chunk size! </color>");
            return false;
        }

        return true;
    }
}
