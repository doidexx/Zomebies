using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxAmmo : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null)
            return;
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player playr in players)
        {
            Weapon[] weapons = playr.weapons;
            foreach (Weapon weapon in weapons)
            {
                if (weapon != null)
                    weapon.MaxOutAmmo();
            }
        }
        Destroy(gameObject);
    }
}
