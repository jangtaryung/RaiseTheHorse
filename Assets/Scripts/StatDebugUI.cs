using UnityEngine;
using UnityEngine.InputSystem;

public class StatDebugUI : MonoBehaviour
{
    public ArcherView archerView;
    public LancerView lancerView;
    public SwordsmanView swordsmanView;
    public CoachmanView coachmanView;
    [SerializeField] private MonoBehaviour[] chariotCombatTargets;

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame && archerView != null && archerView.RuntimeModel != null)
        {
            var m = archerView.RuntimeModel;
            Debug.Log($"[궁병] Lv:{m.Level} Atk:{m.GetAttack():F1} Skill:{m.ArcherySkill:F1} Range:{archerView.GetEffectiveRange():F1} Dmg:{archerView.GetDamage():F1}");
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame && lancerView != null && lancerView.RuntimeModel != null)
        {
            var m = lancerView.RuntimeModel;
            Debug.Log($"[창병] Lv:{m.Level} Atk:{m.GetAttack():F1} Skill:{m.LanceSkill:F1} Range:{lancerView.GetEffectiveRange():F1} Dmg:{lancerView.GetDamage():F1}");
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame && swordsmanView != null && swordsmanView.RuntimeModel != null)
        {
            var m = swordsmanView.RuntimeModel;
            Debug.Log($"[검병] Lv:{m.Level} Atk:{m.GetAttack():F1} Skill:{m.SwordSkill:F1} Range:{swordsmanView.GetEffectiveRange():F1} Dmg:{swordsmanView.GetDamage():F1}");
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame && coachmanView != null && coachmanView.RuntimeModel != null)
        {
            var m = coachmanView.RuntimeModel;
            Debug.Log($"[마부] Lv:{m.Level} Handling:{m.ChariotHandlingSkill:F1}");
        }

    }

    // ===== 디버그: 배타적 권역 시각화 =====
    private void OnDrawGizmos()
    {
        if (chariotCombatTargets == null) return;

        foreach (var target in chariotCombatTargets)
        {
            if (target is not IChariotCombat chariot) continue;

            chariot.GetZoneBoundaries(out float sMax, out float lMax, out float aMax);
            Vector3 pos = chariot.transform.position;
            float y = pos.y;
            float height = 1f;

            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
            DrawZone(pos, 0f, sMax, y, height);
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            DrawZoneBorder(pos, 0f, sMax, y, height);

            Gizmos.color = new Color(0.2f, 0.4f, 1f, 0.3f);
            DrawZone(pos, sMax, lMax, y, height);
            Gizmos.color = new Color(0.2f, 0.4f, 1f, 0.8f);
            DrawZoneBorder(pos, sMax, lMax, y, height);

            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.3f);
            DrawZone(pos, lMax, aMax, y, height);
            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.8f);
            DrawZoneBorder(pos, lMax, aMax, y, height);
        }
    }

    private void DrawZone(Vector3 center, float minDist, float maxDist, float y, float height)
    {
        Vector3 rCenter = new Vector3(center.x + (minDist + maxDist) * 0.5f, y, 0f);
        Vector3 rSize = new Vector3(maxDist - minDist, height, 0f);
        Gizmos.DrawCube(rCenter, rSize);

        Vector3 lCenter = new Vector3(center.x - (minDist + maxDist) * 0.5f, y, 0f);
        Gizmos.DrawCube(lCenter, rSize);
    }

    private void DrawZoneBorder(Vector3 center, float minDist, float maxDist, float y, float height)
    {
        float halfH = height * 0.5f;

        Gizmos.DrawLine(new Vector3(center.x + maxDist, y - halfH, 0f), new Vector3(center.x + maxDist, y + halfH, 0f));
        if (minDist > 0f)
            Gizmos.DrawLine(new Vector3(center.x + minDist, y - halfH, 0f), new Vector3(center.x + minDist, y + halfH, 0f));

        Gizmos.DrawLine(new Vector3(center.x - maxDist, y - halfH, 0f), new Vector3(center.x - maxDist, y + halfH, 0f));
        if (minDist > 0f)
            Gizmos.DrawLine(new Vector3(center.x - minDist, y - halfH, 0f), new Vector3(center.x - minDist, y + halfH, 0f));
    }
}
