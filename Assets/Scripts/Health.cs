using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float healthPoints = 100;
    public float maxHealth = 100;
    public bool dead = false;
    public bool iZombie = false;

    float healingTime = 1;
    float healingTimer = Mathf.Infinity;

    GameManager gameManager = null;
    Animator animator = null;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        healingTimer += Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        if (dead)
            return;
        healthPoints = Mathf.Max(0, healthPoints - damage);
        if (healthPoints == 0)
            Dead();
        if (iZombie)
            gameManager.AddPoints(10);
    }

    public void Heal()
    {
        if (healingTimer < healingTime)
            return;
        healingTimer = 0;
        healthPoints = Mathf.Max(maxHealth, healthPoints + 10);
    }

    public void HealBarricade()
    {
        if (healingTimer < healingTime || healthPoints == maxHealth)
            return;
        healingTimer = 0;
        healthPoints = Mathf.Min(maxHealth, healthPoints + 10);
        gameManager.AddPoints(10);
    }

    void Dead() 
    {
        dead = (healthPoints == 0);
        if (iZombie)
            GetComponent<Zombie>().Dead();
        else if (GetComponent<Player>())
            GetComponent<Player>().Dead();
    }
}
