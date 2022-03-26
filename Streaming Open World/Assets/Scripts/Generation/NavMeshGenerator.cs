using System.Collections;
using System.Collections.Generic;
using Chilli.Terrain;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    private NavMeshSurface[] surfaces;

    void Start ()
    {
        //StartCoroutine(WaitForTerrainToGenerate());
    }

    public static void GenerateNavMesh()
    {
        if (TerrainGenerator.instance.GetContainer() != null)
        {
            foreach (var chunk in TerrainGenerator.instance.GetChunks())
            {
                // Add a NavMeshSurface component to the chunk if it doesn't already have one 
                if (chunk.GetComponent<NavMeshSurface>() == null)
                {
                    chunk.AddComponent<NavMeshSurface>();
                }
                
                var chunkNavSurface = chunk.GetComponent<NavMeshSurface>();
                if (chunk.GetComponent<Chunk>().IsLoaded())
                {
                    // Build a nav mesh for the loaded chunk if doesn't already exist
                    if (chunkNavSurface.navMeshData == null)
                    {
                        chunkNavSurface.BuildNavMesh();
                    }
                    // Update the nav mesh for the loaded chunk if it already has a nav mesh
                    else
                    {
                        chunkNavSurface.UpdateNavMesh(chunkNavSurface.navMeshData);
                    }
                }
            }
            
            /*for (int i = 0; i < terrain.transform.childCount; i++)
            {
                terrain.transform.GetChild(i).AddComponent<NavMeshSurface>();

                if (terrain.transform.GetChild(i).GetComponent<Chunk>().isLoaded)
                {
                    terrain.transform.GetChild(i).GetComponent<NavMeshSurface>().BuildNavMesh();
                }
            }*/
        }
    }

    /*private IEnumerator WaitForTerrainToGenerate()
    {
        yield return new WaitForSeconds(1.0f);
        terrain = GameObject.FindWithTag("MainTerrain");

        for (int i = 0; i < terrain.transform.childCount; i++)
        {
            terrain.transform.GetChild(i).AddComponent<NavMeshSurface>();

            if (terrain.transform.GetChild(i).GetComponent<Chunk>().isLoaded)
            {
                yield return new WaitForSeconds(2.5f);
                terrain.transform.GetChild(i).GetComponent<NavMeshSurface>().BuildNavMesh();
            }
        }
    }*/
}
