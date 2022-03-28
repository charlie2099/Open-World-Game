using System;
using System.Collections;
using System.Security.Cryptography;
using Chilli.Quests;
using Chilli.Terrain;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Chilli.Ai.Zombies
{
    public class Zombie : MonoBehaviour
    { 
        private Transform _playerRef;
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private float _detectionRange;
        private float _chaseRange;
        private Vector3 _spawnPos;
        private float _health;
        private bool _isDying;

        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Run = Animator.StringToHash("Run");

        private void Awake() // cache references
        {
            _isDying = false;
            _playerRef = GameObject.FindWithTag("Player").transform;
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _health = 100;
            _spawnPos = transform.position;
            _detectionRange = 30; /*Random.Range(15, 20)*/
            _chaseRange = _detectionRange / 2.0f;
            
            InvokeRepeating(nameof(PlayerInDetectionRange), 0, 0.5f);
            InvokeRepeating(nameof(PlayerInChaseRange), 0, 0.5f);
        }

        private void Update()
        {
            if (!_isDying && _playerRef != null)
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
                    //float step =  _speed * Time.deltaTime;
                    //transform.LookAt(_playerRef.position);
                    //transform.position = Vector3.MoveTowards(transform.position, _playerRef.position, step);
                    _navMeshAgent.SetDestination(_playerRef.position);
                    _navMeshAgent.speed = 0.75f;
                }
            }
        }

        private bool PlayerInDetectionRange()
        {
            if (_playerRef == null) { return false; }
            return Vector3.Distance(transform.position, _playerRef.position) < _detectionRange && 
                   Vector3.Distance(transform.position, _playerRef.position) > _chaseRange;
        }
        
        private bool PlayerInChaseRange()
        {
            if (_playerRef == null) { return false; }
            return Vector3.Distance(transform.position, _playerRef.position) <= _chaseRange;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Chunk>() != null)
            {
                var chunk = collision.gameObject.GetComponent<Chunk>();
                
                if (chunk.IsLoaded())
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

            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.transform.parent != null)
                {
                    collision.transform.parent.GetComponent<Player>().TakeDamage(10);
                    print("Player health: " + collision.transform.parent.GetComponent<Player>().GetHealth());
                }
            }
        }

        private void OnCollisionExit(Collision collision) 
        {
            if (collision.gameObject.GetComponent<Chunk>() != null)
            {
                var chunk = collision.gameObject.GetComponent<Chunk>();
                
                // Remove from list of previous chunk
                if (transform.parent != chunk.transform && !chunk.IsLoaded())
                {
                    chunk.GetComponent<Chunk>().chunkObjects.Remove(gameObject);
                }
            }
        }

        private void Investigate()
        {
            GetComponent<Animator>().SetBool(Walk, true);
            GetComponent<Animator>().SetBool(Run, false);
        }
        
        private void Chase()
        {
            GetComponent<Animator>().SetBool(Run, true);
        }

        private void WanderAroundRandomly()
        {
            // If zombie is roughly at their start position, return to idle state
            var xRange = Mathf.Clamp(transform.position.x, _spawnPos.x - 1, _spawnPos.x + 1);
            var zRange = Mathf.Clamp(transform.position.z, _spawnPos.z - 1, _spawnPos.z + 1);
            if (transform.position != new Vector3(xRange, transform.position.y, zRange))
            {
                GetComponent<Animator>().SetBool(Run, false);
                GetComponent<Animator>().SetBool(Walk, true);
                //_speed = 0.25f;
                //float step =  _speed * Time.deltaTime;
                //transform.LookAt(new Vector3(_spawnPos.x, transform.position.y, _spawnPos.z));
                //transform.position = Vector3.MoveTowards(transform.position, _spawnPos, step);]
                _navMeshAgent.SetDestination(_spawnPos);
                _navMeshAgent.speed = 0.25f;
            }
            else
            {
                GetComponent<Animator>().SetBool(Walk, false);
            }
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                EventManager.TriggerEvent("ZombieKilled", new EventParam());
                StartCoroutine(PlayDeathAnimation());
            }
        }

        public bool IsDying()
        {
            return _isDying;
        }
        
        private IEnumerator PlayDeathAnimation()
        {
            _isDying = true;
            _animator.SetBool("Dying", true);
            Destroy(GetComponentInChildren<CapsuleCollider>());
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            _navMeshAgent.speed = 0;
            yield return new WaitForSeconds(10.0f);
            transform.parent.GetComponent<Chunk>().chunkObjects.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
