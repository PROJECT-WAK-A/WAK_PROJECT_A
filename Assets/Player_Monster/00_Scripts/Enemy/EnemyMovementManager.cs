using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementManager : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimatorManager enemyAnimatorManager;
    NavMeshAgent navMeshAgent;
    public Rigidbody rb;

    public CharacterStats currentTarget = null;
    public LayerMask detectionLayer;

    public float distanceFromTarget;
    public float stoppingDistance;

    public float rotationSpeed = 15f;
    public float movementSpeed = 10f;
    private void Awake()
    {
        TryGetComponent(out enemyManager);
        TryGetComponent(out rb);
        TryGetComponent(out enemyAnimatorManager);
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

    }

    private void Start()
    {
        navMeshAgent.enabled = false;
        rb.isKinematic = false;

        enemyAnimatorManager.anim.SetBool("Grounded", true);
        enemyAnimatorManager.anim.SetFloat("MotionSpeed", 1f);
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
        RotateTowardsTarget(targetDirection);

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
    }
    private void ProcssingOnDistanceState(Vector3 direction)
    {
        distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        float viewAbleAngle = Vector3.Angle(direction, transform.forward);

        // 액션 중일 때
        if (enemyManager.isPerformingAction)
        {
            enemyAnimatorManager.anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
            navMeshAgent.enabled = false;
        }
        else
        {
            // 애니메이션 처리
            if (distanceFromTarget > stoppingDistance)
            {
                enemyAnimatorManager.anim.SetFloat("Speed", 5f, 0.1f, Time.deltaTime);
            }
            else if (distanceFromTarget <= stoppingDistance)
            {
                enemyAnimatorManager.anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
                navMeshAgent.enabled = false;
            }
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
            Vector3 targetVelocity = navMeshAgent.velocity;

            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(currentTarget.transform.position);
            rb.velocity = targetVelocity;

            transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
        }
    }
}
