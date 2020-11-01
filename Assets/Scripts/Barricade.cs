using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class Barricade : MonoBehaviour
{
    public string message = "";
    public MeshCollider barrierCollider = null;

    Health health = null;
    UIManager uIManager = null;
    OffMeshLink link = null;

    private void Start()
    {
        health = GetComponent<Health>();
        uIManager = FindObjectOfType<UIManager>();
        link = GetComponent<OffMeshLink>();
    }

    private void Update()
    {
        if (health.healthPoints == 0)
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
            barrierCollider.enabled = false;
            // link.activated = true;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().enabled = true;
            barrierCollider.enabled = true;
            // link.activated = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uIManager.UpdateInteractableText(message);
            if (Input.GetKey(KeyCode.E))
            {
                health.HealBarricade();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uIManager.UpdateInteractableText("");
        }
    }
}
