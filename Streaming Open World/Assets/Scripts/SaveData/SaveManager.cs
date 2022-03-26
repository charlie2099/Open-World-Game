using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Chilli.Ai;
using Chilli.Ai.Zombies;
using Chilli.Terrain;
using Chilli.Quests;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Saves chunk data, terrain data, and npc data to json files.
/// </summary>

public class SaveManager : MonoBehaviour
{
    public class ChunkData
    {
        // Chunk
        public string name;
        public Mesh chunkMesh;
        public Material material;
        public Vector3 position;

        // Chunk objects
        public string[] objectNames;
        public List<GameObject> objects; // don't write gameobjects to file?
        public Mesh[] objectMeshes;
        public Material[] objectMaterials;
        public Vector3[] objectPos;
        
        // Spawners
        public GameObject spawnerPrefab;
        
        // LOD
        public Mesh[] treeLODMeshes;
    }

    public class TerrainData
    {
        public int chunkSize;
        public int terrainWidth;
        public int terrainHeight;
    }

    public class NPCData
    {
        public string npcName;
        public Vector3 position;
        public string questName;
        public int rewardPoints;
        public Quest.QuestType questType;
        public Quest.QuestStatus questStatus;
    }

    private static string _jsonFile;

    public static void LoadChunk(GameObject chunk) 
    { 
        string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
        ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(path));
        chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.chunkMesh;
        chunk.AddComponent<MeshRenderer>().sharedMaterial = loadedData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.chunkMesh;
        //chunk.AddComponent<Chunk>();

        // Load chunk objects 
        for (int i = 0; i < loadedData.objects.Count; i++) // TODO: Clean this mess up!
        {
            GameObject newObj = new GameObject(loadedData.objectNames[i]);

            if (loadedData.objectNames[i] != "EnemySpawner(Clone)" && loadedData.objectNames[i] != "NPC_Father(Clone)")
            {
                newObj.AddComponent<MeshFilter>().sharedMesh       = loadedData.objectMeshes[i];
                newObj.AddComponent<MeshRenderer>().sharedMaterial = loadedData.objectMaterials[i];
            }

            if (loadedData.objectNames[i] == "EnemySpawner(Clone)")
            {
                newObj.AddComponent<EnemySpawner>().enemyPrefab = loadedData.spawnerPrefab;
            }

            if (loadedData.objectNames[i] == "Tree2(Clone)")
            {
                newObj.AddComponent<MeshCollider>().sharedMesh = loadedData.objectMeshes[i];
                newObj.AddComponent<LOD>();
                newObj.GetComponent<LOD>().lodMesh = new Mesh[3];
                for (int j = 0; j < newObj.GetComponent<LOD>().lodMesh.Length; j++)
                {
                    newObj.GetComponent<LOD>().lodMesh[j] = loadedData.treeLODMeshes[j];
                }
                newObj.GetComponent<LOD>().distanceLOD1   = 30;
                newObj.GetComponent<LOD>().distanceLOD2   = 50;
                newObj.GetComponent<LOD>().updateInterval = 2;
                newObj.transform.localScale = new Vector3(2, 2, 2);
            }
            
            if (loadedData.objectNames[i] == "NPC_Father(Clone)")
            {
                // Instantiate on generation 
                // On load - set enabled to true, load quest data and position from file
                // On Unload - set enabled to false, save quest data and position to file

                newObj.name = "DestroyThis";
                GameObject npc = Instantiate(QuestManager.instance.npcPrefab);
                npc.transform.parent = chunk.transform;
                                
                string npcDataDir = Application.dataPath + "/SaveData/NPCData/" + loadedData.objectNames[i] + i + ".json";  
                if (File.Exists(npcDataDir))
                {
                    NPCData loadedNpcData = JsonUtility.FromJson<NPCData>(File.ReadAllText(npcDataDir));
                    npc.name = loadedNpcData.npcName;
                    npc.transform.position = loadedNpcData.position;
                    npc.GetComponent<Quest>().GetQuestData().name = loadedNpcData.questName;
                    npc.GetComponent<Quest>().GetQuestData().rewardPoints = loadedNpcData.rewardPoints;
                    npc.GetComponent<Quest>().GetQuestData().questType = loadedNpcData.questType;
                    npc.GetComponent<Quest>().GetQuestData().questStatus = loadedNpcData.questStatus;
                    chunk.GetComponent<Chunk>().chunkObjects.Add(npc);
                }
                                
                Destroy(newObj);
            }

            if (loadedData.objectNames[i] != "NPC_Father(Clone)")
            {
                newObj.transform.position = loadedData.objectPos[i];
                newObj.transform.parent = chunk.transform;

                chunk.GetComponent<Chunk>().chunkObjects.Add(newObj);
            }
        }
        
        chunk.transform.position = loadedData.position;
        // bake nav mesh

        chunk.GetComponent<Chunk>().SetLoaded(true);
        chunk.tag = "TerrainChunk";
    }

    public static void UnloadChunk(GameObject chunk)
    {
        chunk.GetComponent<Chunk>().SetLoaded(false);

        // Get latest chunk data when it was unloaded
        ChunkData newChunkData = new ChunkData();
        newChunkData.material           = chunk.GetComponent<MeshRenderer>().sharedMaterial;
        newChunkData.chunkMesh          = chunk.GetComponent<MeshFilter>().sharedMesh;
        newChunkData.position           = chunk.transform.position;
        newChunkData.name               = chunk.name;
        
        // chunk objects
        newChunkData.objects            = chunk.GetComponent<Chunk>().chunkObjects;
        newChunkData.objectNames        = new string[newChunkData.objects.Count];
        newChunkData.objectPos          = new Vector3[newChunkData.objects.Count];
        newChunkData.objectMeshes       = new Mesh[newChunkData.objects.Count];
        newChunkData.objectMaterials    = new Material[newChunkData.objects.Count];
        newChunkData.treeLODMeshes      = new Mesh[3];

        // Loop through all chunk objects
        for (int i = 0; i < newChunkData.objects.Count; i++)
        {
            if (newChunkData.objectPos != null)
            {
                var chunkObj = chunk.GetComponent<Chunk>().chunkObjects[i];
                newChunkData.objectNames[i]     = chunkObj.name;
                newChunkData.objectPos[i]       = chunkObj.transform.position;

                if (chunkObj.GetComponent<MeshFilter>() != null)
                {
                    newChunkData.objectMeshes[i]    = chunkObj.GetComponent<MeshFilter>().sharedMesh;
                    newChunkData.objectMaterials[i] = chunkObj.GetComponent<MeshRenderer>().sharedMaterial;
                }

                // Spawners
                if (chunkObj.GetComponent<EnemySpawner>() != null)
                {
                    newChunkData.spawnerPrefab = chunkObj.GetComponent<EnemySpawner>().enemyPrefab;
                }
                
                // Objects with LOD components
                if (chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<LOD>() != null)
                {
                    for (int j = 0; j < chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<LOD>().lodMesh.Length; j++)
                    {
                        newChunkData.treeLODMeshes[j] = chunk.GetComponent<Chunk>().chunkObjects[i].GetComponent<LOD>().lodMesh[j];
                    }
                }
                
                // NPC data - Assumption that all NPCs have quests
                if (chunkObj.GetComponent<Quest>() != null)
                {
                    NPCData newNpcData = new NPCData();
                    newNpcData.npcName      = chunkObj.name;
                    newNpcData.position     = chunkObj.transform.position;
                    newNpcData.questName    = chunkObj.GetComponent<Quest>().GetQuestData().name;
                    newNpcData.rewardPoints = chunkObj.GetComponent<Quest>().GetQuestData().rewardPoints;
                    newNpcData.questType    = chunkObj.GetComponent<Quest>().GetQuestData().questType;
                    newNpcData.questStatus  = chunkObj.GetComponent<Quest>().GetQuestData().questStatus;
                    
                    Destroy(chunkObj);

                    // Converts new npc data to JSON form.
                    _jsonFile = JsonUtility.ToJson(newNpcData, true);

                    // Writes npcData to json file
                    string npcDataDir = Application.dataPath + "/SaveData/NPCData/" + newNpcData.npcName + i + ".json"; 
                    File.WriteAllText(npcDataDir, _jsonFile);
                }
            }
        }
        
        // Converts new chunk data to JSON form.
        _jsonFile = JsonUtility.ToJson(newChunkData, true);

        // Writes chunkData to json file
        string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
        File.WriteAllText(path, _jsonFile);

        // Remove it from the active chunks list and strip its components
        Destroy(chunk.GetComponent<MeshRenderer>());
        Destroy(chunk.GetComponent<MeshFilter>());
        Destroy(chunk.GetComponent<MeshCollider>());
        
        // Destroy chunk objects and clear the list
        foreach (var obj in chunk.GetComponent<Chunk>().chunkObjects.ToArray())
        {
            if (obj.name != "NPC_Father(Clone)")
            {
                Destroy(obj);
            }
        }
        chunk.GetComponent<Chunk>().chunkObjects.Clear();
        //Destroy(chunk.GetComponent<Chunk>());
        chunk.tag = "Untagged";
    }

    public static void SaveToFile() // Saves terrain, chunk, and Ai data to file
    {
        TerrainData newTerrainData = new TerrainData();
        newTerrainData.chunkSize     = TerrainGenerator.instance.GetChunkSize();
        newTerrainData.terrainWidth  = TerrainGenerator.instance.GetTerrainWidth();
        newTerrainData.terrainHeight = TerrainGenerator.instance.GetTerrainHeight();

        // Converts new terrain data to JSON form.
        _jsonFile = JsonUtility.ToJson(newTerrainData, true);

        // Writes terrainData to json file
        string terrainPath = Application.dataPath + "/SaveData/TerrainData/terrain.json";
        File.WriteAllText(terrainPath, _jsonFile);

        // Update chunk data when unloaded
        foreach (var chunk in TerrainGenerator.instance.GetChunks())
        {
            ChunkData newChunkData = new ChunkData();
            newChunkData.material           = chunk.GetComponent<MeshRenderer>().sharedMaterial;
            newChunkData.chunkMesh          = chunk.GetComponent<MeshFilter>().sharedMesh;
            newChunkData.position           = chunk.transform.position;
            newChunkData.name               = chunk.name;
            
            newChunkData.objects            = chunk.GetComponent<Chunk>().chunkObjects;
            newChunkData.objectNames        = new string[newChunkData.objects.Count];
            newChunkData.objectPos          = new Vector3[newChunkData.objects.Count];
            newChunkData.objectMeshes       = new Mesh[newChunkData.objects.Count];
            newChunkData.objectMaterials    = new Material[newChunkData.objects.Count];
            newChunkData.treeLODMeshes      = new Mesh[3];

            // All objects (trees, houses, spawners, npcs)
            for (int i = 0; i < newChunkData.objects.Count; i++)
            {
                if (newChunkData.objectPos != null)
                {
                    var chunkObj = chunk.GetComponent<Chunk>().chunkObjects[i];
                    newChunkData.objectNames[i]     = chunkObj.name;
                    newChunkData.objectPos[i]       = chunkObj.transform.position;

                    if (chunkObj.GetComponent<MeshFilter>() != null)
                    {
                        newChunkData.objectMeshes[i]    = chunkObj.GetComponent<MeshFilter>().sharedMesh;
                    }

                    if (chunkObj.GetComponent<MeshRenderer>() != null)
                    {
                        newChunkData.objectMaterials[i] = chunkObj.GetComponent<MeshRenderer>().sharedMaterial; 
                    }
                    
                    // Spawners
                    if (chunkObj.GetComponent<EnemySpawner>() != null)
                    {
                        newChunkData.spawnerPrefab = chunkObj.GetComponent<EnemySpawner>().enemyPrefab;
                    }

                    // Objects with LOD components
                    if (chunkObj.GetComponent<LOD>() != null)
                    {
                        for (int j = 0; j < chunkObj.GetComponent<LOD>().lodMesh.Length; j++)
                        {
                            newChunkData.treeLODMeshes[j] = chunkObj.GetComponent<LOD>().lodMesh[j];
                        }
                    }
                    
                    // NPC data - Assumption that all NPCs have quests
                    if (chunkObj.GetComponent<Quest>() != null)
                    {
                        NPCData newNpcData = new NPCData();
                        newNpcData.npcName      = chunkObj.name;
                        newNpcData.position     = chunkObj.transform.position;
                        newNpcData.questName    = chunkObj.GetComponent<Quest>().GetQuestData().name;
                        newNpcData.rewardPoints = chunkObj.GetComponent<Quest>().GetQuestData().rewardPoints;
                        newNpcData.questType    = chunkObj.GetComponent<Quest>().GetQuestData().questType;
                        newNpcData.questStatus  = chunkObj.GetComponent<Quest>().GetQuestData().questStatus;
                        
                        // Converts new npc data to JSON form.
                        _jsonFile = JsonUtility.ToJson(newNpcData, true);

                        // Writes npcData to json file
                        string npcDataDir = Application.dataPath + "/SaveData/NPCData/" + newNpcData.npcName + i + ".json";
                        File.WriteAllText(npcDataDir, _jsonFile);
                    }
                }
            }

            // Converts new chunk data to JSON form.
            _jsonFile = JsonUtility.ToJson(newChunkData, true);

            // Writes chunkData to json file
            string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
            File.WriteAllText(path, _jsonFile);
        }
        //print("<color=orange> Chunks unloaded </color>");
        //print("<color=orange> Chunk data written to file </color>");
        Debug.Log("<color=orange>Saving!</color>");
    }
}