using System;
using UnityEngine;

public class Chariot
{
    public ChariotCrew Crew { get; private set; }

    // 하드웨어
    public HorseSpec Horse { get; private set; }
    public ArmorSpec Armor { get; private set; }
    public WheelSpec Wheel { get; private set; }

    // HP
    private float currentHP;
    private bool godMode;
    private Action onDeath;

    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => GetChariotDurability();

    /// <summary>회피 발생 시 호출되는 콜백 (UI/이펙트 연동용).</summary>
    public event Action OnEvasion;

    public Chariot(
        HorseSpec horse,
        ArmorSpec armor,
        WheelSpec wheel,
        ChariotCrew crew,
        bool godMode = false,
        Action onDeath = null)
    {
        Horse = horse;
        Armor = armor;
        Wheel = wheel;
        Crew = crew;
        this.godMode = godMode;
        this.onDeath = onDeath;
        currentHP = GetMaxHP();
    }

    public void SetGodMode(bool enabled) => godMode = enabled;
    public void SetOnDeath(Action callback) => onDeath = callback;

    public void ResetHP()
    {
        currentHP = GetMaxHP();
    }

    /// <summary>현재 HP를 MaxHP까지 회복합니다. Durability 업그레이드 후 호출 가능.</summary>
    public void HealToMax()
    {
        currentHP = GetMaxHP();
    }

    public void TakeDamage(float dmg)
    {
        if (godMode)
            return;

        // 회피 판정: 마부 숙련도 + 무게 기반 회피율
        float evasion = GetCurrentEvasion();
        if (UnityEngine.Random.value < evasion)
        {
            OnEvasion?.Invoke();
            return;
        }

        float finalDamage = Mathf.Max(1f, dmg - GetChariotDefense());
        currentHP -= finalDamage;

        if (currentHP <= 0f)
        {
            currentHP = 0f;
            onDeath?.Invoke();
        }
    }

    public void Embark(ChariotCrew crew)
    {
        Crew = crew;
    }

    // ====== 무게 계산 ======
    public float GetTotalWeight()
    {
        float playersWeight = Crew != null ? Crew.GetTotalWeight() : 0f;

        float hardwareWeight =
            (Horse != null ? Horse.Weight : 0f) +
            (Armor != null ? Armor.Weight : 0f) +
            (Wheel != null ? Wheel.Weight : 0f);

        return playersWeight + hardwareWeight;
    }

    // ====== 이동 속도: 말 + 무게 + 마부 숙련 ======
    public float GetCurrentMoveSpeed()
    {
        float baseSpeed = Horse != null ? Horse.BaseMoveSpeed : 0f;
        if (baseSpeed <= 0f) return 0f;

        float totalWeight = GetTotalWeight();
        float pullCapacity = Horse != null ? Horse.PullCapacity : 1f;

        float loadRatio = totalWeight / pullCapacity;
        float loadPenalty;

        if (loadRatio <= 1.0f)
        {
            loadPenalty = 1f - (loadRatio - 0.5f) * 0.4f;
            loadPenalty = Mathf.Clamp(loadPenalty, 0.8f, 1f);
        }
        else
        {
            loadPenalty = 1f - (loadRatio - 1.0f) * 0.7f;
            loadPenalty = Mathf.Clamp(loadPenalty, 0.2f, 0.8f);
        }

        float handling = (Crew != null && Crew.Coachman != null) ? Crew.Coachman.ChariotHandlingSkill : 0f;
        float handlingBonus = 1f + handling * 0.02f;

        return baseSpeed * loadPenalty * handlingBonus;
    }

    // ====== 회피율 ======
    public float GetCurrentEvasion()
    {
        float baseEvasion = 0.05f;
        float handling = (Crew != null && Crew.Coachman != null) ? Crew.Coachman.ChariotHandlingSkill : 0f;
        float handlingBonus = handling * 0.003f;

        float totalWeight = GetTotalWeight();
        float pullCapacity = Horse != null ? Horse.PullCapacity : 1f;
        float loadRatio = totalWeight / pullCapacity;

        float weightPenalty = (loadRatio - 1f) * 0.05f;
        if (weightPenalty < 0f) weightPenalty = 0f;

        float evasion = baseEvasion + handlingBonus - weightPenalty;
        return Mathf.Clamp(evasion, 0.01f, 0.6f);
    }

    // ====== 장갑 ======
    public float GetChariotDurability()
    {
        return Armor != null ? Armor.Durability : 0f;
    }

    public float GetChariotDefense()
    {
        return Armor != null ? Armor.Defense : 0f;
    }

    // ====== 바퀴 ======
    public float GetAccel()
    {
        float accel = Wheel != null ? Wheel.Accel : 0f;
        float totalWeight = GetTotalWeight();
        float pullCapacity = Horse != null ? Horse.PullCapacity : 1f;
        float loadRatio = totalWeight / pullCapacity;

        float penalty = loadRatio > 1f ? 1f - (loadRatio - 1f) * 0.5f : 1f;
        penalty = Mathf.Clamp(penalty, 0.3f, 1f);

        return accel * penalty;
    }

    public float GetCollisionDamage()
    {
        float baseCollision = Wheel != null ? Wheel.CollisionDamage : 0f;
        float totalWeight = GetTotalWeight();

        float weightFactor = 1f + (totalWeight / 500f);
        return baseCollision * weightFactor;
    }
}
