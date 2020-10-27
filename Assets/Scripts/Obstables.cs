using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstables : MonoBehaviour
{
    public int cost = 10;

    public void Buy()
    {
        //trigger animation
        //destroy after some time, not immediately
        Destroy(gameObject);
    }
}
