using UnityEngine;

/// <summary>
/// 적 전차 전투 지휘관. ChariotCombat과 동일한 구조.
/// "누가 공격할지"만 결정하고, "어떻게 공격하느냐"는 각 View(ICrewCombat)에 위임.
/// 공격 권역: 검병(0~sMax) / 창병(sMax~lMax) / 궁병(lMax~aMax) 배타적 구간.
/// </summary>
public class EnemyChariotCombat : MonoBehaviour
{
    [Header("접근 정지 거리")]
    public float stopDistance = 1.5f;

    [Header("승무원 참조")]
    [SerializeField] private ArcherView archerView;
    [SerializeField] private LancerView lancerView;
    [SerializeField] private SwordsmanView swordsmanView;

    private ChariotStats stats;
    private Transform target;
    private ChariotStats targetStats;

    // 캐싱된 권역 경계
    private float cachedSwordsmanMax;
    private float cachedLancerMax;
    private float cachedArcherMax;

    private void Awake()
    {
        stats = GetComponent<ChariotStats>();
    }

    public void Init(Transform playerChariot, ChariotStats playerStats)
    {
        target = playerChariot;
        targetStats = playerStats;
        RecalculateZones();
    }

    /// <summary>권역 경계를 재계산합니다.</summary>
    private void RecalculateZones()
    {
        cachedSwordsmanMax = swordsmanView != null ? swordsmanView.GetEffectiveRange() : 0f;
        cachedLancerMax = cachedSwordsmanMax + (lancerView != null ? lancerView.GetEffectiveRange() : 0f);
        cachedArcherMax = cachedLancerMax + (archerView != null ? archerView.GetEffectiveRange() : 0f);
    }

    /// <summary>공유 투사체 풀을 각 View에 주입합니다.</summary>
    public void SetPools(ObjectPool arrowPool, ObjectPool spearPool)
    {
        if (archerView != null) archerView.SetPool(arrowPool);
        if (lancerView != null) lancerView.SetPool(spearPool);
    }

    private void Update()
    {
        if (stats.currentHP <= 0f || target == null || targetStats == null) return;

        float dist = Mathf.Abs(target.position.x - transform.position.x);

        // 이동: 플레이어를 향해 접근
        if (dist > stopDistance)
        {
            float dirX = target.position.x > transform.position.x ? 1f : -1f;
            transform.position += new Vector3(dirX * stats.moveSpeed * Time.deltaTime, 0f, 0f);
        }

        // 권역 판정 → 해당 병종에게 "공격해" 명령
        if (dist <= cachedSwordsmanMax)
            TryAttack(swordsmanView);
        else if (dist <= cachedLancerMax)
            TryAttack(lancerView);
        else if (dist <= cachedArcherMax)
            TryAttack(archerView);
    }

    /// <summary>통합 공격 명령. 병종이 알아서 자기 방식대로 공격.</summary>
    private void TryAttack(ICrewCombat crew)
    {
        if (crew == null || !crew.IsReady()) return;
        crew.ExecuteAttack(target.position, targetStats);
    }
}
