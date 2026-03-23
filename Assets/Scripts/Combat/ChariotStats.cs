using System;
using UnityEngine;

/// <summary>
/// 전차 하드웨어 스펙 보유 + Chariot 조립.
/// ChariotCrewController로부터 승무원을 받아 Chariot을 생성합니다.
/// 런타임 전투(HP, 데미지, 사망)는 Chariot이 담당합니다.
/// </summary>
public class ChariotStats : MonoBehaviour
{
    [Header("디버그")]
    [SerializeField] private bool godMode;

    [Header("플레이어 여부")]
    [SerializeField] private bool isPlayer;

    [Header("말")]
    [SerializeField] private float horsePullCapacity = 800f;
    [SerializeField] private float horseBaseMoveSpeed = 12f;
    [SerializeField] private float horseWeight = 400f;

    [Header("갑옷")]
    [SerializeField] private float armorDurability = 500f;
    [SerializeField] private float armorDefense = 30f;
    [SerializeField] private float armorWeight = 300f;

    [Header("바퀴")]
    [SerializeField] private float wheelAccel = 5f;
    [SerializeField] private float wheelCollisionDamage = 50f;
    [SerializeField] private float wheelWeight = 100f;

    private Chariot chariot;

    /// <summary>HP가 0이 되는 순간 발행됩니다.</summary>
    public event Action OnDeathEvent;

    public Chariot GetChariot() => chariot;

    /// <summary>승무원을 받아 Chariot을 조립합니다.</summary>
    public void BuildChariot(ChariotCrew crew)
    {
        var horse = new HorseSpec("Horse", horsePullCapacity, horseBaseMoveSpeed, horseWeight);
        var armor = new ArmorSpec("Armor", armorDurability, armorDefense, armorWeight);
        var wheel = new WheelSpec("Wheel", wheelAccel, wheelCollisionDamage, wheelWeight);

        chariot = new Chariot(horse, armor, wheel, crew, godMode);
        chariot.SetOnDeath(() =>
        {
            OnDeathEvent?.Invoke();

            if (isPlayer)
                GameStateManager.Instance?.OnPlayerDeath();

            gameObject.SetActive(false);
        });
    }
}
