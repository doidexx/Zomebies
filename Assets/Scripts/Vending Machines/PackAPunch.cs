using UnityEngine;

public class PackAPunch : MonoBehaviour
{
    public string _name = "PackAPunch";
    public int cost = 0;
    public float waitTime = 8f;
    public float despawnTime = 10f;
    public BuyableWeapons[] weapons = null;

    float timeSinceBought = Mathf.Infinity;
    float timeSinceAvailable = Mathf.Infinity;
    int weaponIndex = 0;
    bool waiting = false;
    float inUseTime = 0;
    float inUseTimer = Mathf.Infinity;

    private void Start()
    {
        inUseTime = waitTime + despawnTime + 1;
    }

    private void Update()
    {
        timeSinceBought += Time.deltaTime;
        timeSinceAvailable += Time.deltaTime;
        inUseTimer += Time.deltaTime;

        if (timeSinceBought > waitTime && waiting == true)
        {
            timeSinceAvailable = 0;
            waiting = false;
        }
        if (timeSinceAvailable < despawnTime)
            weapons[weaponIndex].gameObject.SetActive(true);
        else
            weapons[weaponIndex].gameObject.SetActive(false);
    }

    private bool InUse()
    {
        return inUseTimer < inUseTime;
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
        if (InUse() == true || GetPackedWeapon(weapon.ID) == false)
            return;
        FindObjectOfType<GameManager>().ConsumePoints(cost);
        player.RemoveActiveGun();
        timeSinceBought = 0;
        inUseTimer = 0;
        waiting = true;
    }

    public void StopUsing()
    {
        inUseTimer = Mathf.Infinity;
    }
}