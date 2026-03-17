using UnityEngine;

/// <summary>
/// 전차 이동 + 지휘관 역할.
/// "누가 공격할지"만 결정하고, "어떻게 공격하느냐"는 각 View(ICrewCombat)에 위임.
/// 공격 권역: 검병(0~sMax) / 창병(sMax~lMax) / 궁병(lMax~aMax) 배타적 구간.
/// </summary>
public class ChariotCombat : MonoBehaviour
{
    [Header("접근 정지 거리")]
    public float stopDistance = 1.5f;

    [Header("승무원 참조")]
    [SerializeField] private ArcherView archerView;
    [SerializeField] private LancerView lancerView;
    [SerializeField] private SwordsmanView swordsmanView;

    private EnemyManager enemyManager;
    private ChariotStats stats;

    // 캐싱된 권역 경계 (스킬 변동 시 재계산)
    private float cachedSwordsmanMax;
    private float cachedLancerMax;
    private float cachedArcherMax;

    private void Awake()
    {
        stats = GetComponent<ChariotStats>();
    }

    public void Init(EnemyManager manager)
    {
        enemyManager = manager;
        RecalculateZones();
    }

    /// <summary>권역 경계를 재계산합니다. 스킬/장비 변경 시 호출.</summary>
    public void RecalculateZones()
    {
        cachedSwordsmanMax = swordsmanView != null ? swordsmanView.GetEffectiveRange() : 0f;
        cachedLancerMax = cachedSwordsmanMax + (lancerView != null ? lancerView.GetEffectiveRange() : 0f);
        cachedArcherMax = cachedLancerMax + (archerView != null ? archerView.GetEffectiveRange() : 0f);
    }

    private void Update()
    {
        if (enemyManager == null) return;

        // 최대 사거리(궁병) 내에서만 적 탐색
        if (!enemyManager.TryGetClosestWithinRange(transform.position, cachedArcherMax, out int targetId, out Vector3 targetPos))
            return;

        float dist = Mathf.Abs(targetPos.x - transform.position.x);

        // 이동
        if (dist > stopDistance)
        {
            float dirX = targetPos.x > transform.position.x ? 1f : -1f;
            transform.position += new Vector3(dirX * stats.moveSpeed * Time.deltaTime, 0f, 0f);
        }

        // 권역 판정 → 해당 병종에게 "공격해" 명령
        if (dist <= cachedSwordsmanMax)
            TryAttack(swordsmanView, targetId, targetPos);
        else if (dist <= cachedLancerMax)
            TryAttack(lancerView, targetId, targetPos);
        else
            TryAttack(archerView, targetId, targetPos);
    }

    /// <summary>통합 공격 명령. 병종이 알아서 자기 방식대로 공격.</summary>
    private void TryAttack(ICrewCombat crew, int targetId, Vector3 targetPos)
    {
        if (crew == null || !crew.IsReady()) return;
        crew.ExecuteAttack(targetPos, targetId, enemyManager);
    }

    // ===== 디버그: 배타적 권역 시각화 =====
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        float sMax = cachedSwordsmanMax;
        float lMax = cachedLancerMax;
        float aMax = cachedArcherMax;

        float y = pos.y;
        float height = 1f;

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        DrawZone(pos, 0f, sMax, y, height);
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
        DrawZoneBorder(pos, 0f, sMax, y, height);

        Gizmos.color = new Color(0.2f, 0.4f, 1f, 0.3f);
        DrawZone(pos, sMax, lMax, y, height);
        Gizmos.color = new Color(0.2f, 0.4f, 1f, 0.8f);
        DrawZoneBorder(pos, sMax, lMax, y, height);

        Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.3f);
        DrawZone(pos, lMax, aMax, y, height);
        Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.8f);
        DrawZoneBorder(pos, lMax, aMax, y, height);
    }

    private void DrawZone(Vector3 center, float minDist, float maxDist, float y, float height)
    {
        Vector3 rCenter = new Vector3(center.x + (minDist + maxDist) * 0.5f, y, 0f);
        Vector3 rSize = new Vector3(maxDist - minDist, height, 0f);
        Gizmos.DrawCube(rCenter, rSize);

        Vector3 lCenter = new Vector3(center.x - (minDist + maxDist) * 0.5f, y, 0f);
        Gizmos.DrawCube(lCenter, rSize);
    }

    private void DrawZoneBorder(Vector3 center, float minDist, float maxDist, float y, float height)
    {
        float halfH = height * 0.5f;

        Gizmos.DrawLine(new Vector3(center.x + maxDist, y - halfH, 0f), new Vector3(center.x + maxDist, y + halfH, 0f));
        if (minDist > 0f)
            Gizmos.DrawLine(new Vector3(center.x + minDist, y - halfH, 0f), new Vector3(center.x + minDist, y + halfH, 0f));

        Gizmos.DrawLine(new Vector3(center.x - maxDist, y - halfH, 0f), new Vector3(center.x - maxDist, y + halfH, 0f));
        if (minDist > 0f)
            Gizmos.DrawLine(new Vector3(center.x - minDist, y - halfH, 0f), new Vector3(center.x - minDist, y + halfH, 0f));
    }
}
