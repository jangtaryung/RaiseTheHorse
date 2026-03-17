using System;
using UnityEngine;

[Serializable]
public class ArcherPlayer : CrewMemberBase
{
    // 궁병: 궁술 숙련 (원거리 재장전 속도 및 명중률)
    [field: SerializeField] public float ArcherySkill { get; private set; } = 0f;
    [field: SerializeField] public WeaponSpec Bow { get; private set; }

    public override ChariotRole Role => ChariotRole.Archer;

    public ArcherPlayer(
        string displayName,
        WeaponSpec bow,
        float archerySkill = 0f,
        float bodyWeight = 70f,
        float baseAttack = 18f,
        float baseDefense = 3f,
        float baseMaxHp = 90f)
        : base(displayName, bodyWeight, baseAttack, baseDefense, baseMaxHp)
    {
        Bow = bow;
        ArcherySkill = Mathf.Max(0f, archerySkill);
    }

    public void EquipBow(WeaponSpec bow) => Bow = bow;

    public void TrainArchery(float amount)
    {
        if (amount <= 0f) return;
        ArcherySkill = Mathf.Max(0f, ArcherySkill + amount);
    }

    protected override void OnLevelUp()
    {
        ArcherySkill += 0.15f;
    }

    public float GetBowWeight() => Bow != null ? Bow.Weight : 0f;

    public float GetBaseRange()
    {
        return Bow != null ? Bow.Range : 0f;
    }

    public override float GetTotalWeight()
    {
        return base.GetTotalWeight() + GetBowWeight();
    }
}
