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

    private void Awake()
    {
        stats = GetComponent<ChariotStats>();
    }

    public void Init(Transform playerChariot, ChariotStats playerStats)
    {
        target = playerChariot;
        targetStats = playerStats;
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
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += new Vector3(dir.x, 0f, 0f) * stats.moveSpeed * Time.deltaTime;
        }

        // 배타적 권역 계산
        float swordsmanMax = swordsmanView != null ? swordsmanView.GetEffectiveRange() : 0f;
        float lancerMax = swordsmanMax + (lancerView != null ? lancerView.GetEffectiveRange() : 0f);
        float archerMax = lancerMax + (archerView != null ? archerView.GetEffectiveRange() : 0f);

        // 권역 판정 → 해당 병종에게 "공격해" 명령
        if (dist <= swordsmanMax)
            TryAttack(swordsmanView, target.position);
        else if (dist <= lancerMax)
            TryAttack(lancerView, target.position);
        else if (dist <= archerMax)
            TryAttack(archerView, target.position);
    }

    /// <summary>통합 공격 명령. 병종이 알아서 자기 방식대로 공격.</summary>
    private void TryAttack(ICrewCombat crew, Vector3 targetPos)
    {
        if (crew == null || !crew.IsReady()) return;
        crew.ExecuteAttack(targetPos, targetStats);
    }
}
