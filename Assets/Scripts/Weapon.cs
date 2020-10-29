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
    [Range(0,1)]
    public float spread = 5;
    public float reloadSpeed = 1f;
    public int ammoPerShot = 1;
    public int pellets = 1;
    public float fireRate = 0.5f;
    public float recoil = 5;
    public float range = 40;
    public float aimDownSpeed = 0.1f;

    [Header("Specifics")]
    public bool packed = false;
    public bool ads = false;
    public bool auto = false;
    public bool shootsThroughTarget = false;
    public GameObject hitEffectPlaceHolder = null;
    public Vector3 adsPosition = Vector3.zero;

    float impactForce = 1000f;
    float timeSinceLastShot = Mathf.Infinity;
    Vector3 startingPosition = Vector3.zero;

    RaycastHit hit;
    RaycastHit[] hits;
    Player player = null;

    private void Start()
    {
        startingPosition = transform.localPosition;
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        AimDown();

        if (Input.GetKeyDown(KeyCode.R) && magazineAmmo != magazineSize)
            Reload();//replace for animation trigger

        Fire();
    }

    private void Fire()
    {
        if (timeSinceLastShot < fireRate) return;
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
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, adsPosition, aimDownSpeed);
            ads = true;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startingPosition, aimDownSpeed);
            ads = false;
        }
    }

    public void Shoot()
    {
        if (magazineAmmo == 0) return;
        ConsumeAmmo();
        timeSinceLastShot = 0;
        
        for (int i = 0; i < pellets; i++)
        {
            //replace for actual muzzle flash
            hitEffectPlaceHolder.GetComponent<ParticleSystem>().Play();
            //create a spark going in the ray direction coming out the gun
            float distance = Vector3.Distance(Player.hit.point, Player.GetRay().origin);
            float xDirection = Player.hit.point.x + Random.Range(-spread, spread) * distance;
            float yDirection = Player.hit.point.y + Random.Range(-spread, spread) * distance;
            float zDirection = Player.hit.point.z + Random.Range(-spread, spread) * distance;

            Vector3 offsetPosition = new Vector3(xDirection, yDirection, zDirection);
            Vector3 offsetDirection = Vector3.Normalize(offsetPosition - Camera.main.transform.position);

            if (ads == false)
            {
                if (Physics.Raycast(Camera.main.transform.position, offsetDirection, out hit, range))
                    HitTarget();
            }
            else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
            {
                HitTarget();
            }
        }
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

    public void MaxOutAmmo()
    {
        currentAmmo = maxAmmo;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Player.GetRay().origin, Player.hit.point);
    }
}
