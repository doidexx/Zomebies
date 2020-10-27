using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text pointsText = null;
    public Text roundText = null;
    public Text ammoText = null;
    public Text buyableText = null;
    public Text interactableText = null;

    GameManager gameManager = null;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        UpdatePointsText();
        buyableText.text = "";
    }

    public void UpdateBuyableText(bool buyingWeapon)
    {
        if (buyableText != null)
        {
            string weaponName = Player.newWeaponToBuy.name;
            string weaponCost = Player.newWeaponToBuy.cost.ToString();
            string weaponAmmoCost = Player.newWeaponToBuy.ammoCost.ToString();
            if (buyingWeapon == true)
            {
                if (weaponCost == "0")
                    buyableText.text = weaponName;
                else
                    buyableText.text = weaponName + ": $" + weaponCost;
            }
            else
                buyableText.text = weaponName + " Ammo" + ": $" + weaponCost;
        }
    }

    private void UpdatePointsText()
    {
        if (pointsText != null)
            pointsText.text = "$ " + gameManager.points.ToString();
    }

    public void UpdateAmmoText(Weapon weapon)
    {
        if (ammoText != null)
        {
            string magazine = weapon.magazineAmmo.ToString();
            string ammo = weapon.currentAmmo.ToString();
            ammoText.text = magazine + " / " + ammo; //current magazine ammo over current ammo
        }
    }

    public void UpdateInteractableText(string message)
    {
        if (interactableText == null)
            return;
        interactableText.text = message;
    }
}
