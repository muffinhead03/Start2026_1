using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("연결 필요")]
    [SerializeField] OllamaClient        ollamaClient;
    [SerializeField] GameObject          hintPanel;
    [SerializeField] TextMeshProUGUI     hintText;
    [SerializeField] TextMeshProUGUI     usageCountText;

    [Header("버튼 3개")]
    [SerializeField] Button puzzleHintButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button entityButton;

    [Header("현재 퍼즐 ID (씬마다 변경)")]
    public string currentPuzzleId = "wine_glass_room";

    [HideInInspector]
    public PlayerState currentPlayerState;

    const int MAX_HINTS = 2;
    bool isOpen = false;

    void Start()
    {
        currentPlayerState = new PlayerState
        {
            staySeconds         = 0f,           // 실제 타이머로 채움
            hintCount           = 0,
            failCount           = 0,            // TODO: 엔티티 사망 시 ++
            hintType            = "indirect",
            completedSteps      = new List<int>(),
            foundClues          = new List<string>(),   // WineRackLabel, StainIntersection에서 Add()
            missedClues         = new List<string>(),   // TODO: 씬 종료 시 미수집 단서 계산
            visitedRooms        = new List<string>(),   // TODO: 구역 진입 트리거에서 Add()
            lastActions         = new List<string>(),   // WineGlass, WineRack 등 조사 시 Add()
            repeatedInspections = new List<RepeatedInspection>()  // TODO: 반복 조사 카운트
        };

        hintPanel.SetActive(false);
        UpdateUsageUI();

        puzzleHintButton.onClick.AddListener(() => OnHintButtonClicked("puzzle"));
        exitButton.onClick.AddListener(()        => OnHintButtonClicked("exit"));
        entityButton.onClick.AddListener(()      => OnHintButtonClicked("entity"));
    }

    void Update()
    {
        // 씬 체류 시간 실시간 측정
        currentPlayerState.staySeconds += Time.deltaTime;

        if (Keyboard.current.fKey.wasPressedThisFrame)
            TogglePanel();

        if (isOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePanel();
    }

    void TogglePanel()
    {
        isOpen = !isOpen;
        hintPanel.SetActive(isOpen);

        if (isOpen)
        {
            hintText.text = "치지직... 도움이 필요해?";
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }

    // 마지막 행동 기록 (최대 5개 유지)
    public void AddLastAction(string actionName)
    {
        currentPlayerState.lastActions.Add(actionName);
        if (currentPlayerState.lastActions.Count > 5)
            currentPlayerState.lastActions.RemoveAt(0);
    }

    public void OnHintButtonClicked(string questionType)
    {
        if (currentPlayerState.hintCount >= MAX_HINTS)
        {
            hintText.text = "더 이상 도움을 받을 수 없어.";
            return;
        }

        var config = PuzzleConfigData.GetConfig(currentPuzzleId);
        if (config == null)
        {
            Debug.LogError("[HintManager] 퍼즐 설정을 찾을 수 없음: " + currentPuzzleId);
            return;
        }

        var result = HintEngine.Calculate(currentPlayerState, config);
        if (result.nextStep == null)
        {
            hintText.text = "이미 모든 단서를 찾았어.";
            return;
        }

        string systemPrompt = PromptBuilder.SystemPrompt;
        string userPrompt   = PromptBuilder.Build(result, questionType);

        hintText.text = "치지직.. 교신 중...";

        StartCoroutine(ollamaClient.RequestHint(systemPrompt, userPrompt, (hint) =>
        {
            hintText.text = hint;
            Debug.Log($"[힌트 결과] 유형: {questionType} / 레벨: {result.hintLevel} / 상태: {result.playerStatus}");
        }));

        currentPlayerState.hintCount++;
        UpdateUsageUI();
    }

    void UpdateUsageUI()
    {
        int remaining = MAX_HINTS - currentPlayerState.hintCount;
        if (usageCountText != null)
            usageCountText.text = $"{remaining}/{MAX_HINTS}";
    }

    public void OnSceneChanged(string newPuzzleId)
    {
        currentPuzzleId = newPuzzleId;
        currentPlayerState.hintCount = 0;
        UpdateUsageUI();
    }
}