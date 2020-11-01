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
    public float fireRate = 0.5f;
    public float reloadSpeed = 1f;
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
    bool canShoot = true;

    RaycastHit hit;
    RaycastHit[] hits;
    Player player = null;
    Animator animator = null;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
        animator.SetFloat("Shoot Animation Speed", fireRate);
    }

    private void Update()
    {
        AimDown();

        if (Input.GetKeyDown(KeyCode.R) && magazineAmmo != magazineSize)
            Reload();//replace for animation trigger

        Fire();
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
        if (magazineAmmo == 0 || canShoot == false) return;
        animator.SetTrigger("Shoot");
        canShoot = false;
        ConsumeAmmo();
        for (int i = 0; i < pellets; i++)
        {
            if (ads == false)
                if (Physics.Raycast(Camera.main.transform.position, GetOffsetDireaction(hipSpread), out hit, range))
                    HitTarget();

            else if (Physics.Raycast(Camera.main.transform.position, GetOffsetDireaction(aDSSpread), out hit, range))
                HitTarget();
        }
    }

    public void CanShootAgain()//Animation Event
    {
        canShoot = true;
    }

    private Vector3 GetOffsetDireaction(float spread)
    {
        float distance = Vector3.Distance(Player.hit.point, Player.GetRay().origin);
        float xDirection = Player.hit.point.x + Random.Range(-spread, spread) * distance;
        float yDirection = Player.hit.point.y + Random.Range(-spread, spread) * distance;
        float zDirection = Player.hit.point.z + Random.Range(-spread, spread) * distance;

        Vector3 offsetPosition = new Vector3(xDirection, yDirection, zDirection);
        Vector3 offsetDirection = Vector3.Normalize(offsetPosition - Camera.main.transform.position);
        return offsetDirection;
    }

    public void ConsumeAmmo()
    {
        magazineAmmo = Mathf.Max(0, magazineAmmo - ammoPerShot);
    }

    public void Reload() //Animation Event
    {
        int ammoToConsume = magazineSize - magazineAmmo;
        currentAmmo = Mathf.Max(0, currentAmmo - ammoToConsume);
        magazineAmmo = Mathf.Min(magazineSize, magazineAmmo + currentAmmo);
        if (player.CheckOwnDrinks(DrinkNames.SpeedCola))
        {
            //increase reload animtion speed
        }
        if (player.CheckOwnDrinks(DrinkNames.ElectricCherry))
        {
            //Do Damage to every zombie around the player
        }
    }

    void HitTarget()
    {
        //create a line in the ray direction to simulate bullet direction
        var hitEffect = Instantiate(hitEffectPlaceHolder, hit.point, Quaternion.LookRotation(hit.normal));
        var corpseRB = hit.transform.GetComponent<Rigidbody>();
        if (corpseRB != null)
            corpseRB.AddForce(-hit.normal * impactForce);
        Destroy(hitEffect, 0.2f);
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

    public void MaxOutAmmo()
    {
        currentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        // animator.SetTrigger("ReadyUp");
    }

    private void OnDisable()
    {
        // canShoot = false; uncomment after making the ready up animation
        animator.Rebind();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Player.GetRay().origin, Player.hit.point);
    }
}
