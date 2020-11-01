using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCast : MonoBehaviour
{
    public float launchForce = 100;
    public Vector3 launchDirection = Vector3.zero;

    private void Start()
    {
        var direction = Vector3.zero;
        var directionX = transform.right * launchDirection.x;
        var directionY = transform.up * launchDirection.y;
        var directionZ = transform.forward * -launchDirection.z;
        direction = directionX + directionY + directionZ;
        GetComponent<Rigidbody>().AddForce(direction * launchForce);
        Destroy(gameObject, 5f);
    }
}
