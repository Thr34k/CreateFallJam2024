using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitCore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string unitName;
    #region Level

    public int level = 1;
    public float xp = 0f;
    public float xpToLevel = 10;

    #endregion
    public float health = 100f;

    #region Combat
    public float damage = 10f;
    public float reloadSpeed = 10f;
    public float fireRate = 2f;
    public int MagazineSize = 5;
    #endregion

    public float movementSpeed;
    
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
        var randomRange = Random.Range(0, 1);
        //switch (randomRange)
    }
}
