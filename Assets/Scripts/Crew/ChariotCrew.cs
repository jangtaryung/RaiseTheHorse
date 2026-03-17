using System;
using UnityEngine;

[Serializable]
public class ChariotCrew
{
    [field: SerializeField] public CoachmanPlayer Coachman { get; private set; }
    [field: SerializeField] public ArcherPlayer Archer { get; private set; }
    [field: SerializeField] public LancerPlayer Lancer { get; private set; }
    [field: SerializeField] public SwordsmanPlayer Swordsman { get; private set; }

    public ChariotCrew(CoachmanPlayer coachman, ArcherPlayer archer, LancerPlayer lancer, SwordsmanPlayer swordsman)
    {
        Coachman = coachman;
        Archer = archer;
        Lancer = lancer;
        Swordsman = swordsman;
    }

    public float GetTotalWeight()
    {
        return
            (Coachman != null ? Coachman.GetTotalWeight() : 0f) +
            (Archer != null ? Archer.GetTotalWeight() : 0f) +
            (Lancer != null ? Lancer.GetTotalWeight() : 0f) +
            (Swordsman != null ? Swordsman.GetTotalWeight() : 0f);
    }
}
