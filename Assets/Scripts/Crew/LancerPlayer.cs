using System;
using UnityEngine;

[Serializable]
public class LancerPlayer : CrewMemberBase
{
    // 창병: 창술 숙련 (중거리 공격 범위 및 저지력/넉백)
    [field: SerializeField] public float LanceSkill { get; private set; } = 0f;
    [field: SerializeField] public WeaponSpec Spear { get; private set; }

    public override ChariotRole Role => ChariotRole.Lancer;

    public LancerPlayer(
        string displayName,
        WeaponSpec spear,
        float lanceSkill = 0f,
        float bodyWeight = 80f,
        float baseAttack = 16f,
        float baseDefense = 6f,
        float baseMaxHp = 105f)
        : base(displayName, bodyWeight, baseAttack, baseDefense, baseMaxHp)
    {
        Spear = spear;
        LanceSkill = Mathf.Max(0f, lanceSkill);
    }

    public void EquipSpear(WeaponSpec spear) => Spear = spear;

    public void TrainLance(float amount)
    {
        if (amount <= 0f) return;
        LanceSkill = Mathf.Max(0f, LanceSkill + amount);
    }

    protected override void OnLevelUp()
    {
        LanceSkill += 0.12f;
    }

    public float GetSpearWeight() => Spear != null ? Spear.Weight : 0f;
    public float GetBaseRange() => Spear != null ? Spear.Range : 0f;

    public override float GetTotalWeight()
    {
        return base.GetTotalWeight() + GetSpearWeight();
    }
}
