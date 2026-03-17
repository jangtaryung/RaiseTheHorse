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

    private void Awake()
    {
        stats = GetComponent<ChariotStats>();
    }

    public void Init(EnemyManager manager)
    {
        enemyManager = manager;
    }

    private void Update()
    {
        if (enemyManager == null) return;

        if (!enemyManager.TryGetClosest(transform.position, out int targetId, out Vector3 targetPos))
            return;

        float dist = Mathf.Abs(targetPos.x - transform.position.x);

        // 이동
        if (dist > stopDistance)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position += new Vector3(dir.x, 0f, 0f) * stats.moveSpeed * Time.deltaTime;
        }

        // 배타적 권역 계산
        float swordsmanMax = swordsmanView != null ? swordsmanView.GetEffectiveRange() : 0f;
        float lancerMax = swordsmanMax + (lancerView != null ? lancerView.GetEffectiveRange() : 0f);
        float archerMax = lancerMax + (archerView != null ? archerView.GetEffectiveRange() : 0f);

        // 권역 판정 → 해당 병종에게 "공격해" 명령
        if (dist <= swordsmanMax)
            TryAttack(swordsmanView, targetId, targetPos);
        else if (dist <= lancerMax)
            TryAttack(lancerView, targetId, targetPos);
        else if (dist <= archerMax)
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
        float sMax = swordsmanView != null ? swordsmanView.GetEffectiveRange() : 0f;
        float lMax = sMax + (lancerView != null ? lancerView.GetEffectiveRange() : 0f);
        float aMax = lMax + (archerView != null ? archerView.GetEffectiveRange() : 0f);

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
