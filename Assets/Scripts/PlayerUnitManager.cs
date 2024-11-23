using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerUnitManager : MonoBehaviour
{
    [Header("Spawnpoints")]
    List<GameObject> playerSpawns = new List<GameObject>();

    [Header("AI references")]
    List<GameObject> activePlayerUnits = new List<GameObject>();
    public GameObject nextSpawnToUse;
    public GameObject playerUnitPrefab;
    public int maxPlayerUnitCount;

    [Header("Selected character variables")]
    public GameObject selectedCharacterMenu;
    public CharacterMenu SelectedCharacterMenuComponent;
    public bool openSwitchWeaponsMenu;
    [Header("SwitchWeapons Text Fields")]
    public TextMeshProUGUI assaultRifleCostText;
    public TextMeshProUGUI boltRifleCostText;
    public TextMeshProUGUI doubleBarrelCostText;
    public TextMeshProUGUI energyRifleCostText;
    public TextMeshProUGUI pumpActionCostText;


    string[] possibleUnitNames = new string[] { "Bob", "John", "Eric", "Terrence", "Derrek", "The Dude", "Jeremy", "Broski" };

    List<(string weaponName, string projectileName)> spriteNames = new List<(string, string)>();

    void Awake()
    {
        playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn").ToList();
        spriteNames.Add(("assaultrifle", "assault_rifle_bullet_large"));
        spriteNames.Add(("boltactionrifle", "boltaction_rifle_bullet_large"));
        spriteNames.Add(("doublebarrel", "shotgun_shot_large"));
        spriteNames.Add(("energyrifle", "energy_rifle_bullet"));
        spriteNames.Add(("pumpactionshotgun", "shotgun_shot_large"));
    }

    // Start is called before the first frame update
    private void Start()
    {
        selectedCharacterMenu = GameObject.Find("SelectedCharacterMenu");
        assaultRifleCostText = GameObject.Find("SwitchWeaponAssaultRifleText").GetComponent<TextMeshProUGUI>();
        //boltRifleCostText = GameObject.Find("SwitchWeaponBoltActionRifleText").GetComponent<TextMeshProUGUI>();
        doubleBarrelCostText = GameObject.Find("SwitchWeaponDoubleBarrelText").GetComponent<TextMeshProUGUI>();
        energyRifleCostText = GameObject.Find("SwitchWeaponEnergyRifleText").GetComponent<TextMeshProUGUI>();
        pumpActionCostText = GameObject.Find("SwitchWeaponPumpActionShotgunText").GetComponent<TextMeshProUGUI>();
        SelectedCharacterMenuComponent = selectedCharacterMenu.GetComponent<CharacterMenu>();
        Button switchWeaponBtn = SelectedCharacterMenuComponent.SwitchWeaponBtn;
        if (switchWeaponBtn != null) 
        {
            switchWeaponBtn.onClick.AddListener(OpenSwitchWeaponsMenu);
        }

        var switchWeaponsMenuObj = selectedCharacterMenu.GetComponent<CharacterMenu>();
        SelectedCharacterMenuComponent.AssaultRifleBtn.onClick.AddListener(() => SwitchWeapon(spriteTuple: spriteNames[0], weaponDamage: 10, weaponReloadSpeed: 2f, weaponFireRate: .1f, weaponMagazineSize: 30, weaponCost: 250, pelletsPerShot: 1));
        assaultRifleCostText.text += 250;
        //SelectedCharacterMenuComponent.BoltActionRifleBtn.onClick.AddListener(() => SwitchWeapon(spriteTuple: spriteNames[1], weaponDamage: 30, weaponReloadSpeed: 1f, weaponFireRate: .3f, weaponMagazineSize: 1, weaponCost:200, pelletsPerShot: 1));
        //boltRifleCostText.text += 200;
        SelectedCharacterMenuComponent.DoubleBarrelBtn.onClick.AddListener(() => SwitchWeapon(spriteTuple: spriteNames[2], weaponDamage: 40, weaponReloadSpeed: 2.5f, weaponFireRate: .15f, weaponMagazineSize: 2, weaponCost: 200, pelletsPerShot: 5));
        doubleBarrelCostText.text += 200;
        SelectedCharacterMenuComponent.EnergyRifleBtn.onClick.AddListener(() => SwitchWeapon(spriteTuple: spriteNames[3], weaponDamage: 50, weaponReloadSpeed: 1.5f, weaponFireRate: .1f, weaponMagazineSize: 50, weaponCost: 500, pelletsPerShot: 1));
        energyRifleCostText.text += 500;
        SelectedCharacterMenuComponent.PumpActionShotgunBtn.onClick.AddListener(() => SwitchWeapon(spriteTuple: spriteNames[4], weaponDamage: 60, weaponReloadSpeed: 4f, weaponFireRate: .4f, weaponMagazineSize: 8, weaponCost: 200, pelletsPerShot: 1));
        pumpActionCostText.text += 200;

        //NOTE: I use gameObject.SetActive here, because at this point using OpenSwitchWeaponsMenu would cause the menu to be active when the game is started
        SelectedCharacterMenuComponent.SwitchWeaponsMenu.SetActive(false);
        selectedCharacterMenu.gameObject.SetActive(false);    
        mainCamera = Camera.main;
        InitializeCharacters();

        if (addXpButton != null)
        {
            addXpButton.onClick.AddListener(AddXPToSelectedCharacter);  // Add listener to button click
        }
    }

    // Update is called once per frame
    private void Update()
    {
        HandleCharacterSelection();
        HandleUnitCommands();

        if (activePlayerUnits.Count < maxPlayerUnitCount) 
        {
            SpawnPlayerUnit();
        }
    }

    public Button addXpButton;
    public Camera mainCamera;
    public PlayerUnitCore selectedCharacter; // The currently selected character
    private GameObject selectedCharacterInstance; // The selected character's instance
    private Vector3 targetMovePosition;

    void SpawnPlayerUnit() 
    {
        if (nextSpawnToUse != null)
        {
            var viableNextSpawns = playerSpawns.Where(x => x != nextSpawnToUse).ToList();
            int nextSpawnIndex = UnityEngine.Random.Range(0, viableNextSpawns.Count);
            nextSpawnToUse = playerSpawns[nextSpawnIndex];
        }
        else
        {
            int currentSpawnIndex = UnityEngine.Random.Range(0, playerSpawns.Count);
            nextSpawnToUse = playerSpawns[currentSpawnIndex];
        }

        var spawnedPlayer = Instantiate(playerUnitPrefab, nextSpawnToUse.transform.position, playerUnitPrefab.transform.rotation);
        int chosenPlayerNameIndex = UnityEngine.Random.Range(0, possibleUnitNames.Length);
        spawnedPlayer.name = possibleUnitNames[chosenPlayerNameIndex];
        spawnedPlayer.GetComponent<PlayerUnitCore>().PlayerUnitDied += HandlePlayerUnitDied;
        Sprite loadedProjectileSprite = Resources.Load<Sprite>("Sprites/CreateJamFall2024/Weapons/Bullets/" + spriteNames[1].projectileName);
        Sprite loadedWeaponSprite = Resources.Load<Sprite>("Sprites/CreateJamFall2024/Weapons/" + spriteNames[1].weaponName);
        spawnedPlayer.GetComponent<PlayerUnitCore>().SetWeapon(weaponSprite: loadedWeaponSprite, projectileSprite: loadedProjectileSprite, weaponDamage: 30, weaponReloadSpeed: 1f, weaponFireRate: .3f, weaponMagazineSize: 1, pelletsPerShot: 1);
        activePlayerUnits.Add(spawnedPlayer);
    }

    void HandlePlayerUnitDied(GameObject deadPlayerUnit) 
    { 
        activePlayerUnits.Remove(deadPlayerUnit);
    }


    // Initialize characters in the game (can be loaded from a pool or directly in the scene)
    private void InitializeCharacters()
    {
        // Example for initialization - Create characters with their data (e.g., Health, Speed)
        // Instantiate prefabs for each character
    }


    // Handle the selection of a single unit
    private void HandleCharacterSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            if (EventSystem.current.IsPointerOverGameObject()) 
            {
                return;
            }

            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit2D = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit2D.collider)
            {
                PlayerUnitCore clickedCharacter = hit2D.collider.GetComponent<PlayerUnitCore>();
                if (clickedCharacter != null)
                {
                    SelectCharacter(clickedCharacter);
                }
            }
            else 
            { 
                selectedCharacterInstance.GetComponent<Renderer>().material.color = Color.white;
                selectedCharacterMenu.GetComponent<CharacterMenu>().SwitchWeaponsMenu.SetActive(false);
                selectedCharacterMenu.SetActive(false);
                selectedCharacter = null;
            }
        }
    }

    // Select the clicked character
    private void SelectCharacter(PlayerUnitCore character)
    {
        selectedCharacter = character;

        // If the character instance is already in the scene, set it as selected
        if (selectedCharacterInstance != null)
        {
            // Optionally change the appearance of selected character (e.g., outline, color change)
            selectedCharacterInstance.GetComponent<Renderer>().material.color = Color.white; // Reset previous selection
        }

        selectedCharacterInstance = selectedCharacter.characterInstance;
        selectedCharacter.UnitMenu = selectedCharacterMenu.GetComponent<CharacterMenu>();
        selectedCharacter.UnitMenu.UpdateCharacterMenu(selectedCharacter);
        selectedCharacterMenu.gameObject.SetActive(true);
        selectedCharacterInstance.GetComponent<Renderer>().material.color = Color.green; // Change color to indicate selection
    }

    // Handle commands for the selected unit
    private void HandleUnitCommands()
    {
        if (selectedCharacter != null)
        {
            // If the right mouse button is clicked, issue a move command
            if (Input.GetMouseButtonDown(1)) // Right mouse button clicked
            {
                Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log($"Right mousebutton clicked at position {worldPoint}");
                MoveSelectedCharacter(worldPoint);

            }
        }
    }

    // Move the selected unit to the target position
    private void MoveSelectedCharacter(Vector2 positionToMoveTo)
    {
        Debug.Log($"Moving {selectedCharacter} to position {positionToMoveTo}");

        if (selectedCharacterInstance != null)
        {
            selectedCharacterInstance.GetComponent<PlayerUnitCore>().targetPosition = positionToMoveTo;
            selectedCharacterInstance.GetComponent<PlayerUnitCore>().MoveCharacter();
        }
    }

    private void OpenSwitchWeaponsMenu() 
    {
        openSwitchWeaponsMenu = !openSwitchWeaponsMenu;
        SelectedCharacterMenuComponent.SwitchWeaponsMenu.SetActive(openSwitchWeaponsMenu);

    }

    void SwitchWeapon((string weaponName, string projectileName) spriteTuple, float weaponDamage, float weaponReloadSpeed, float weaponFireRate, int weaponMagazineSize, int pelletsPerShot, int weaponCost) 
    {
        if (GameManager.Instance.playerCurrency >= weaponCost) 
        { 
            Sprite loadedWeaponSprite = Resources.Load<Sprite>("Sprites/CreateJamFall2024/Weapons/" + spriteTuple.weaponName);
            Sprite loadedProjectileSprite = Resources.Load<Sprite>("Sprites/CreateJamFall2024/Weapons/Bullets/" + spriteTuple.projectileName);
            selectedCharacter.SetWeapon(loadedWeaponSprite, loadedProjectileSprite, weaponDamage, weaponReloadSpeed, weaponFireRate, weaponMagazineSize, pelletsPerShot: pelletsPerShot);
            GameManager.Instance.playerCurrency -= weaponCost;
        }
    }

    private void AddXPToSelectedCharacter()
    {
        if (selectedCharacter != null)
        {
            int xpAmount = 10; // Amount of XP to give
            selectedCharacter.GainXp(xpAmount);
        }
        else
        {
            Debug.Log("No character selected!");
        }
    }


}


