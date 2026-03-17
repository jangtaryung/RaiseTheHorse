using UnityEngine;

/// <summary>
/// 마부 GameObject 래퍼. 씬에 배치하면 ChariotCrewController가 참조합니다.
/// </summary>
public class CoachmanView : MonoBehaviour
{
    [Header("마부 성장 데이터")]
    [SerializeField] private int level = 1;
    [SerializeField] private float handlingSkill = 0f;

    [Header("신체")]
    [SerializeField] private float bodyWeight = 75f;
    [SerializeField] private float baseAttack = 5f;
    [SerializeField] private float baseDefense = 8f;
    [SerializeField] private float baseMaxHp = 110f;

    public CoachmanPlayer RuntimeModel { get; private set; }

    public CoachmanPlayer Build()
    {
        var model = new CoachmanPlayer(
            displayName: gameObject.name,
            handlingSkill: handlingSkill,
            bodyWeight: bodyWeight,
            baseAttack: baseAttack,
            baseDefense: baseDefense,
            baseMaxHp: baseMaxHp);

        ForceSetLevel(model, level);
        RuntimeModel = model;
        return model;
    }

    private static void ForceSetLevel(CrewMemberBase member, int targetLevel)
    {
        targetLevel = Mathf.Max(1, targetLevel);
        while (member.Level < targetLevel)
            member.GainExp(member.ExpToNextLevel());
    }
}
