using UnityEngine;

namespace Chilli.Ai
{
    public class EnemySpawner : MonoBehaviour
    {
        //public GameObject enemyPrefab;
        public PrefabCatalogueSO prefabCatalogueSo;
        [SerializeField] private float spawnRate = 5.0f;
        private LayerMask mask;
        private float radius;

        private void Start()
        {
            mask = LayerMask.GetMask("Default");

            InvokeRepeating(nameof(SpawnEnemy), 0, spawnRate);
        }

        private void SpawnEnemy()
        {
            GameObject enemy = Instantiate(prefabCatalogueSo.prefab[0], transform.position, Quaternion.identity);
            enemy.transform.parent = transform.parent;
                    
            if (transform.parent != null)
            {
                transform.parent.GetComponent<Chunk>().chunkObjects.Add(enemy);
            }
            
            
            
            
            
            // Shoots down a ray from a 100 units above the enemy spawner. If it hits the layerMaskCollider, a 
            // zombie is spawned.
            /*var raycastStartHeight = 100;
            RaycastHit hit;
            Ray ray = new Ray (transform.position + Vector3.up * raycastStartHeight, Vector3.down);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) 
            {        
                if (hit.collider != null)
                {
                    GameObject enemy = Instantiate(prefabCatalogueSo.prefab[0]);
                    //enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    
                    // Sets the vertical offset to the object's collider bounds' extends
                    // To ensure part of the zombie isn't clipping through the chunk upon spawning and then falling
                    // through the map?
                    // Doesn't work properly due to the terrain mesh
                    if (enemy.GetComponentInChildren<Collider>() != null)
                    {
                        radius = enemy.GetComponentInChildren<Collider>().bounds.extents.y;
                    } 
                    else
                    {
                        radius = 1f;
                    }
                    
                    var xPos = Random.Range(transform.position.x - 5, transform.position.x + 5);
                    var zPos = Random.Range(transform.position.z - 5, transform.position.z + 5);
                    enemy.transform.position = new Vector3(xPos, hit.point.y + radius, zPos);
                    enemy.transform.parent = transform.parent;
                    
                    if (transform.parent != null)
                    {
                        transform.parent.GetComponent<Chunk>().chunkObjects.Add(enemy);
                    }
                }
            }*/
        }
    }
}
