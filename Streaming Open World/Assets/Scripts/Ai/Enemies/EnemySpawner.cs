using UnityEngine;

namespace Chilli.Ai
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject enemyPrefab;
        [SerializeField] private float spawnRate = 20.0f;

        private void Start()
        {
            InvokeRepeating(nameof(SpawnEnemy), 0, spawnRate);
        }

        private void SpawnEnemy()
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            enemy.transform.parent = transform.parent;
            transform.parent.GetComponent<Chunk>().chunkObjects.Add(enemy);
        }
    }
}
