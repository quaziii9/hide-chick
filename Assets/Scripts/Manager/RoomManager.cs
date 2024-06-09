using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using EventLibrary;
using EnumTypes;

public class RoomManager : NetworkRoomManager
{
    [SerializeField] LoginPopup _loginPopup;
    [SerializeField] GameObject aiPrefab; // AI 프리팹
    public int maxPlayerCount = 30;
    public int minPlayerCount = 1;

    public int alivePlayerCount = NetworkServer.connections.Count;

    public static RoomManager Instance;


    private List<Transform> startPositions = new List<Transform>();

    public void OnInputValueChanged_SetHostName(string hostName)
    {
        this.networkAddress = hostName;
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("Client disconnected");

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
                    NetworkServer.Destroy(roomPlayer.gameObject); // 로비 플레이어 객체 제거
                }
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        NetworkClient.Ready();//
        alivePlayerCount = NetworkServer.connections.Count;
        if (sceneName != RoomScene)
        {
            // 씬이 로비 씬이 아닐 때 AI 스폰
            SpawnAIs();
        }

        foreach (NetworkConnection conn in NetworkServer.connections.Values)
        {
            if (conn.identity != null)
            {
                Vector3 spawnPos = FindObjectOfType<SpawnPositions>().GetSpawnPosition();
                conn.identity.transform.position = spawnPos;
            }
        }
    }

    private void SpawnAIs()
    {
        // 현재 연결된 실제 플레이어 수
        int playerCount = NetworkServer.connections.Count;

        // 생성해야 할 AI 캐릭터 수
        int aiCount = maxPlayerCount - playerCount;

        Debug.Log($"Spawning {aiCount} AIs");

        // AI 캐릭터 생성
        for (int i = 0; i < aiCount; i++)
        {
            Vector3 spawnPos = GetStartPosition().position;
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Y축 회전을 랜덤하게 설정
            GameObject aiInstance = Instantiate(aiPrefab, spawnPos, randomRotation);

            NetworkServer.Spawn(aiInstance);
            //Debug.Log($"Spawned AI at {spawnPos}");
        }
    }

    public override Transform GetStartPosition()
    {
        if (startPositions.Count == 0)
        {
            foreach (var startPos in FindObjectsOfType<NetworkStartPosition>())
            {
                startPositions.Add(startPos.transform);
            }
        }

        if (startPositions.Count > 0)
        {
            Transform startPos = startPositions[0];
            startPositions.RemoveAt(0);
            return startPos;
        }

        return base.GetStartPosition();
    }


    public void PlayerKill(PlayerController player)
    {
        Debug.Log("playerdied");
        alivePlayerCount--;
        CheckForWinner(player);
    }

    private void CheckForWinner(PlayerController player)
    {
        Debug.Log("Check");
        if (alivePlayerCount == 1)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RpcDeclareWinner(player.connectionToClient);
            }
            else
            {
                Debug.LogError("GameManager instance is null!");
            }
        }
    }
}
