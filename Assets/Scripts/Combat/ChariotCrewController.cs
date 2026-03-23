using UnityEngine;

/// <summary>
/// 승무원(4인) 관리 전용.
/// 승무원 View를 빌드하고, ChariotStats에 전달하여 Chariot을 조립합니다.
/// </summary>
public class ChariotCrewController : MonoBehaviour
{
    [Header("전차 상태 참조")]
    [SerializeField] private ChariotStats chariotStats;
    [SerializeField] private bool isPlayer;

    [Header("승무원 GameObjects")]
    [SerializeField] private CoachmanView coachmanView;
    [SerializeField] private ArcherView archerView;
    [SerializeField] private LancerView lancerView;
    [SerializeField] private SwordsmanView swordsmanView;

    private void Reset()
    {
        chariotStats = GetComponent<ChariotStats>();
    }

    private void Awake()
    {
        if (chariotStats == null) chariotStats = GetComponent<ChariotStats>();

        var crew = BuildCrew();
        chariotStats.BuildChariot(crew);

        if (isPlayer && GameStateManager.Instance != null)
            GameStateManager.Instance.RegisterPlayer(chariotStats.GetChariot(), chariotStats);
    }

    private ChariotCrew BuildCrew()
    {
        var coachman = coachmanView != null ? coachmanView.Build() : null;
        var archer = archerView != null ? archerView.Build() : null;
        var lancer = lancerView != null ? lancerView.Build() : null;
        var swordsman = swordsmanView != null ? swordsmanView.Build() : null;

        return new ChariotCrew(coachman, archer, lancer, swordsman);
    }
}
