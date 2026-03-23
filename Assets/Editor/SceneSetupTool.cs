using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메뉴: Tools > Setup Missing Scene Objects
/// 씬에 누락된 GameStateManager, ExperienceSystem, PlayerHPUI,
/// UpgradeSelectionUI, GameOverUI를 자동 생성하고 참조를 연결합니다.
/// </summary>
public static class SceneSetupTool
{
    [MenuItem("Tools/Setup Missing Scene Objects")]
    public static void SetupScene()
    {
        int created = 0;

        // ── 1. GameStateManager ──
        if (Object.FindAnyObjectByType<GameStateManager>() == null)
        {
            var go = new GameObject("GameStateManager");
            go.AddComponent<GameStateManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create GameStateManager");
            Debug.Log("[SceneSetup] GameStateManager 생성 완료");
            created++;
        }

        // ── 2. ExperienceSystem ──
        if (Object.FindAnyObjectByType<ExperienceSystem>() == null)
        {
            var go = new GameObject("ExperienceSystem");
            var exp = go.AddComponent<ExperienceSystem>();

            // ChariotStats 자동 연결 (Chariot 오브젝트에서 찾기)
            var chariotStats = FindPlayerChariotStats();
            if (chariotStats != null)
            {
                var so = new SerializedObject(exp);
                so.FindProperty("chariotStats").objectReferenceValue = chariotStats;
                so.ApplyModifiedProperties();
                Debug.Log("[SceneSetup] ExperienceSystem → chariotStats 연결 완료");
            }

            Undo.RegisterCreatedObjectUndo(go, "Create ExperienceSystem");
            Debug.Log("[SceneSetup] ExperienceSystem 생성 완료");
            created++;
        }

        // ── Canvas 찾기 ──
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[SceneSetup] Canvas가 씬에 없습니다!");
            return;
        }

        // ── 3. PlayerHPUI ──
        if (Object.FindAnyObjectByType<PlayerHPUI>() == null)
        {
            var go = new GameObject("PlayerHPUI");
            go.transform.SetParent(canvas.transform, false);
            var hpUI = go.AddComponent<PlayerHPUI>();

            var chariotStats = FindPlayerChariotStats();
            if (chariotStats != null)
                hpUI.playerChariotStats = chariotStats;

            Undo.RegisterCreatedObjectUndo(go, "Create PlayerHPUI");
            Debug.Log("[SceneSetup] PlayerHPUI 생성 완료");
            created++;
        }

        // ── 4. GameOverUI ──
        if (Object.FindAnyObjectByType<GameOverUI>() == null)
        {
            CreateGameOverUI(canvas.transform);
            created++;
        }

        // ── 5. UpgradeSelectionUI ──
        if (Object.FindAnyObjectByType<UpgradeSelectionUI>() == null)
        {
            CreateUpgradeSelectionUI(canvas.transform);
            created++;
        }

        if (created == 0)
            Debug.Log("[SceneSetup] 모든 오브젝트가 이미 존재합니다.");
        else
            Debug.Log($"[SceneSetup] 총 {created}개 오브젝트 생성 완료. 씬을 저장하세요!");
    }

    // ===== GameOverUI 자동 생성 =====
    private static void CreateGameOverUI(Transform canvasTransform)
    {
        // 루트
        var root = new GameObject("GameOverUI");
        root.transform.SetParent(canvasTransform, false);
        var gameOverUI = root.AddComponent<GameOverUI>();

        // 패널 (풀스크린 반투명 배경)
        var panel = CreatePanel(root.transform, "GameOverPanel",
            new Color(0f, 0f, 0f, 0.7f));

        // 타이틀
        var titleObj = CreateText(panel.transform, "TitleText",
            "GAME OVER", 48, new Vector2(0f, 60f));

        // 처치 수
        var killObj = CreateText(panel.transform, "KillCountText",
            "처치: 0", 28, new Vector2(0f, -10f));

        // 재시작 버튼
        var btnObj = CreateButton(panel.transform, "RestartButton",
            "재시작", new Vector2(0f, -80f), new Vector2(200f, 50f));

        // 참조 연결
        var so = new SerializedObject(gameOverUI);
        so.FindProperty("panel").objectReferenceValue = panel;
        so.FindProperty("titleText").objectReferenceValue = titleObj.GetComponent<Text>();
        so.FindProperty("killCountText").objectReferenceValue = killObj.GetComponent<Text>();
        so.FindProperty("restartButton").objectReferenceValue = btnObj.GetComponent<Button>();
        so.ApplyModifiedProperties();

        panel.SetActive(false);
        Undo.RegisterCreatedObjectUndo(root, "Create GameOverUI");
        Debug.Log("[SceneSetup] GameOverUI 생성 완료 (패널 + 버튼 포함)");
    }

    // ===== UpgradeSelectionUI 자동 생성 =====
    private static void CreateUpgradeSelectionUI(Transform canvasTransform)
    {
        var root = new GameObject("UpgradeSelectionUI");
        root.transform.SetParent(canvasTransform, false);
        var upgradeUI = root.AddComponent<UpgradeSelectionUI>();

        // 패널 (반투명 배경)
        var panel = CreatePanel(root.transform, "UpgradePanel",
            new Color(0f, 0f, 0f, 0.6f));

        // 안내 텍스트
        CreateText(panel.transform, "HeaderText",
            "업그레이드 선택", 32, new Vector2(0f, 130f));

        // 카드 3개 생성
        var buttons = new Button[3];
        var titleTexts = new Text[3];
        var descTexts = new Text[3];

        float[] xPositions = { -220f, 0f, 220f };

        for (int i = 0; i < 3; i++)
        {
            var card = CreateCard(panel.transform, $"Card_{i}",
                new Vector2(xPositions[i], -20f),
                out var titleText, out var descText);

            buttons[i] = card.GetComponent<Button>();
            titleTexts[i] = titleText;
            descTexts[i] = descText;
        }

        // 기본 업그레이드 풀 생성
        var upgradePool = CreateDefaultUpgradePool();

        // 참조 연결
        var so = new SerializedObject(upgradeUI);
        so.FindProperty("panel").objectReferenceValue = panel;

        var btnProp = so.FindProperty("cardButtons");
        btnProp.arraySize = 3;
        for (int i = 0; i < 3; i++)
            btnProp.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];

        var titleProp = so.FindProperty("cardTitleTexts");
        titleProp.arraySize = 3;
        for (int i = 0; i < 3; i++)
            titleProp.GetArrayElementAtIndex(i).objectReferenceValue = titleTexts[i];

        var descProp = so.FindProperty("cardDescTexts");
        descProp.arraySize = 3;
        for (int i = 0; i < 3; i++)
            descProp.GetArrayElementAtIndex(i).objectReferenceValue = descTexts[i];

        // upgradePool 배열 설정
        var poolProp = so.FindProperty("upgradePool");
        poolProp.arraySize = upgradePool.Length;
        for (int i = 0; i < upgradePool.Length; i++)
        {
            var elem = poolProp.GetArrayElementAtIndex(i);
            elem.FindPropertyRelative("displayName").stringValue = upgradePool[i].displayName;
            elem.FindPropertyRelative("description").stringValue = upgradePool[i].description;
            elem.FindPropertyRelative("upgradeType").enumValueIndex = (int)upgradePool[i].upgradeType;
            elem.FindPropertyRelative("value").floatValue = upgradePool[i].value;
        }

        // 플레이어 참조 연결
        var chariotStats = FindPlayerChariotStats();
        if (chariotStats != null)
            so.FindProperty("playerStats").objectReferenceValue = chariotStats;

        var crewCtrl = Object.FindAnyObjectByType<ChariotCrewController>();
        if (crewCtrl != null)
            so.FindProperty("crewController").objectReferenceValue = crewCtrl;

        so.ApplyModifiedProperties();

        panel.SetActive(false);
        Undo.RegisterCreatedObjectUndo(root, "Create UpgradeSelectionUI");
        Debug.Log("[SceneSetup] UpgradeSelectionUI 생성 완료 (카드 3개 + 업그레이드 풀 포함)");
    }

    // ===== 기본 업그레이드 풀 =====
    private static UpgradeOption[] CreateDefaultUpgradePool()
    {
        return new[]
        {
            new UpgradeOption
            {
                displayName = "말 속도 강화",
                description = "말의 기본 이동속도 +1",
                upgradeType = UpgradeType.HorseSpeed,
                value = 1f
            },
            new UpgradeOption
            {
                displayName = "말 견인력 강화",
                description = "말의 견인력 +50",
                upgradeType = UpgradeType.HorsePull,
                value = 50f
            },
            new UpgradeOption
            {
                displayName = "장갑 방어력 강화",
                description = "갑옷 방어력 +5",
                upgradeType = UpgradeType.ArmorDefense,
                value = 5f
            },
            new UpgradeOption
            {
                displayName = "장갑 내구도 강화",
                description = "전차 최대 HP +50",
                upgradeType = UpgradeType.ArmorDurability,
                value = 50f
            },
            new UpgradeOption
            {
                displayName = "바퀴 가속 강화",
                description = "바퀴 가속도 +1",
                upgradeType = UpgradeType.WheelAccel,
                value = 1f
            },
            new UpgradeOption
            {
                displayName = "충돌 데미지 강화",
                description = "바퀴 충돌 데미지 +10",
                upgradeType = UpgradeType.WheelCollision,
                value = 10f
            },
            new UpgradeOption
            {
                displayName = "궁병 공격력 강화",
                description = "궁병 기본 공격력 +3",
                upgradeType = UpgradeType.ArcherAttack,
                value = 3f
            },
            new UpgradeOption
            {
                displayName = "창병 공격력 강화",
                description = "창병 기본 공격력 +3",
                upgradeType = UpgradeType.LancerAttack,
                value = 3f
            },
            new UpgradeOption
            {
                displayName = "검병 공격력 강화",
                description = "검병 기본 공격력 +3",
                upgradeType = UpgradeType.SwordsmanAttack,
                value = 3f
            },
            new UpgradeOption
            {
                displayName = "마부 숙련 강화",
                description = "마부 조종 능력 +2",
                upgradeType = UpgradeType.CoachmanAttack,
                value = 2f
            },
        };
    }

    // ===== UI 헬퍼 =====

    private static ChariotStats FindPlayerChariotStats()
    {
        // "Chariot" 이름의 오브젝트에서 ChariotStats 찾기
        var allStats = Object.FindObjectsByType<ChariotStats>(FindObjectsSortMode.None);
        foreach (var s in allStats)
        {
            if (s.gameObject.name == "Chariot")
                return s;
        }
        return allStats.Length > 0 ? allStats[0] : null;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color bgColor)
    {
        var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        panel.GetComponent<Image>().color = bgColor;
        return panel;
    }

    private static GameObject CreateText(Transform parent, string name,
        string content, int fontSize, Vector2 anchoredPos)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(400f, 60f);

        var text = obj.GetComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return obj;
    }

    private static GameObject CreateButton(Transform parent, string name,
        string label, Vector2 anchoredPos, Vector2 size)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        obj.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.3f, 1f);

        // 버튼 텍스트
        var textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(obj.transform, false);
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var text = textObj.GetComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 22;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return obj;
    }

    private static GameObject CreateCard(Transform parent, string name,
        Vector2 anchoredPos, out Text titleText, out Text descText)
    {
        var card = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        card.transform.SetParent(parent, false);
        var rect = card.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(190f, 220f);
        card.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.35f, 0.95f);

        // 제목
        var titleObj = CreateText(card.transform, "Title", "업그레이드", 20,
            new Vector2(0f, 70f));
        titleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(170f, 40f);
        titleText = titleObj.GetComponent<Text>();

        // 설명
        var descObj = CreateText(card.transform, "Desc", "설명", 16,
            new Vector2(0f, -10f));
        var descRect = descObj.GetComponent<RectTransform>();
        descRect.sizeDelta = new Vector2(170f, 120f);
        descText = descObj.GetComponent<Text>();
        descText.alignment = TextAnchor.UpperCenter;

        return card;
    }
}
