using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

/// <summary>
/// MinimalExampleDemo를 위한 최소 UI(버튼 2개 + 텍스트)를 자동으로 배치하고 씬으로 저장하는 1회용 도구.
/// Unity 메뉴에서 "HintSystem > Generate Minimal Example Scene" 실행.
/// </summary>
public static class MinimalExampleSceneGenerator
{
    const string ScenePath = "Assets/Scripts/AI_test/Samples/MinimalExample/MinimalExampleScene.unity";

    [MenuItem("HintSystem/Generate Minimal Example Scene (1회용)")]
    public static void Generate()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // EventSystem (버튼 클릭에 필요) — 이 프로젝트는 새 Input System을 쓰므로 그에 맞는 UI 입력 모듈 사용
        var eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<InputSystemUIInputModule>();

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Fail 버튼
        Button failButton = CreateButton(canvasGO.transform, "FailButton", "실패 시뮬레이션 (failCount++)", new Vector2(0, 190));

        // Hint 버튼
        Button hintButton = CreateButton(canvasGO.transform, "HintButton", "힌트 요청", new Vector2(0, 100));

        // 결과 텍스트
        var textGO = new GameObject("ResultText");
        textGO.transform.SetParent(canvasGO.transform, false);
        var text = textGO.AddComponent<Text>();
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 24);
        text.fontSize = 24;
        text.alignment = TextAnchor.UpperLeft;
        text.color = Color.black;
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(900, 500);
        textRect.anchoredPosition = new Vector2(0, -280);

        // 데모 스크립트 GameObject + 참조 연결
        var demoGO = new GameObject("MinimalExampleDemo");
        var demo = demoGO.AddComponent<MinimalExampleDemo>();
        var so = new SerializedObject(demo);
        so.FindProperty("failButton").objectReferenceValue = failButton;
        so.FindProperty("hintButton").objectReferenceValue = hintButton;
        so.FindProperty("resultText").objectReferenceValue = text;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log($"[MinimalExampleSceneGenerator] 완료. {ScenePath} 생성됨. Play 눌러서 확인해보기.");
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos)
    {
        var buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent, false);
        var image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.85f, 0.85f, 0.85f);
        var button = buttonGO.AddComponent<Button>();

        var rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300, 55);
        rect.anchoredPosition = anchoredPos;

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        var text = textGO.AddComponent<Text>();
        text.text = label;
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 18);
        text.fontSize = 18;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return button;
    }
}