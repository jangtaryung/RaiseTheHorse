using UnityEngine;

/// <summary>
/// 전차 이동 + 플레이어 매니저 역할.
/// 가장 가까운 적 전차를 한 번 탐색하고, 사거리 내 준비된 승무원이 공격합니다.
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

        // 적 전차를 한 번만 탐색
        if (!enemyManager.TryGetClosest(transform.position, out int targetId, out Vector3 targetPos))
            return;

        float dist = Mathf.Abs(targetPos.x - transform.position.x);

        // 이동
        if (dist > stopDistance)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position += new Vector3(dir.x, 0f, 0f) * stats.moveSpeed * Time.deltaTime;
        }

        // 각 승무원: 사거리 내면 공격
        TryAttack(archerView, targetId, dist, "궁병");
        TryAttack(lancerView, targetId, dist, "창병");
        TryAttack(swordsmanView, targetId, dist, "검병");
    }

    private void TryAttack(ArcherView view, int targetId, float dist, string role)
    {
        if (view == null || !view.IsReady()) return;
        if (dist > view.GetEffectiveRange()) return;

        float damage = view.GetDamage();
        view.ConsumeAttack();
        enemyManager.ApplyDamage(targetId, damage);
        Debug.Log($"[{role}] {view.RuntimeModel.DisplayName} dmg:{damage:F1}");
    }

    private void TryAttack(LancerView view, int targetId, float dist, string role)
    {
        if (view == null || !view.IsReady()) return;
        if (dist > view.GetEffectiveRange()) return;

        float damage = view.GetDamage();
        view.ConsumeAttack();
        enemyManager.ApplyDamage(targetId, damage);
        Debug.Log($"[{role}] {view.RuntimeModel.DisplayName} dmg:{damage:F1}");
    }

    private void TryAttack(SwordsmanView view, int targetId, float dist, string role)
    {
        if (view == null || !view.IsReady()) return;
        if (dist > view.GetEffectiveRange()) return;

        float damage = view.GetDamage();
        view.ConsumeAttack();
        enemyManager.ApplyDamage(targetId, damage);
        Debug.Log($"[{role}] {view.RuntimeModel.DisplayName} dmg:{damage:F1}");
    }
}
