using UnityEngine;
using UnityEngine.InputSystem;

public class StatsManager : MonoBehaviour
{
    public GameStatsSO gameStats; // Reference to the GameStats ScriptableObject

    [SerializeField]
    private GameStatsSO.GameState selectedState; // Expose the enum in the inspector

    private void OnValidate()
    {
        // Synchronize the selected state with the gameStats
        UpdateGameState();
    }

    private void Start()
    {
        // Set the initial game state
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        if (gameStats != null)
        {
            // Update previousState before changing currentState
            gameStats.previousState = gameStats.currentState;
            gameStats.currentState = selectedState;
        }
    }
}
