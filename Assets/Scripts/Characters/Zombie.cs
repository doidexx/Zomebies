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

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        baseSpeed = agent.speed;
    }

    private void Update()
    {
        if (agent.enabled == false)
            return;
        PrioritizeTarget();
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(target.position, transform.position);
            if (distanceToTarget <= agent.stoppingDistance)
                AttackTarget();
            else
                animator.SetBool("Attack", false);
        }
        if (agent != null && agent.enabled == true)
            animator.SetFloat("Speed", agent.velocity.magnitude);
        if (animator.enabled)
            animator.SetBool("Inside", inside);
    }

    private void AttackTarget()
    {
        //trigger animation
        Vector3 lookAtPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.LookAt(lookAtPosition);
        animator.SetBool("Attack", true);
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
            float distance = Vector3.Distance(barricade.transform.position, this.transform.position);
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
