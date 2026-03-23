using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas 아래에 배치. 적 전차 HP바를 풀링하며 월드 위치를 추적합니다.
/// EnemyManager에서 Spawn/회수 시 Assign/Release를 호출합니다.
/// </summary>
public class EnemyHPBarManager : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private int prewarmCount = 20;
    [SerializeField] private Vector2 barSize = new Vector2(80f, 8f);
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    private Camera mainCam;
    private readonly Queue<RectTransform> pool = new Queue<RectTransform>();
    private readonly List<ActiveBar> activeBars = new List<ActiveBar>();

    private struct ActiveBar
    {
        public RectTransform rect;
        public RectTransform fillRect;
        public Transform target;
        public Chariot chariot;
    }

    private void Start()
    {
        mainCam = Camera.main;

        for (int i = 0; i < prewarmCount; i++)
            pool.Enqueue(CreateBar());
    }

    private RectTransform CreateBar()
    {
        var root = new GameObject("EnemyHPBar", typeof(RectTransform));
        root.transform.SetParent(transform, false);
        var rect = root.GetComponent<RectTransform>();
        rect.sizeDelta = barSize;

        // 배경
        var bg = new GameObject("BG", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

        // 체력바 (anchor 기반 fill)
        var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(root.transform, false);
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fill.GetComponent<Image>().color = new Color(0.9f, 0.15f, 0.15f, 1f);

        root.SetActive(false);
        return rect;
    }

    /// <summary>적 전차에 HP바를 할당합니다.</summary>
    public void Assign(EnemyChariot enemy)
    {
        if (enemy == null || enemy.GetChariot() == null) return;

        RectTransform rect = pool.Count > 0 ? pool.Dequeue() : CreateBar();
        rect.gameObject.SetActive(true);

        var fillRect = rect.GetChild(1).GetComponent<RectTransform>();
        fillRect.anchorMax = Vector2.one;

        activeBars.Add(new ActiveBar
        {
            rect = rect,
            fillRect = fillRect,
            target = enemy.transform,
            chariot = enemy.GetChariot()
        });
    }

    /// <summary>적 전차의 HP바를 풀로 반환합니다.</summary>
    public void Release(EnemyChariot enemy)
    {
        for (int i = activeBars.Count - 1; i >= 0; i--)
        {
            if (activeBars[i].target == enemy.transform)
            {
                activeBars[i].rect.gameObject.SetActive(false);
                pool.Enqueue(activeBars[i].rect);

                int last = activeBars.Count - 1;
                activeBars[i] = activeBars[last];
                activeBars.RemoveAt(last);
                return;
            }
        }
    }

    private void LateUpdate()
    {
        if (mainCam == null) return;

        for (int i = activeBars.Count - 1; i >= 0; i--)
        {
            var bar = activeBars[i];

            if (bar.target == null || !bar.target.gameObject.activeSelf)
            {
                bar.rect.gameObject.SetActive(false);
                pool.Enqueue(bar.rect);
                int last = activeBars.Count - 1;
                activeBars[i] = activeBars[last];
                activeBars.RemoveAt(last);
                continue;
            }

            // HP 비율 → anchor로 fill 너비 제어
            float ratio = Mathf.Clamp01(bar.chariot.GetCurrentHP() / bar.chariot.GetMaxHP());
            bar.fillRect.anchorMax = new Vector2(ratio, 1f);

            // 월드 → 스크린 좌표
            Vector3 screenPos = mainCam.WorldToScreenPoint(bar.target.position + worldOffset);

            if (screenPos.z < 0f)
            {
                bar.rect.gameObject.SetActive(false);
                continue;
            }

            bar.rect.gameObject.SetActive(true);
            bar.rect.position = screenPos;
        }
    }
}
