using UnityEngine;

public class Chariot
{
    public ChariotCrew Crew { get; private set; }

    // 하드웨어
    public HorseSpec Horse { get; private set; }
    public ArmorSpec Armor { get; private set; }
    public WheelSpec Wheel { get; private set; }

    public Chariot(
        HorseSpec horse,
        ArmorSpec armor,
        WheelSpec wheel,
        ChariotCrew crew)
    {
        Horse = horse;
        Armor = armor;
        Wheel = wheel;
        Crew = crew;
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

        // 무게 패널티: 허용 중량보다 무거우면 급격히 저하
        float loadRatio = totalWeight / pullCapacity;  // 1.0 이하면 정상
        float loadPenalty;

        if (loadRatio <= 1.0f)
        {
            // 약간의 중량 증가에 따른 소폭 감소 (최대 -20%)
            loadPenalty = 1f - (loadRatio - 0.5f) * 0.4f; // 0.5~1.0 구간
            loadPenalty = Mathf.Clamp(loadPenalty, 0.8f, 1f);
        }
        else
        {
            // 초과하는 만큼 급격히 속도 감소
            loadPenalty = 1f - (loadRatio - 1.0f) * 0.7f;
            loadPenalty = Mathf.Clamp(loadPenalty, 0.2f, 0.8f);
        }

        // 마부의 전차 운용 숙련 보정
        float handling = (Crew != null && Crew.Coachman != null) ? Crew.Coachman.ChariotHandlingSkill : 0f;
        float handlingBonus = 1f + handling * 0.02f; // 숙련 50 → +100%

        return baseSpeed * loadPenalty * handlingBonus;
    }

    // ====== 회피율: 기본 + 마부 숙련 - 무게 패널티 ======
    public float GetCurrentEvasion()
    {
        float baseEvasion = 0.05f; // 5%
        float handling = (Crew != null && Crew.Coachman != null) ? Crew.Coachman.ChariotHandlingSkill : 0f;
        float handlingBonus = handling * 0.003f; // 숙련 50 → +15%

        float totalWeight = GetTotalWeight();
        float pullCapacity = Horse != null ? Horse.PullCapacity : 1f;
        float loadRatio = totalWeight / pullCapacity;

        // 무거울수록 회피율 감소
        float weightPenalty = (loadRatio - 1f) * 0.05f; // 초과 0.5 → -2.5% 등
        if (weightPenalty < 0f) weightPenalty = 0f;

        float evasion = baseEvasion + handlingBonus - weightPenalty;
        return Mathf.Clamp(evasion, 0.01f, 0.6f);
    }

    // ====== 장갑: 내구도 & 방어 ======
    public float GetChariotDurability()
    {
        return Armor != null ? Armor.Durability : 0f;
    }

    public float GetChariotDefense()
    {
        return Armor != null ? Armor.Defense : 0f;
    }

    // ====== 바퀴: 가속 & 충돌 데미지 ======
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

        // 무거울수록 충돌 데미지 증가
        float weightFactor = 1f + (totalWeight / 500f); // 500 단위당 +100%
        return baseCollision * weightFactor;
    }

    // ====== 예시 행동 메서드 ======
    public void Move(float deltaTime)
    {
        float speed = GetCurrentMoveSpeed();
        float distance = speed * deltaTime;
        Debug.Log($"Chariot moves {distance:F2} units. Speed={speed:F2}, Weight={GetTotalWeight():F1}");
    }

    public void CollisionAttack()
    {
        float damage = GetCollisionDamage();
        Debug.Log($"Chariot collides for {damage:F1} damage!");
    }
}
