using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    EnemyMovementManager enemyMovementManager;
    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out enemyMovementManager);
    }

}
