using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public string PlayerName;


    private TextMeshProUGUI NickName;
    public override void OnStartServer()
    {
        PlayerName = (string)connectionToClient.authenticationData;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetPlayerNameText();
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
    private void Start()
    {
        base.Start();
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
