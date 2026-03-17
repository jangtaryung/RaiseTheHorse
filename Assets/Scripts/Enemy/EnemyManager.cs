using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 전차(EnemyChariot) 관리자. 내장 풀링으로 100+마리도 GC 부담 없이 처리합니다.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    [Header("적 전차 프리팹")]
    public EnemyChariot enemyChariotPrefab;

    [Header("풀링")]
    public int prewarmCount = 20;

    [Header("플레이어 전차 참조")]
    public Transform playerChariot;
    public ChariotStats playerStats;

    [Header("적 투사체 공유 풀")]
    public ObjectPool enemyArrowPool;
    public ObjectPool enemySpearPool;

    private readonly List<EnemyChariot> activeEnemies = new List<EnemyChariot>();
    private readonly Queue<EnemyChariot> pool = new Queue<EnemyChariot>();

    public int AliveCount => activeEnemies.Count;

    private int prewarmRemaining;

    private void Awake()
    {
        prewarmRemaining = prewarmCount;
    }

    private void Update()
    {
        // prewarm을 프레임당 2개씩 분산 생성 (시작 시 병목 방지)
        if (prewarmRemaining > 0)
        {
            int batch = Mathf.Min(2, prewarmRemaining);
            for (int i = 0; i < batch; i++)
                pool.Enqueue(CreateInstance());
            prewarmRemaining -= batch;
        }
    }

    private EnemyChariot CreateInstance()
    {
        // 프리팹을 비활성 상태로 복제 → Awake() 체인이 실행되지 않음
        // 나중에 SetActive(true) 될 때 Awake()가 최초 1회 실행됨
        bool prefabWasActive = enemyChariotPrefab.gameObject.activeSelf;
        enemyChariotPrefab.gameObject.SetActive(false);

        var enemy = Instantiate(enemyChariotPrefab, transform);

        enemyChariotPrefab.gameObject.SetActive(prefabWasActive);
        return enemy;
    }

    public void Spawn(Vector3 position)
    {
        if (enemyChariotPrefab == null) return;

        EnemyChariot enemy;

        if (pool.Count > 0)
        {
            enemy = pool.Dequeue();
            enemy.ResetForPool(position, playerChariot, playerStats);
        }
        else
        {
            enemy = CreateInstance();
            enemy.gameObject.SetActive(true);
            enemy.transform.position = position;
            enemy.Init(playerChariot, playerStats);
        }

        enemy.SetPools(enemyArrowPool, enemySpearPool);
        activeEnemies.Add(enemy);
    }

    /// <summary>가장 가까운 살아있는 적 전차를 찾습니다.</summary>
    public bool TryGetClosest(Vector3 from, out int id, out Vector3 position)
    {
        id = -1;
        position = default;
        float best = float.MaxValue;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] == null || activeEnemies[i].IsDead) continue;

            float d = Mathf.Abs(activeEnemies[i].transform.position.x - from.x);
            if (d < best)
            {
                best = d;
                id = i;
                position = activeEnemies[i].transform.position;
            }
        }

        return id >= 0;
    }

    /// <summary>사거리 내 가장 가까운 살아있는 적 전차를 찾습니다.</summary>
    public bool TryGetClosestWithinRange(Vector3 from, float maxRange, out int id, out Vector3 position)
    {
        id = -1;
        position = default;
        float best = maxRange;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] == null || activeEnemies[i].IsDead) continue;

            float d = Mathf.Abs(activeEnemies[i].transform.position.x - from.x);
            if (d <= maxRange && d < best)
            {
                best = d;
                id = i;
                position = activeEnemies[i].transform.position;
            }
        }

        return id >= 0;
    }

    /// <summary>적 전차의 현재 위치를 가져옵니다.</summary>
    public bool TryGetPosition(int id, out Vector3 position)
    {
        position = default;
        if (id < 0 || id >= activeEnemies.Count) return false;
        if (activeEnemies[id] == null || activeEnemies[id].IsDead) return false;

        position = activeEnemies[id].transform.position;
        return true;
    }

    /// <summary>적 전차에 데미지를 적용합니다.</summary>
    public void ApplyDamage(int id, float damage)
    {
        if (id < 0 || id >= activeEnemies.Count) return;
        if (activeEnemies[id] == null || activeEnemies[id].IsDead) return;

        activeEnemies[id].TakeDamage(damage);
    }

    private void LateUpdate()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            if (enemy == null)
            {
                activeEnemies.RemoveAt(i);
                continue;
            }

            if (enemy.IsDead && !enemy.gameObject.activeSelf)
            {
                activeEnemies.RemoveAt(i);
                pool.Enqueue(enemy);
            }
        }
    }
}
