using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ghoul enemy = collision.GetComponent<Ghoul>();
        if (enemy != null)
        {
            takeDamage(1);
            enemy.Die();
            if (currentHealth <= 0)
            {
                GetComponent<Collider2D>().enabled = false;
                GameManager.Instance.EndGame(false);
            }
        }
    }

    private void takeDamage(int amount)
    {
        currentHealth -= amount;
    }
}
