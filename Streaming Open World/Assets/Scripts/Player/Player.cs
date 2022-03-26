using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health;

    private void Start()
    {
        health = 100;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public int GetHealth()
    {
        return health;
    }

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
