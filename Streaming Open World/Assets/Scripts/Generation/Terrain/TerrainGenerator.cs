using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chilli.Ai;
using Chilli.Quests;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

namespace Chilli.Terrain
{
    /// <summary>
    /// Generates a chunked terrain and reads chunk data from file should it exist.
    /// </summary>

    public class TerrainGenerator : MonoBehaviour
    {
        public static TerrainGenerator instance;
        
        // Terrain
        [SerializeField] private Texture2D heightmap;
        [SerializeField] private Material serialisedChunkMaterial;
        [SerializeField] private int serialisedChunkSize;
        [SerializeField] private int serialisedTerrainWidth;
        [SerializeField] private int serialisedTerrainHeight;
    
        private int _rowLength;
        private int _totalChunks;
        private int _chunkSize;
        private int _terrainWidth;
        private int _terrainHeight;
        private GameObject _container;
        private List<GameObject> _generatedChunks = new List<GameObject>();
        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _triangles;
    
        // Terrain Objects
        //private static ObjectGenerator _objectGenerator;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            // Destroy any existing terrain in scene on play
            foreach (var gObj in FindObjectsOfType<GameObject>())
            {
                if (gObj.CompareTag("MainTerrain"))
                {
                    DestroyImmediate(gObj);
                }
            }
            _generatedChunks.Clear();

            // Loads a terrain from file on start
            GenerateMap(heightmap, serialisedChunkMaterial, serialisedChunkSize, serialisedTerrainWidth, serialisedTerrainHeight, true);
        }

        public void GenerateMap(Texture2D heightMap, Material terrainMaterial, int chunkSize, int terrainWidth, int terrainHeight, bool readFromFile)
        {
            // Create chunk container 
            _container = new GameObject("Terrain ");
        
            // Remove previously existing terrain from list
            foreach (var chunk in _generatedChunks.ToArray())
            {
                _generatedChunks.Remove(chunk);
            }
        
            // Read terrain related data from file should it exist
            string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
            if (File.Exists(terrainPath) && readFromFile)
            {
                SaveManager.TerrainData loadedData = JsonUtility.FromJson<SaveManager.TerrainData>(File.ReadAllText(terrainPath));
                chunkSize     = loadedData.chunkSize;
                terrainWidth  = loadedData.terrainWidth;
                terrainHeight = loadedData.terrainHeight;
            }
        
            // getter data
            _chunkSize = chunkSize;
            _terrainWidth = terrainWidth;
            _terrainHeight = terrainHeight;

            _rowLength = terrainWidth / chunkSize;
            _totalChunks = _rowLength * _rowLength;

            for (int x = 0; x < _rowLength; x++)
            {
                for (int z = 0; z < _rowLength; z++)
                {
                    GameObject chunk = new GameObject("chunk " + x + " , " + z);
                    chunk.tag = "TerrainChunk";
                
                    _mesh = new Mesh();
                    Material testMaterial = new Material(terrainMaterial);

                    CreateMesh(heightMap, chunkSize, terrainHeight,x * chunkSize, z * chunkSize);
                
                    chunk.AddComponent<MeshFilter>().sharedMesh = _mesh;
                    chunk.AddComponent<MeshRenderer>().sharedMaterial = testMaterial;
                    chunk.AddComponent<MeshCollider>().sharedMesh = _mesh;
                    chunk.AddComponent<Chunk>().SetLoaded(true);
                    chunk.AddComponent<NavMeshSurface>().collectObjects = CollectObjects.Volume;

                    string chunkPath = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
                    if (File.Exists(chunkPath) && readFromFile)
                    {
                        SaveManager.ChunkData loadedData = JsonUtility.FromJson<SaveManager.ChunkData>(File.ReadAllText(chunkPath));
                        chunk.GetComponent<MeshRenderer>().sharedMaterial = testMaterial;
                        chunk.transform.position = loadedData.position;

                        // Load terrain objects in each chunk 
                        for (int i = 0; i < loadedData.objects.Count; i++)
                        {
                            GameObject chunkObj = new GameObject(loadedData.objectNames[i]);

                            // Not a great method of handling this...
                            if (loadedData.objectNames[i] != "EnemySpawner(Clone)" && loadedData.objectNames[i] != "NPC_Father(Clone)")
                            {
                                print("hello");
                                chunkObj.AddComponent<MeshFilter>().mesh       = loadedData.objectMeshes[i];
                                chunkObj.AddComponent<MeshRenderer>().material = loadedData.objectMaterials[i];
                            }

                            if (loadedData.objectNames[i] == "EnemySpawner(Clone)")
                            {
                                chunkObj.AddComponent<EnemySpawner>().prefabCatalogueSo = loadedData.spawnerScriptableObject;
                            }

                            if (loadedData.objectNames[i] == "Tree2(Clone)")
                            {
                                chunkObj.AddComponent<MeshCollider>().sharedMesh = chunkObj.GetComponent<MeshFilter>().sharedMesh; /*loadedData.objectMeshes[i]*/;
                                chunkObj.AddComponent<LOD>();
                                chunkObj.GetComponent<LOD>().lodMesh = new Mesh[3];
                                for (int j = 0; j < chunkObj.GetComponent<LOD>().lodMesh.Length; j++)
                                {
                                    chunkObj.GetComponent<LOD>().lodMesh[j] = loadedData.treeLODMeshes[j];
                                }
                                chunkObj.GetComponent<LOD>().distanceLOD1   = 30;
                                chunkObj.GetComponent<LOD>().distanceLOD2   = 50;
                                chunkObj.GetComponent<LOD>().updateInterval = 2;
                                chunkObj.transform.localScale = new Vector3(2, 2, 2);
                            }

                            if (loadedData.objectNames[i] == "NPC_Father(Clone)")
                            {
                                // Instantiate on generation 
                                // On load - set enabled to true, load quest data and position from file
                                // On Unload - set enabled to false, save quest data and position to file

                                chunkObj.name = "DestroyThis";
                                if (QuestManager.instance == null)
                                {
                                    QuestManager.instance = GameObject.FindWithTag("Player").transform.parent.GetComponent<QuestManager>();
                                }
                                
                                GameObject npc = Instantiate(QuestManager.instance.prefabCatalogueSo.prefab[1]);
                                npc.transform.parent = chunk.transform;
                                
                                string npcDataDir = Application.dataPath + "/SaveData/NPCData/" + loadedData.objectNames[i] + i + ".json"; 
                                if (File.Exists(npcDataDir)) 
                                {
                                    SaveManager.NPCData loadedNpcData = JsonUtility.FromJson<SaveManager.NPCData>(File.ReadAllText(npcDataDir));
                                    npc.name = loadedNpcData.npcName;
                                    npc.transform.position = loadedNpcData.position;
                                    npc.GetComponent<Quest>().GetQuestData().name = loadedNpcData.questName;
                                    npc.GetComponent<Quest>().GetQuestData().rewardPoints = loadedNpcData.rewardPoints;
                                    npc.GetComponent<Quest>().GetQuestData().questType = loadedNpcData.questType;
                                    npc.GetComponent<Quest>().GetQuestData().questStatus = loadedNpcData.questStatus;
                                    chunk.GetComponent<Chunk>().chunkObjects.Add(npc);
                                }
                                
                                DestroyImmediate(chunkObj);
                            }

                            if (loadedData.objectNames[i] != "NPC_Father(Clone)")
                            {
                                chunkObj.transform.position = loadedData.objectPos[i];
                                chunkObj.transform.parent = chunk.transform;
                                chunk.GetComponent<Chunk>().chunkObjects.Add(chunkObj);
                            }
                        }
                    }
                    else
                    {
                        chunk.transform.position = new Vector3(x * chunkSize, 0, z * chunkSize);
                    }

                    UpdateMesh();
                    var chunkNavSurface = chunk.GetComponent<NavMeshSurface>();
                    chunkNavSurface.size = new Vector3(64.3f, 29.41867f, 64.14941f);
                    chunkNavSurface.center = new Vector3(32, 47.70573f, 32.0011f);
                    chunkNavSurface.BuildNavMesh();

                    chunk.transform.parent = _container.transform;
                    _container.tag = "MainTerrain";
                    _generatedChunks.Add(chunk);
                }
            }
            
            _container.name = "Terrain [chunks: " + _totalChunks + "] [" + _rowLength + "x" + _rowLength + "] [chunk size: " + chunkSize + "]"; 
            //SaveManager.SaveToFile();
        }


        // Creates mesh for a single chunk
        public void CreateMesh(Texture2D heightMap, int chunkSize, int multiplier, int offsetX, int offsetZ)
        {
            _vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];

            for (int i = 0, z = 0; z <= chunkSize; z++)
            {
                for (int x = 0; x <= chunkSize; x++)
                {
                    _vertices[i] = new Vector3(x, heightMap.GetPixel(x + offsetX, z + offsetZ).r * multiplier, z);
                    i++;
                }
            }

            _triangles = new int[chunkSize * chunkSize * 6];
            int vertexIndex = 0;
            int trianglesIndex = 0;

            for (int z = 0; z < chunkSize; z++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    _triangles[trianglesIndex + 0] = vertexIndex + 0;
                    _triangles[trianglesIndex + 1] = vertexIndex + chunkSize + 1;
                    _triangles[trianglesIndex + 2] = vertexIndex + 1;
                    _triangles[trianglesIndex + 3] = vertexIndex + 1;
                    _triangles[trianglesIndex + 4] = vertexIndex + chunkSize + 1;
                    _triangles[trianglesIndex + 5] = vertexIndex + chunkSize + 2;

                    vertexIndex++;
                    trianglesIndex += 6;
                }
                vertexIndex++;
            }
        }

        private void UpdateMesh()
        {
            _mesh.Clear();
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        public GameObject GetContainer()
        {
            return _container;
        }

        public List<GameObject> GetChunks()
        {
            return _generatedChunks;
        }

        public int GetChunkSize()
        {
            return _chunkSize;
        }

        public int GetTerrainWidth()
        {
            return _terrainWidth;
        }
    
        public int GetTerrainHeight()
        {
            return _terrainHeight;
        }

        public Texture2D GetHeightMap()
        {
            return heightmap;
        }

        public Material GetMaterial()
        {
            return serialisedChunkMaterial;
        }
    }
}



