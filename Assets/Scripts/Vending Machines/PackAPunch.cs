using UnityEngine;

public class PackAPunch : MonoBehaviour
{
    public string _name = "PackAPunch";
    public int cost = 0;
    public float waitTime = 8f;
    public float despawnTime = 10f;
    public bool inUse = false;
    public BuyableWeapons[] weapons = null;

    float timeSinceBought = Mathf.Infinity;
    float timeSinceAvailable = Mathf.Infinity;
    int weaponIndex = 0;
    bool waiting = false;

    private void Update()
    {
        timeSinceBought += Time.deltaTime;
        timeSinceAvailable += Time.deltaTime;

        if (timeSinceBought > waitTime && waiting == true)
        {
            timeSinceAvailable = 0;
            waiting = false;
        }
        if (timeSinceAvailable < despawnTime)
            weapons[weaponIndex].gameObject.SetActive(true);
        else
        {
            inUse = false;
            weapons[weaponIndex].gameObject.SetActive(false);
        }
    }

    private bool GetPackedWeapon(int id)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].ID != id)
                continue;
            weaponIndex = i;
            return true;
        }
        return false;
    }

    public void Buy(Player player, Weapon weapon)
    {
        if (GetPackedWeapon(weapon.ID) == false)
            return;
        FindObjectOfType<GameManager>().ConsumePoints(cost);
        player.RemoveActiveGun();
        timeSinceBought = 0;
        inUse = true;
        waiting = true;
    }
}