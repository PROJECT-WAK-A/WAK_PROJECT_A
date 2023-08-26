using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    EnemyMovementManager enemyMovementManager;
    bool isPerformingAction;

    [Header("A.I Settings")]
    public float detectionRadius;
    public float minimumDetectionAngle = -50f;
    public float maximumDetectionAngle = 50f;

    private void Awake()
    {
        TryGetComponent(out enemyMovementManager);
    }

    private void Update()
    {
        CurrentAction();
    }
    public void CurrentAction()
    {
        if (enemyMovementManager.currentTarget != null) { return; }

        enemyMovementManager.Detection();
    }
}
