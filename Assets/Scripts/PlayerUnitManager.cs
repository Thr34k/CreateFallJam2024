using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
        InitializeCharacters();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleCharacterSelection();
        HandleUnitCommands();
    }


    public Camera mainCamera;
    public PlayerUnitCore selectedCharacter; // The currently selected character
    private GameObject selectedCharacterInstance; // The selected character's instance
    private Vector3 targetMovePosition;


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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                CharacterModel clickedCharacter = hit.transform.GetComponent<CharacterModel>();
                if (clickedCharacter != null)
                {
                    SelectCharacter(clickedCharacter);
                }
            }
        }
    }

    // Select the clicked character
    private void SelectCharacter(CharacterModel character)
    {
        selectedCharacter = character;

        // If the character instance is already in the scene, set it as selected
        if (selectedCharacterInstance != null)
        {
            // Optionally change the appearance of selected character (e.g., outline, color change)
            selectedCharacterInstance.GetComponent<Renderer>().material.color = Color.white; // Reset previous selection
        }

        selectedCharacterInstance = selectedCharacter.characterInstance;
        selectedCharacterInstance.GetComponent<Renderer>().material.color = Color.green; // Change color to indicate selection
        Debug.Log($"{selectedCharacter.characterName} is selected.");
    }

    // Handle commands for the selected unit
    private void HandleUnitCommands()
    {
        if (selectedCharacter != null)
        {
            // If the right mouse button is clicked, issue a move command
            if (Input.GetMouseButtonDown(1)) // Right mouse button clicked
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    targetMovePosition = hit.point;
                    MoveSelectedCharacter();
                }
            }
        }
    }

    // Move the selected unit to the target position
    private void MoveSelectedCharacter()
    {
        if (selectedCharacterInstance != null)
        {
            selectedCharacterInstance.GetComponent<CharacterMovement>().MoveToPosition(targetMovePosition);
        }
    }
}


