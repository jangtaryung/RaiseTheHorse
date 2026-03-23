using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 전차(EnemyChariot) 관리자. 내장 풀링으로 100+마리도 GC 부담 없이 처리합니다.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>적 전차가 사망할 때 발행됩니다. float = expReward.</summary>
    public static event Action<float> OnEnemyDied;

    [Header("적 전차 프리팹")]
    public EnemyChariot enemyChariotPrefab;

    [Header("풀링")]
    [SerializeField] private int prewarmCount = 20;

    [Header("플레이어 전차 참조")]
    public Transform playerChariot;
    public ChariotStats playerChariotStats;

    [Header("적 투사체 공유 풀")]
    public ObjectPool enemyArrowPool;
    public ObjectPool enemySpearPool;

    [Header("적 HP UI")]
    public EnemyHPBarManager enemyHPBarManager;

    private readonly List<EnemyChariot> activeEnemies = new List<EnemyChariot>();
    private readonly Queue<EnemyChariot> pool = new Queue<EnemyChariot>();

    public int AliveCount => activeEnemies.Count;

    private int prewarmRemaining;
    private Chariot playerChariotModel;

    private void Awake()
    {
        prewarmRemaining = prewarmCount;
    }

    private void Start()
    {
        if (playerChariotStats != null)
            playerChariotModel = playerChariotStats.GetChariot();
    }

    private void Update()
    {
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
        bool prefabWasActive = enemyChariotPrefab.gameObject.activeSelf;
        enemyChariotPrefab.gameObject.SetActive(false);

        var enemy = Instantiate(enemyChariotPrefab, transform);

        enemyChariotPrefab.gameObject.SetActive(prefabWasActive);
        return enemy;
    }

    private void ResolveChariot(EnemyChariot enemy)
    {
        if (enemy.GetChariot() != null) return;

        var stats = enemy.GetComponent<ChariotStats>();
        if (stats != null)
            enemy.SetChariot(stats.GetChariot());
    }

    public void Spawn(Vector3 position)
    {
        if (enemyChariotPrefab == null) return;

        if (playerChariotModel == null && playerChariotStats != null)
            playerChariotModel = playerChariotStats.GetChariot();

        EnemyChariot enemy;

        if (pool.Count > 0)
        {
            enemy = pool.Dequeue();
            enemy.ResetForPool(position, playerChariot, playerChariotModel);
            ResolveChariot(enemy);
        }
        else
        {
            enemy = CreateInstance();
            enemy.gameObject.SetActive(true);
            enemy.transform.position = position;
            ResolveChariot(enemy);
            enemy.Init(playerChariot, playerChariotModel);
        }

        enemy.SetPools(enemyArrowPool, enemySpearPool);
        activeEnemies.Add(enemy);

        if (enemyHPBarManager != null)
            enemyHPBarManager.Assign(enemy);
    }

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

    public bool TryGetPosition(int id, out Vector3 position)
    {
        position = default;
        if (id < 0 || id >= activeEnemies.Count) return false;
        if (activeEnemies[id] == null || activeEnemies[id].IsDead) return false;

        position = activeEnemies[id].transform.position;
        return true;
    }

    public void ApplyDamage(int id, float damage)
    {
        if (id < 0 || id >= activeEnemies.Count) return;
        if (activeEnemies[id] == null || activeEnemies[id].IsDead) return;

        activeEnemies[id].TakeDamage(damage);
    }

    /// <summary>적을 공격자 반대 방향으로 밀어냅니다.</summary>
    public void ApplyKnockback(int id, float knockbackDistance, Vector3 attackerPos)
    {
        if (id < 0 || id >= activeEnemies.Count) return;
        if (activeEnemies[id] == null || activeEnemies[id].IsDead) return;

        float dirX = activeEnemies[id].transform.position.x > attackerPos.x ? 1f : -1f;
        activeEnemies[id].transform.position += new Vector3(dirX * knockbackDistance, 0f, 0f);
    }

    private void LateUpdate()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            bool remove = enemy == null || (enemy.IsDead && !enemy.gameObject.activeSelf);

            if (!remove) continue;

            if (enemy != null)
            {
                OnEnemyDied?.Invoke(enemy.ExpReward);

                if (enemyHPBarManager != null)
                    enemyHPBarManager.Release(enemy);
                pool.Enqueue(enemy);
            }

            int last = activeEnemies.Count - 1;
            activeEnemies[i] = activeEnemies[last];
            activeEnemies.RemoveAt(last);
        }
    }
}
