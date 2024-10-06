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

    private void Update()
    {
        if (m_ServerMode == ServerMode.Server)
        {
            if (SceneManager.GetActiveScene().name == m_GameplayOnlineSceneName)
                m_NetworkManager.StartServer();
        }
        else if (m_ServerMode == ServerMode.Client)
        {
            if (SceneManager.GetActiveScene().name == m_GameplayOnlineSceneName)
                m_NetworkManager.StartClient();
        }
        else if (m_ServerMode == ServerMode.Host)
        {
            if (SceneManager.GetActiveScene().name == m_GameplayOnlineSceneName)
                m_NetworkManager.StartHost();
        }
    }
}


