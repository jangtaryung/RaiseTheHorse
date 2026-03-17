using UnityEngine;

/// <summary>
/// 적 전차 풀링/라이프사이클 관리 전용.
/// 전투 로직은 EnemyChariotCombat, HP/이동속도는 ChariotStats가 담당.
/// EnemyManager가 이 컴포넌트를 통해 스폰/회수를 관리합니다.
/// </summary>
public class EnemyChariot : MonoBehaviour
{
    private ChariotStats stats;
    private EnemyChariotCombat combat;

    public bool IsDead => stats != null && stats.currentHP <= 0f;

    private void Awake()
    {
        stats = GetComponent<ChariotStats>();
        combat = GetComponent<EnemyChariotCombat>();
    }

    public void Init(Transform playerChariot, ChariotStats playerStats)
    {
        if (combat != null)
            combat.Init(playerChariot, playerStats);
    }

    /// <summary>공유 투사체 풀을 전투 컴포넌트에 전달합니다.</summary>
    public void SetPools(ObjectPool arrowPool, ObjectPool spearPool)
    {
        if (combat != null)
            combat.SetPools(arrowPool, spearPool);
    }

    /// <summary>풀에서 꺼낼 때 호출. HP/위치를 초기화합니다.</summary>
    public void ResetForPool(Vector3 position, Transform playerChariot, ChariotStats playerStats)
    {
        transform.position = position;
        gameObject.SetActive(true);

        if (stats != null)
            stats.currentHP = stats.maxHP;

        if (combat != null)
            combat.Init(playerChariot, playerStats);
    }

    public void TakeDamage(float dmg)
    {
        if (stats != null)
            stats.TakeDamage(dmg);
    }
}
