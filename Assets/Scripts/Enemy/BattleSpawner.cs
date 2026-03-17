using UnityEngine;

public class BattleSpawner : MonoBehaviour
{
    [Header("참조")]
    public Transform chariot;
    public EnemyManager enemyManager;

    [Header("스폰 설정")]
    public float spawnInterval = 2f;
    public float spawnDistance = 15f;
    public int maxEnemies = 10;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        if (chariot == null || enemyManager == null) return;
        if (enemyManager.AliveCount >= maxEnemies) return;

        // 화면 오른쪽 밖에서만 생성
        float camRight = Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;
        float minSpawnX = Mathf.Max(chariot.position.x + spawnDistance, camRight + 1f);
        Vector3 spawnPos = new Vector3(minSpawnX, chariot.position.y, 0f);

        enemyManager.Spawn(spawnPos);
    }
}
