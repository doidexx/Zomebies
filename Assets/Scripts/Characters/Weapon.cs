using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string _name = "";
    public int ID = 0;
    [Header("Ammo Capacity")]
    public int maxAmmo = 60;
    public int currentAmmo = 60;
    [Header("Magazine")]
    public int magazineSize = 10;
    public int magazineAmmo = 10;
    [Header("Stats")]
    public int damage = 30;
    public float range = 40;
    public float fireRate = 1f;
    public float reloadSpeed = 1f;
    public float sCReloadSpeed = 1.5f;
    [Header("Settings")]
    [Range(0, 0.15f)]
    public float hipSpread = 0.1f;
    [Range(0, 0.15f)]
    public float aDSSpread = 0;
    public int ammoPerShot = 1;
    public int pellets = 1;
    public float aimDownSpeed = 0.1f;
    [Header("Specifics")]
    public bool packed = false;
    public bool ads = false;
    public bool auto = false;
    public bool shootsThroughTarget = false;
    [Header("External Gameobjects")]
    public GameObject hitEffectPlaceHolder = null;
    public GameObject bulletCast = null;
    public Transform castSpawnPoint = null;

    float impactForce = 1000f;
    bool canShoot = false;

    RaycastHit hit;
    RaycastHit[] hits;
    Player player = null;
    Animator animator = null;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
        animator.SetFloat("Shooting Speed", fireRate);
    }

    private void Update()
    {
        AimDown();
        ProcessReload();
        Fire();
        animator.SetBool("Moving", player.moving);
        animator.SetBool("Running", player.running);
        if (player.running == true)
        animator.SetBool("Reloading", false);
    }

    private void ProcessReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && magazineAmmo != magazineSize)
        {
            if (currentAmmo == 0)
                return;
            if (player.CheckOwnDrinks(DrinkNames.SpeedCola))
                animator.SetFloat("Reload Speed", reloadSpeed * sCReloadSpeed);
            else
                animator.SetFloat("Reload Speed", reloadSpeed);
            animator.SetBool("Reloading", true);
        }
    }

    private void Fire()
    {
        if (auto == false)
        {
            if (Input.GetMouseButtonDown(0))
                Shoot();
        }
        else
        {
            if (Input.GetMouseButton(0))
                Shoot();
        }

        if (!Input.GetMouseButton(0))
            animator.SetBool("Shooting", false);
    }

    private void AimDown()
    {
        if (Input.GetMouseButton(1))
            ads = true;
        else
            ads = false;
        animator.SetBool("ADS", ads);
    }

    public void Shoot()
    {
        if (magazineAmmo == 0 || canShoot == false || player.running == true || animator.GetBool("Reloading") == true)
            return;
        animator.SetBool("Shooting", true);
    }

    public void Shooting()//Animation Event
    {
        ConsumeAmmo();
        for (int i = 0; i < pellets; i++)
        {
            if (ads == false)
            {
                if (Physics.Raycast(Camera.main.transform.position, GetOffsetDirection(hipSpread), out hit, range, -1 ,QueryTriggerInteraction.Ignore))
                    HitTarget();
            }
            else
            {
                if (Physics.Raycast(Camera.main.transform.position, GetOffsetDirection(aDSSpread), out hit, range, -1, QueryTriggerInteraction.Ignore))
                    HitTarget();
            }
        }
    }

    public void CanShootAgain()//Animation Event
    {
        animator.SetBool("Shooting", false);
        canShoot = true;
    }

    private Vector3 GetOffsetDirection(float spread)
    {
        float distance = Vector3.Distance(Player.hit.point, Player.GetRay().origin);
        float xDirection = Player.hit.point.x + Random.Range(-spread, spread) * distance;
        float yDirection = Player.hit.point.y + Random.Range(-spread, spread) * distance;
        float zDirection = Player.hit.point.z + Random.Range(-spread, spread) * distance;

        Vector3 offsetPosition = new Vector3(xDirection, yDirection, zDirection);
        Vector3 offsetDirection = Vector3.Normalize(offsetPosition - Camera.main.transform.position);
        print("Hitting ");
        return offsetDirection;
    }

    public void ConsumeAmmo()
    {
        magazineAmmo = Mathf.Max(0, magazineAmmo - ammoPerShot);
    }

    public void Reload() //Animation Event
    {
        animator.SetBool("Reloading", false);
        int ammoToConsume = magazineSize - magazineAmmo;
        magazineAmmo = Mathf.Min(magazineSize, magazineAmmo + currentAmmo);
        currentAmmo = Mathf.Max(0, currentAmmo - ammoToConsume);

        if (player.CheckOwnDrinks(DrinkNames.ElectricCherry))
        {
            //Do Damage to every zombie around the player
        }
    }

    void HitTarget()
    {
        //create a line in the ray direction to simulate bullet direction
        var hitEffect = Instantiate(hitEffectPlaceHolder, hit.point, Quaternion.LookRotation(hit.normal));
        hitEffect.transform.position = hitEffect.transform.position + (hitEffect.transform.forward * 0.01f);
        var corpseRB = hit.transform.GetComponent<Rigidbody>();
        if (corpseRB != null)
            corpseRB.AddForce(-hit.normal * impactForce);
        Destroy(hitEffect, 1f);
        DamageTarget();
    }

    private void DamageTarget()
    {
        Health health = hit.transform.GetComponentInParent<Health>();
        if (health == null) return;
        health.TakeDamage(CalculateDamage());
    }

    private int CalculateDamage()
    {
        int damageToDo = 0;
        string objectTag = hit.transform.tag;
        switch (objectTag)
        {
            case "Head":
                damageToDo = damage * 2;
                break;
            case "Extremity":
                damageToDo = damage / 2;
                break;
            case "Torso":
                damageToDo = damage;
                break;
        }
        if (GetComponentInParent<Player>().CheckOwnDrinks(DrinkNames.DoubleTap))
            damageToDo *= 2;
        return damageToDo;
    }
    
    public void SpawnBulletCast()//Animation Event
    {
        Instantiate(bulletCast, castSpawnPoint.position, castSpawnPoint.rotation);
    }

    public void ReadyWeapon()//Animation Event
    {
        //make canshoot false by default for this
        canShoot = true;
    }

    public void PutAway()
    {
        if (player == null) return;
        if (player.CheckOwnDrinks(DrinkNames.SpeedCola))
            animator.SetFloat("Put Away Speed", 2);
        else
            animator.SetFloat("Put Away Speed", 1);
        animator.SetBool("Reloading", false);
        animator.SetTrigger("Put Away");
    }

    public void Deactivation()//Animation Event
    {
        gameObject.SetActive(false);
        player.ActivateCurrentWeapon();
        //Activate next weapon
    }

    private void OnDisable()
    {
        animator.Rebind();
    }

    private void OnEnable()
    {
        if (player == null) return;
        if (player.CheckOwnDrinks(DrinkNames.SpeedCola))
            animator.SetFloat("Take out speed", 2);
        else
            animator.SetFloat("Take out speed", 1);
    }

    public void MaxOutAmmo()
    {
        currentAmmo = maxAmmo;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Player.GetRay().origin, Player.hit.point);
    }
}
