using System;
using UnityEngine;

[Serializable]
public class CoachmanPlayer : CrewMemberBase
{
    // 마부: 전차 운용 숙련 (기동 효율 및 회피 판정)
    [field: SerializeField] public float ChariotHandlingSkill { get; private set; } = 0f;

    public override ChariotRole Role => ChariotRole.Coachman;

    public CoachmanPlayer(
        string displayName,
        float handlingSkill = 0f,
        float bodyWeight = 75f,
        float baseAttack = 5f,
        float baseDefense = 8f,
        float baseMaxHp = 110f)
        : base(displayName, bodyWeight, baseAttack, baseDefense, baseMaxHp)
    {
        ChariotHandlingSkill = Mathf.Max(0f, handlingSkill);
    }

    public void TrainHandling(float amount)
    {
        if (amount <= 0f) return;
        ChariotHandlingSkill = Mathf.Max(0f, ChariotHandlingSkill + amount);
    }

    protected override void OnLevelUp()
    {
        // 키우기 게임 감성: 레벨업 시 숙련도도 조금씩 오름
        ChariotHandlingSkill += 0.2f;
    }
}
