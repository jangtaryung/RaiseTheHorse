using UnityEngine;

/// <summary>
/// 궁병 GameObject. 쿨타임/사거리/데미지 상태를 관리하고,
/// 공격 시 화살(Projectile)을 발사합니다.
/// </summary>
public class ArcherView : MonoBehaviour, ICrewCombat
{
    [Header("궁병 성장 데이터")]
    [SerializeField] private int level = 1;
    [SerializeField] private float archerySkill = 0f;

    [Header("무장 - 활")]
    [SerializeField] private float bowRange = 30f;
    [SerializeField] private float bowBasePower = 0f;
    [SerializeField] private float bowWeight = 5f;

    [Header("신체")]
    [SerializeField] private float bodyWeight = 70f;
    [SerializeField] private float baseAttack = 18f;
    [SerializeField] private float baseDefense = 3f;
    [SerializeField] private float baseMaxHp = 90f;

    [Header("전투")]
    [SerializeField] private float attackInterval = 1.2f;

    [Header("투사체 풀")]
    [SerializeField] private ObjectPool arrowPool;

    public ArcherPlayer RuntimeModel { get; private set; }

    private float attackTimer;

    /// <summary>런타임에 투사체 풀을 주입합니다 (적 전차용 공유 풀).</summary>
    public void SetPool(ObjectPool pool) => arrowPool = pool;

    public ArcherPlayer Build()
    {
        var bow = new WeaponSpec("Bow", bowRange, bowBasePower, bowWeight);
        var model = new ArcherPlayer(
            displayName: gameObject.name,
            bow: bow,
            archerySkill: archerySkill,
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

    /// <summary>활 기본 사거리 * 궁술 숙련 보정</summary>
    public float GetEffectiveRange()
    {
        float baseRange = RuntimeModel != null ? RuntimeModel.GetBaseRange() : bowRange;
        float skill = RuntimeModel != null ? RuntimeModel.ArcherySkill : 0f;
        return baseRange * (1f + skill * 0.01f);
    }

    /// <summary>궁병 공격력 * 궁술 숙련 보정</summary>
    public float GetDamage()
    {
        float atk = RuntimeModel != null ? RuntimeModel.GetAttack() : baseAttack;
        float skill = RuntimeModel != null ? RuntimeModel.ArcherySkill : 0f;
        return atk * (1f + skill * 0.01f);
    }

    /// <summary>화살을 발사합니다. 풀이 없으면 즉시 데미지.</summary>
    public void ExecuteAttack(Vector3 targetPos, int targetId, EnemyManager enemyManager)
    {
        if (!IsReady()) return;
        float damage = GetDamage();
        ConsumeAttack();

        if (arrowPool != null)
        {
            var projectile = arrowPool.Get(transform.position) as Projectile;
            if (projectile != null)
            {
                projectile.Launch(transform.position, targetPos, damage, targetId, enemyManager);
                return;
            }
        }
        else
        {
        }

        // 풀이 없으면 즉시 데미지 (폴백)
        // enemyManager.ApplyDamage(targetId, damage);
        // Debug.Log($"[궁병] {RuntimeModel?.DisplayName} 즉시 dmg:{damage:F1}");
    }

    /// <summary>적 궁병이 플레이어를 향해 화살 발사.</summary>
    public void ExecuteAttack(Vector3 targetPos, ChariotStats targetStats)
    {
        if (!IsReady()) return;
        float damage = GetDamage();
        ConsumeAttack();

        if (arrowPool != null)
        {
            var projectile = arrowPool.Get(transform.position) as Projectile;
            if (projectile != null)
            {
                projectile.Launch(transform.position, targetPos, damage, targetStats);
                return;
            }
        }

        // targetStats.TakeDamage(damage); // 풀링 투사체로만 데미지
    }

    private static void ForceSetLevel(CrewMemberBase member, int targetLevel)
    {
        targetLevel = Mathf.Max(1, targetLevel);
        while (member.Level < targetLevel)
            member.GainExp(member.ExpToNextLevel());
    }
}
