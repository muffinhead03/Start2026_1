using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("연결 필요")]
    [SerializeField] OllamaClient        ollamaClient;
    [SerializeField] GameObject          hintPanel;        // HintPanel 오브젝트
    [SerializeField] TextMeshProUGUI     hintText;         // HintText
    [SerializeField] TextMeshProUGUI     usageCountText;   // UsageText

    [Header("버튼 3개")]
    [SerializeField] Button puzzleHintButton;  // PuzzleHintButton
    [SerializeField] Button exitButton;        // ExitButton
    [SerializeField] Button entityButton;      // EntityButton

    [Header("현재 퍼즐 ID (씬마다 변경)")]
    public string currentPuzzleId = "wine_glass_room";

    [HideInInspector]
    public PlayerState currentPlayerState;

    const int MAX_HINTS = 2;
    bool isOpen = false;

    void Start()
    {
        // 테스트용 더미 데이터
        // TODO: 실제 게임 시스템에서 아래 값들을 채워줘야 함
        // staySeconds  → 씬 체류 타이머
        // failCount    → 사망 카운터
        // foundClues   → 단서 수집 이벤트에서 Add()
        // missedClues  → PuzzleConfig.requiredClues 미수집 항목
        // visitedRooms → 구역 진입 트리거에서 Add()
        // lastActions  → 상호작용마다 Add() (최대 5개 유지)
        currentPlayerState = new PlayerState
        {
            staySeconds  = 200f,
            hintCount    = 0,
            failCount    = 1,
            hintType     = "indirect",
            completedSteps = new List<int> { 1 },
            foundClues     = new List<string> { "clue_B" },
            missedClues    = new List<string> { "clue_C" },
            visitedRooms   = new List<string> { "room_a", "room_b" },
            lastActions    = new List<string>
                             { "inspect_wine_glass_red", "inspect_wine_rack", "inspect_wine_rack" },
            repeatedInspections = new List<RepeatedInspection>
            {
                new RepeatedInspection { objectName = "wine_rack", count = 2 }
            }
        };

        // 패널 시작 시 비활성화
        hintPanel.SetActive(false);
        UpdateUsageUI();

        // 버튼 이벤트 연결
        puzzleHintButton.onClick.AddListener(() => OnHintButtonClicked("puzzle"));
        exitButton.onClick.AddListener(()        => OnHintButtonClicked("exit"));
        entityButton.onClick.AddListener(()      => OnHintButtonClicked("entity"));
    }

    void Update()
    {
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
            Cursor.lockState = CursorLockMode.None;  // 커서 잠금 해제
            Cursor.visible   = true;                 // 커서 보이게
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // 커서 다시 잠금
            Cursor.visible   = false;
        }
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