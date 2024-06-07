using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public string PlayerName;

    public TextMeshProUGUI NickName;

    public override void OnStartServer()
    {
        PlayerName = Database.Instance.PlayerNickName;
        NickName.text = Database.Instance.PlayerNickName;
    }


    private void Start()
    {
        base.Start();
        PlayerName = Database.Instance.PlayerNickName;
        NickName.text = Database.Instance.PlayerNickName;

        //LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCount();

        //if (isServer)
        //{
        //    LobbyUIManager.Instance.ActiveStartButton();
        //}
    }

    private void OnDestroy()
    {
        //if (LobbyUIManager.Instance != null)
        //{
        //    LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCount();
        //}
    }
}
