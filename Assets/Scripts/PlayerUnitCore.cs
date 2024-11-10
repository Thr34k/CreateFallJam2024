using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PlayerUnitCore : MonoBehaviour
{
    #region Events
    public event Action<GameObject> PlayerUnitDied;
    #endregion

    [Header("Necessary references and fields")]
    public GameObject targetEnemy;
    public GameObject bulletPrefab;
    public string unitName;
    public GameObject characterInstance;
    public GameObject projectileSpawn;
    public Weapon currentWeapon;
    public Vector2 targetPosition;
    public CharacterMenu UnitMenu;
    public PlayerUnitCore playerUnitComponent;
    #region Level
    [Header("Experience")]

    public int level = 1;
    public float xp = 0f;
    public float xpToLevel = 10;
    public float xpModifier = 1f;

    #endregion

    [Header("Stats")]
    public float health = 100f;
    public int maxHealth = 100;

    #region Combat
    public float damage = 10f;
    public float reloadSpeed = 10f;
    public float fireRateModifier = 0f;
    public float playerFireRate;
    public int MagazineSize = 5;
    public int currentAmmo = 0;
    #endregion

    [Header("Shooting Variables")]
    public bool reloaded = true;
    public float characterReloadTime = 2.5f;
    public float remainingReloadTime = 0;
    public bool canShoot = true;
    public float timeUntilNextShot;

    [Header("Movement variables")]
    public float turningSpeed;
    public float movementSpeed;

    [Header("Debugging variables")]
    public bool debug = true;
    GUIStyle gui = new GUIStyle();

    void Awake()
    {
        Init();    
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnGUI()
    {
        if (debug) 
        {
            gui.fontSize = 25;
            gui.normal.textColor = Color.green;
            GUI.BeginGroup(new Rect(10, 10, 600, 350));
            GUI.Label(new Rect(10, 25, 500, 30), $"PlayerUnits Current Level: {level}", gui);
            GUI.EndGroup();
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (targetEnemy != null)
        {
            float angleToTarget = Turn();
            if (angleToTarget < 1f && reloaded)
            {
                Shoot();
            }
        }
        else 
        {
            FindNewTarget();
        }

        MoveCharacter();
    }

    private void FindNewTarget()
    {
        var viableTargets = GameObject.FindGameObjectsWithTag("GhoulUnit").ToList();
        targetEnemy = viableTargets.FirstOrDefault();
    }

    void Init()
    {
        targetEnemy = GameObject.FindGameObjectWithTag("GhoulUnit");
        playerUnitComponent = GetComponent<PlayerUnitCore>();
        characterInstance = this.gameObject;
        projectileSpawn = this.gameObject.transform.GetChild(0).gameObject;
        currentWeapon = this.gameObject.transform.GetChild(1).gameObject.GetComponent<Weapon>();
        currentAmmo = MagazineSize;
        timeUntilNextShot = 0f;
        targetPosition = transform.position;
    }

    public void SetWeapon(Sprite weaponSprite, Sprite projectileSprite, float weaponDamage, float weaponReloadSpeed, float weaponFireRate, int weaponMagazineSize) 
    {
        CalculateStatsFromNewWeapon(weaponDamage, weaponReloadSpeed, weaponFireRate, weaponMagazineSize);
        if (UnitMenu != null) 
        {
            UnitMenu.UpdateCharacterMenu(this);
        }
        bulletPrefab.GetComponent<SpriteRenderer>().sprite = projectileSprite;
        currentWeapon.SwitchWeapon(sprite: weaponSprite, _baseDamage: weaponDamage, _baseReloadSpeed: weaponReloadSpeed, _baseFireRate: weaponFireRate, _baseMagazineSize: weaponMagazineSize);
    }

    private void CalculateStatsFromNewWeapon(float newDamage, float newReloadSpeed, float newFirerate, int newWeaponMagazineSize)
    {
        //TODO: There is a bug with how the stats are calculated here, because we dont have weapon stats and player stats separated

        //Calculate base damage
        damage = damage + (newDamage - currentWeapon.baseDamage);

        //Set base reloadspeed
        reloadSpeed = reloadSpeed + (newReloadSpeed - currentWeapon.baseReloadSpeed);

        //Calculate base firerate
        playerFireRate = newFirerate + (fireRateModifier * newFirerate);

        //Calculate magazineSize;
        MagazineSize = MagazineSize + (newWeaponMagazineSize - currentWeapon.baseMagazineSize);
    }

    public void MoveCharacter()
    {
        Debug.Log($"{gameObject.name}: Moving position: {targetPosition}");
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
    }

    float Turn()
    {
        var targetDirection = targetEnemy.transform.position - transform.position;
        var angleToTarget = Vector3.Angle(transform.right, targetDirection);
        transform.right = Vector3.MoveTowards(transform.right, targetDirection, turningSpeed * Time.deltaTime);
        return angleToTarget;
    }

    void Shoot()
    {
        if (canShoot) 
        { 
            var spawnedProjectile = Instantiate(bulletPrefab, projectileSpawn.transform.position, this.gameObject.transform.rotation);
            ProjectileBehavior projectile = spawnedProjectile.GetComponent<ProjectileBehavior>();
            if (projectile != null) 
            { 
                projectile.SetShooter(this);
                var projectileDamage = damage;
            }
            else { throw new Exception(); }
            currentAmmo--;
            canShoot = false;
            timeUntilNextShot = playerFireRate;
            StartCoroutine("WaitForNextShot");
        }


        if (currentAmmo <= 0)
        {
            reloaded = false;
            remainingReloadTime = characterReloadTime * currentWeapon.baseReloadSpeed;
            StartCoroutine("StartReloading");
        }
        else
        {
            reloaded = true;

        }

    }
    IEnumerator StartReloading()
    {
        while (remainingReloadTime > 0)
        {
            remainingReloadTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (remainingReloadTime <= 0)
        {
            currentAmmo = MagazineSize;
            reloaded = true;
        }
    }

    IEnumerator WaitForNextShot() 
    {
        while (timeUntilNextShot > 0) 
        { 
            timeUntilNextShot -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (timeUntilNextShot <= 0) 
        {
            canShoot = true;
        }
    }

    public void TakeDamage(int damageToTake) 
    {
        health -= damageToTake;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        PlayerUnitDied?.Invoke(this.gameObject);
        PlayerUnitDied = null;
        Destroy(this.gameObject);
    }


    public void GainXp(float amount)
    {
        xp += amount;
        if(xp >= xpToLevel)
        {
            LevelUp();           
        }
    }

    public void LevelUp()
    {
        level++;
        var diff = xp - xpToLevel;
        xp = diff;
        xpToLevel *= 1.2f;

        IncreaseRandomStat();

        if (UnitMenu != null) 
        {
            UnitMenu.UpdateCharacterMenu(this);
        }
    }

    public void IncreaseRandomStat()
    {
        var randomRange = UnityEngine.Random.Range(0, 6);
        switch (randomRange)
        {
            case 0:
                int healthIncrease = UnityEngine.Random.Range(5, 21);
                maxHealth += healthIncrease;
                health = health + healthIncrease;
                break;

            case 1:
                float damageIncrease = UnityEngine.Random.Range(2, 6);
                damage += damageIncrease;
                break;

            case 2:
                float fireRateIncrease = UnityEngine.Random.Range(0.01f, 0.06f);
                fireRateModifier = Mathf.Clamp(fireRateModifier + fireRateIncrease, 0.01f, 0.9f);
                break;

            case 3:
                float reloadSpeedIncrease = UnityEngine.Random.Range(0.01f, 0.06f);
                reloadSpeed = Mathf.Clamp(reloadSpeed + reloadSpeedIncrease, 0.01f, 0.9f);
                break;

            case 4:
                int magazineSizeIncrease = UnityEngine.Random.Range(1, 3);
                MagazineSize += magazineSizeIncrease;
                break;

            case 5:
                float xpModifierIncrease = UnityEngine.Random.Range(0.05f, 0.16f);
                xpModifier += xpModifierIncrease;
                break;
        }
    }
}
