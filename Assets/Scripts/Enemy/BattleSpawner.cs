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
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

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
        if (chariot == null || enemyManager == null || mainCam == null) return;
        if (enemyManager.AliveCount >= maxEnemies) return;

        // 화면 오른쪽 밖에서만 생성
        float camRight = mainCam.transform.position.x + mainCam.orthographicSize * mainCam.aspect;
        float minSpawnX = Mathf.Max(chariot.position.x + spawnDistance, camRight + 1f);
        Vector3 spawnPos = new Vector3(minSpawnX, chariot.position.y, -10f);

        enemyManager.Spawn(spawnPos);
    }
}
