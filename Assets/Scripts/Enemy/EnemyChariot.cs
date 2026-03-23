using UnityEngine;

/// <summary>
/// 적 전차 풀링/라이프사이클 관리 전용.
/// 전투 로직은 EnemyChariotCombat, 스펙/HP는 Chariot이 담당.
/// EnemyManager가 이 컴포넌트를 통해 스폰/회수를 관리합니다.
/// </summary>
public class EnemyChariot : MonoBehaviour
{
    [Header("EXP")]
    [SerializeField] private float expReward = 20f;

    private Chariot chariot;
    private EnemyChariotCombat combat;

    public Chariot GetChariot() => chariot;
    public bool IsDead => chariot != null && chariot.GetCurrentHP() <= 0f;

    /// <summary>사망 시 플레이어 크루에게 분배될 EXP 양.</summary>
    public float ExpReward => expReward;

    private void Awake()
    {
        combat = GetComponent<EnemyChariotCombat>();
    }

    /// <summary>빌드 완료된 Chariot을 주입합니다.</summary>
    public void SetChariot(Chariot model)
    {
        chariot = model;
    }

    public void Init(Transform playerChariot, Chariot playerChariotModel)
    {
        if (combat != null)
            combat.Init(playerChariot, playerChariotModel);
    }

    /// <summary>공유 투사체 풀을 전투 컴포넌트에 전달합니다.</summary>
    public void SetPools(ObjectPool arrowPool, ObjectPool spearPool)
    {
        if (combat != null)
            combat.SetPools(arrowPool, spearPool);
    }

    /// <summary>풀에서 꺼낼 때 호출. HP/위치를 초기화합니다.</summary>
    public void ResetForPool(Vector3 position, Transform playerChariot, Chariot playerChariotModel)
    {
        transform.position = position;
        gameObject.SetActive(true);

        if (chariot != null)
            chariot.ResetHP();

        if (combat != null)
            combat.Init(playerChariot, playerChariotModel);
    }

    public void TakeDamage(float dmg)
    {
        if (chariot != null)
            chariot.TakeDamage(dmg);
    }
}
