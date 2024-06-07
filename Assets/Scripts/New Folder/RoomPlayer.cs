using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class RoomPlayer : NetworkBehaviour
{
    [SyncVar]
    public string PlayerName;

    // private TextMeshProUGUI NickName;


    public void Awake()
    {
       
    }
    // 호스트 또는 서버에서만 호출되는 함수
    public override void OnStartServer()
    {
        PlayerName = (string)connectionToClient.authenticationData;
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    SetPlayerNameText();
    //}


    //private void OnPlayerNameChanged(string oldName, string newName)
    //{
    //    SetPlayerNameText();
    //}

    //private void SetPlayerNameText()
    //{
    //    if (NickName == null)
    //    {
    //        NickName = GetComponentInChildren<TextMeshProUGUI>();
    //    }

    //    if (NickName != null)
    //    {
    //        NickName.text = PlayerName;
    //    }
    //}
}
