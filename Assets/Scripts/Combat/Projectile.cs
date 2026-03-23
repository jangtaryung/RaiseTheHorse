using UnityEngine;

/// <summary>
/// 포물선 궤적으로 날아가는 범용 투사체. 화살, 투창 등에 사용.
/// ObjectPool과 연동되며, Launch()로 발사 → 도착 시 OverlapBox로 피격 판정 → 풀 반환.
/// </summary>
public class Projectile : PooledObject
{
    [Header("비행")]
    [SerializeField] private float flightSpeed = 15f;
    [SerializeField] private float arcHeight = 3f;

    [Header("착탄 분산")]
    [SerializeField] private float spreadX = 1.5f;

    [Header("착탄 판정")]
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1f, 1f);

    private LayerMask hitLayers;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float damage;

    private float journeyLength;
    private float traveled;
    private bool launched;
    private float randomArcHeight;

    public void Launch(Vector3 from, Vector3 to, float dmg, LayerMask targetLayers)
    {
        hitLayers = targetLayers;
        LaunchInternal(from, to, dmg, spreadX);
    }

    /// <summary>착탄 분산을 외부에서 지정하는 오버로드. 궁술 숙련 등으로 분산 감소 시 사용.</summary>
    public void Launch(Vector3 from, Vector3 to, float dmg, LayerMask targetLayers, float overrideSpread)
    {
        hitLayers = targetLayers;
        LaunchInternal(from, to, dmg, overrideSpread);
    }

    private void LaunchInternal(Vector3 from, Vector3 to, float dmg, float spread)
    {
        startPos = from;
        targetPos = new Vector3(to.x + Random.Range(-spread, spread), to.y, to.z);
        damage = dmg;

        journeyLength = Vector3.Distance(startPos, targetPos);
        traveled = 0f;
        launched = true;
        randomArcHeight = Random.Range(1f, arcHeight);

        transform.position = from;
    }

    private void Update()
    {
        if (!launched) return;

        traveled += flightSpeed * Time.deltaTime;
        float t = Mathf.Clamp01(traveled / journeyLength);

        float oneMinusT = 1f - t;
        float posX = startPos.x * oneMinusT + targetPos.x * t;
        float posY = startPos.y * oneMinusT + targetPos.y * t + randomArcHeight * 4f * t * oneMinusT;
        transform.position = new Vector3(posX, posY, startPos.z);

        if (t < 1f)
        {
            float nextT = Mathf.Clamp01((traveled + 0.1f) / journeyLength);
            float nextOneMinusT = 1f - nextT;
            float nextX = startPos.x * nextOneMinusT + targetPos.x * nextT;
            float nextY = startPos.y * nextOneMinusT + targetPos.y * nextT + randomArcHeight * 4f * nextT * nextOneMinusT;

            float dirX = nextX - posX;
            float dirY = nextY - posY;
            if (dirX * dirX + dirY * dirY > 0.0001f)
            {
                float angle = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        if (t >= 1f)
        {
            ApplyHit();
            launched = false;
            ReturnToPool();
        }
    }

    private void ApplyHit()
    {
        var hit = Physics2D.OverlapBox(targetPos, hitBoxSize, 0f, hitLayers);
        if (hit == null) 
            return;

        var hitbox = hit.GetComponent<ChariotHitbox>();

        if (hitbox != null)
        {
            hitbox.ApplyDamage(damage);
            //Debug.Log($"[Projectile] Hit {hit.gameObject.name} for {damage:F1} damage.");
        }
    }

    public override void OnReturned()
    {
        launched = false;
        traveled = 0f;
    }
}
