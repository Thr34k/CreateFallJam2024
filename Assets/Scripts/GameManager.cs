using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Playing, Paused, GameOver, Victory }
    public GameState currentState = GameState.Playing;

    public GameObject gameOverText;

    public int ghouldDeaths = 0;
    public int gameDifficulty = 1;

    public int playerCurrency = 100;

    public GameObject playerCurrecnyTxt;
    public GameObject nexusHealthTxt;
    public GameObject nexus;

    [Header("Spawnpoints")]
    List<GameObject> playerSpawns = new List<GameObject>();

    [Header("AI references")]
    List<GameObject> activePlayerUnits = new List<GameObject>();

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        nexus = GameObject.FindGameObjectWithTag("Nexus");
        playerCurrecnyTxt = GameObject.Find("PlayerCurrency");
        nexusHealthTxt = GameObject.Find("NexusHealth");
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOverText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        playerCurrecnyTxt.GetComponent<TextMeshProUGUI>().text = "Currency: " + playerCurrency;
        nexusHealthTxt.GetComponent<TextMeshProUGUI>().text = "Remaining eye health: " + nexus.GetComponent<Nexus>().currentHealth;
    }

    public void GhoulDeatchCounter()
    {
        ghouldDeaths++;
        this.playerCurrency += 10;
        if (ghouldDeaths % 20 == 0)
        {
            this.IncreaseDifficulty();
        }
        //playerCurrency += 10;
    }

    private void IncreaseDifficulty()
    {
        gameDifficulty++;

        MonsterUnitManager.Instance.maxActiveGhouls += 1;
        MonsterUnitManager.Instance.ghoulStartingHealth += 10;
    }

    public void EndGame(bool victory)
    {
        currentState = victory ? GameState.Victory : GameState.GameOver;
        if (currentState == GameState.GameOver)
        {
            ShowGameOverUI();
        }
        Time.timeScale = 0;  // Pause the game
        Debug.Log(victory ? "Victory!" : "Game Over! The building has been destroyed.");

        // skal lige lave en UI Endscreen der popper her!!
    }

    private void ShowGameOverUI()
    {
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);  // Make the Game Over UI visible
        }
        else
        {
            Debug.LogWarning("Game Over UI is not assigned in the GameManager.");
        }
    }
}
