using UnityEngine;

/// <summary>
/// 창병 GameObject. 자기 쿨타임/사거리/데미지 상태를 제공하고,
/// 실제 공격 실행은 ChariotCombat이 판단합니다.
/// </summary>
public class LancerView : MonoBehaviour
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

    public LancerPlayer RuntimeModel { get; private set; }

    private float attackTimer;

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

    /// <summary>창병 공격력 * 창술 숙련 보정</summary>
    public float GetDamage()
    {
        float atk = RuntimeModel != null ? RuntimeModel.GetAttack() : baseAttack;
        float skill = RuntimeModel != null ? RuntimeModel.LanceSkill : 0f;
        return atk * (1f + skill * 0.01f);
    }

    private static void ForceSetLevel(CrewMemberBase member, int targetLevel)
    {
        targetLevel = Mathf.Max(1, targetLevel);
        while (member.Level < targetLevel)
            member.GainExp(member.ExpToNextLevel());
    }
}
