using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chilli.Ai.Zombies
{
    public class Zombie : MonoBehaviour
    { 
        private Transform playerRef;
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Run = Animator.StringToHash("Run");
        private float detectionRange;
        private float chaseRange;
        private float speed;
        private Vector3 spawnPos;

        private void Awake() // cache references
        {
            playerRef = GameObject.FindWithTag("Player").transform;
        }

        private void Start()
        {
            spawnPos = transform.position;
            detectionRange = 30; /*Random.Range(15, 20)*/
            chaseRange = detectionRange / 2.0f;
            
            InvokeRepeating(nameof(PlayerInDetectionRange), 0, 0.5f);
            InvokeRepeating(nameof(PlayerInChaseRange), 0, 0.5f);
        }

        private void Update()
        {
            if (PlayerInDetectionRange())
            {
                Investigate();
            }
            else if (PlayerInChaseRange())
            {
                Chase();
            }
            else
            {
                WanderAroundRandomly();
            }
            
            if (PlayerInDetectionRange() || PlayerInChaseRange())
            {
                float step =  speed * Time.deltaTime;
                transform.LookAt(playerRef.position);
                transform.position = Vector3.MoveTowards(transform.position, playerRef.position, step);
            }
        }

        private bool PlayerInDetectionRange()
        {
            return Vector3.Distance(transform.position, playerRef.position) < detectionRange && 
                   Vector3.Distance(transform.position, playerRef.position) > chaseRange;
        }
        
        private bool PlayerInChaseRange()
        {
            return Vector3.Distance(transform.position, playerRef.position) <= chaseRange;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var chunk = collision.gameObject.GetComponent<Chunk>(); // null check here?
            if (chunk != null)
            {
                // If the zombie doesn't already belong to the chunk, parent itself to it and add to it's list
                if (transform.parent != chunk.transform)
                {
                    transform.parent = chunk.transform;
                    
                    // NOTE 1:
                    //
                    // Because zombies are dynamic objects and don't belong to any one chunk for the duration of play;
                    // they should not be added to the list of chunk objects. Chunk objects unload with it's chunk and
                    // if the zombie has moved chunk it would not make sense to unload it if it's current chunk remains
                    // loaded.
                    
                    // NOTE 2:
                    // 
                    // Zombie data doesn't need to be loaded and unloaded from file as they are spawned with spawners.
                    // Each zombie is the same as each other so it is a waste of memory and cpu cycles to read and 
                    // write data for each and every zombie when loaded/unloaded.
                    // NPCs however possess data about quests, which is important to the player and progress should
                    // be written to file for that npc's quest. Therefore NPCs should be loaded / unloaded from file.
                    
                    chunk.GetComponent<Chunk>().chunkObjects.Add(gameObject);
                }
            }
        }

        private void OnCollisionExit(Collision collision) 
        {
            var chunk = collision.gameObject.GetComponent<Chunk>(); // null check here?
            if (chunk != null)
            {
                // Remove from list of previous chunk
                if (transform.parent != chunk.transform)
                {
                    chunk.GetComponent<Chunk>().chunkObjects.Remove(gameObject);
                }
            }
        }

        private void Investigate()
        {
            GetComponent<Animator>().SetBool(Walk, true);
            GetComponent<Animator>().SetBool(Run, false);
            speed = 0.25f;
        }
        
        private void Chase()
        {
            GetComponent<Animator>().SetBool(Run, true);
            speed = 1.0f;
        }

        private void WanderAroundRandomly()
        {
            // If zombie is roughly at their start position, return to idle state
            var xRange = Mathf.Clamp(transform.position.x, spawnPos.x - 1, spawnPos.x + 1);
            var zRange = Mathf.Clamp(transform.position.z, spawnPos.z - 1, spawnPos.z + 1);
            if (transform.position != new Vector3(xRange, transform.position.y, zRange))
            {
                GetComponent<Animator>().SetBool(Run, false);
                GetComponent<Animator>().SetBool(Walk, true);
                speed = 0.25f;
                float step =  speed * Time.deltaTime;
                transform.LookAt(new Vector3(spawnPos.x, transform.position.y, spawnPos.z));
                transform.position = Vector3.MoveTowards(transform.position, spawnPos, step);
            }
            else
            {
                GetComponent<Animator>().SetBool(Walk, false);
            }
        }
    }
}
