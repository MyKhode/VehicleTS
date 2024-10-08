using UnityEngine;

[CreateAssetMenu(fileName = "New Material Data", menuName = "Custom/Material data")]
public class MaterialVehiclePaint : ScriptableObject
{
    public string MaterialName;
    public int Material_Id;
    public Material material;
}
