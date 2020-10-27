using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float movementSpeed = 10f;
    public float horizontalMovementMultiplier = .7f;
    public float jumpHeight = 10f;
    public float gravityMultiplier = 2f;
    [Header("Other")]
    public GameObject flashlight = null;
    public float pickupDistance = 3f;
    public Weapon[] weapons = new Weapon[2];
    public int currentWeaponIndex = 0;

    float vertical = 0;

    CharacterController controller = null;
    GameManager gameManager = null;
    UIManager uiManager = null;
    public static BuyableWeapons newWeaponToBuy = null;
    public static RaycastHit hit;

    private void Start() 
    {
        controller = GetComponent<CharacterController>();
        gameManager = FindObjectOfType<GameManager>();
        uiManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        Movement();
        Flashlight();
        SwitchWeapons();
        Interactions();
        Physics.Raycast(GetRay(), out hit, Mathf.Infinity);
        if (weapons[currentWeaponIndex] != null)
            uiManager.UpdateAmmoText(weapons[currentWeaponIndex]);
    }

    public static Ray GetRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void Movement()
    {
        float forward = Input.GetAxis("Vertical") * movementSpeed;
        float horizontal = Input.GetAxis("Horizontal") * movementSpeed * horizontalMovementMultiplier;
        if (Input.GetKey(KeyCode.LeftShift) && forward > 0)
            forward = Input.GetAxis("Vertical") * movementSpeed * 2;
        
        CalculateVerticalVelocity();
        Vector3 direction = (transform.right * horizontal) + (transform.up * vertical) + (transform.forward * forward);
        controller.Move(direction * Time.deltaTime);
    }

    private void CalculateVerticalVelocity()
    {
        if (controller.isGrounded)
        {
            vertical = 0;
            if (Input.GetButton("Jump"))
                vertical = jumpHeight;
        }
        vertical += Physics.gravity.y * Time.deltaTime * gravityMultiplier;
    }

    private void Flashlight()
    {
        if (flashlight == null) return;
        if (Input.GetKeyDown(KeyCode.F))
            flashlight.SetActive(!flashlight.activeSelf);
    }

    private void Interactions()
    {
        RaycastHit localHit;
        if (Physics.Raycast(Player.GetRay(), out localHit, pickupDistance))
        {
            WeaponPickup(localHit);
            InteractWithObstacles(localHit);
            InteractWithMachines(localHit);
        }
    }

    private void WeaponPickup(RaycastHit hit)
    {
        newWeaponToBuy = hit.transform.GetComponent<BuyableWeapons>();
        if (newWeaponToBuy == null)
            return;

        Weapon newWeapon = CheckOwnedWeapon(newWeaponToBuy.ID);
        if (newWeapon == null || newWeaponToBuy.ID != newWeapon.ID)
        {
            PurchaseWeapon(newWeaponToBuy);
            uiManager.UpdateBuyableText(true);
        }
        else
        {
            PurchaseAmmo(newWeaponToBuy.ammoCost, newWeapon);
            uiManager.UpdateBuyableText(false);
        }
    }

    //make it so it return the ID in stead
    public Weapon CheckOwnedWeapon(int ID)
    {
        foreach (Weapon w in weapons)
        {
            if (w == null)
                continue;
            if (w.ID == ID)
                return w;
        }
        return null;
    }

    private void SwitchWeapons()
    {
        int index = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            index = 0;
            if (index == currentWeaponIndex)
                return;
            SwtichCurrentWeapon(index);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            index = 1;
            if (index == currentWeaponIndex)
                return;
            SwtichCurrentWeapon(index);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            index = 2;
            if (index == currentWeaponIndex)
                return;
            SwtichCurrentWeapon(index);
        }
    }

    public void SwtichCurrentWeapon(int index)
    {
        if (index == weapons.Length || weapons[index] == null)
            return;
        if (weapons[currentWeaponIndex] != null)
            weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex = index;
        weapons[currentWeaponIndex].gameObject.SetActive(true);
    }

    private void PurchaseWeapon(BuyableWeapons weapon)
    {
        if (Input.GetKeyDown(KeyCode.E) && weapon.cost <= gameManager.points)
        {
            gameManager.ConsumePoints(weapon.cost);

            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                    continue;
                ReplaceWeaponFor(weapon.ID, i, weapon.packAPunched);
                currentWeaponIndex = i;
                if (weapon.cost == 0)
                    weapon.gameObject.SetActive(false);
                return;
            }
            ReplaceWeaponFor(weapon.ID, currentWeaponIndex, weapon.packAPunched);
            if (weapon.cost == 0)
                weapon.gameObject.SetActive(false);
        }
    }

    public void ReplaceWeaponFor(int ID, int slot, bool packed)
    {
        if (weapons[slot] != null)
            weapons[slot].gameObject.SetActive(false);
        if (packed == false)
            weapons[slot] = gameManager.GetNewWeapon(ID);
        else
            weapons[slot] = gameManager.GetPakckedWeapon(ID);
        weapons[slot].MaxOutAmmo();
        weapons[slot].gameObject.SetActive(true);
        SwtichCurrentWeapon(slot);
    }

    public void RemoveActiveGun()
    {
        int index = currentWeaponIndex;
        if (currentWeaponIndex + 1 == weapons.Length)
            currentWeaponIndex = 0;
        else
            currentWeaponIndex++;
        weapons[index].gameObject.SetActive(false);
        weapons[index] = null;
        SwtichCurrentWeapon(currentWeaponIndex);
    }

    private void PurchaseAmmo(int cost, Weapon weapon)
    {
        if (Input.GetKeyDown(KeyCode.E) && cost <= gameManager.points)
        {
            if (weapon.currentAmmo == weapon.maxAmmo)
                return;
            gameManager.ConsumePoints(cost);
            weapon.MaxOutAmmo();
        }
    }

    private void InteractWithObstacles(RaycastHit hit)
    {
        Obstables obstacle = hit.transform.GetComponent<Obstables>();
        if (obstacle == null)
        {
            //pop up next zone cost
            return;
        }
        if (!Input.GetKeyDown(KeyCode.E) || obstacle.cost > gameManager.points)
            return;
        gameManager.ConsumePoints(obstacle.cost);
        obstacle.Buy();
    }

    private void InteractWithMachines(RaycastHit hit)
    {
        VendingMachine machine = hit.transform.GetComponent<VendingMachine>();
        if (machine == null)
            return;
        
        if (!Input.GetKeyDown(KeyCode.E) || machine.cost > gameManager.points)
            return;
        machine.GetEffect(this, weapons[currentWeaponIndex]);
    }

    public void AddWeaponSlot()
    {
        Weapon[] oldWeapons = weapons;
        weapons = new Weapon[3];
        for (int i = 0; i < oldWeapons.Length; i++)
        {
            weapons[i] = oldWeapons[i];
        }
    }

    public void ReduceWeaponSlots()
    {
        Weapon[] oldWeapons = weapons;
        weapons = new Weapon[2];
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = oldWeapons[i];
        }
    }

    public void Dead()
    {
        ReduceWeaponSlots();
        GameManager.ClearDrinks();
    }
}
