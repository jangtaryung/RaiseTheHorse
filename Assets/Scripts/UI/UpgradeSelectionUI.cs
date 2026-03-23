using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 레벨업 시 3개 업그레이드 선택지 카드를 팝업합니다.
/// Time.timeScale = 0 상태에서 작동하므로, 애니메이션/입력은 Time.unscaledDeltaTime 기준으로 구현합니다.
///
/// 씬 설정:
/// - panel: 선택지 팝업 루트 GameObject
/// - cardButtons[3]: 각 선택지 카드 Button
/// - cardTitleTexts[3]: 선택지 이름 Text
/// - cardDescTexts[3]: 선택지 설명 Text
/// - crewController: 플레이어 ChariotCrewController
/// - playerStats: 플레이어 ChariotStats
/// - upgradePool: 인스펙터에서 채울 UpgradeOption 목록
/// </summary>
public class UpgradeSelectionUI : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject panel;

    [Header("카드 (3개 고정)")]
    [SerializeField] private Button[] cardButtons = new Button[3];
    [SerializeField] private Text[] cardTitleTexts = new Text[3];
    [SerializeField] private Text[] cardDescTexts = new Text[3];

    [Header("게임 참조")]
    [SerializeField] private ChariotCrewController crewController;
    [SerializeField] private ChariotStats playerStats;

    [Header("업그레이드 풀")]
    [SerializeField] private UpgradeOption[] upgradePool;

    private readonly UpgradeOption[] currentOptions = new UpgradeOption[3];

    private void Awake()
    {
        GameStateManager.OnStateChanged += HandleStateChanged;

        if (panel != null) panel.SetActive(false);

        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i; // 클로저 캡처
            if (cardButtons[idx] != null)
                cardButtons[idx].onClick.AddListener(() => OnCardSelected(idx));
        }
    }

    private void OnDestroy()
    {
        GameStateManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.LevelUpSelection)
            ShowCards();
        else if (panel != null)
            panel.SetActive(false);
    }

    private void ShowCards()
    {
        PickRandomOptions();

        for (int i = 0; i < 3; i++)
        {
            var opt = currentOptions[i];
            if (opt == null) continue;

            if (cardTitleTexts[i] != null)
                cardTitleTexts[i].text = opt.displayName;
            if (cardDescTexts[i] != null)
                cardDescTexts[i].text = opt.description;
        }

        if (panel != null) panel.SetActive(true);
    }

    private void PickRandomOptions()
    {
        if (upgradePool == null || upgradePool.Length == 0) return;

        // Fisher-Yates 셔플 후 앞 3개 선택
        int[] indices = new int[upgradePool.Length];
        for (int i = 0; i < indices.Length; i++) indices[i] = i;

        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        int count = Mathf.Min(3, upgradePool.Length);
        for (int i = 0; i < 3; i++)
            currentOptions[i] = i < count ? upgradePool[indices[i]] : null;
    }

    private void OnCardSelected(int index)
    {
        var opt = currentOptions[index];
        if (opt == null) return;

        Chariot chariot = playerStats != null
            ? playerStats.GetChariot()
            : GameStateManager.Instance?.PlayerChariot;

        opt.Apply(chariot, playerStats);

        GameStateManager.Instance?.ResumeAfterUpgrade();
    }
}
