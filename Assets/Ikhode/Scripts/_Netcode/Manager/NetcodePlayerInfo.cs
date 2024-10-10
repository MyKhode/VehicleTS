using UnityEngine;
using Unity.Netcode;

public class NetcodePlayerInfo : NetworkBehaviour
{
    public void GetPlayerInfo(ulong clientId)
    {
        if (IsClient && clientId == NetworkManager.Singleton.LocalClientId)
        {
            string username = PlayerPrefs.GetString("OAuth_Name", "Unknown Player");
            string avatarUrl = PlayerPrefs.GetString("OAuth_ProfilePic", "");

            Debug.Log($"Sending player info from {clientId}: {username}, {avatarUrl}");

            SendPlayerInfoServerRpc(username, avatarUrl);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void SendPlayerInfoServerRpc(string username, string avatarUrl, ServerRpcParams rpcParams = default)
    {
        var sessionHandler = FindObjectOfType<NetcodeGameplayHandleSession>();
        if (sessionHandler != null)
        {
            Debug.Log($"Setting player info for {rpcParams.Receive.SenderClientId}: {username}, {avatarUrl}");
            sessionHandler.SetPlayerInfo(rpcParams.Receive.SenderClientId, username, avatarUrl);
        }
        else
        {
            Debug.LogError("Cannot find NetcodeGameplayHandleSession");
        }
    }
}

