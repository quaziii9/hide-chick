using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using EventLibrary;
using EnumTypes;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [TargetRpc]
    public void RpcDeclareWinner(NetworkConnection target)
    {
        // 우승 처리 로직 (UI 표시 등)
        EventManager<UIEvents>.TriggerEvent(UIEvents.BackGroundUION);
        EventManager<UIEvents>.TriggerEvent(UIEvents.WinnerUION);
    }
}
