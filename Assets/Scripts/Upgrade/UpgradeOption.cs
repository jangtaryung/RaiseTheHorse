using System;
using UnityEngine;

/// <summary>
/// 레벨업 시 제시되는 업그레이드 선택지 데이터.
/// UpgradeSelectionUI의 upgradePool 배열에 채워 넣습니다.
/// </summary>
[Serializable]
public class UpgradeOption
{
    public string displayName;
    [TextArea] public string description;
    public UpgradeType upgradeType;
    public float value;

    /// <summary>선택 시 Chariot 모델과 ChariotStats에 스탯을 반영합니다.</summary>
    public void Apply(Chariot chariot, ChariotStats stats)
    {
        if (chariot == null) return;

        switch (upgradeType)
        {
            // ── 말 ──────────────────────────────────────────────
            case UpgradeType.HorseSpeed:
                chariot.Horse?.ApplySpeedUpgrade(value);
                break;
            case UpgradeType.HorsePull:
                chariot.Horse?.ApplyPullUpgrade(value);
                break;

            // ── 장갑 ────────────────────────────────────────────
            case UpgradeType.ArmorDefense:
                chariot.Armor?.ApplyDefenseUpgrade(value);
                break;
            case UpgradeType.ArmorDurability:
                chariot.Armor?.ApplyDurabilityUpgrade(value);
                break;

            // ── 바퀴 ────────────────────────────────────────────
            case UpgradeType.WheelAccel:
                chariot.Wheel?.ApplyAccelUpgrade(value);
                break;
            case UpgradeType.WheelCollision:
                chariot.Wheel?.ApplyCollisionUpgrade(value);
                break;

            // ── 크루 스킬 ────────────────────────────────────────
            case UpgradeType.CoachmanAttack:
                chariot.Crew?.Coachman?.ApplyAttackBonus(value);
                break;
            case UpgradeType.ArcherAttack:
                chariot.Crew?.Archer?.ApplyAttackBonus(value);
                break;
            case UpgradeType.LancerAttack:
                chariot.Crew?.Lancer?.ApplyAttackBonus(value);
                break;
            case UpgradeType.SwordsmanAttack:
                chariot.Crew?.Swordsman?.ApplyAttackBonus(value);
                break;
        }
    }
}

public enum UpgradeType
{
    HorseSpeed,
    HorsePull,
    ArmorDefense,
    ArmorDurability,
    WheelAccel,
    WheelCollision,
    CoachmanAttack,
    ArcherAttack,
    LancerAttack,
    SwordsmanAttack,
}
