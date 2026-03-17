using UnityEngine;

/// <summary>
/// 적 전차. 플레이어 전차와 완전히 동일한 구조.
/// ChariotStats + ChariotCrewController + 4 View를 자식으로 가집니다.
/// 이동 + 타이머 + 공격을 이 스크립트에서 직접 관리합니다.
/// (View의 Update()를 사용하지 않아 100마리+ 스케일에서 성능 유리)
/// </summary>
public class EnemyChariot : MonoBehaviour
{
    [Header("접근 정지 거리")]
    public float stopDistance = 1.5f;

    [Header("승무원 참조 (데미지/사거리 계산용)")]
    [SerializeField] private ArcherView archerView;
    [SerializeField] private LancerView lancerView;
    [SerializeField] private SwordsmanView swordsmanView;

    [Header("공격 주기 (View와 별도로 EnemyChariot이 관리)")]
    [SerializeField] private float archerInterval = 1.2f;
    [SerializeField] private float lancerInterval = 1.0f;
    [SerializeField] private float swordsmanInterval = 0.6f;

    private ChariotStats stats;
    private Transform target;
    private ChariotStats targetStats;

    private float archerTimer;
    private float lancerTimer;
    private float swordsmanTimer;

    public bool IsDead => stats != null && stats.currentHP <= 0f;

    private void Awake()
    {
        stats = GetComponent<ChariotStats>();

        // 자식 View의 Update() 비활성화 (타이머를 여기서 관리)
        DisableViewUpdates();
    }

    private void DisableViewUpdates()
    {
        if (archerView != null) archerView.enabled = false;
        if (lancerView != null) lancerView.enabled = false;
        if (swordsmanView != null) swordsmanView.enabled = false;
    }

    public void Init(Transform playerChariot, ChariotStats playerStats)
    {
        target = playerChariot;
        targetStats = playerStats;
    }

    /// <summary>풀에서 꺼낼 때 호출. HP/타이머/위치를 초기화합니다.</summary>
    public void ResetForPool(Vector3 position, Transform playerChariot, ChariotStats playerStats)
    {
        transform.position = position;
        target = playerChariot;
        targetStats = playerStats;

        if (stats != null)
            stats.currentHP = stats.maxHP;

        archerTimer = 0f;
        lancerTimer = 0f;
        swordsmanTimer = 0f;

        DisableViewUpdates();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (IsDead || target == null || targetStats == null) return;

        float dt = Time.deltaTime;
        float dist = Mathf.Abs(target.position.x - transform.position.x);

        // 이동: 플레이어를 향해 접근
        if (dist > stopDistance)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += new Vector3(dir.x, 0f, 0f) * stats.moveSpeed * dt;
        }

        // 타이머 축적 + 공격
        archerTimer += dt;
        if (archerTimer >= archerInterval && archerView != null && dist <= archerView.GetEffectiveRange())
        {
            archerTimer = 0f;
            targetStats.TakeDamage(archerView.GetDamage());
            Debug.Log($"[적 궁병] dmg:{archerView.GetDamage():F1}");
        }

        lancerTimer += dt;
        if (lancerTimer >= lancerInterval && lancerView != null && dist <= lancerView.GetEffectiveRange())
        {
            lancerTimer = 0f;
            targetStats.TakeDamage(lancerView.GetDamage());
            Debug.Log($"[적 창병] dmg:{lancerView.GetDamage():F1}");
        }

        swordsmanTimer += dt;
        if (swordsmanTimer >= swordsmanInterval && swordsmanView != null && dist <= swordsmanView.GetEffectiveRange())
        {
            swordsmanTimer = 0f;
            targetStats.TakeDamage(swordsmanView.GetDamage());
            Debug.Log($"[적 검병] dmg:{swordsmanView.GetDamage():F1}");
        }
    }

    public void TakeDamage(float dmg)
    {
        if (stats != null)
            stats.TakeDamage(dmg);
    }
}
