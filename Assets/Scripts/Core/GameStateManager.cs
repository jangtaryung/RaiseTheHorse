using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Idle,
    Battle,
    LevelUpSelection,
    GameOver
}

/// <summary>
/// 게임 전체 상태 머신. Idle → Battle → LevelUpSelection ↔ Battle → GameOver.
/// Time.timeScale 제어: LevelUpSelection/GameOver 시 0, Battle/Idle 시 1.
/// 씬에 하나만 배치. ChariotCrewController.Awake() 이후 RegisterPlayer()를 호출해야 합니다.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Idle;

    /// <summary>플레이어 전차 모델. ChariotCrewController가 RegisterPlayer()로 주입.</summary>
    public Chariot PlayerChariot { get; private set; }
    public ChariotStats PlayerChariotStats { get; private set; }

    /// <summary>이번 판 총 처치 수.</summary>
    public int TotalKills { get; private set; }

    /// <summary>상태가 바뀔 때마다 발행됩니다.</summary>
    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 씬이 로드되면 즉시 전투 시작
        TransitionTo(GameState.Battle);
        EnemyManager.OnEnemyDied += HandleEnemyDied;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            EnemyManager.OnEnemyDied -= HandleEnemyDied;
        }
    }

    // ===== 외부 등록 =====

    /// <summary>ChariotCrewController 또는 BattleManager에서 Awake 이후 호출.</summary>
    public void RegisterPlayer(Chariot chariot, ChariotStats stats)
    {
        PlayerChariot = chariot;
        PlayerChariotStats = stats;
    }

    // ===== 상태 전이 API =====

    public void OnPlayerDeath()
    {
        if (CurrentState == GameState.GameOver) return;
        TransitionTo(GameState.GameOver);
    }

    /// <summary>레벨업 발생 시 ExperienceSystem이 호출.</summary>
    public void TriggerLevelUp()
    {
        if (CurrentState != GameState.Battle) return;
        TransitionTo(GameState.LevelUpSelection);
    }

    /// <summary>업그레이드 선택 완료 후 UpgradeSelectionUI가 호출.</summary>
    public void ResumeAfterUpgrade()
    {
        if (CurrentState != GameState.LevelUpSelection) return;
        TransitionTo(GameState.Battle);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ===== 내부 =====

    private void HandleEnemyDied(float expReward)
    {
        TotalKills++;
    }

    private void TransitionTo(GameState next)
    {
        CurrentState = next;

        switch (next)
        {
            case GameState.Idle:
            case GameState.Battle:
                Time.timeScale = 1f;
                break;
            case GameState.LevelUpSelection:
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }

        OnStateChanged?.Invoke(next);
    }
}
