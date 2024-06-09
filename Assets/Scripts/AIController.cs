using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public enum AIState { Idle, Walking, Running }
    private AIState currentState;

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
        ChangeState(GetRandomState()); // 시작 상태를 랜덤으로 설정
        agent.SetDestination(GetRandomPosition()); // 시작하자마자 새로운 위치 설정
        positionChangeTimer = positionChangeInterval; // 위치 변경 타이머 초기화
    }

    void Update()
    {
        stateChangeTimer -= Time.deltaTime;
        positionChangeTimer -= Time.deltaTime;

        if (stateChangeTimer <= 0)
        {
            ChangeState(GetRandomState());
        }

        if (positionChangeTimer <= 0)
        {
            agent.SetDestination(GetRandomPosition()); // 새로운 위치 설정
            positionChangeTimer = positionChangeInterval; // 위치 변경 타이머 리셋
        }

        // 목표 위치에 도달하면 새로운 위치 설정
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                agent.SetDestination(GetRandomPosition());
            }
        }

        // 상태에 따른 행동 처리
        switch (currentState)
        {
            case AIState.Idle:
                agent.isStopped = true;
                animator.SetFloat("Speed", 0);
                break;
            case AIState.Walking:
            case AIState.Running:
                agent.isStopped = false;
                HandleMovement();
                break;
        }
    }

    private void HandleMovement()
    {
        float speed = (currentState == AIState.Walking) ? 1.5f : 3f;
        agent.speed = speed;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void ChangeState(AIState newState)
    {
        currentState = newState;
        stateChangeTimer = newState == AIState.Idle ? Random.Range(stateDurationMin, idleMaxDuration) : Random.Range(stateDurationMin, stateDurationMax);
    }

    private AIState GetRandomState()
    {
        float randomValue = Random.value * 100; // 0에서 100 사이의 랜덤 값
        if (randomValue < 10) // 20% 확률로 Idle
        {
            return AIState.Idle;
        }
        else if (randomValue < 60) // 40% 확률로 Walking
        {
            return AIState.Walking;
        }
        else // 40% 확률로 Running
        {
            return AIState.Running;
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * movementRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, movementRadius, NavMesh.AllAreas);
        return hit.position;
    }
}
