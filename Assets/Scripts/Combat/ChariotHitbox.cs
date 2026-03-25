using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전차 피격 판정 컴포넌트.
/// BoxCollider2D 크기를 참조하여 OverlapBox로 충돌을 판정합니다.
/// Rigidbody2D 없이 동작합니다.
/// </summary>
public class ChariotHitbox : MonoBehaviour
{
    [Header("충돌 판정")]
    [SerializeField] private Vector2 boxSize = new Vector2(2f, 1.5f);
    [SerializeField] private Vector2 boxOffset = Vector2.zero;
    [SerializeField] private LayerMask hitLayers;

    [Header("전차 충돌 데미지")]
    [SerializeField] private float collisionCheckInterval = 0.3f;
    [SerializeField] private LayerMask chariotCollisionLayers;

    private Chariot chariot;
    private float collisionTimer;
    private static readonly List<Collider2D> hitBuffer = new List<Collider2D>(16);

    public Vector2 BoxSize => boxSize;
    public Vector2 BoxOffset => boxOffset;

    public void SetChariot(Chariot model)
    {
        chariot = model;
    }

    public Chariot GetChariot() => chariot;

    private void Start()
    {
        if (chariotCollisionLayers == 0)
            Debug.LogWarning($"[ChariotHitbox] {gameObject.name}: chariotCollisionLayers가 미설정. 전차 충돌 데미지가 작동하지 않습니다.", this);
    }

    private void Update()
    {
        if (chariot == null || chariot.GetCurrentHP() <= 0f) return;

        collisionTimer -= Time.deltaTime;
        if (collisionTimer > 0f) return;
        collisionTimer = collisionCheckInterval;

        CheckChariotCollision();
    }

    private void CheckChariotCollision()
    {
        if (chariotCollisionLayers == 0) return;

        Vector2 center = (Vector2)transform.position + boxOffset;
        var filter = new ContactFilter2D();
        filter.SetLayerMask(chariotCollisionLayers);
        filter.useLayerMask = true;

        hitBuffer.Clear();
        int count = Physics2D.OverlapBox(center, boxSize, 0f, filter, hitBuffer);

        for (int i = 0; i < count; i++)
        {
            if (hitBuffer[i].gameObject == gameObject) continue;

            var otherHitbox = hitBuffer[i].GetComponent<ChariotHitbox>();
            if (otherHitbox == null || otherHitbox.chariot == null) continue;

            float collisionDmg = chariot.GetCollisionDamage();
            otherHitbox.chariot.TakeDamage(collisionDmg);
        }
    }

    /// <summary>외부에서 이 전차에 투사체 데미지를 적용할 때 사용.</summary>
    public void ApplyDamage(float damage)
    {
        if (chariot != null)
            chariot.TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Vector3 center = transform.position + (Vector3)boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}
