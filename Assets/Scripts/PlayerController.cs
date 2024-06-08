using Cinemachine;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;
    public Transform Transform_Player;

    [Header("Camera")]
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject cameraPos;
    [SerializeField] private CinemachineVirtualCamera PlayerCamera;

    private CharacterController cc;

    [SyncVar] private Vector2 moveVector = Vector2.zero;
    private Vector2 moveVectorTarget;
    private Vector2 mouseDeltaPos = Vector2.zero;

    private float moveSpeed = 2f;
    private float delayCount = 0.5f;

    [Header("Attack")]
    public KeyCode _attKey = KeyCode.Mouse0;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        
        Debug.Log(Database.Instance.PlayerNickName + " : " + isLocalPlayer);
        SetCameraPosition();

        if (!isLocalPlayer)
        {
            Debug.Log("camera false");
            cameraObject.SetActive(false);
        }
        else
        {
            Debug.Log("camera true");
            cameraObject.SetActive(true);
        }


    }

    private void Update()
    {
        if (!Application.isFocused || !isLocalPlayer)
        {
            return;
        }

        HandleAttack();
        HandleRotation();
        HandleMovement();
    }

    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            UpdateCameraPosition();
        }
    }

    [Command]
    private void CommandMove()
    {
        HandleMovement();
    }


//    [ClientRpc]
    private void HandleMovement()
    {
        // 로컬 플레이어의 이동
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        NavAgent_Player.velocity = forward * Mathf.Max(vertical, 0) * NavAgent_Player.speed;
    }

    private void HandleRotation()
    {
        Vector3 direction = (cameraPos.transform.forward).normalized;

        direction = new Vector3(direction.x, 0, direction.z);

        Quaternion rotationBody = Quaternion.LookRotation(direction);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationBody, Time.deltaTime * 8f);
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(_attKey))
        {
            CommandAtk();
        }
    }

    [Command]
    private void CommandAtk()
    {
        RpcOnAtk();
    }

    [ClientRpc]
    private void RpcOnAtk()
    {
        Animator_Player.SetTrigger("Atk");
    }

    private void SetCameraPosition()
    {
        cameraPos.transform.position = this.transform.position + new Vector3(0, 0, 0);

        Vector3 camAngle = cameraPos.transform.rotation.eulerAngles;
        Debug.Log("setcamera");
    }

    private void UpdateCameraPosition()
    {
        cameraPos.transform.position = this.transform.position + new Vector3(0, 0, 0);

        Vector3 camAngle = cameraPos.transform.rotation.eulerAngles;

        mouseDeltaPos *= 0.5f;

        float x = camAngle.x;

        if (x < 180f) x = Mathf.Clamp(x, 0f, 15f);
        else x = Mathf.Clamp(x, 345f, 361f);

        // 현재 회전 상태와 목표 회전 상태를 쿼터니언으로 변환합니다.
        Quaternion currentRotation = cameraPos.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(x, camAngle.y + mouseDeltaPos.x, camAngle.z);

        // 현재 회전 상태에서 목표 회전 상태로 부드럽게 보간합니다.
        cameraPos.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 80);
        mouseDeltaPos *= 0.3f;
    }

    void OnAim(InputValue inputValue)
    {
        mouseDeltaPos = inputValue.Get<Vector2>();
    }
}
