using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerUnitCore : MonoBehaviour
{
    [Header("Necessary references and fields")]
    public GameObject targetEnemy;
    public GameObject bulletPrefab;
    public string unitName;
    public GameObject characterInstance;
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
        float angleToTarget = Turn();
        if (angleToTarget < 1f && canShoot)
        {
            Shoot();
        }
    }

    void Init()
    {
        targetEnemy = GameObject.FindGameObjectWithTag("GhoulUnit");
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
        Debug.Log($"{gameObject.name}: Pew!");
        canShoot = false;
        remainingReloadTime = characterReloadTime;
        Debug.Log($"{gameObject.name}: Reloading!");
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
            Debug.Log($"{gameObject.name}; Reloaded!");
            canShoot = true;
        }
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
        var randomRange = Random.Range(0, 5);
        switch (randomRange)
        {
            case 0:
                int healthIncrease = Random.Range(5, 20);
                maxHealth += healthIncrease;
                health = health + healthIncrease;
                break;

            case 1:
                float damageIncrease = Random.Range(2, 5);
                damage += damageIncrease;
                break;

            case 2:
                float fireRateIncrease = Random.Range(0.1f, 0.5f);
                fireRate += fireRateIncrease;
                break;

            case 3:
                float reloadSpeedIncrease = Random.Range(5f, 10f);
                reloadSpeed += reloadSpeedIncrease;
                break;

            case 4:
                int magazineSizeIncrease = Random.Range(1, 2);
                MagazineSize += magazineSizeIncrease;
                break;

            case 5:
                float xpModifierIncrease = Random.Range(0.05f, 0.15f);
                xpModifier += xpModifierIncrease;
                break;
        }
    }
}
