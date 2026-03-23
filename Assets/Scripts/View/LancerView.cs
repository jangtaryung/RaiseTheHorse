using UnityEngine;

/// <summary>
/// 창병 GameObject. 원거리(투창)와 근거리(찌르기) 모두 가능.
/// 거리에 따라 자동으로 공격 방식을 선택합니다.
/// </summary>
public class LancerView : MonoBehaviour, ICrewCombat
{
    [Header("창병 성장 데이터")]
    [SerializeField] private int level = 1;
    [SerializeField] private float lanceSkill = 0f;

    [Header("무장 - 창")]
    [SerializeField] private float spearRange = 4.5f;
    [SerializeField] private float spearBasePower = 0f;
    [SerializeField] private float spearWeight = 8f;

    [Header("신체")]
    [SerializeField] private float bodyWeight = 80f;
    [SerializeField] private float baseAttack = 16f;
    [SerializeField] private float baseDefense = 6f;
    [SerializeField] private float baseMaxHp = 105f;

    [Header("전투")]
    [SerializeField] private float attackInterval = 1.0f;

    [Header("넉백")]
    [SerializeField] private float baseKnockback = 1.5f;

    [Header("근거리/원거리 경계")]
    [SerializeField] private float meleeRange = 2.0f;

    [Header("투사체 풀 (투창용)")]
    [SerializeField] private ObjectPool spearPool;
    [SerializeField] private LayerMask targetLayers;

    public LancerPlayer RuntimeModel { get; private set; }

    private float attackTimer;

    /// <summary>런타임에 투사체 풀을 주입합니다 (적 전차용 공유 풀).</summary>
    public void SetPool(ObjectPool pool) => spearPool = pool;

    public LancerPlayer Build()
    {
        var spear = new WeaponSpec("Spear", spearRange, spearBasePower, spearWeight);
        var model = new LancerPlayer(
            displayName: gameObject.name,
            spear: spear,
            lanceSkill: lanceSkill,
            bodyWeight: bodyWeight,
            baseAttack: baseAttack,
            baseDefense: baseDefense,
            baseMaxHp: baseMaxHp);

        ForceSetLevel(model, level);
        RuntimeModel = model;
        return model;
    }

    private void Update()
    {
        if (attackTimer < attackInterval)
            attackTimer += Time.deltaTime;
    }

    public bool IsReady() => attackTimer >= attackInterval;
    public void ConsumeAttack() => attackTimer = 0f;

    /// <summary>창 기본 사거리 * 창술 숙련 보정</summary>
    public float GetEffectiveRange()
    {
        float baseRange = RuntimeModel != null ? RuntimeModel.GetBaseRange() : spearRange;
        float skill = RuntimeModel != null ? RuntimeModel.LanceSkill : 0f;
        return baseRange * (1f + skill * 0.01f);
    }

    /// <summary>창병 공격력 + 창 기본위력, 창술 숙련 보정</summary>
    public float GetDamage()
    {
        float atk = RuntimeModel != null ? RuntimeModel.GetAttack() : baseAttack;
        float basePower = RuntimeModel != null ? RuntimeModel.GetBasePower() : spearBasePower;
        float skill = RuntimeModel != null ? RuntimeModel.LanceSkill : 0f;
        return (atk + basePower) * (1f + skill * 0.01f);
    }

    /// <summary>창술 숙련에 따른 넉백 거리. 스킬 1당 2% 증가.</summary>
    public float GetKnockbackDistance()
    {
        float skill = RuntimeModel != null ? RuntimeModel.LanceSkill : 0f;
        return baseKnockback * (1f + skill * 0.02f);
    }

    /// <summary>
    /// 거리에 따라 공격 방식 자동 선택.
    /// 근거리(meleeRange 이하) → 즉시 찌르기 데미지
    /// 원거리(meleeRange 초과) → 투창 투사체 발사
    /// </summary>
    public void ExecuteAttack(Vector3 targetPos, int targetId, EnemyManager enemyManager)
    {
        if (!IsReady()) return;
        float damage = GetDamage();
        ConsumeAttack();

        float dist = Mathf.Abs(targetPos.x - transform.position.x);

        if (dist <= meleeRange)
        {
            // 근거리: 즉시 찌르기 + 넉백
            enemyManager.ApplyDamage(targetId, damage);
            enemyManager.ApplyKnockback(targetId, GetKnockbackDistance(), transform.position);
        }
        else if (spearPool != null)
        {
            // 원거리: 투창 (넉백은 근거리에서만 적용)
            var projectile = spearPool.Get(transform.position) as Projectile;
            if (projectile != null)
            {
                projectile.Launch(transform.position, targetPos, damage, targetLayers);
                return;
            }
        }
    }

    /// <summary>적 창병이 플레이어를 공격. 거리에 따라 찌르기/투창.</summary>
    public void ExecuteAttack(Vector3 targetPos, Chariot targetChariot)
    {
        if (!IsReady()) return;
        float damage = GetDamage();
        ConsumeAttack();

        float dist = Mathf.Abs(targetPos.x - transform.position.x);

        if (dist <= meleeRange)
        {
            targetChariot.TakeDamage(damage);
        }
        else if (spearPool != null)
        {
            var projectile = spearPool.Get(transform.position) as Projectile;
            if (projectile != null)
            {
                projectile.Launch(transform.position, targetPos, damage, targetLayers);
                return;
            }
        }
    }

    private static void ForceSetLevel(CrewMemberBase member, int targetLevel)
    {
        targetLevel = Mathf.Max(1, targetLevel);
        while (member.Level < targetLevel)
            member.GainExp(member.ExpToNextLevel());
    }
}
