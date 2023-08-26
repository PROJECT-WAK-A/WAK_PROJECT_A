using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementManager : MonoBehaviour
{
    EnemyManager enemyManager;
    public CharacterStats currentTarget;
    public LayerMask detectionLayer;

    private void Awake()
    {
        TryGetComponent(out enemyManager);
    }
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
}
