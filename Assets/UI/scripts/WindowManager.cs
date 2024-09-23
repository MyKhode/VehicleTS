using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AHUI
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] private Button[] buttons;       // Array to hold buttons that open windows
        [SerializeField] private Button[] closeButtons;  // Array to hold buttons that close windows (optional)
        [SerializeField] private GameObject[] windows;   // Array to hold corresponding windows

        private void Start()
        {
            if (buttons.Length != windows.Length)
            {
                Debug.LogError("The number of buttons and windows must be equal!");
                return;
            }

            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() => OnButtonClick(index));
            }

            HideAllWindows();
        }

        private void OnButtonClick(int index)
        {
            HideAllWindows();
            windows[index].SetActive(true);

            buttons[index].onClick.RemoveAllListeners();

            if (closeButtons.Length > index && closeButtons[index] != null)
            {
                closeButtons[index].onClick.AddListener(() => OnCloseButtonClick(index));
            }
        }

        private void OnCloseButtonClick(int index)
        {
            windows[index].SetActive(false);

            if (closeButtons.Length > index && closeButtons[index] != null)
            {
                closeButtons[index].onClick.RemoveAllListeners();
            }

            buttons[index].onClick.AddListener(() => OnButtonClick(index));
        }

        private void HideAllWindows()
        {
            foreach (var window in windows)
            {
                window.SetActive(false);
            }
        }
    }
}