using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float movementSpeed = 10f;
    public float jumpHeight = 10f;
    [Header("Settings")]
    public float interactionDistance = 3f;
    public float gravityMultiplier = 2f;
    public float horizontalMovementMultiplier = .7f;
    [Header("Weapons")]
    public Weapon[] weapons = new Weapon[2];
    public int currentWeaponIndex = 0;
    [Header("Drinks")]
    public Drink[] drinks = new Drink[4];

    float vertical = 0;
    int weaponIndexer = 0;

    CharacterController controller = null;
    GameManager gameManager = null;
    UIManager uiManager = null;
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
        CheckInteractions();
        Physics.Raycast(GetRay(), out hit, Mathf.Infinity);
        if (weapons[currentWeaponIndex] != null)
            uiManager.UpdateAmmoText(weapons[currentWeaponIndex]);
        ScrollWeapon();
    }

    private void ScrollWeapon()
    {
        weaponIndexer += (int)Input.mouseScrollDelta.y;
        if (weaponIndexer == currentWeaponIndex) return;
        if (weaponIndexer == weapons.Length)
            weaponIndexer = 0;
        else if (weaponIndexer < 0)
            weaponIndexer = weapons.Length - 1;
        SwitchCurrentWeapon(weaponIndexer);
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

    private void CheckInteractions()
    {
        RaycastHit hit;
        if (!Physics.Raycast(GetRay(), out hit, interactionDistance))
        {
            //turn UI off
            return;
        }
        Drink drink = hit.transform.GetComponent<Drink>();
        MysteryBox mysteryBox = hit.transform.GetComponent<MysteryBox>();
        PackAPunch packAPunch = hit.transform.GetComponent<PackAPunch>();
        BuyableWeapons buyable = hit.transform.GetComponent<BuyableWeapons>();
        Obstacle obstacle = hit.transform.GetComponent<Obstacle>();
        if (!Input.GetKeyDown(KeyCode.E))
            return;
        if (buyable != null)
            InteractWithBuyable(buyable);
        else if (drink != null)
            InteractWithDrink(drink);
        else if (mysteryBox != null)
            InteractWithMysteryBox(mysteryBox);
        else if (packAPunch != null)
            InteractWithPackAPunch(packAPunch);
        else if (obstacle != null)
        {
            if (gameManager.points < obstacle.cost)
                return;
            obstacle.Buy();
            gameManager.ConsumePoints(obstacle.cost);
        }
    }

    private void InteractWithDrink(Drink drink)
    {
        //Turn UI on with drink drinkMame
        if (gameManager.points >= drink.cost)
            drink.Buy(this);
    }

    private void InteractWithMysteryBox(MysteryBox mysteryBox)
    {
        //Turn UI on with drink drinkMame
        if (mysteryBox.inUse == true)
            return;
        if (gameManager.points < mysteryBox.cost)
            return;
        mysteryBox.Buy(this);
        gameManager.ConsumePoints(mysteryBox.cost);
    }

    private void InteractWithPackAPunch(PackAPunch packAPunch)
    {
        if (packAPunch.inUse == true)
            return;
        if (weapons[currentWeaponIndex] == null || weapons[currentWeaponIndex].packed == true)
            return;
        if (gameManager.points < packAPunch.cost)
            return;
        packAPunch.Buy(this, weapons[currentWeaponIndex]);
    }

    private void InteractWithBuyable(BuyableWeapons buyable)
    {
        if (gameManager.points < buyable.cost)
            return;

        if (GetOwnWeapon(buyable.ID) != null)
        {
            GetOwnWeapon(buyable.ID).MaxOutAmmo();
            gameManager.ConsumePoints(buyable.ammoCost);
        }
        else
        {
            AssignWeapon(buyable.ID, buyable.packAPunched);
            gameManager.ConsumePoints(buyable.cost);
        }
        if (buyable.machine)
            buyable.gameObject.SetActive(false);
    }

    private void AssignWeapon(int id, bool packed)
    {
        int slot = CheckAvailableSlot();
        if (weapons[slot] != null)
            weapons[slot].gameObject.SetActive(false);
        
        if (packed)
            weapons[slot] = gameManager.packedWeapons[id];
        else
            weapons[slot] = gameManager.weapons[id];
        SwitchCurrentWeapon(slot);
    }

    private int CheckAvailableSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
                return i;
        }
        return currentWeaponIndex;
    }

    private void SwitchCurrentWeapon(int index)
    {
        if (weapons[index] == null)
            return;
        if (weapons[currentWeaponIndex] != null)
            weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex = index;
        weapons[index].gameObject.SetActive(true);
    }

    private Weapon GetOwnWeapon(int ID)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon == null)
                continue;
            if (weapon.ID == ID)
                return weapon;
        }
        return null;
    }

    public void ActivateDrink(Drink drink)
    {
        for (int i = 0; i < drinks.Length; i++)
        {
            if (drinks[i] == drink)
                break;
            if (drinks[i] != null)
                continue;
            drinks[i] = drink;
            gameManager.ConsumePoints(drink.cost);
            drink.GetEffect(this);
            break;
        }
    }

    public void RemoveActiveGun()
    {
        int index = 0;
        if (currentWeaponIndex + 1 == weapons.Length)
            index = 0;
        else
            index = currentWeaponIndex + 1;
        weapons[currentWeaponIndex].gameObject.SetActive(false);
        weapons[currentWeaponIndex] = null;
        SwitchCurrentWeapon(index);
    }

    public void ChangeNumberOfSlots(int slots)
    {
        Weapon[] oldWeapons = weapons;
        weapons = new Weapon[slots];
        for (int i = 0; i < oldWeapons.Length; i++)
            weapons[i] = oldWeapons[i];
    }

    public void Dead()
    {
        drinks = new Drink[4];
        ChangeNumberOfSlots(2);
        GameManager.ClearDrinks();
    }
}
