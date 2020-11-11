using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshCollider), typeof(NavMeshObstacle))]
public class Obstacle : MonoBehaviour
{
    public string blockedArea = "";
    public int cost = 0;

    private void Start()
    {
        GetComponent<NavMeshObstacle>().carving = true;
    }

    public void Buy()
    {
        //trigger animation
        //destroy after some time, not immediately
        Destroy(gameObject);
    }
}
