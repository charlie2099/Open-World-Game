using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Chilli.Ai
{
    public class EnemySpawner : MonoBehaviour
    {
        public PrefabCatalogueSO prefabCatalogueSo;
        public float spawnArea = 5.0f;
        [SerializeField] private float spawnRate = 5.0f;
        private LayerMask _mask;

        private void Start()
        {
            _mask = LayerMask.GetMask("Default");
            InvokeRepeating(nameof(SpawnEnemy), 0, spawnRate);
        }

        private void SpawnEnemy()
        {
            // Shoots down a ray from a 100 units above the enemy spawner. If it hits the layerMaskCollider, a 
            // zombie is spawned.
            var raycastStartHeight = 100;
            RaycastHit hit;
            Ray ray = new Ray (transform.position + Vector3.up * raycastStartHeight, Vector3.down);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _mask)) 
            {        
                if (hit.collider != null)
                {
                    GameObject enemy = Instantiate(prefabCatalogueSo.prefab[0]);
                    enemy.GetComponent<NavMeshAgent>().enabled = false;

                    Vector2 randomOnCircle = Random.insideUnitCircle * spawnArea;
                    Vector3 randomPosition = new Vector3(transform.position.x + randomOnCircle.x, hit.point.y, transform.position.z + randomOnCircle.y);
                    enemy.transform.position = randomPosition;
                    enemy.transform.parent = transform.parent;
                    enemy.GetComponent<NavMeshAgent>().enabled = true;
                    
                    if (transform.parent != null)
                    {
                        transform.parent.GetComponent<Chunk>().chunkObjects.Add(enemy);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Handles.color = new Color(0.0f,0.0f,1.0f, 0.05f);
            Handles.DrawSolidDisc(transform.position, transform.up, spawnArea);

            spawnArea = Handles.ScaleValueHandle(
                spawnArea, transform.position + transform.forward * spawnArea, 
                transform.rotation, 1, Handles.ConeHandleCap, 1);
        }
    }
}
