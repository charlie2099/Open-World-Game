using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chilli.Terrain
{
    /// <summary>
    /// Generates a chunked terrain and reads chunk data from file should it exist.
    /// </summary>

    public class TerrainGenerator : MonoBehaviour
    {
        // Terrain
        [SerializeField] private Texture2D heightmap;
        [SerializeField] private Material serialisedChunkMaterial;
        [SerializeField] private int serialisedChunkSize;
        [SerializeField] private int serialisedTerrainWidth;
        [SerializeField] private int serialisedTerrainHeight;
    
        private static int _numberOfChunks;
        private static int _totalChunks;
        private static int _chunkSize;
        private static int _terrainWidth;
        private static int _terrainHeight;
        private static GameObject _container;
        private static List<GameObject> _generatedChunks = new List<GameObject>();
        private static Mesh _mesh;
        private static Vector3[] _vertices;
        private static int[] _triangles;
    
        // Terrain Objects
        private static ObjectGenerator _objectGenerator;

        private void Awake()
        {
            GenerateMap(heightmap, serialisedChunkMaterial, serialisedChunkSize, serialisedTerrainWidth, serialisedTerrainHeight);
        }

        public static void GenerateMap(Texture2D heightMap, Material terrainMaterial, int chunkSize, int terrainWidth, int terrainHeight)
        {
            // Create chunk container 
            _container = new GameObject("Terrain");
        
            // Remove previously existing terrain from list
            foreach (var chunk in _generatedChunks.ToArray())
            {
                _generatedChunks.Remove(chunk);
            }
        
            // Read terrain related data from file should it exist
            string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
            if (File.Exists(terrainPath))
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

            _numberOfChunks = terrainWidth / chunkSize;
            _totalChunks = _numberOfChunks * _numberOfChunks;

            for (int x = 0; x < _numberOfChunks; x++)
            {
                for (int z = 0; z < _numberOfChunks; z++)
                {
                    GameObject chunk = new GameObject("chunk " + x + " , " + z);
                    chunk.tag = "TerrainChunk";
                
                    _mesh = new Mesh();
                    Material testMaterial = new Material(terrainMaterial);

                    CreateMesh(heightMap, chunkSize, terrainHeight,x * chunkSize, z * chunkSize);
                
                    chunk.AddComponent<MeshFilter>().sharedMesh = _mesh;
                    chunk.AddComponent<MeshRenderer>().sharedMaterial = testMaterial;
                    chunk.AddComponent<MeshCollider>().sharedMesh = _mesh;
                    chunk.AddComponent<Chunk>().isLoaded = true;
                
                    string chunkPath = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
                    if (File.Exists(chunkPath))
                    {
                        SaveManager.ChunkData loadedData = JsonUtility.FromJson<SaveManager.ChunkData>(File.ReadAllText(chunkPath));
                        chunk.GetComponent<MeshRenderer>().sharedMaterial = testMaterial;
                        chunk.transform.position = loadedData.position;
                    
                        //print("loaded chunk (" + x + " , " + z + ") obj count: " + loadedData.objects.Count);
                    
                        // Load terrain objects in each chunk 
                        for (int i = 0; i < loadedData.objects.Count; i++)
                        {
                            GameObject chunkObj = new GameObject(loadedData.objectNames[i]);

                            if (loadedData.objectNames[i] != "EnemySpawner(Clone)")
                            {
                                chunkObj.AddComponent<MeshFilter>().sharedMesh       = loadedData.objectMeshes[i];
                                chunkObj.AddComponent<MeshRenderer>().sharedMaterial = loadedData.objectMaterials[i];
                            }

                            if (loadedData.objectNames[i] == "EnemySpawner(Clone)")
                            {
                                chunkObj.AddComponent<EnemySpawner>().enemyPrefab = loadedData.spawnerPrefab;
                            }

                            if (loadedData.objectNames[i] == "Tree2(Clone)")
                            {
                                chunkObj.AddComponent<MeshCollider>().sharedMesh = loadedData.objectMeshes[i];
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
                        
                            chunkObj.transform.position = loadedData.objectPos[i];
                            chunkObj.transform.parent = chunk.transform;
                            chunk.GetComponent<Chunk>().chunkObjects.Add(chunkObj);
                        }
                    }
                    else
                    {
                        chunk.transform.position = new Vector3(x * chunkSize, 0, z * chunkSize);
                    }

                    UpdateMesh();

                    chunk.transform.parent = _container.transform;
                    _container.tag = "MainTerrain";
                    _generatedChunks.Add(chunk);
                }
            }
            SaveManager.SaveToFile();
        }


        // Creates mesh for a single chunk
        public static void CreateMesh(Texture2D heightMap, int chunkSize, int multiplier, int offsetX, int offsetZ)
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

        private static void UpdateMesh()
        {
            _mesh.Clear();
            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        public static List<GameObject> GetChunks()
        {
            return _generatedChunks;
        }

        public static int GetChunkSize()
        {
            return _chunkSize;
        }

        public static int GetTerrainWidth()
        {
            return _terrainWidth;
        }
    
        public static int GetTerrainHeight()
        {
            return _terrainHeight;
        }
    
    }
}



