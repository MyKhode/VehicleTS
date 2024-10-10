using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public enum ServerMode
{
    Client,
    Host,
    Server
}

public class NetcodeServerManager : MonoBehaviour
{
    [SerializeField] private ServerMode m_ServerMode = ServerMode.Host;
    [SerializeField] private NetworkManager m_NetworkManager;
    [SerializeField] private string m_GameplayOnlineSceneName = "Gameplay_Online";

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == m_GameplayOnlineSceneName)
        {
            switch (m_ServerMode)
            {
                case ServerMode.Server:
                    // Start the server if it is not already running
                    if (!NetworkManager.Singleton.IsServer)
                    {
                        m_NetworkManager.StartServer();
                    }
                    break;

                case ServerMode.Client:
                    // Start the client if it is not already connected
                    if (!NetworkManager.Singleton.IsClient)
                    {
                        m_NetworkManager.StartClient();
                    }
                    break;

                case ServerMode.Host:
                    // Start the host if it is not already hosting
                    if (!NetworkManager.Singleton.IsHost)
                    {
                        m_NetworkManager.StartHost();
                    }
                    break;
            }
        }
    }
}
