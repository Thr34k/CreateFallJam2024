using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Playing, Paused, GameOver, Victory }
    public GameState currentState = GameState.Playing;

    public GameObject gameOverText;

    public int ghouldDeaths = 0;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOverText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

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
