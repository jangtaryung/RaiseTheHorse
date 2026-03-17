using UnityEngine;

/// <summary>
/// 궁병 GameObject. 자기 쿨타임/사거리/데미지 상태를 제공하고,
/// 실제 공격 실행은 ChariotCombat이 판단합니다.
/// </summary>
public class ArcherView : MonoBehaviour
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

    public ArcherPlayer RuntimeModel { get; private set; }

    private float attackTimer;

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

    /// <summary>쿨타임이 찼는지 확인</summary>
    public bool IsReady() => attackTimer >= attackInterval;

    /// <summary>공격 실행 후 쿨타임 초기화 (ChariotCombat이 호출)</summary>
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

    private static void ForceSetLevel(CrewMemberBase member, int targetLevel)
    {
        targetLevel = Mathf.Max(1, targetLevel);
        while (member.Level < targetLevel)
            member.GainExp(member.ExpToNextLevel());
    }
}
