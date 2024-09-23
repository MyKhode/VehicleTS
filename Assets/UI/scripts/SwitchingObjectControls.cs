using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchingObjectControls : MonoBehaviour
{
    [System.Serializable]
    public class SwitchEvent
    {
        public KeyCode key;
        public UnityEvent onSwitch;
    }

    public GameObject[] object1;
    public GameObject[] object2;
    public SwitchEvent[] switchEvents;
    public bool pauseTimeWhenObject2Active = false;
    public float customTimeScaleWhenObject2Active = 1f;

    private bool isObject1Active = true;

    void Start()
    {
        if (object1 != null && object1.Length > 0 && object2 != null && object2.Length > 0)
        {
            SetActiveObjects(object1, true);
            SetActiveObjects(object2, false);
        }
        else
        {
            Debug.LogWarning("Please assign objects to both arrays in the inspector.");
        }
    }

    void Update()
    {
        for (int i = 0; i < switchEvents.Length; i++)
        {
            if (Input.GetKeyDown(switchEvents[i].key))
            {
                SwitchObjects();
                switchEvents[i].onSwitch.Invoke();
                break;
            }
        }
    }

    void SwitchObjects()
    {
        if (object1 != null && object1.Length > 0 && object2 != null && object2.Length > 0)
        {
            isObject1Active = !isObject1Active;
            SetActiveObjects(object1, isObject1Active);
            SetActiveObjects(object2, !isObject1Active);

            // Set time scale based on which object is active
            if (!isObject1Active && pauseTimeWhenObject2Active)
            {
                Time.timeScale = 0f;
            }
            else if (!isObject1Active)
            {
                Time.timeScale = customTimeScaleWhenObject2Active;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    void SetActiveObjects(GameObject[] objects, bool active)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
        }
    }
}
