using UnityEngine;

public class DemoVehicleManager : MonoBehaviour
{
    public Camera _display1; // Camera for Display 1
    public Camera _display2; // Camera for Display 2
    public GameObject clip1;
    public GameObject clip2;

    private void Start()
    {
        Display1();
    }

    // Call this to enable Camera 1 and disable Camera 2
    public void Display1()
    {
        _display1.enabled = true;  // Enable the first camera
        _display2.enabled = false; // Disable the second camera

        // container object in scene clip1
        clip1.SetActive(true);
        clip2.SetActive(false);
    }

    // Call this to enable Camera 2 and disable Camera 1
    public void Display2()
    {
        _display1.enabled = false; // Disable the first camera
        _display2.enabled = true;  // Enable the second camera

        // container object in scene clip2
        clip1.SetActive(false);
        clip2.SetActive(true);
    }
}
