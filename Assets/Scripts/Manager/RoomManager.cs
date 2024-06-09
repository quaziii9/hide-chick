using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    [SerializeField] LoginPopup _loginPopup;
    [SerializeField] GameObject aiPrefab; // AI 프리팹
    public int maxPlayerCount = 20;
    public int minPlayerCount = 1;

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

        // 연결에 이미 플레이어가 할당되어 있지 않은 경우에만 실행
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
        // 씬 전환 전에 모든 로비 플레이어를 비활성화 또는 제거
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
            else
            {
                return;
            }
        }
       
    }

    private void SpawnAIs()
    {
        // 현재 연결된 실제 플레이어 수
        int playerCount = NetworkServer.connections.Count;

        // 생성해야 할 AI 캐릭터 수
        int aiCount = maxPlayerCount - playerCount;

        // AI 캐릭터 생성
        for (int i = 0; i < aiCount; i++)
        {
            Vector3 spawnPos = GetStartPosition().position;
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Y축 회전을 랜덤하게 설정
            GameObject aiInstance = Instantiate(aiPrefab, spawnPos, randomRotation);
            NetworkServer.Spawn(aiInstance);
        }
    }

    public override Transform GetStartPosition()
    {
        // NetworkStartPosition 사용하여 스폰 위치 반환
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

        // 기본 위치 반환 (네트워크 시작 위치가 없을 경우)
        return base.GetStartPosition();
    }
}
