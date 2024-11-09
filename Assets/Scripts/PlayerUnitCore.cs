using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerUnitCore : MonoBehaviour
{

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

    public float movementSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
