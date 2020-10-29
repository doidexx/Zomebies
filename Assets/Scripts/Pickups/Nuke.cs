using UnityEngine;

public class Nuke : MonoBehaviour
{
    public int pointsForPickup = 300;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null)
            return;
        Zombie[] zombies = FindObjectsOfType<Zombie>();
        foreach (Zombie zombie in zombies)
        {
            if (zombie.gameObject.activeSelf)
                zombie.Dead();
        }
        FindObjectOfType<GameManager>().AddPoints(pointsForPickup);
        Destroy(gameObject);
    }
}