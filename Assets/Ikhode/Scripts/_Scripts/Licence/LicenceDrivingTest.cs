using UnityEngine;

public class LicenceDrivingTest : MonoBehaviour
{
    public static int PlayerLicenceScore = 0; // Initialize the score
    public Collider exitCollider;
    public GameObject driveLicenceStatsUI;
    public LayerMask vehicleLayerMask;

    void Start()
    {
        if (driveLicenceStatsUI != null)
        {
            driveLicenceStatsUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is on the vehicle layer
        if ((vehicleLayerMask & (1 << other.gameObject.layer)) != 0)
        {
            if (driveLicenceStatsUI != null)
            {
                driveLicenceStatsUI.SetActive(true);
                Debug.Log("Licence Score = " + PlayerLicenceScore + " pts");
            }
        }
    }
}
