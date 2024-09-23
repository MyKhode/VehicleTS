using System.Collections;
using UnityEngine;
using TMPro;

namespace AHUI
{
    public class NotificationManager : MonoBehaviour
    {
        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _descriptionText;

        public void ShowNotification(string title, string description, Transform notificationParent, GameObject notificationElementPrefab = null, float displayDuration = 3f)
        {
            // If no prefab is provided, return and log an error
            if (notificationElementPrefab == null)
            {
                Debug.LogError("Notification element prefab is missing.");
                return;
            }

            // Instantiate notification object
            GameObject notificationInstance = Instantiate(notificationElementPrefab, notificationParent);

            // Find and set title and description TextMeshProUGUI components
            _titleText = notificationInstance.transform.Find("title").GetComponent<TextMeshProUGUI>();
            _descriptionText = notificationInstance.transform.Find("description").GetComponent<TextMeshProUGUI>();

            _titleText.text = title;
            _descriptionText.text = description;

            // Start coroutine to auto-close after duration
            StartCoroutine(AutoCloseNotification(notificationInstance, displayDuration));
        }

        private IEnumerator AutoCloseNotification(GameObject notification, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(notification);  // Automatically close/destroy the notification
        }

        public void CloseNotification(GameObject notification)
        {
            Destroy(notification);  // Close and remove notification manually
        }
    }
}
