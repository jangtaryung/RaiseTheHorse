using UnityEngine;

/// <summary>
/// 포물선 궤적으로 날아가는 범용 투사체. 화살, 투창 등에 사용.
/// ObjectPool과 연동되며, Launch()로 발사 → 도착 시 데미지 적용 → 풀 반환.
/// 플레이어→적(EnemyManager), 적→플레이어(ChariotStats) 양방향 지원.
/// </summary>
public class Projectile : PooledObject
{
    [Header("비행")]
    [SerializeField] private float flightSpeed = 15f;
    [SerializeField] private float arcHeight = 3f;

    [Header("착탄 분산")]
    [SerializeField] private float spreadX = 1.5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float damage;

    // 데미지 대상 (둘 중 하나만 사용)
    private int targetId;
    private EnemyManager enemyManager;
    private ChariotStats targetStats;

    private float journeyLength;
    private float traveled;
    private bool launched;

    /// <summary>플레이어 → 적 공격용</summary>
    public void Launch(Vector3 from, Vector3 to, float dmg, int enemyId, EnemyManager manager)
    {
        enemyManager = manager;
        targetId = enemyId;
        targetStats = null;
        LaunchInternal(from, to, dmg);
    }

    /// <summary>적 → 플레이어 공격용</summary>
    public void Launch(Vector3 from, Vector3 to, float dmg, ChariotStats player)
    {
        targetStats = player;
        enemyManager = null;
        targetId = -1;
        LaunchInternal(from, to, dmg);
    }

    private void LaunchInternal(Vector3 from, Vector3 to, float dmg)
    {
        startPos = from;
        targetPos = new Vector3(to.x + Random.Range(-spreadX, spreadX), to.y, to.z);
        damage = dmg;

        journeyLength = Vector3.Distance(startPos, targetPos);
        traveled = 0f;
        launched = true;

        transform.position = from;

        //Debug.Log($"[Projectile] Launch from {from} to {to} dmg:{dmg:F1}");
    }

    private void Update()
    {
        if (!launched) return;

        traveled += flightSpeed * Time.deltaTime;
        float t = Mathf.Clamp01(traveled / journeyLength);

        // 선형 보간 + 포물선 높이 (Vector3.Lerp 대신 직접 계산으로 할당 절감)
        float oneMinusT = 1f - t;
        float posX = startPos.x * oneMinusT + targetPos.x * t;
        float posY = startPos.y * oneMinusT + targetPos.y * t + arcHeight * 4f * t * oneMinusT;
        transform.position = new Vector3(posX, posY, startPos.z);

        // 진행 방향으로 회전
        if (t < 1f)
        {
            float nextT = Mathf.Clamp01((traveled + 0.1f) / journeyLength);
            float nextOneMinusT = 1f - nextT;
            float nextX = startPos.x * nextOneMinusT + targetPos.x * nextT;
            float nextY = startPos.y * nextOneMinusT + targetPos.y * nextT + arcHeight * 4f * nextT * nextOneMinusT;

            float dirX = nextX - posX;
            float dirY = nextY - posY;
            if (dirX * dirX + dirY * dirY > 0.0001f)
            {
                float angle = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        // 도착
        if (t >= 1f)
        {
            if (enemyManager != null)
                enemyManager.ApplyDamage(targetId, damage);
            else if (targetStats != null)
                targetStats.TakeDamage(damage);

            launched = false;
            ReturnToPool();
        }
    }

    public override void OnReturned()
    {
        launched = false;
        traveled = 0f;
        enemyManager = null;
        targetStats = null;
    }
}
