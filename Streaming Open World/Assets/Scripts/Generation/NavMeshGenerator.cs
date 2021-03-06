using System.Collections;
using System.Collections.Generic;
using Chilli.Terrain;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    private NavMeshSurface[] surfaces;

    public static void GenerateNavMesh()
    {
        if (TerrainGenerator.instance.GetContainer() != null)
        {
            foreach (var chunk in TerrainGenerator.instance.GetChunks())
            {
                // Add a NavMeshSurface component to the chunk if it doesn't already have one 
                if (chunk.GetComponent<NavMeshSurface>() == null)
                {
                    chunk.AddComponent<NavMeshSurface>().collectObjects = CollectObjects.Volume;
                }
                
                var chunkNavSurface = chunk.GetComponent<NavMeshSurface>();
              
                // Build a nav mesh for the loaded chunk if doesn't already exist
                if (chunkNavSurface.navMeshData == null)
                {
                    print("Building nav mesh");
                    chunkNavSurface.size = new Vector3(64.3f, 29.41867f, 64.48733f);
                    chunkNavSurface.center = new Vector3(32, 47.70573f, 31.84137f);
                    chunkNavSurface.BuildNavMesh();
                }
                // Update the nav mesh for the loaded chunk if it already has a nav mesh
                else
                {
                    print("Updating nav mesh");
                    chunkNavSurface.UpdateNavMesh(chunkNavSurface.navMeshData);
                }
            }
        }
    }

    public static void ClearNavMeshData()
    {
        if (TerrainGenerator.instance.GetContainer() != null)
        {
            foreach (var chunk in TerrainGenerator.instance.GetChunks())
            {
                if (chunk.GetComponent<NavMeshSurface>() != null)
                {
                    print("Removing nav mesh data");
                    chunk.GetComponent<NavMeshSurface>().RemoveData();
                    chunk.GetComponent<NavMeshSurface>().navMeshData = null;
                }
            }
        }
    }
}
