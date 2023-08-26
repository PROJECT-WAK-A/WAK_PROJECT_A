using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementManager : MonoBehaviour
{
    EnemyManager enemyManager;
    NavMeshAgent navMeshAgent;

    public CharacterStats currentTarget = null;
    public LayerMask detectionLayer;

    public float distanceFromTarget;
    public float stoppingDistance;

    public float rotationSpeed = 15f;
    public float movementSpeed = 10f;
    private void Awake()
    {
        TryGetComponent(out enemyManager);
        TryGetComponent(out navMeshAgent);
    }

    private void Start()
    {
        navMeshAgent.enabled = false;
    }

    // Player 감지
    public void Detection()
    {
        // 주위 collider 컴포넌트를 가진 특정 객체를 가져옴 (detectionLayer)
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].transform.TryGetComponent(out CharacterStats characterStats);
            // 캐릭터만 처리
            if (characterStats == null) { return; }

            Vector3 targetDirection = characterStats.transform.position - transform.position;
            float viewAbleAngle = Vector3.Angle(targetDirection, transform.forward);

            if (viewAbleAngle > enemyManager.minimumDetectionAngle && viewAbleAngle < enemyManager.maximumDetectionAngle)
            {
                currentTarget = characterStats;
            }
        }
    }

    // 추격
    public void MoveToTarget()
    {
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        ProcssingOnDistanceState(targetDirection);
        SetVelocity(targetDirection);
        RotateTowardsTarget(targetDirection);

        // navMeshAgent.transform.localPosition = Vector3.zero;
        // navMeshAgent.transform.localRotation = Quaternion.identity;
    }
    private void ProcssingOnDistanceState(Vector3 direction)
    {
        distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        float viewAbleAngle = Vector3.Angle(direction, transform.forward);

        // 액션 중일 때
        if (enemyManager.isPerformingAction)
        {
            navMeshAgent.enabled = false;
        }
        else
        {
            // 애니메이션 처리
            if (distanceFromTarget > stoppingDistance)
            {

            }
            else if (distanceFromTarget <= stoppingDistance)
            {
                navMeshAgent.enabled = false;
            }
        }
    }
    private void SetVelocity(Vector3 direction)
    {
        if (enemyManager.isPerformingAction)
        {

        }
        else
        {
            // direction.y = 0;

            // Vector3 targetVelocity = direction.normalized * movementSpeed  Time.deltaTime;
            // enemyRigidBody.velocity = targetVelocity;
        }
    }
    private void RotateTowardsTarget(Vector3 direction)
    {
        // 액션 중일때
        if (enemyManager.isPerformingAction)
        {
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetPosition = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition, rotationSpeed / Time.deltaTime);
        }
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
            Debug.Log(relativeDirection);
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(currentTarget.transform.position);
            // Debug.Log(currentTarget.transform.position);

            // transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
        }
    }
}
