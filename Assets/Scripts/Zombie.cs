using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class Zombie : MonoBehaviour
{
    [Range(0,1)]
    public float linkSpeed = 1;
    public float distanceToTargetBarricade = 8f;
    public int damage = 20;
    public Transform target = null;

    float baseSpeed = 4.5f;
    bool inside = false;

    NavMeshAgent agent = null;
    Transform player = null;
    Animator animator = null;

    //temporary attack delay
    float attackRate = 1f;
    float timeSinceLastAttack = Mathf.Infinity;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
            {
                agent.isStopped = false;
                animator.SetBool("Attack", false);
            }
            
            if (agent.isStopped)
                AttackTarget();
        }
        if (agent != null && agent.enabled == true)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void AttackTarget()
    {
        //trigger animation
        Vector3 lookAtPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(lookAtPosition);
        if (timeSinceLastAttack < attackRate)
        {
            animator.SetBool("Attack", true);
            return;
        }
        timeSinceLastAttack = 0;
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
        if (agent.isOnOffMeshLink && !inside)
        {
            agent.speed = linkSpeed;
            animator.SetTrigger("Climb");
        }
        else if (agent.isOnNavMesh)
        {
            agent.speed = baseSpeed;
        }
    }

    public void Inside() //animation event
    {
        inside = true;
        animator.ResetTrigger("Climb");
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
