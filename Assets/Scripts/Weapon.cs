using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Necessary references")]
    public SpriteRenderer sprite;

    [Header("Gun stats")]
    public float baseDamage = 10f;
    public float baseReloadSpeed = 10f;
    public float baseFireRate = 2f;
    public int baseMagazineSize = 5;


    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchWeapon(SpriteRenderer sprite, float _baseDamage, float _baseReloadSpeed, float _baseFireRate, int _baseMagazineSize) 
    { 
        baseDamage = _baseDamage;
        baseReloadSpeed = _baseReloadSpeed;
        baseFireRate = _baseFireRate;
        baseMagazineSize = _baseMagazineSize;
    }
}
