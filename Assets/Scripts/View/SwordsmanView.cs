using UnityEngine;

/// <summary>
/// 검병 GameObject. 자기 쿨타임/사거리/데미지 상태를 제공하고,
/// 실제 공격 실행은 ChariotCombat이 판단합니다.
/// </summary>
public class SwordsmanView : MonoBehaviour
{
    [Header("검병 성장 데이터")]
    [SerializeField] private int level = 1;
    [SerializeField] private float swordSkill = 0f;

    [Header("무장 - 검")]
    [SerializeField] private float swordRange = 1.5f;
    [SerializeField] private float swordBasePower = 20f;
    [SerializeField] private float swordWeight = 6f;

    [Header("신체")]
    [SerializeField] private float bodyWeight = 78f;
    [SerializeField] private float baseAttack = 22f;
    [SerializeField] private float baseDefense = 4f;
    [SerializeField] private float baseMaxHp = 100f;

    [Header("전투")]
    [SerializeField] private float attackInterval = 0.6f;

    public SwordsmanPlayer RuntimeModel { get; private set; }

    private float attackTimer;

    public SwordsmanPlayer Build()
    {
        var sword = new WeaponSpec("Sword", swordRange, swordBasePower, swordWeight);
        var model = new SwordsmanPlayer(
            displayName: gameObject.name,
            sword: sword,
            swordSkill: swordSkill,
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

    /// <summary>검 기본 사거리 (숙련으로 미세 증가)</summary>
    public float GetEffectiveRange()
    {
        float skill = RuntimeModel != null ? RuntimeModel.SwordSkill : 0f;
        return swordRange * (1f + skill * 0.005f);
    }

    /// <summary>검병 공격력 + 검 기본위력, 검술 숙련 보정</summary>
    public float GetDamage()
    {
        float atk = RuntimeModel != null ? RuntimeModel.GetAttack() : baseAttack;
        float basePower = RuntimeModel != null ? RuntimeModel.GetBasePower() : swordBasePower;
        float skill = RuntimeModel != null ? RuntimeModel.SwordSkill : 0f;
        return (atk + basePower) * (1f + skill * 0.04f);
    }

    private static void ForceSetLevel(CrewMemberBase member, int targetLevel)
    {
        targetLevel = Mathf.Max(1, targetLevel);
        while (member.Level < targetLevel)
            member.GainExp(member.ExpToNextLevel());
    }
}
