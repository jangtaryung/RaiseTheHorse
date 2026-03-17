using UnityEngine;

/// <summary>
/// 전차 이동 + 플레이어 매니저 역할.
/// 공격 권역: 검병(0~sMax) / 창병(sMax~lMax) / 궁병(lMax~aMax) 배타적 구간.
/// </summary>
public class ChariotCombat : MonoBehaviour
{
    [Header("접근 정지 거리")]
    public float stopDistance = 1.5f;

    [Header("플레이어 참조")]
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

        // 권역 판정: 해당 구간에 있을 때만 공격
        if (dist <= swordsmanMax)
        {
            // 검병 권역
            TryAttack(swordsmanView, targetId, "검병");
        }
        else if (dist <= lancerMax)
        {
            // 창병 권역
            TryAttack(lancerView, targetId, "창병");
        }
        else if (dist <= archerMax)
        {
            // 궁병 권역
            TryAttack(archerView, targetId, "궁병");
        }
    }

    private void TryAttack(ArcherView view, int targetId, string role)
    {
        if (view == null || !view.IsReady()) return;
        float damage = view.GetDamage();
        view.ConsumeAttack();
        enemyManager.ApplyDamage(targetId, damage);
        Debug.Log($"[{role}] {view.RuntimeModel.DisplayName} dmg:{damage:F1}");
    }

    private void TryAttack(LancerView view, int targetId, string role)
    {
        if (view == null || !view.IsReady()) return;
        float damage = view.GetDamage();
        view.ConsumeAttack();
        enemyManager.ApplyDamage(targetId, damage);
        Debug.Log($"[{role}] {view.RuntimeModel.DisplayName} dmg:{damage:F1}");
    }

    private void TryAttack(SwordsmanView view, int targetId, string role)
    {
        if (view == null || !view.IsReady()) return;
        float damage = view.GetDamage();
        view.ConsumeAttack();
        enemyManager.ApplyDamage(targetId, damage);
        Debug.Log($"[{role}] {view.RuntimeModel.DisplayName} dmg:{damage:F1}");
    }
}
