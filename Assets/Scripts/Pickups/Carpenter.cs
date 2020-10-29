using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carpenter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null)
            return;
        Barricade[] barricades = FindObjectsOfType<Barricade>();
        foreach (Barricade barricade in barricades)
        {
            barricade.GetComponent<Health>().healthPoints = barricade.GetComponent<Health>().maxHealth;
        }
        Destroy(gameObject);
    }
}
