using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public string drinkName = "";
    public int cost = 10;
    [Header("Vending Machine Effect")]
    public bool doubleTap = false;      //double damage
    public bool speedCola = false;      //reduce reload time
    public bool juggernog = false;      //increases player health
    public bool quickRevive = false;    //reduce reviving time
    public bool muleKick = false;       //let players have a third weapon
    public bool electricCherry = false; //damage zombies around player when reloading
    [Header("Other Machines")]
    public bool mysteryBox = false;
    public bool packAPunch = false;
    [Header("Pack a punched Settings")]
    public BuyableWeapons[] buyablePackedWeapons = null;
    public bool inUse = false;
    public float packingTime = 4f;
    [Header("Mystery Box Settings")]
    public float loopTime = 8f;
    public float buyTime = 10f;
    public float showTime = 0.2f;
    public Transform mysteryBoxSpawns = null;
    public Transform buyableWeapons = null;

    float showTimer = Mathf.Infinity;
    float loopTimer = Mathf.Infinity;
    GameManager gameManager = null;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        showTimer += Time.deltaTime;
        loopTimer += Time.deltaTime;
        RunMysteryBox();
    }
    
    public void GetEffect(Player player, Weapon weapon)
    {
        if (doubleTap && GameManager.doubleTap == false)
        {
            GameManager.doubleTap = true;
            gameManager.ConsumePoints(cost);
        }
        else if (speedCola && GameManager.speedCola == false)
        {
            GameManager.speedCola = true;
            gameManager.ConsumePoints(cost);
        }
        else if (juggernog && GameManager.juggernog == false)
        {
            GameManager.juggernog = true;
            gameManager.ConsumePoints(cost);
        }
        else if (quickRevive && GameManager.quickRevive == false)
        {
            GameManager.quickRevive = true;
            gameManager.ConsumePoints(cost);
        }
        else if (muleKick && GameManager.muleKick == false)
        {
            GameManager.muleKick = true;
            gameManager.ConsumePoints(cost);
            player.AddWeaponSlot();
        }
        else if (electricCherry && GameManager.electricCherry == false)
        {
            GameManager.electricCherry = true;
            gameManager.ConsumePoints(cost);
        }
        else if (packAPunch && inUse == false)
        {
            if (weapon.packed)
                return;
            if (gameManager.GetPakckedWeapon(weapon.ID) == null)
                return;
            player.RemoveActiveGun();
            StartCoroutine(SpawnPackedGun(weapon));
            gameManager.ConsumePoints(cost);
        }
        else if (mysteryBox && !CheckMysteryBoxUse())
        {
            loopTimer = 0;
            gameManager.ConsumePoints(cost);
        }
    }

    private bool CheckPackAPunchUse()
    {
        foreach (Weapon w in gameManager.packedWeapons)
        {
            if (w.gameObject.activeSelf == false)
                continue;
            return true;
        }
        return false;
    }

    private void RunMysteryBox()
    {
        if (loopTimer < loopTime)
        {
            if (showTimer > showTime)
            {
                int randomIndex = Random.Range(0, buyableWeapons.childCount);
                foreach (Transform w in buyableWeapons)
                {
                    w.gameObject.SetActive(false);
                    w.GetComponent<Collider>().enabled = false;
                }
                buyableWeapons.GetChild(randomIndex).gameObject.SetActive(true);
                showTimer = 0;
            }
        }
        else if (mysteryBox == true)
        {
            foreach (Transform w in buyableWeapons)
            {
                if (w.gameObject.activeSelf)
                    w.GetComponent<Collider>().enabled = true;
            }
        }
    }

    private bool CheckMysteryBoxUse()
    {
        foreach (Transform w in buyableWeapons)
        {
            if (w.gameObject.activeSelf == false)
                continue;
            return true;
        }
        return false;
    }

    IEnumerator SpawnPackedGun(Weapon weapon)
    {
        inUse = true;
        yield return new WaitForSeconds(packingTime);
        for (int i = 0; i < buyablePackedWeapons.Length; i++)
        {
            if (buyablePackedWeapons[i].ID != weapon.ID)
                continue;
            buyablePackedWeapons[i].gameObject.SetActive(true);
        }
        StartCoroutine(DespawnGun());
    }

    IEnumerator DespawnGun()
    {
        yield return new WaitForSeconds(buyTime);
        foreach (BuyableWeapons w in buyablePackedWeapons)
        {
            if (w.gameObject.activeSelf)
                w.gameObject.SetActive(false);
        }
        inUse = false;
    }
}
