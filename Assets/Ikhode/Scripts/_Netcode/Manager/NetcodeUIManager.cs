using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Net;  // To get local IP
using System.Net.Sockets;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetcodeUIManager : MonoBehaviour
{
    // Singleton instance
    public static NetcodeUIManager Instance { get; private set; }

    [SerializeField] private NetworkManager m_NetworkManager;

    // Buttons for server, host, client, and cancel
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject ServerPanelManagerObject;

    // Input field for entering the host's IP address
    [SerializeField] private TMP_InputField ipInputField;

    // Text field to display the host/server IP address
    [SerializeField] private TMP_Text ipAddressText;

    private void Awake()
    {
        // Ensure this is the only instance without destroying duplicates
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
            InitializeButtons();
        }
    }

    private void InitializeButtons()
    {
        // Attach event listeners only once
        if (serverButton != null)
            serverButton.onClick.AddListener(StartServer);
        
        if (hostButton != null)
            hostButton.onClick.AddListener(StartHost);
        
        if (clientButton != null)
            clientButton.onClick.AddListener(StartClient);
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(StopNetworkSession);
            cancelButton.gameObject.SetActive(false); // Hide cancel button by default
        }

        // Hide IP address field initially
        if (ipAddressText != null)
            ipAddressText.gameObject.SetActive(false);

        // Hide the input field for IP at the beginning
        if (ipInputField != null)
            ipInputField.gameObject.SetActive(true); // Visible only when connecting as a client
    }

    private void StartServer()
    {
        if (m_NetworkManager != null && !m_NetworkManager.IsServer)
        {
            m_NetworkManager.StartServer();
            Debug.Log("Server Started");
            ShowCancelButton();
            DisplayIPAddress(); // Display local IP after server starts

            ServerPanelManagerObject.SetActive(true);
        }
    }

    private void StartHost()
    {
        if (m_NetworkManager != null && !m_NetworkManager.IsHost)
        {
            m_NetworkManager.StartHost();
            Debug.Log("Host Started");
            ShowCancelButton();
            DisplayIPAddress(); // Display local IP after host starts
            ServerPanelManagerObject.SetActive(false);
        }
    }

    private void StartClient()
    {
        if (m_NetworkManager != null && !m_NetworkManager.IsClient)
        {
            string ipAddress = ipInputField.text; // Get the inputted IP address

            // Use the entered IP to connect to the host
            if (!string.IsNullOrEmpty(ipAddress))
            {
                m_NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address = ipAddress;
                m_NetworkManager.StartClient();
                Debug.Log("Client Connecting to " + ipAddress);
                ShowCancelButton();
                ServerPanelManagerObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Please enter a valid IP address.");
            }
        }
    }

    private void StopNetworkSession()
    {
        if (m_NetworkManager != null)
        {
            if (m_NetworkManager.IsServer || m_NetworkManager.IsHost)
            {
                m_NetworkManager.Shutdown();
                Debug.Log("Server/Host Stopped");
            }
            else if (m_NetworkManager.IsClient)
            {
                m_NetworkManager.Shutdown();
                Debug.Log("Client Stopped");
            }
        }
        ShowNetworkButtons(); // Show server, host, and client buttons again
        if (ipAddressText != null)
            ipAddressText.gameObject.SetActive(false); // Hide the IP address
    }

    private void ShowCancelButton()
    {
        // Hide the server, host, and client buttons
        if (serverButton != null) serverButton.gameObject.SetActive(false);
        if (hostButton != null) hostButton.gameObject.SetActive(false);
        if (clientButton != null) clientButton.gameObject.SetActive(false);

        // Show the cancel button
        if (cancelButton != null) cancelButton.gameObject.SetActive(true);
    }

    private void ShowNetworkButtons()
    {
        // Show the server, host, and client buttons again
        if (serverButton != null) serverButton.gameObject.SetActive(true);
        if (hostButton != null) hostButton.gameObject.SetActive(true);
        if (clientButton != null) clientButton.gameObject.SetActive(true);

        // Hide the cancel button
        if (cancelButton != null) cancelButton.gameObject.SetActive(false);
    }

    private void DisplayIPAddress()
    {
        string localIP = GetLocalIPAddress();
        if (!string.IsNullOrEmpty(localIP) && ipAddressText != null)
        {
            ipAddressText.gameObject.SetActive(true); // Show the text
            ipAddressText.text = "Your IP Address: " + localIP; // Display IP
            Debug.Log("Local IP Address: " + localIP);
        }
    }

    private string GetLocalIPAddress()
    {
        string localIP = "";
        try
        {
            // Get host name
            var host = Dns.GetHostEntry(Dns.GetHostName());

            // Loop through all IP addresses in the host entry
            foreach (var ip in host.AddressList)
            {
                // Select the first IPv4 address that is not a loopback
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            // If no valid IPv4 address found
            if (string.IsNullOrEmpty(localIP))
                Debug.LogError("No network adapters with an IPv4 address in the system!");
        }
        catch (SocketException ex)
        {
            Debug.LogError("SocketException: " + ex.ToString());
        }

        return localIP;
    }
}
