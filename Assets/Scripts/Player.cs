using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;


public class Player : NetworkBehaviour
{

    [Header("Components")]
    public NavMeshAgent NavAgent_Player;        // 플레이어 현재 속도 판단용으로 사용
    public Animator Animator_Player;            // 플레이어 애니메이션 제어용
    public Transform Transform_Player;          // 플레이어 위치, 회전 제어용

    [Header("Movement")]
    public float _rotationSpeed = 100.0f;       // 회전 속도 값

    [Header("Camera")]
    public CinemachineVirtualCamera PlayerCamera;

    [SyncVar] private Vector2 moveVector = Vector2.zero;
    private Vector2 moveVectorTarget;
    private Vector2 mouseDeltaPos = Vector2.zero;

    [Header("Attack")]
    public KeyCode _attKey = KeyCode.Mouse0;


    private void Update()
    {

        if (CheckIsFocuseOnUpdate() == false)
        {
            return;
        }
        CheckIsLocalPlayerOnUpdate();
    }

    private void LateUpdate()
    {
        if (isLocalPlayer) CamRotate();    // 카메라 회전
    }

    private bool CheckIsFocuseOnUpdate()
    {
        return Application.isFocused;
    }

    private void CheckIsLocalPlayerOnUpdate()
    {
        // isLocalPlayer 검사를 안해주면 게임 내의 모든 오브젝트가 똑같은 로직이 적용됨
        if (isLocalPlayer == false)
            return;

        // 회전
        Vector3 direction = (PlayerCamera.transform.forward).normalized;

        direction = new Vector3(direction.x, 0, direction.z);

        Quaternion rotationBody = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationBody, Time.deltaTime * 8f);

        // 이동
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        NavAgent_Player.velocity = forward * Mathf.Max(vertical, 0) * NavAgent_Player.speed;
        Animator_Player.SetBool("Moving", NavAgent_Player.velocity != Vector3.zero);

        if (Input.GetKeyDown(_attKey))
        {
            CommandAtk();
        }

        RotateLocalPlayer();
    }

    // 서버사이드,
    [Command]   // 클라이언트에서 호출되어 서버에서 수행
    void CommandAtk()   // 서버에 내 플레이어의 공격 요청
    {
        RpcOnAtk();
    }

    [ClientRpc]         // 서버에서 호출되어서 클라이언트에서 수행
    void RpcOnAtk()     // 서버에 내 플레이어의 공격 애니메이션 재생 요청
    {
        Animator_Player.SetTrigger("Atk");
    }

    void RotateLocalPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.DrawLine(ray.origin, hit.point);
            Vector3 lookRotate = new Vector3(hit.point.x, Transform_Player.position.y, hit.point.z);
            Transform_Player.LookAt(lookRotate);
        }
    }

    void CamRotate()
    {
        PlayerCamera.transform.position = this.transform.position + new Vector3(0, 1.5f, 0);

        Vector3 camAngle = PlayerCamera.transform.rotation.eulerAngles;

        mouseDeltaPos *= 0.2f;

        float x = camAngle.x - mouseDeltaPos.y;

        if (x < 180f) x = Mathf.Clamp(x, 0f, 15f);
        else x = Mathf.Clamp(x, 345f, 361f);

        // 현재 회전 상태와 목표 회전 상태를 쿼터니언으로 변환합니다.
        Quaternion currentRotation = PlayerCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(x, camAngle.y + mouseDeltaPos.x, camAngle.z);

        // 현재 회전 상태에서 목표 회전 상태로 부드럽게 보간합니다.
        PlayerCamera.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 80);
        mouseDeltaPos *= 0.3f;
    }
}
