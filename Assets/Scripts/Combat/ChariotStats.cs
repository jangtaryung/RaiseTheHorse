using UnityEngine;

/// <summary>
/// 전차 자체의 상태: HP, 방어력, 이동 속도.
/// 이동 속도는 Chariot 모델(마부 숙련 + 말 + 무게)에서 실시간 계산됩니다.
/// </summary>
public class ChariotStats : MonoBehaviour
{
    [Header("방어력/체력")]
    public float baseDefense = 5f;
    public float maxHP = 100f;
    public float currentHP;

    [Header("이동 속도 (fallback)")]
    [SerializeField] private float fallbackMoveSpeed = 3f;

    private Chariot chariot;

    /// <summary>Chariot 모델과 실시간 연동. 마부 숙련/무게 변화가 즉시 반영됩니다.</summary>
    public float moveSpeed => chariot != null
        ? Mathf.Max(0.5f, chariot.GetCurrentMoveSpeed())
        : fallbackMoveSpeed;

    public void SetChariot(Chariot model)
    {
        chariot = model;
    }

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float dmg)
    {
        float finalDamage = Mathf.Max(1f, dmg - baseDefense);
        currentHP -= finalDamage;

        if (currentHP <= 0f)
        {
            currentHP = 0f;
            OnDeath();
        }
    }

    private void OnDeath()
    {
        Debug.Log(this.tag + "Chariot Destroyed");
        gameObject.SetActive(false);
    }
}
