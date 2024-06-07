using Cinemachine;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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

        if (!isLocalPlayer)
        {
            cameraObject.SetActive(false);
        }
        else
        {
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
        // 로컬플레이어의 회전
        //float horizontal = Input.GetAxis("Horizontal");
        //transform.Rotate(0, horizontal * 100f * Time.deltaTime, 0);

        // 로컬 플레이어의 이동
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        NavAgent_Player.velocity = forward * Mathf.Max(vertical, 0) * NavAgent_Player.speed;

        //Animator_Player.SetFloat("XSpeed", moveVector.x);
        ////animator.SetFloat("ZSpeed", moveVector.y);

        //moveVector = Vector2.Lerp(moveVector, moveVectorTarget * moveSpeed, Time.deltaTime * 5);

        //Vector3 moveVector3 = new Vector3(moveVector.x * 0.5f, Physics.gravity.y, moveVector.y);

        //cc.Move(this.transform.rotation * moveVector3 * Time.deltaTime);
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

    //void OnMove(InputValue inputValue) // 이동(WASD)
    //{
    //    Debug.Log($"isLocalPlayer : {isLocalPlayer}");
    //    if (isLocalPlayer)
    //        moveVectorTarget = inputValue.Get<Vector2>();//인풋 벡터 받아옴
    //}

    //void OnSprint(InputValue inputValue)
    //{
    //    if (isLocalPlayer)
    //    {
    //        float value = inputValue.Get<float>();
    //        moveSpeed = (value * 2f) + 2f;
    //    }
    //}

    void OnAim(InputValue inputValue)
    {
        mouseDeltaPos = inputValue.Get<Vector2>();
    }
}
