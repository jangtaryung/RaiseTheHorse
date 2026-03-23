using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 사망 시 결과 화면을 표시합니다.
/// GameStateManager.OnStateChanged 이벤트를 구독해 GameOver 전환 시 자동 활성화됩니다.
/// Canvas 하위에 이 컴포넌트가 붙은 GameObject를 배치하고, panel 필드에 루트 Panel을 연결합니다.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Text killCountText;
    [SerializeField] private Text titleText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        GameStateManager.OnStateChanged += HandleStateChanged;

        if (panel != null) panel.SetActive(false);
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnDestroy()
    {
        GameStateManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        if (state != GameState.GameOver) return;

        if (killCountText != null && GameStateManager.Instance != null)
            killCountText.text = $"처치: {GameStateManager.Instance.TotalKills}";

        if (titleText != null)
            titleText.text = "GAME OVER";

        if (panel != null) panel.SetActive(true);
    }

    private void OnRestartClicked()
    {
        GameStateManager.Instance?.RestartGame();
    }
}
