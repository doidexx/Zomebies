using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class Zombie : MonoBehaviour
{
    public float linkSpeed = 1;
    public float distanceToTargetBarricade = 8f;
    public int damage = 20;

    float baseSpeed = 4.5f;
    NavMeshAgent agent = null;
    public Transform target = null;
    Transform player = null;
    bool inside = false;

    //temporary attack delay
    float attackRate = 1f;
    float timeSinceLastAttack = Mathf.Infinity;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        if (agent.enabled == false)
            return;
        PrioritizeTarget();
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(target.position, transform.position);
            if (distanceToTarget < agent.stoppingDistance)
            {
                agent.isStopped = true;
            }
            else
                agent.isStopped = false;
            
            if (agent.isStopped)
                AttackTarget();
        }
    }

    private void AttackTarget()
    {
        //trigger animation
        Vector3 lookAtPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(lookAtPosition);
        if (timeSinceLastAttack < attackRate)
            return;
        timeSinceLastAttack = 0;
        DamageTarget(); //remove after applying animation
    }

    private void DamageTarget() //Trigger on animation event
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
            targetHealth.TakeDamage(damage);
    }

    private void PrioritizeTarget()
    {
        OffMesh();
        if (inside == true)
            target = player;
        else
            target = GetBarricade();

        if (target != null)
            agent.SetDestination(target.position);
    }

    private Transform GetBarricade()
    {
        Barricade[] barricades = FindObjectsOfType<Barricade>();
        foreach (var barricade in barricades)
        {
            float distance = Vector3.Distance(barricade.transform.position, transform.position);
            if (distance > distanceToTargetBarricade || barricade.GetComponent<Health>().dead == true)
                continue;
            return barricade.transform;
        }
        return player;
    }

    private void OffMesh()
    {
        if (agent.isOnOffMeshLink)
        {
            inside = true;
            agent.speed = linkSpeed;
        }
        else if (agent.isOnNavMesh)
        {
            agent.speed = baseSpeed;
        }
    }

    public void Dead()
    {
        Destroy(gameObject, 15f);
        GetComponent<Animator>().enabled = false;
        agent.enabled = false;
        Rigidbody[] ragdoll = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in ragdoll)
            rb.isKinematic = false;
    }
}
