using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerUnitManager))]
public class PlayerCharacterManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector layout
        DrawDefaultInspector();

        // Reference to the PlayerCharacterManager script
        PlayerUnitManager manager = (PlayerUnitManager)target;

        // Check if there's an active character to prevent errors
        if (manager.selectedCharacter != null)
        {
            // Add a button that adds 10 XP to the active character
            if (GUILayout.Button("Add 10 XP"))
            {
                manager.selectedCharacter.GainXp(10);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No active character selected!", MessageType.Warning);
        }
    }
}
