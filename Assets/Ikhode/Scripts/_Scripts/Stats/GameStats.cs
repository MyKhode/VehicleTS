using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "Game/Game Stats")]
public class GameStatsSO : ScriptableObject
{
    public enum GameState
    {
        Menu,
        CarSelection,
        Gameplay,
        OpeningSettings,
        Garage,
        SelectVehicle,
        BuyCar,
    }

    public GameState currentState;   // Current game state
    public GameState previousState;  // Previous game state
}
