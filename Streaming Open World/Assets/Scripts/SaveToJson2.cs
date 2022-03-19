using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveToJson2 : MonoBehaviour
{
    private class ChunkData
    {
        public string name;
        public Mesh mesh;
        public Material material;
        public Vector3 position;
    }
    
    private static string jsonFile;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadChunk();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UnloadChunk();
        }
    }

    private void LoadChunk()
    {
        // Read contents of json file

        for (int x = 0; x < 23; x++)
        {
            for (int z = 0; z < 23; z++)
            {
                ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(Application.dataPath + "/SaveData/ChunkData/" + "chunk " + x + " , " + z + ".json"));
                
                GameObject chunk = new GameObject(loadedData.name);
                Mesh mesh = new Mesh();

                chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.mesh;
                chunk.AddComponent<MeshRenderer>().material = loadedData.material;
                chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.mesh;
                chunk.AddComponent<Chunk>();

                chunk.transform.position = loadedData.position;

                mesh.Clear();
                mesh.vertices = loadedData.mesh.vertices;
                mesh.triangles = loadedData.mesh.triangles;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
    
                TerrainGenerator.GetChunks().Add(chunk);
            }
        }
        print("Terrain List: " + TerrainGenerator.GetChunks().Count);


        /*ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(Application.dataPath + "/SaveData/ChunkData/" + _chunk.name + ".json"));

        GameObject chunk = new GameObject(loadedData.name);
        Mesh mesh = new Mesh();

        chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.mesh;
        chunk.AddComponent<MeshRenderer>().material = loadedData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.mesh;
        chunk.AddComponent<Chunk>();

        chunk.transform.position = loadedData.position;

        mesh.Clear();
        mesh.vertices = loadedData.mesh.vertices;
        mesh.triangles = loadedData.mesh.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    
        activeChunk.Add(chunk);*/
        
        //print("<color=lime> Chunk 1 data loaded from file </color>");
    }

    private void UnloadChunk()
    {
        SaveToFile();

        foreach (var chunk in TerrainGenerator.GetChunks().ToArray())
        {
            TerrainGenerator.GetChunks().Remove(chunk);
            Destroy(chunk);
        }
    }

    public static void SaveToFile()
    {
        // Update chunk data when unloaded
        foreach (var chunk in TerrainGenerator.GetChunks())
        {
            ChunkData newChunkData = new ChunkData();
            newChunkData.material     = chunk.GetComponent<MeshRenderer>().material;
            newChunkData.mesh         = chunk.GetComponent<MeshFilter>().sharedMesh;
            newChunkData.position     = chunk.transform.position;
            newChunkData.name         = chunk.name;
            
            // Converts new chunk data to JSON form.
            jsonFile = JsonUtility.ToJson(newChunkData, true);

            // Writes chunkData to json file
            string path = Application.dataPath + "/SaveData/ChunkData/" + chunk.name + ".json";
            File.WriteAllText(path, jsonFile);
        }
        print("<color=orange> Chunks unloaded </color>");
        print("<color=orange> Chunk data written to file </color>");
    }
}












/*using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveToJson2 : MonoBehaviour
{
    private class ChunkData
    {
        //public TextAsset chunkData;
        public string name;
        public Mesh mesh;
        public Material material;
        public Vector3 position;
        //public Vector3[] vertices;
        //public int[] triangles;
        //public Vector2[] uvs;
    }
    
    private List<GameObject> activeChunk = new List<GameObject>();
    private string jsonFile;

    private void Start()
    {
        activeChunk.Add(TerrainGenerator.GetChunks()[0]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadChunk();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UnloadChunk();
        }
    }

    private void LoadChunk()
    {
        // Read contents of json file
        ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(Application.dataPath + "/chunkData.json"));

        GameObject chunk = new GameObject(loadedData.name);
        Mesh mesh = new Mesh();

        chunk.AddComponent<MeshFilter>().sharedMesh = loadedData.mesh;
        chunk.AddComponent<MeshRenderer>().material = loadedData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedData.mesh;
        chunk.AddComponent<Chunk>();

        chunk.transform.position = loadedData.position;

        mesh.Clear();
        mesh.vertices = loadedData.mesh.vertices;
        mesh.triangles = loadedData.mesh.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        activeChunk.Add(chunk);
        
        print("<color=lime> Chunk 1 data loaded from file </color>");
    }

    private void UnloadChunk()
    {
        // Update chunk data when unloaded
        ChunkData newChunkData = new ChunkData();
        newChunkData.material     = activeChunk[0].GetComponent<MeshRenderer>().material;
        newChunkData.mesh         = activeChunk[0].GetComponent<MeshFilter>().sharedMesh;
        //newChunkData.vertices     = activeChunk[0].GetComponent<MeshFilter>().sharedMesh.vertices;
        //newChunkData.triangles    = activeChunk[0].GetComponent<MeshFilter>().sharedMesh.triangles;
        //newChunkData.uvs          = activeChunk[0].GetComponent<MeshFilter>().sharedMesh.uv;
        newChunkData.position     = activeChunk[0].transform.position;
        newChunkData.name         = activeChunk[0].name;

        // Converts new chunk data to JSON form.
        jsonFile = JsonUtility.ToJson(newChunkData, true);
        
        // Writes chunkData to json file
        File.WriteAllText(Application.dataPath + "/chunkData.json", jsonFile);
        
        print("<color=orange> Chunk 1 unloaded </color>");
        print("<color=orange> Chunk 1 data written to file </color>");

        foreach (var chunk in activeChunk.ToArray())
        {
            activeChunk.Remove(chunk);
            Destroy(chunk);
        }
    }
}*/






/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveToJson2 : MonoBehaviour
{
    [Serializable]
    public class ChunkData
    {
        public GameObject gameObject;
        //public Mesh mesh;
        public Material material;
        [HideInInspector] public Vector3 position;
    }
    
    [SerializeField] private ChunkData chunkData;
    private List<GameObject> activeChunk = new List<GameObject>();
    private string jsonFile;

    private void Start()
    {
        // Converts serialised chunk data to JSON form.
        jsonFile = JsonUtility.ToJson(chunkData, true);
        
        GameObject chunk = new GameObject(chunkData.gameObject.name);
        Mesh mesh = new Mesh();
        
        //TerrainGenerator.CreateMesh(heightMap, chunkSize, multiplier,x * chunkSize, z * chunkSize);

        var loadedMeshFilter = chunkData.gameObject.GetComponent<MeshFilter>();
        chunk.AddComponent<MeshFilter>().sharedMesh = loadedMeshFilter.sharedMesh;
        chunk.AddComponent<MeshRenderer>().material = chunkData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedMeshFilter.sharedMesh;
        chunk.AddComponent<Chunk>();

        chunk.transform.position = chunkData.gameObject.transform.position;
        print("Initial loaded chunk pos from serialized inspector data: " + chunkData.gameObject.transform.position);

        mesh.Clear();
        mesh.vertices = loadedMeshFilter.sharedMesh.vertices;
        mesh.triangles = loadedMeshFilter.sharedMesh.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        activeChunk.Add(chunk);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadChunk();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            UnloadChunk();
        }
    }

    private void LoadChunk()
    {
        // Read contents of json file
        ChunkData loadedData = JsonUtility.FromJson<ChunkData>(File.ReadAllText(Application.dataPath + "/chunkData.json"));
        print("Loaded Material Data:" + loadedData.material);
        print("Loaded Position Data:" + loadedData.position);
        print("Loaded Gameobject Data:" + loadedData.gameObject);

        GameObject chunk = new GameObject(loadedData.gameObject.name);
        Mesh mesh = new Mesh();
        
        //TerrainGenerator.CreateMesh(heightMap, chunkSize, multiplier,x * chunkSize, z * chunkSize);

        var loadedMeshFilter = loadedData.gameObject.GetComponent<MeshFilter>();
        chunk.AddComponent<MeshFilter>().sharedMesh = loadedMeshFilter.sharedMesh;
        chunk.AddComponent<MeshRenderer>().material = loadedData.material;
        chunk.AddComponent<MeshCollider>().sharedMesh = loadedMeshFilter.sharedMesh;
        chunk.AddComponent<Chunk>();

        chunk.transform.position = loadedData.gameObject.transform.position;
        print("Loaded chunk pos: " + loadedData.gameObject.transform.position);

        mesh.Clear();
        mesh.vertices = loadedMeshFilter.sharedMesh.vertices;
        mesh.triangles = loadedMeshFilter.sharedMesh.triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        activeChunk.Add(chunk);
    }

    private void UnloadChunk()
    {
        // Update chunk data when unloaded
        ChunkData newChunkData = new ChunkData();
        newChunkData.material = activeChunk[0].GetComponent<MeshRenderer>().material;
        newChunkData.gameObject = activeChunk[0];
        newChunkData.position = activeChunk[0].transform.position;
        
        // Converts new chunk data to JSON form.
        jsonFile = JsonUtility.ToJson(newChunkData, true);
        
        // Writes chunkData to json file
        File.WriteAllText(Application.dataPath + "/chunkData.json", jsonFile);
        
        print("Chunk pos written to file: " + newChunkData.position);

        foreach (var chunk in activeChunk.ToArray())
        {
            activeChunk.Remove(chunk);
            Destroy(chunk);
        }
    }
}
*/
