using UnityEngine;
using UnityEngine.AI;

namespace Chilli.Terrain
{
    /// <summary>
    /// Handles loading and unloading of chunks based on the distance between the player and
    /// the central position of each chunk
    /// </summary>

    public class ChunkLoader : MonoBehaviour
    {
        [SerializeField] private Transform activeOceanChunks;
        [SerializeField] private int maxChunkLoadDistance = 400;
        [SerializeField] private float checkRate = 2.0f;
        private Transform player;

        private void Start()
        {
            player = transform;
            InvokeRepeating(nameof(CheckChunkDistance), 0.0f, checkRate);
        }

        private void CheckChunkDistance()
        {
            if (player != null)
            {
                // Player yPos is set to zero so that no matter high/low the player is in the world, it's always the same distance
                // it's calculating i.e. more chunks won't unload if the player is really high up
                var playerPos = player.position;
                playerPos.y = 0;

                foreach (var chunk in TerrainGenerator.instance.GetChunks().ToArray())
                {
                    // Center pos of chunk
                    Vector3 chunkCenterPos = chunk.transform.position + new Vector3(TerrainGenerator.instance.GetChunkSize() / 2.0f, 0, TerrainGenerator.instance.GetChunkSize() / 2.0f);
            
                    // If distance between player and center of chunk is more than maxViewDistance AND chunk is loaded, unload it
                    if (Vector3.Distance(playerPos, chunkCenterPos) > maxChunkLoadDistance) 
                    {
                        if (chunk.GetComponent<Chunk>().IsLoaded())
                        {
                            SaveManager.UnloadChunk(chunk);
                            chunk.GetComponent<NavMeshSurface>().RemoveData();
                            chunk.GetComponent<NavMeshSurface>().navMeshData = null;
                        }
                    }
            
                    // If distance between player and center of chunk is less than maxViewDistance AND chunk is unloaded, load it
                    if (Vector3.Distance(playerPos, chunkCenterPos) < maxChunkLoadDistance)
                    {
                        if (!chunk.GetComponent<Chunk>().IsLoaded())
                        {
                            SaveManager.LoadChunk(chunk);
                            chunk.AddComponent<NavMeshSurface>().collectObjects = CollectObjects.Volume;
                            var chunkNavSurface = chunk.GetComponent<NavMeshSurface>();
                            chunkNavSurface.size = new Vector3(64.3f, 29.41867f, 64.48733f);
                            chunkNavSurface.center = new Vector3(32, 47.70573f, 31.84137f);
                            chunkNavSurface.BuildNavMesh();
                        }
                    }
                }
        
                // Ocean Terrain
                for (int i = 0; i < activeOceanChunks.childCount; i++)
                {
                    if (Vector3.Distance(playerPos, activeOceanChunks.GetChild(i).position) > maxChunkLoadDistance)
                    {
                        activeOceanChunks.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        activeOceanChunks.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
