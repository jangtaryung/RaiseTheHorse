using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas 아래에 배치. 플레이어 전차 HP를 실시간 표시합니다.
/// 인스펙터에서 ChariotStats를 할당하면 GetChariot()에서 HP를 읽습니다.
/// </summary>
public class PlayerHPUI : MonoBehaviour
{
    [Header("참조")]
    public ChariotStats playerChariotStats;

    [Header("레이아웃")]
    [SerializeField] private Vector2 barSize = new Vector2(300f, 30f);
    [SerializeField] private Vector2 anchoredPosition = new Vector2(10f, -10f);

    private Image bgImage;
    private RectTransform fillRect;
    private Text hpText;
    private Chariot chariot;

    private void Start()
    {
        CreateHPBar();
    }

    private void CreateHPBar()
    {
        var root = new GameObject("HP Bar Root", typeof(RectTransform));
        root.transform.SetParent(transform, false);
        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(0f, 1f);
        rootRect.pivot = new Vector2(0f, 1f);
        rootRect.anchoredPosition = anchoredPosition;
        rootRect.sizeDelta = barSize;

        var bgObj = new GameObject("BG", typeof(RectTransform), typeof(Image));
        bgObj.transform.SetParent(root.transform, false);
        var bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgImage = bgObj.GetComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        var fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillObj.transform.SetParent(root.transform, false);
        fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fillObj.GetComponent<Image>().color = new Color(0.9f, 0.2f, 0.2f, 1f);

        var textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(root.transform, false);
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        hpText = textObj.GetComponent<Text>();
        hpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hpText.fontSize = 18;
        hpText.alignment = TextAnchor.MiddleCenter;
        hpText.color = Color.white;
    }

    private void Update()
    {
        if (fillRect == null) return;

        if (chariot == null && playerChariotStats != null)
            chariot = playerChariotStats.GetChariot();

        if (chariot == null) return;

        float curHP = chariot.GetCurrentHP();
        float maxHP = chariot.GetMaxHP();
        float ratio = Mathf.Clamp01(curHP / maxHP);
        fillRect.anchorMax = new Vector2(ratio, 1f);

        if (hpText != null)
            hpText.text = $"{curHP:F0}/{maxHP:F0}";
    }
}
