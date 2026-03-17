using UnityEngine;

public class StatDebugUI : MonoBehaviour
{
    public ArcherView archerView;
    public LancerView lancerView;
    public SwordsmanView swordsmanView;
    public CoachmanView coachmanView;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && archerView != null && archerView.RuntimeModel != null)
        {
            var m = archerView.RuntimeModel;
            Debug.Log($"[궁병] Lv:{m.Level} Atk:{m.GetAttack():F1} Skill:{m.ArcherySkill:F1} Range:{archerView.GetEffectiveRange():F1} Dmg:{archerView.GetDamage():F1}");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && lancerView != null && lancerView.RuntimeModel != null)
        {
            var m = lancerView.RuntimeModel;
            Debug.Log($"[창병] Lv:{m.Level} Atk:{m.GetAttack():F1} Skill:{m.LanceSkill:F1} Range:{lancerView.GetEffectiveRange():F1} Dmg:{lancerView.GetDamage():F1}");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && swordsmanView != null && swordsmanView.RuntimeModel != null)
        {
            var m = swordsmanView.RuntimeModel;
            Debug.Log($"[검병] Lv:{m.Level} Atk:{m.GetAttack():F1} Skill:{m.SwordSkill:F1} Range:{swordsmanView.GetEffectiveRange():F1} Dmg:{swordsmanView.GetDamage():F1}");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && coachmanView != null && coachmanView.RuntimeModel != null)
        {
            var m = coachmanView.RuntimeModel;
            Debug.Log($"[마부] Lv:{m.Level} Handling:{m.ChariotHandlingSkill:F1}");
        }
    }
}
