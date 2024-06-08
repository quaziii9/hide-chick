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
        CmdSetPlayerName(Database.Instance.PlayerNickName);
    }

    [Command]
    void CmdSetPlayerName(string playerName)
    {
        PlayerName = playerName;
    }

    private void OnPlayerNameChanged(string oldName, string newName)
    {
        SetPlayerNameText();
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
    }
}
