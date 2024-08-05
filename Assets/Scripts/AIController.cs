using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class AIController : NetworkBehaviour
{
    public enum AIState { Idle, Walking, Running, Dead } // Dead 상태 추가

    [SyncVar]
    private AIState currentState;

    [SyncVar]
    private Vector3 syncDestination;

    [SyncVar]
    private float syncSpeed;

    private NavMeshAgent agent;
    private Animator animator;
    private float stateChangeTimer;
    private float positionChangeTimer;

    [SerializeField] private float stateDurationMin = 5f; // 상태 최소 지속 시간
    [SerializeField] private float stateDurationMax = 10f; // 상태 최대 지속 시간
    [SerializeField] private float idleMaxDuration = 3f; // Idle 상태 최대 지속 시간
    [SerializeField] private float positionChangeInterval = 15f; // 위치 변경 간격
    [SerializeField] private float movementRadius = 20f; // 이동 반경

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (isServer)
        {
            ChangeState(GetRandomState()); // 시작 상태를 랜덤으로 설정
            SetNewDestination(); // 시작하자마자 새로운 위치 설정
            positionChangeTimer = positionChangeInterval; // 위치 변경 타이머 초기화
        }
    }

    void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
        else
        {
            ClientUpdate();
        }
    }

    [ServerCallback]
    private void ServerUpdate()
    {
        if (currentState == AIState.Dead) return; // Dead 상태에서는 동작 중지

        stateChangeTimer -= Time.deltaTime;
        positionChangeTimer -= Time.deltaTime;

        if (stateChangeTimer <= 0)
        {
            ChangeState(GetRandomState());
        }

        if (positionChangeTimer <= 0)
        {
            SetNewDestination();
            positionChangeTimer = positionChangeInterval; // 위치 변경 타이머 리셋
        }

        // 목표 위치에 도달하면 새로운 위치 설정
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                SetNewDestination();
            }
        }
        // 상태에 따른 행동 처리
        HandleMovement();

        agent.destination = syncDestination;
        agent.speed = syncSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    [ClientCallback]
    private void ClientUpdate()
    {
        if (currentState == AIState.Dead) return; // Dead 상태에서는 동작 중지

        agent.destination = syncDestination;
        agent.speed = syncSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void HandleMovement()
    {
        switch (currentState)
        {
            case AIState.Idle:
                agent.isStopped = true;
                syncSpeed = 0f;
                animator.SetFloat("Speed", 0);
                break;
            case AIState.Walking:
                agent.isStopped = false;
                syncSpeed = 1.2f;
                agent.speed = syncSpeed;
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;
            case AIState.Running:
                agent.isStopped = false;
                syncSpeed = 2f;
                agent.speed = syncSpeed;
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;
            case AIState.Dead:
                agent.isStopped = true;
                syncSpeed = 0f;
                agent.velocity = Vector3.zero;
                animator.SetFloat("Speed", 0);
                break;
        }
    }

    [Server]
    private void ChangeState(AIState newState)
    {
        currentState = newState;
        stateChangeTimer = newState == AIState.Idle ? Random.Range(stateDurationMin, idleMaxDuration) : Random.Range(stateDurationMin, stateDurationMax);
    }

    private AIState GetRandomState()
    {
        float randomValue = Random.value * 100; // 0에서 100 사이의 랜덤 값
        if (randomValue < 20) // 20% 확률로 Idle
        {
            return AIState.Idle;
        }
        else if (randomValue < 70) // 40% 확률로 Walking
        {
            return AIState.Walking;
        }
        else // 40% 확률로 Running
        {
            return AIState.Running;
        }
    }

    [Server]
    private void SetNewDestination()
    {
        syncDestination = GetRandomPosition();
        agent.SetDestination(syncDestination);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * movementRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, movementRadius, NavMesh.AllAreas);
        return hit.position;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    [Server]
    public void Die()
    {
        if (currentState == AIState.Dead) return; // 이미 죽은 상태이면 실행하지 않음

        currentState = AIState.Dead; // 상태를 Dead로 설정
        RpcDie(); // 모든 클라이언트에 사망을 알림
    }

    [ClientRpc]
    public void RpcDie()
    {
        animator.SetTrigger("Die");
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(DisableAfterDelay(2f));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 하위 오브젝트들을 비활성화
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
