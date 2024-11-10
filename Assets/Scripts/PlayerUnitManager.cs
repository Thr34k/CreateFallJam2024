using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    List<string> spriteNames = new List<string>();

    void Awake()
    {
        playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn").ToList();
        spriteNames.Add("assaultrifle");
        spriteNames.Add("boltactionrifle");
        spriteNames.Add("doublebarrel");
        spriteNames.Add("energyrifle");
        spriteNames.Add("pumpactionshotgun");
    }

    // Start is called before the first frame update
    private void Start()
    {
        selectedCharacterMenu = GameObject.Find("SelectedCharacterMenu");
        SelectedCharacterMenuComponent = selectedCharacterMenu.GetComponent<CharacterMenu>();
        Button switchWeaponBtn = SelectedCharacterMenuComponent.SwitchWeaponBtn;
        if (switchWeaponBtn != null) 
        {
            switchWeaponBtn.onClick.AddListener(OpenSwitchWeaponsMenu);
        }

        var switchWeaponsMenuObj = selectedCharacterMenu.GetComponent<CharacterMenu>();
        //TODO: Pass bullet sprite to use here as well
        SelectedCharacterMenuComponent.AssaultRifleBtn.onClick.AddListener(() => SwitchWeapon(weaponName: spriteNames[0], weaponDamage: 10, weaponReloadSpeed: 2f, weaponFireRate: .1f, weaponMagazineSize: 30));
        SelectedCharacterMenuComponent.BoltActionRifleBtn.onClick.AddListener(() => SwitchWeapon(spriteNames[1], 30, 1f, .3f, 1));
        SelectedCharacterMenuComponent.DoubleBarrelBtn.onClick.AddListener(() => SwitchWeapon(weaponName: spriteNames[2], weaponDamage: 40, weaponReloadSpeed: 2.5f, weaponFireRate: .15f, weaponMagazineSize: 2));
        SelectedCharacterMenuComponent.EnergyRifleBtn.onClick.AddListener(() => SwitchWeapon(weaponName: spriteNames[3], weaponDamage: 15, weaponReloadSpeed: 1.5f, weaponFireRate: .2f, weaponMagazineSize: 15));
        SelectedCharacterMenuComponent.PumpActionShotgunBtn.onClick.AddListener(() => SwitchWeapon(weaponName: spriteNames[4], weaponDamage: 35, weaponReloadSpeed: 4f, weaponFireRate: .4f, weaponMagazineSize: 8));

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

        var spawnedEnemy = Instantiate(playerUnitPrefab, nextSpawnToUse.transform.position, playerUnitPrefab.transform.rotation);
        spawnedEnemy.GetComponent<PlayerUnitCore>().PlayerUnitDied += HandlePlayerUnitDied;
        activePlayerUnits.Add(spawnedEnemy);
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

    void SwitchWeapon(string weaponName, float weaponDamage, float weaponReloadSpeed, float weaponFireRate, int weaponMagazineSize) 
    { 
        //TODO Pass bullet to use here as well
        Sprite loadedSprite = Resources.Load<Sprite>("Sprites/CreateJamFall2024/Weapons/" + weaponName);
        selectedCharacter.SetWeapon(loadedSprite, weaponDamage, weaponReloadSpeed, weaponFireRate, weaponMagazineSize);
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


