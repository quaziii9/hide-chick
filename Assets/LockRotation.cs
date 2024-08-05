using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : NetworkBehaviour
{

    public Transform canvasTransform; // 캔버스 Transform 참조

    void Start()
    {
        if (isLocalPlayer)
        {
            canvasTransform = GetComponent<Transform>();
            // 로컬 플레이어인 경우 캔버스를 180도 회전
            canvasTransform.Rotate(0, 180, 0);
            Debug.Log("?");
        }
    }
}
