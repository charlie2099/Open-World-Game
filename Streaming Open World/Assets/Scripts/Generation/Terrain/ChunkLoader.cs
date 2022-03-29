using UnityEngine;

namespace Chilli.Terrain
{
    /// <summary>
    /// Handles loading and unloading of chunks based on the distance between the player and
    /// the central position of each chunk
    /// </summary>

    public class ChunkLoader : MonoBehaviour
    {
        [SerializeField] private int maxChunkLoadDistance = 400;
        [SerializeField] private float checkRate = 2.0f;
        private Transform _player;

        private void Start()
        {
            _player = transform;
            InvokeRepeating(nameof(CheckChunkDistance), 0.0f, checkRate);
        }

        private void CheckChunkDistance()
        {
            if (_player != null)
            {
                // Player yPos is set to zero so that no matter high/low the player is in the world, it's always the same distance
                // it's calculating i.e. more chunks won't unload if the player is really high up
                var playerPos = _player.position; playerPos.y = 0;

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
                        }
                    }
            
                    // If distance between player and center of chunk is less than maxViewDistance AND chunk is unloaded, load it
                    if (Vector3.Distance(playerPos, chunkCenterPos) < maxChunkLoadDistance)
                    {
                        if (!chunk.GetComponent<Chunk>().IsLoaded())
                        {
                            SaveManager.LoadChunk(chunk);
                        }
                    }
                }
            }
        }
    }
}
