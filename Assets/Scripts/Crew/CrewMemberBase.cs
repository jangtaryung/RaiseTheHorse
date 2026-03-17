using System;
using UnityEngine;

[Serializable]
public abstract class CrewMemberBase
{
    [field: SerializeField] public string DisplayName { get; private set; }

    // 키우기(성장) 핵심 상태
    [field: SerializeField] public int Level { get; private set; } = 1;
    [field: SerializeField] public double Exp { get; private set; } = 0;

    // 전차 무게 계산(플레이어 본인 무게)
    [field: SerializeField] public float BodyWeight { get; private set; } = 70f;

    // 전투용 “기본 스탯”(레벨에 따라 파생 스탯으로 스케일)
    [field: SerializeField] public float BaseAttack { get; private set; } = 10f;
    [field: SerializeField] public float BaseDefense { get; private set; } = 5f;
    [field: SerializeField] public float BaseMaxHP { get; private set; } = 100f;

    protected CrewMemberBase(
        string displayName,
        float bodyWeight = 70f,
        float baseAttack = 10f,
        float baseDefense = 5f,
        float baseMaxHp = 100f)
    {
        DisplayName = displayName;
        BodyWeight = Mathf.Max(1f, bodyWeight);
        BaseAttack = Mathf.Max(0f, baseAttack);
        BaseDefense = Mathf.Max(0f, baseDefense);
        BaseMaxHP = Mathf.Max(1f, baseMaxHp);
    }

    public double ExpToNextLevel()
    {
        // 키우기 게임용 완만한 곡선(원하면 나중에 테이블/곡선으로 교체 가능)
        // L1=50, L10≈ 300대, L50≈ 수천대
        return 50.0 * Math.Pow(Level, 1.20);
    }

    public void GainExp(double amount)
    {
        if (amount <= 0) return;

        Exp += amount;
        while (Exp >= ExpToNextLevel())
        {
            Exp -= ExpToNextLevel();
            Level += 1;
            OnLevelUp();
        }
    }

    protected virtual void OnLevelUp()
    {
        // 필요하면 파생 클래스에서 “레벨업 시 추가 숙련 상승” 같은 걸 구현
    }

    // 레벨에 따른 파생 전투 스탯(키우기 게임 방향: level이 핵심 성장축)
    public float GetAttack()
    {
        return BaseAttack * GetLevelMultiplier_Attack();
    }

    public float GetDefense()
    {
        return BaseDefense * GetLevelMultiplier_Defense();
    }

    public float GetMaxHP()
    {
        return BaseMaxHP * GetLevelMultiplier_HP();
    }

    protected virtual float GetLevelMultiplier_Attack() => 1f + (Level - 1) * 0.05f;
    protected virtual float GetLevelMultiplier_Defense() => 1f + (Level - 1) * 0.04f;
    protected virtual float GetLevelMultiplier_HP() => 1f + (Level - 1) * 0.06f;

    public abstract ChariotRole Role { get; }

    // Crew 무게 계산에서 호출(무기는 각 직업 클래스가 포함)
    public virtual float GetTotalWeight() => BodyWeight;
}
