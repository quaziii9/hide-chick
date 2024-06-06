using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomPlayer : NetworkRoomPlayer
{
    private void Start()
    {
        base.Start();
    }

    private void OnDestroy()
    {
    }
}
