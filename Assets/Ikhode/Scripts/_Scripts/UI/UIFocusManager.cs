using UnityEngine;
using UnityEngine.EventSystems;

public class UIFocusManager : MonoBehaviour
{
    public GameObject firstSelectedUI;

    void Update()
    {
        // Check if the EventSystem has lost focus
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Check if the user presses the keyboard input to refocus (e.g., Tab or a specific key)
            if (Input.GetKeyDown(KeyCode.Tab)) // You can change Tab to another key
            {
                // Set the first UI element to be selected
                EventSystem.current.SetSelectedGameObject(firstSelectedUI);
            }
        }
    }
}
