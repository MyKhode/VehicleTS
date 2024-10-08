using UnityEngine;

public class DrivingObstacle : MonoBehaviour
{
    public int scorePenaltyOnTouch = 2;
    public int scorePenaltyOnPositionChange = 5;
    public int scorePenaltyOnCrash = 10;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool hasAppliedPenalty = false; // Flag to track if the penalty has been applied

    // Define the layer mask for vehicles
    public LayerMask vehicleLayerMask;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Check if the object has moved or rotated significantly
        float positionChange = Vector3.Distance(initialPosition, transform.position);
        float rotationChange = Quaternion.Angle(initialRotation, transform.rotation);

        if (!hasAppliedPenalty && (positionChange > 0.1f || rotationChange > 15f))
        {
            ApplyPenalty(scorePenaltyOnCrash);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is on the vehicle layer
        if ((vehicleLayerMask & (1 << collision.gameObject.layer)) != 0)
        {
            float impactForce = collision.relativeVelocity.magnitude;

            if (!hasAppliedPenalty)
            {
                if (impactForce < 2f)
                {
                    ApplyPenalty(scorePenaltyOnTouch);
                }
                else if (impactForce < 5f)
                {
                    ApplyPenalty(scorePenaltyOnPositionChange);
                }
                else
                {
                    ApplyPenalty(scorePenaltyOnCrash);
                }
            }
        }
    }

    void ApplyPenalty(int penalty)
    {
        UpdateScore(penalty);
        hasAppliedPenalty = true; // Set flag to true after applying penalty
        // Reset the position and rotation to avoid multiple penalties
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void UpdateScore(int penalty)
    {
        // Assuming you have a game manager or score manager to handle the score
        LicenceDrivingTest.PlayerLicenceScore -= penalty;
    }
}
