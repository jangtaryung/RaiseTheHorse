using UnityEngine;

/// <summary>
/// 적 사망 시 EXP를 크루 전원에게 분배하고,
/// 레벨업이 발생하면 GameStateManager.TriggerLevelUp()을 호출합니다.
/// ChariotCrewController와 같은 GameObject에 붙이거나,
/// crewController 필드에 씬의 ChariotCrewController를 연결합니다.
/// </summary>
public class ExperienceSystem : MonoBehaviour
{
    [Header("크루 참조")]
    [SerializeField] private ChariotStats chariotStats;

    private void Awake()
    {
        EnemyManager.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemyDied -= HandleEnemyDied;
    }

    private void HandleEnemyDied(float expReward)
    {
        ChariotCrew crew = GetCrew();
        if (crew == null) return;

        bool anyLevelUp = false;
        anyLevelUp |= TryGainExp(crew.Coachman, expReward);
        anyLevelUp |= TryGainExp(crew.Archer, expReward);
        anyLevelUp |= TryGainExp(crew.Lancer, expReward);
        anyLevelUp |= TryGainExp(crew.Swordsman, expReward);

        if (anyLevelUp)
            GameStateManager.Instance?.TriggerLevelUp();
    }

    private bool TryGainExp(CrewMemberBase member, float exp)
    {
        if (member == null) return false;
        int levelBefore = member.Level;
        member.GainExp(exp);
        return member.Level > levelBefore;
    }

    private ChariotCrew GetCrew()
    {
        if (chariotStats != null)
        {
            var chariot = chariotStats.GetChariot();
            if (chariot != null) return chariot.Crew;
        }

        var playerChariot = GameStateManager.Instance?.PlayerChariot;
        return playerChariot?.Crew;
    }
}
