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
    public float fireRate = 2f;
    public int MagazineSize = 5;
    #endregion

    [Header("Shooting Variables")]
    public bool canShoot = true;
    public float characterReloadTime = 2.5f;
    public float remainingReloadTime = 0;

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
            if (angleToTarget < 1f && canShoot)
            {
                Shoot();
            }
        }
        else 
        {
            FindNewTarget();
        }
    }

    private void FindNewTarget()
    {
        var viableTargets = GameObject.FindGameObjectsWithTag("GhoulUnit").ToList();
        targetEnemy = viableTargets.FirstOrDefault();
    }

    void Init()
    {
        targetEnemy = GameObject.FindGameObjectWithTag("GhoulUnit");
        characterInstance = this.gameObject;
        projectileSpawn = this.gameObject.transform.GetChild(0).gameObject;
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
        var spawnedProjectile = Instantiate(bulletPrefab, projectileSpawn.transform.position, this.gameObject.transform.rotation);
        canShoot = false;
        remainingReloadTime = characterReloadTime;
        StartCoroutine("StartReloading");
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
                float fireRateIncrease = UnityEngine.Random.Range(0.1f, 0.6f);
                fireRate += fireRateIncrease;
                break;

            case 3:
                float reloadSpeedIncrease = UnityEngine.Random.Range(5f, 10.1f);
                reloadSpeed += reloadSpeedIncrease;
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
