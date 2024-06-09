using Cinemachine;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using EnumTypes;
using EventLibrary;

public class PlayerController : NetworkBehaviour
{
    [SyncVar] public bool isAlive = true;

    [Header("Components")]
    public NavMeshAgent NavAgent_Player;
    private Animator Animator_Player;
    public Transform Transform_Player;
    public GameObject weapon;
    private Collider playerCollider;

    [Header("Camera")]
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject cameraPos;
    [SerializeField] private CinemachineVirtualCamera PlayerCamera;

    private Vector2 mouseDeltaPos = Vector2.zero;


    [Header("Attack")]
    public KeyCode _attKey = KeyCode.Mouse0;

    private bool isMovementEnabled = false;
    public bool isAtk = true;

    private void Start()
    {
        Animator_Player = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();

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

    private void OnEnable()
    {
        EventManager<PlayerEvents>.StartListening(PlayerEvents.isAtkTrue, isAtkTrue);
        EventManager<PlayerEvents>.StartListening(PlayerEvents.isAtkFalse, isAtkFalse);
        EventManager<PlayerEvents>.StartListening(PlayerEvents.WeaponColliderFalse, WeaponColliderFalse);
    }

    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isMovementEnabled = true;
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

    private void HandleMovement()
    {
        // 로컬 플레이어의 이동
        Animator_Player.SetFloat("Speed", NavAgent_Player.velocity.magnitude); 

        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        float playerSpeed = NavAgent_Player.speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerSpeed *= 2;
        }

        NavAgent_Player.velocity = forward * Mathf.Max(vertical, 0) * playerSpeed;
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
        if (Input.GetKeyDown(_attKey) &&isAtk == true)
        {
            Animator_Player.SetTrigger("Atk");

            EventManager<UIEvents>.TriggerEvent(UIEvents.atkTime);            
        }
    }

    public void Die()
    {
        isAlive = false;
        Animator_Player.SetTrigger("Die");
        playerCollider.enabled = false;
        isMovementEnabled = false;
        NavAgent_Player.isStopped = true;
        NavAgent_Player.velocity = Vector3.zero;
        isAtk = false;

        EventManager<UIEvents>.TriggerEvent(UIEvents.atkImageSetActiveFalse);
        //EventManager<UIEvents>.TriggerEvent(UIEvents.BackGroundUION);
        StartCoroutine(DisableAfterDelay(2f));

        EventManager<GameEvents>.TriggerEvent(GameEvents.playerDie);

    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 하위 오브젝트들을 비활성화
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        isMovementEnabled = true;
    }


    public void WeaponColliderTrue()
    {
        Collider weaponCollider = weapon.GetComponent<Collider>();
        weaponCollider.enabled = true;
    }
    public void WeaponColliderFalse()
    {
        Collider weaponCollider = weapon.GetComponent<Collider>();
        weaponCollider.enabled = false;;
    }

    private void isAtkTrue()
    {
        isAtk = true;
    }

    private void isAtkFalse()
    {
        isAtk = false;
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