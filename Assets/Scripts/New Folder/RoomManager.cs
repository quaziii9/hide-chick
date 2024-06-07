using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    [SerializeField] LoginPopup _loginPopup;
    public int maxPlayerCount = 20;
    public int minPlayerCount = 1;

    public void OnInputValueChanged_SetHostName(string hostName)
    {
        this.networkAddress = hostName;
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if (_loginPopup != null)
        {
            _loginPopup.SetUIOnClientDisconnected();
        }
    }

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        if (conn.identity == null)
        {
            Vector3 spawnPos = FindObjectOfType<SpawnPositions>().GetSpawnPosition();
            GameObject player = Instantiate(spawnPrefabs[0], spawnPos, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        else
        {
            Debug.Log("Player already assigned to this connection.");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity != null)
            {
                var roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();
                if (roomPlayer != null)
                {
                    NetworkServer.Destroy(roomPlayer.gameObject);
                }
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        foreach (NetworkConnection conn in NetworkServer.connections.Values)
        {
            if (conn.identity != null)
            {
                Vector3 spawnPos = FindObjectOfType<SpawnPositions>().GetSpawnPosition();
                conn.identity.transform.position = spawnPos;
            }
        }
    }
}
