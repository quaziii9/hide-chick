using Cinemachine;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent NavAgent_Player;
    private Animator Animator_Player;
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

    private bool isMovementEnabled = false;
    [SerializeField] GameObject LoadingImage;
    //[Header("Stats Server")]
    //[SyncVar(hook = nameof(OnSpeedChanged))] private float syncSpeed;

    private void Start()
    {
        Animator_Player = GetComponent<Animator>();

        if (!isLocalPlayer)
        {
            cameraObject.SetActive(false);
        }
        else
        {
            cameraObject.SetActive(true);
            StartCoroutine(EnableMovementAfterDelay(1.5f));
        }
    }

    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isMovementEnabled = true;
    }



    private void InitializeLocalPlayer()
    {
        cameraObject.SetActive(true);
        SetCameraPositionImmediate();
    }

    private void Update()
    {
        if (!Application.isFocused || !isLocalPlayer || !isMovementEnabled)
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

    //[Command]
    //private void CommandMove()
    //{
    //    HandleMovement();
    //}


    //    [ClientRpc]
    private void HandleMovement()
    {
        // 로컬 플레이어의 이동
        Animator_Player.SetFloat("Speed", NavAgent_Player.velocity.magnitude);
        Debug.Log(NavAgent_Player.velocity.magnitude);

        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        float playerSpeed = NavAgent_Player.speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerSpeed *= 2;
        }

        NavAgent_Player.velocity = forward * Mathf.Max(vertical, 0) * playerSpeed;

        //Animator_Player.SetBool("Moving", NavAgent_Player.velocity.sqrMagnitude > 0.1f);
        //Animator_Player.SetBool("Dashing", NavAgent_Player.velocity.sqrMagnitude > 9.0f);

        //Animator_Player.SetFloat("Speed", NavAgent_Player.velocity.magnitude);

        //CmdSetSpeed(NavAgent_Player.velocity.magnitude);
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
            Animator_Player.SetTrigger("Atk");
            //CommandAtk();
        }
    }

    //[Command]
    //private void CommandAtk()
    //{
    //    RpcOnAtk();
    //}

    //[Command]
    //private void CmdSetSpeed(float speed)
    //{
    //    syncSpeed = speed;
    //}
    //private void OnSpeedChanged(float oldSpeed, float newSpeed)
    //{
    //    Animator_Player.SetFloat("Speed", newSpeed);
    //}

    //[ClientRpc]
    //private void RpcOnAtk()
    //{
    //    Animator_Player.SetTrigger("Atk");
    //}

    private void SetCameraPositionImmediate()
    {
        cameraPos.transform.position = this.transform.position + new Vector3(0, 0, 0);
        cameraPos.transform.rotation = Quaternion.Euler(0, 0, 0); // 초기 회전 설정
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