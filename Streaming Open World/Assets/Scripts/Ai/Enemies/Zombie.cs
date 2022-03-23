using System;
using UnityEngine;

namespace Ai.Enemies
{
    public class Zombie : MonoBehaviour
    { 
        private Transform playerRef;

        private void Awake() // cache references
        {
            playerRef = GameObject.FindWithTag("Player").transform;
        }

        private void Update()
        {
            float step =  2 * Time.deltaTime;
            transform.LookAt(playerRef);
            transform.position = Vector3.MoveTowards(transform.position, playerRef.position, step);
        }
    }
}
