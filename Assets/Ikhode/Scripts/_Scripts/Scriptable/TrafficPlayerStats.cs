using UnityEngine;

[CreateAssetMenu(fileName = "New Traffic Player Stats", menuName = "Custom/Player Traffic Stats")]
public class TrafficPlayerStats : ScriptableObject
{
    public float NoLeftTurn;
    public float NoRightTurn;
    public float NoUTurns;
    public float NoEntry;
    public float NoStopping;
    public float SpeedLimit20kmh;

}
