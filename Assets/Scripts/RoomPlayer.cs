using Mirror;
using TMPro;
using UnityEngine;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string PlayerName;

    public TextMeshProUGUI NickName;

    private void Start()
    {
        base.Start();
        SetPlayerNameText();

        LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCount();

        if (isServer)
        {
            LobbyUIManager.Instance.ActiveStartButton();
        }


    }

    private void OnDestroy()
    {
        if (LobbyUIManager.Instance != null)
        {
            LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCount();
        }
    }

    public override void OnStartLocalPlayer()
    {
        // 로컬 플레이어가 시작될 때 서버에 자신의 닉네임을 보냅니다.
        CmdSetPlayerName(Database.Instance.PlayerNickName);
        Debug.Log($"OnStartLocalPlayer: Sending PlayerName: {Database.Instance.PlayerNickName}");
    }

    [Command]
    void CmdSetPlayerName(string playerName)
    {
        PlayerName = playerName;
        Debug.Log($"CmdSetPlayerName: PlayerName set to {playerName}");
    }

    private void OnPlayerNameChanged(string oldName, string newName)
    {
        // 모든 클라이언트에서 닉네임이 변경될 때 UI를 업데이트합니다.
        SetPlayerNameText();
        Debug.Log($"OnPlayerNameChanged: PlayerName changed from {oldName} to {newName}");
    }

    private void SetPlayerNameText()
    {
        if (NickName == null)
        {
            NickName = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (NickName != null)
        {
            NickName.text = PlayerName;
        }
        Debug.Log($"SetPlayerNameText: NickName text set to {PlayerName}");
    }

}
