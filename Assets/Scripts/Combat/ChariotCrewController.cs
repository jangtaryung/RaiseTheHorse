using UnityEngine;

/// <summary>
/// Crew(4인) + 하드웨어 스펙을 한 곳에서 관리하고,
/// 전차 상태(ChariotStats)에 방어/HP/이동속도를 반영합니다.
/// 공격은 각 View가 직접 수행합니다.
/// </summary>
public class ChariotCrewController : MonoBehaviour
{
    [Header("전차 상태 참조")]
    [SerializeField] private ChariotStats chariotStats;

    [Header("하드웨어(간단 버전)")]
    [SerializeField] private float horsePullCapacity = 800f;
    [SerializeField] private float horseBaseMoveSpeed = 12f;
    [SerializeField] private float horseWeight = 400f;

    [SerializeField] private float armorDurability = 500f;
    [SerializeField] private float armorDefense = 30f;
    [SerializeField] private float armorWeight = 300f;

    [SerializeField] private float wheelAccel = 5f;
    [SerializeField] private float wheelCollisionDamage = 50f;
    [SerializeField] private float wheelWeight = 100f;

    [Header("Crew GameObjects (씬에 배치된 승무원)")]
    [SerializeField] private CoachmanView coachmanView;
    [SerializeField] private ArcherView archerView;
    [SerializeField] private LancerView lancerView;
    [SerializeField] private SwordsmanView swordsmanView;

    public Chariot RuntimeChariot { get; private set; }

    private void Reset()
    {
        chariotStats = GetComponent<ChariotStats>();
    }

    private void Awake()
    {
        if (chariotStats == null) chariotStats = GetComponent<ChariotStats>();

        BuildRuntimeModel();
        ApplyToChariot();
    }

    public void BuildRuntimeModel()
    {
        var horse = new HorseSpec("Horse", horsePullCapacity, horseBaseMoveSpeed, horseWeight);
        var armor = new ArmorSpec("Armor", armorDurability, armorDefense, armorWeight);
        var wheel = new WheelSpec("Wheel", wheelAccel, wheelCollisionDamage, wheelWeight);

        var coachman = coachmanView != null ? coachmanView.Build() : null;
        var archer = archerView != null ? archerView.Build() : null;
        var lancer = lancerView != null ? lancerView.Build() : null;
        var swordsman = swordsmanView != null ? swordsmanView.Build() : null;

        var crew = new ChariotCrew(coachman, archer, lancer, swordsman);
        RuntimeChariot = new Chariot(horse, armor, wheel, crew);
    }

    public void ApplyToChariot()
    {
        if (RuntimeChariot == null || chariotStats == null) return;

        chariotStats.baseDefense = RuntimeChariot.GetChariotDefense();
        chariotStats.maxHP = RuntimeChariot.GetChariotDurability();
        if (chariotStats.currentHP > chariotStats.maxHP) chariotStats.currentHP = chariotStats.maxHP;

        // Chariot 모델 주입 → moveSpeed가 마부 숙련/무게와 실시간 연동
        chariotStats.SetChariot(RuntimeChariot);
    }
}
