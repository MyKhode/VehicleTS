using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ActivateVibration : MonoBehaviour
{
    private void Update()
    {
        // Check if a gamepad is connected
        if (Gamepad.current != null)
        {
            // Check if any button is pressed on the gamepad
            if (Gamepad.current.allControls.Any(control => control is ButtonControl button && button.isPressed))
            {
                // Set the motor speeds for vibration (left and right motors)
                Gamepad.current.SetMotorSpeeds(0.5f, 0.5f); // 50% strength for both motors

                // Start a coroutine to stop the vibration after 1 second
                StartCoroutine(StopVibrationAfterDelay(1.0f));

                Debug.Log("Gamepad button pressed, starting vibration.");
            }
        }
        else
        {
            Debug.LogWarning("No gamepad connected.");
        }
    }

    private IEnumerator StopVibrationAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Stop the vibration by setting motor speeds to 0
        Gamepad.current.SetMotorSpeeds(0, 0);

        Debug.Log("Vibration stopped.");
    }
}
