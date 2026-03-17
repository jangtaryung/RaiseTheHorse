using System;
using UnityEngine;

[Serializable]
public class SwordsmanPlayer : CrewMemberBase
{
    // 검병: 검술 숙련 (초근접 공격 속도 및 살상력)
    [field: SerializeField] public float SwordSkill { get; private set; } = 0f;
    [field: SerializeField] public WeaponSpec Sword { get; private set; }

    public override ChariotRole Role => ChariotRole.Swordsman;

    public SwordsmanPlayer(
        string displayName,
        WeaponSpec sword,
        float swordSkill = 0f,
        float bodyWeight = 78f,
        float baseAttack = 22f,
        float baseDefense = 4f,
        float baseMaxHp = 100f)
        : base(displayName, bodyWeight, baseAttack, baseDefense, baseMaxHp)
    {
        Sword = sword;
        SwordSkill = Mathf.Max(0f, swordSkill);
    }

    public void EquipSword(WeaponSpec sword) => Sword = sword;

    public void TrainSword(float amount)
    {
        if (amount <= 0f) return;
        SwordSkill = Mathf.Max(0f, SwordSkill + amount);
    }

    protected override void OnLevelUp()
    {
        SwordSkill += 0.12f;
    }

    public float GetSwordWeight() => Sword != null ? Sword.Weight : 0f;
    public float GetBasePower() => Sword != null ? Sword.BasePower : 0f;

    public override float GetTotalWeight()
    {
        return base.GetTotalWeight() + GetSwordWeight();
    }
}
