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
    // LLMClient(구체 타입) 대신 IHintLLMClient(인터페이스)를 참조.
    // 항상 런타임에 LLMSingleton에서 주입되므로 Inspector 직렬화는 필요 없음 (SerializeField 제거)
    IHintLLMClient llmClient;
    [SerializeField] GameObject          hintPanel;
    [SerializeField] TextMeshProUGUI     hintText;
    [SerializeField] TextMeshProUGUI     usageCountText;

    [Header("버튼")]
    [SerializeField] Button puzzleHintButton;   // exitButton, entityButton 제거

    [Header("현재 퍼즐 ID (씬마다 변경)")]
    public string currentPuzzleId = "wine_glass_room";

    // 기본값은 기존과 동일한 PuzzleConfigData. 테스트 등에서 외부에서 교체 가능하도록 set 공개
    public IPuzzleDataProvider PuzzleDataProvider { get; set; } = new PuzzleConfigData();

    [Header("씬 컨텍스트 데이터 (선택 — 비워두면 기존 하드코딩 데이터로 자동 동작)")]
    [SerializeField] SceneContextData[] sceneContexts;

    [Header("타이핑 연출")]
    [SerializeField] float typingSpeed = 0.03f;
    [SerializeField] float loadingDotInterval = 0.4f;

    [Header("실패 쿨다운")]
    [SerializeField] float failCooldown = 2f;
    float lastFailTime = -999f;

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

        PromptBuilder.SceneContextProvider = new DefaultSceneContextProvider();
        // sceneContexts를 Inspector에 연결했을 때만 ScriptableObject 기반으로 교체.
        // 비워두면 PromptBuilder.SceneContextProvider는 기본값(DefaultSceneContextProvider) 그대로 사용됨 → 안전.
        if (sceneContexts != null && sceneContexts.Length > 0)
        {
            PromptBuilder.SceneContextProvider = new ScriptableObjectSceneContextProvider(sceneContexts);
            Debug.Log($"[HintManager] SceneContextData 에셋 {sceneContexts.Length}개 연결됨 — ScriptableObject 기반으로 동작");
        }

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

        var config = PuzzleDataProvider.GetConfig(currentPuzzleId);
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
                hintText.text = partial;
                lastReply = partial;
            },
            onComplete: () =>
            {
                stopwatch.Stop();
                isRequesting = false;
                if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
                Debug.Log($"[힌트 결과] 모델: {llmClient.ModelName} / 레벨: {result.hintLevel} / 상태: {result.playerStatus} / 응답시간: {stopwatch.ElapsedMilliseconds}ms / 응답: {lastReply}");
            },
            hintDirection: PromptBuilder.GetStepHint(result.nextStep, result.hintLevel)
        );

        currentPlayerState.hintCount++;
        UpdateUsageUI();
    }

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

    public void RegisterFail()
    {
        if (Time.time - lastFailTime < failCooldown) return;
        lastFailTime = Time.time;
        currentPlayerState.failCount++;
        Debug.Log($"[HintManager] failCount 증가 → {currentPlayerState.failCount}");
    }
}