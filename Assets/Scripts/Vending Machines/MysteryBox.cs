using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    public string _name = "Mystery Box";
    public int cost = 0;
    public float rotatingTime = 8f;
    public float showTime = 0.2f;
    public float despawnTime = 10f;
    public bool inUse = false;
    public BuyableWeapons[] weapons = null;

    float timeSinceBought = Mathf.Infinity;
    float timeSinceShowed = Mathf.Infinity;
    float timeSinceLocked = Mathf.Infinity;
    int weaponIndex = 0;

    private void Update()
    {
        timeSinceBought += Time.deltaTime;
        timeSinceShowed += Time.deltaTime;
        timeSinceLocked += Time.deltaTime;

        if (timeSinceBought < rotatingTime)
        {
            if (timeSinceShowed > showTime)
            {   
                if (weapons[weaponIndex].gameObject.activeSelf)
                    weapons[weaponIndex].gameObject.SetActive(false);
                weaponIndex = (int)Random.Range(0, weapons.Length);
                weapons[weaponIndex].gameObject.SetActive(true);
                timeSinceShowed = 0;
            }
            timeSinceLocked = 0;
        }
        else if (timeSinceLocked > despawnTime)
        {
            weapons[weaponIndex].GetComponent<Collider>().enabled = false;
            if (weapons[weaponIndex].gameObject.activeSelf)
                weapons[weaponIndex].gameObject.SetActive(false);
            inUse = false;
        }
        else
            weapons[weaponIndex].GetComponent<Collider>().enabled = true;
    }

    public void Buy(Player player)
    {
        inUse = true;
        timeSinceBought = 0;
    }

    private void DeactivateWeapons(BuyableWeapons weapon)
    {
        weapon.gameObject.SetActive(false);
    }

    public void MakeAvailable()
    {
        inUse = false;
        if (weapons[weaponIndex].gameObject.activeSelf)
            weapons[weaponIndex].gameObject.SetActive(false);
    }
}