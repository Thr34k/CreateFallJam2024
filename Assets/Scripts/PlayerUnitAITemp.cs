using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUnitAITemp : MonoBehaviour
{
    [Header("Necessary references")]
    public GameObject targetEnemy;
    public GameObject bulletPrefab;

    [Header("Movement variables")]
    public float turningSpeed;

    [Header("Shooting Variables")]
    public bool canShoot = true;
    public float timeToReload = 2.5f;
    public float remainingReloadTime = 0f;

    void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        remainingReloadTime = timeToReload;
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

    void Init()
    {
        targetEnemy = GameObject.FindGameObjectWithTag("GhoulUnit");    
    }
}
