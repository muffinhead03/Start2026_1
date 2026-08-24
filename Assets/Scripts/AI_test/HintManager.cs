using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("Player Character")]
    [SerializeField] Player_Move player;
    [SerializeField] WalkieTalkieExamine walkieExamine;   // 무전기 3d

    [Header("연결 필요")]
    [SerializeField] LLMClient           llmClient;
    [SerializeField] GameObject          hintPanel;
    [SerializeField] TextMeshProUGUI     hintText;
    [SerializeField] TextMeshProUGUI     usageCountText;

    [Header("버튼")]
    [SerializeField] Button puzzleHintButton;   // exitButton, entityButton 제거

    [Header("현재 퍼즐 ID (씬마다 변경)")]
    public string currentPuzzleId = "wine_glass_room";

    [Header("타이핑 연출")]
    [SerializeField] float typingSpeed = 0.03f;         // 고정 멘트 글자당 간격
    [SerializeField] float loadingDotInterval = 0.4f;   // 로딩 점 애니메이션 간격

    [HideInInspector]
    public PlayerState currentPlayerState;

    const int MAX_HINTS = 4;
    bool isOpen = false;
    bool isRequesting = false;
    bool firstChunkReceived = false;

    Coroutine typingCoroutine;
    Coroutine loadingCoroutine;

    void Start()
    {
        // LLMClient는 이제 이 씬이 아니라 LoadingScene의 LLMSingleton 프리팹에 있음
        // (씬이 다르므로 Inspector로 미리 연결 불가 → 런타임에 싱글톤에서 가져옴)
        Debug.Log("[HintManager] Start 호출됨, LLMSingleton.Instance 확인 중...");

        if (LLMSingleton.Instance == null)
        {
            Debug.LogError("[HintManager] LLMSingleton.Instance가 null입니다! LoadingScene을 거쳐 진입했는지, LLM 프리팹이 DontDestroyOnLoad로 살아있는지 확인하세요.");
        }
        else
        {
            llmClient = LLMSingleton.Instance.LlmClient;
            Debug.Log($"[HintManager] llmClient 연결됨? {(llmClient != null)}");
        }

        currentPlayerState = new PlayerState
        {
            staySeconds         = 0f,
            hintCount           = 0,
            failCount           = 0,
            hintType            = "indirect",
            completedSteps      = new List<int>(),
            foundClues          = new List<string>(),
            missedClues         = new List<string>(),
            visitedRooms        = new List<string>(),
            lastActions         = new List<string>(),
            repeatedInspections = new List<RepeatedInspection>()
        };

        hintPanel.SetActive(false);
        UpdateUsageUI();

        puzzleHintButton.onClick.AddListener(OnHintButtonClicked);
    }

    void Update()
    {
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
            SetHintText("치지직... 도움이 필요해?");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
            player.SetMoveLock(true);
            walkieExamine?.StartExamine();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
            player.SetMoveLock(false);
            walkieExamine?.EndExamine();
        }
    }

    public void AddLastAction(string actionName)
    {
        currentPlayerState.lastActions.Add(actionName);
        if (currentPlayerState.lastActions.Count > 5)
            currentPlayerState.lastActions.RemoveAt(0);
    }

    public void OnHintButtonClicked()
    {
        if (isRequesting)
        {
            Debug.Log("[HintManager] 이미 힌트 요청 처리 중");
            return;
        }

        Debug.Log("[foundClues] " + string.Join(", ", currentPlayerState.foundClues));
        Debug.Log("[completedSteps] " + string.Join(", ", currentPlayerState.completedSteps));

        if (currentPlayerState.hintCount >= MAX_HINTS)
        {
            SetHintText("더 이상 도움을 받을 수 없어.");
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
            SetHintText("이미 모든 단서를 찾았어.");
            return;
        }

        if (llmClient == null)
        {
            Debug.LogError("[HintManager] llmClient가 null입니다. LLMSingleton 연결을 확인하세요.");
            SetHintText("치지직... 지금은 응답할 수 없어.");
            return;
        }

        string systemPrompt = PromptBuilder.SystemPrompt;
        string userPrompt   = PromptBuilder.Build(result);

        isRequesting = true;
        firstChunkReceived = false;
        string lastReply = "";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 고정 멘트 타이핑 정지하고 로딩 애니메이션 시작
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        loadingCoroutine = StartCoroutine(LoadingDots());

        llmClient.RequestHintStream(
            systemPrompt,
            userPrompt,
            onChunk: (partial) =>
            {
                if (!firstChunkReceived)
                {
                    firstChunkReceived = true;
                    if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
                }
                hintText.text = partial;   // 스트리밍은 직접 대입 — 타이핑 효과 걸지 않음
                lastReply = partial;
            },
            onComplete: () =>
            {
                stopwatch.Stop();
                isRequesting = false;
                if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
                Debug.Log($"[힌트 결과] 모델: {llmClient.LlmCharacter.llm.model} / 레벨: {result.hintLevel} / 상태: {result.playerStatus} / 응답시간: {stopwatch.ElapsedMilliseconds}ms / 응답: {lastReply}");   // ← lastReply 추가
            },
            hintDirection: PromptBuilder.GetStepHint(result.nextStep, result.hintLevel)
        );

        currentPlayerState.hintCount++;
        UpdateUsageUI();
    }

    // ─── 타이핑 효과 (고정 멘트 전용) ───
    IEnumerator TypeText(string message)
    {
        hintText.text = "";
        foreach (char c in message)
        {
            hintText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    void SetHintText(string message)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    // ─── 로딩 애니메이션 (스트리밍 대기 중) ───
    IEnumerator LoadingDots()
    {
        string baseText = "치지직.. 교신 중";
        int dotCount = 0;

        while (true)
        {
            hintText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4;
            yield return new WaitForSecondsRealtime(loadingDotInterval);
        }
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