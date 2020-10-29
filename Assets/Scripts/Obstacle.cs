using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public string blockedArea = "";
    public int cost = 0;

    public void Buy()
    {
        //trigger animation
        //destroy after some time, not immediately
        Destroy(gameObject);
    }
}
