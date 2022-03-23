using System;
using UnityEngine;

namespace Chilli.Ai.Zombies
{
    public class Zombie : MonoBehaviour
    { 
        private Transform playerRef;
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Run = Animator.StringToHash("Run");
        private const float DetectionRange = 15;
        private const float ChaseRange = DetectionRange/2.0f;
        private float speed;
        private Vector3 spawnPos;

        private void Awake() // cache references
        {
            playerRef = GameObject.FindWithTag("Player").transform;
            InvokeRepeating(nameof(PlayerInDetectionRange), 0, 0.5f);
            InvokeRepeating(nameof(PlayerInChaseRange), 0, 0.5f);
        }

        private void Start()
        {
            spawnPos = transform.position;
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
            return Vector3.Distance(transform.position, playerRef.position) < DetectionRange && 
                   Vector3.Distance(transform.position, playerRef.position) > ChaseRange;
        }
        
        private bool PlayerInChaseRange()
        {
            return Vector3.Distance(transform.position, playerRef.position) <= ChaseRange;
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
