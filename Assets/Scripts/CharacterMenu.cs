using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    public TextMeshProUGUI characterLevelTxt;
    public TextMeshProUGUI characterNameTxt;
    public TextMeshProUGUI characterCurrentHealthTxt;

    public TextMeshProUGUI characterMaxHealthTxt;
    public TextMeshProUGUI characterMagazineCapacityTxt;
    public TextMeshProUGUI characterFireRateTxt;
    public TextMeshProUGUI characterReloadTxt;

    public GameObject SwitchWeaponsMenu;
    public Button SwitchWeaponBtn;
    public Button AssaultRifleBtn;
    public Button BoltActionRifleBtn;
    public Button EnergyRifleBtn;
    public Button DoubleBarrelBtn;
    public Button PumpActionShotgunBtn;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        SwitchWeaponBtn = GameObject.Find("SwitchWeapon").GetComponent<Button>();
        AssaultRifleBtn = GameObject.Find("SwitchWeaponAssaultRifle").GetComponent<Button>();
        BoltActionRifleBtn = GameObject.Find("SwitchWeaponBoltActionRifle").GetComponent<Button>();
        EnergyRifleBtn = GameObject.Find("SwitchWeaponEnergyRifle").GetComponent<Button>();
        DoubleBarrelBtn = GameObject.Find("SwitchWeaponDoubleBarrel").GetComponent<Button>();
        PumpActionShotgunBtn = GameObject.Find("SwitchWeaponPumpActionShotgun").GetComponent<Button>();

        characterLevelTxt = GameObject.Find("CharacterLevel").GetComponent<TextMeshProUGUI>();
        characterNameTxt = GameObject.Find("CharacterName").GetComponent<TextMeshProUGUI>();
        characterCurrentHealthTxt = GameObject.Find("CharacterCurrentHealth").GetComponent<TextMeshProUGUI>();

        characterMaxHealthTxt = GameObject.Find("CharacterMaxHealth").GetComponent<TextMeshProUGUI>();
        characterMagazineCapacityTxt = GameObject.Find("CharacterMagazineCapacity").GetComponent<TextMeshProUGUI>();
        characterFireRateTxt = GameObject.Find("CharacterFirerate").GetComponent<TextMeshProUGUI>();
        characterReloadTxt = GameObject.Find("CharacterReloadSpeed").GetComponent<TextMeshProUGUI>();
        SwitchWeaponsMenu = GameObject.Find("SwitchWeaponsMenu");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCharacterMenu(PlayerUnitCore character) 
    {
        characterNameTxt.text = $"{character.name}";
        characterLevelTxt.text = $"Level: {character.level}";
        characterCurrentHealthTxt.text = $"Current hp: {character.health}";

        characterMaxHealthTxt.text = $"Max hp{character.maxHealth}";
        characterMagazineCapacityTxt.text = $"Mag size: {character.MagazineSize}";
        characterFireRateTxt.text = $"Firerate: {character.fireRate}";
        characterReloadTxt.text = $"Reload Speed: {character.reloadSpeed}";
    }
}
