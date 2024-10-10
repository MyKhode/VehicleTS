using UnityEngine;
using System.Net.NetworkInformation; // Use this for System.Net.NetworkInformation.Ping
using TMPro; // Use this for TextMeshPro
using UnityEngine.UI; // Use this for Image
using System.Collections;

public class InternetSpeedTest : MonoBehaviour
{
    public TextMeshProUGUI internetSpeedText; // Reference to the internet speed text element
    public Image speedIndicator; // Reference to the image that will change color
    public Color goodColor = Color.green; // Color for good internet speed
    public Color mediumColor = Color.yellow; // Color for medium internet speed
    public Color badColor = Color.red; // Color for bad internet speed
    public string pingIP = "103.216.49.240"; // IP to ping for speed test

    private void Start()
    {
        StartCoroutine(ContinuousSpeedTest());
    }

    private IEnumerator ContinuousSpeedTest()
    {
        while (true) // Loop forever
        {
            yield return TestInternetSpeed();
            yield return new WaitForSeconds(5); // Wait for 5 seconds before next test
        }
    }

    private IEnumerator TestInternetSpeed()
    {
        // Instantiate the Ping class from System.Net.NetworkInformation
        using (System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping())
        {
            float startTime = Time.time;
            PingReply reply = null;

            // Start the ping
            try
            {
                reply = ping.Send(pingIP, 1000); // Send ping with a timeout of 1000ms
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Ping Error: " + ex.Message);
                internetSpeedText.text = "Ping Error!";
                UpdateSpeedIndicator(badColor); // Set to red for error
                yield break; // Exit the method
            }

            float pingTime = Time.time - startTime;

            if (reply.Status == IPStatus.Success)
            {
                float pingMilliseconds = reply.RoundtripTime; // Get ping time in milliseconds
                internetSpeedText.text = $"Ping: {pingMilliseconds:F2} ms"; // Update UI text
                UpdateSpeedIndicator(pingMilliseconds);
            }
            else
            {
                Debug.LogError("Ping Error: " + reply.Status);
                internetSpeedText.text = "Ping Error!";
                UpdateSpeedIndicator(badColor); // Set to red for error
            }

            yield return null; // Wait for the next frame
        }
    }

    private void UpdateSpeedIndicator(float pingMilliseconds)
    {
        if (pingMilliseconds < 50) // Good
        {
            UpdateSpeedIndicator(goodColor); // Set color to good
        }
        else if (pingMilliseconds < 100) // Medium
        {
            UpdateSpeedIndicator(mediumColor); // Set color to medium
        }
        else // Bad
        {
            UpdateSpeedIndicator(badColor); // Set color to bad
        }
    }

    private void UpdateSpeedIndicator(Color color)
    {
        speedIndicator.color = color; // Set color based on speed level
    }
}
