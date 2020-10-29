using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null)
            return;
        FindObjectOfType<GameManager>().doublePointsTimer = 0;
        Destroy(gameObject);
    }
}
