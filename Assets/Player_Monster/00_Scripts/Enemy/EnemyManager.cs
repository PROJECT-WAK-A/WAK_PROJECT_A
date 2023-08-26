using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    EnemyMovementManager enemyMovementManager;

    // 무언가 수행중
    public bool isPerformingAction;

    [Header("A.I Settings")]
    public float detectionRadius;
    public float minimumDetectionAngle = -50f;
    public float maximumDetectionAngle = 50f;

    private void Awake()
    {
        isPerformingAction = false;
        TryGetComponent(out enemyMovementManager);
    }

    private void FixedUpdate()
    {
        CurrentAction();
    }
    public void CurrentAction()
    {
        if (enemyMovementManager.currentTarget == null)
        {
            enemyMovementManager.Detection();

        }
        else
        {
            enemyMovementManager.MoveToTarget();
        }
    }

}
