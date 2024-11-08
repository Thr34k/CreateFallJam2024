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
    public float xp = 0;
    public float xpToLevel = 10;

    #endregion
    public int health;

    #region Combat
    public float damage;
    public float reloadSpeed;

    #endregion
    public float speed;
    


}
