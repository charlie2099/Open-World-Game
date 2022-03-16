using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjLoader : MonoBehaviour
{
    [Serializable]
    public class TestChunk
    {
        public Object obj;
        public Vector3 vertices;
        public Vector3 indicies;
    }

    [SerializeField] private TestChunk testChunk;
    private string jsonFile;

    private void Start()
    {
        print("Name: " + testChunk.obj.name);
        jsonFile = JsonUtility.ToJson(testChunk);
        
        
        TestChunk loadedChunkData = JsonUtility.FromJson<TestChunk>(jsonFile);
        
        
       
        /*Vector3[] listOfVertices = {};
        listOfVertices[0].x = loadedChunkData.vertices.x;
        listOfVertices[0].y = loadedChunkData.vertices.y;
        listOfVertices[0].z = loadedChunkData.vertices.z;
        
        Vector3[] listOfIndices = {};
        listOfIndices[0].x = loadedChunkData.indicies.x;
        listOfIndices[0].y = loadedChunkData.indicies.y;
        listOfIndices[0].z = loadedChunkData.indicies.z;*/


        //GameObject chunk = new GameObject("Generated Chunk");

        

        //chunk.GetComponent<Mesh>() = testChunk.obj.

        //Mesh mesh = new Mesh();
        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = indices.ToArray();
        //mesh.RecalculateNormals();
        //chunkToLoad.sharedMesh = mesh;
        //chunkCollider.sharedMesh = mesh;



        
        
        
        
        
        
        /*string fileContents = File.ReadAllText(file);

        chunk_data = JsonUtility.FromJson<ChunkData>(fileContents);
        Vector3[] vertex_vec = null;
        for (int i = 0; i < chunk_data.vertecies.Length; i++)
        {
            vertex_vec[i].x = chunk_data.vertecies[0];
            vertex_vec[i].y = chunk_data.vertecies[1];
            vertex_vec[i].z = chunk_data.vertecies[2];

            chunk_to_load.GetComponent<MeshFilter>().sharedMesh.vertices[i] = vertex_vec[i];
        }*/

        
        
        
        
        
        
        

        /*string path = Application.streamingAssetsPath + "/" + obj.name + ".obj";

        StreamReader reader = new StreamReader(path);
        string line;
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        while ((line = reader.ReadLine()) != null)
        {
            string[] split = line.Split(' ');
            switch (split[0])
            {
                case "v": // Vertex
                    Vector3 vertex = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
                    vertices.Add(vertex);
                    break;
                case "f": // Index
                    List<int> theseIndexes = new List<int>();
                    for (int i = 1; i < split.Length; i++)
                    {
                        theseIndexes.Add(int.Parse(split[i].Split('/')[0]) - 1);
                    }
                    indices.AddRange(Triangulate(theseIndexes));
                    break;
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        chunkToLoad.sharedMesh = mesh;
        chunkCollider.sharedMesh = mesh;

        reader.Close();*/
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadChunk();
        }
    }

    private void LoadChunk()
    {
        
    }
}
