using UnityEngine;

/// <summary>
/// 전투 중앙 허브. EnemyManager와 ChariotCombat만 참조하고,
/// 전투 시작 시 ChariotCombat에 EnemyManager를 주입합니다.
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ChariotCombat chariotCombat;

    private void Awake()
    {
        if (chariotCombat != null)
            chariotCombat.Init(enemyManager);
    }
}
