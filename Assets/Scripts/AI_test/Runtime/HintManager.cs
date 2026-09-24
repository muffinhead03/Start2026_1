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

    // 손/인벤토리 정보는 씬마다 다른 그랩/인벤토리 시스템을 쓸 수 있으므로
    // 구체 타입(PerceiveObjectHandPivot, InventoryData)을 직접 참조하지 않고
    // IPossessionProvider 인터페이스로만 의존함.
    // 각 씬의 XxxHintBridge가 Start()에서 자기 자신을 등록함 (예: DollHintBridge).
    // 등록이 없으면(null이면) 그냥 기존 체크리스트 로직으로 안전하게 fallback됨.
    // (해석: PuzzleDataProvider와 동일한 방식 — HintManager는 씬의 구체 구현을 모르고,
    //  "질문에 답해줄 대상"이 있는지만 신경 씀)
    public IPossessionProvider PossessionProvider { get; set; }

    [Header("현재 퍼즐 ID (씬마다 변경)")]
    public string currentPuzzleId = "wine_glass_room";

    // 기본값은 기존과 동일한 PuzzleConfigData. 테스트 등에서 외부에서 교체 가능하도록 set 공개
    public IPuzzleDataProvider PuzzleDataProvider { get; set; } = new PuzzleConfigData();

    [Header("씬 컨텍스트 데이터 (선택 — 비워두면 기존 하드코딩 데이터로 자동 동작)")]
    [SerializeField] SceneContextData[] sceneContexts;

    [Header("힌트 사용 횟수")]
    [SerializeField] int maxHints = 4;

    [Header("힌트 레벨 판정 튜닝 (데모용 조절 가능 — 기본값은 기존 하드코딩 값과 동일)")]
    [SerializeField] HintTuningConfig hintTuning = new HintTuningConfig();

    [Header("타이핑 연출")]
    [SerializeField] float typingSpeed = 0.03f;
    [SerializeField] float loadingDotInterval = 0.4f;

    [Header("실패 쿨다운")]
    [SerializeField] float failCooldown = 2f;
    float lastFailTime = -999f;

    [HideInInspector]
    public PlayerState currentPlayerState;

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
            failCount            = 0,
            hintType             = "indirect",
            completedSteps       = new List<int>(),
            foundClues           = new List<string>(),
            missedClues          = new List<string>(),
            visitedRooms         = new List<string>(),
            lastActions          = new List<string>(),
            repeatedInspections  = new List<RepeatedInspection>()
        };

        hintPanel.SetActive(false);

        HintEngine.Tuning = hintTuning;
        UpdateUsageUI();

        GameData.LoadLanguage();
        PromptBuilder.Config.language = GameData.CurrentLanguage;
        Debug.Log($"[HintManager] 힌트 AI 응답 언어: {PromptBuilder.Config.language}");

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

        if (currentPlayerState.hintCount >= maxHints)
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

        // 손/인벤토리 보유 오브젝트 이름 수집 — HintEngine의 override 우선순위 판단용.
        // PossessionProvider가 등록 안 돼있어도(null이어도) 그냥 기존 체크리스트로 fallback되니 안전함.
        string handObjectName = PossessionProvider?.GetHandObjectName();
        List<string> inventoryObjectNames = PossessionProvider?.GetInventoryObjectNames() ?? new List<string>();

        Debug.Log($"[Possessed] Hand={handObjectName ?? "empty"} / Inventory=[{string.Join(", ", inventoryObjectNames)}]");

        var result = HintEngine.Calculate(currentPlayerState, config, handObjectName, inventoryObjectNames);

        Debug.Log($"[힌트 판단] {result.debugReason} / override={result.isOverride} / 레벨: {result.hintLevel} / 상태: {result.playerStatus.ToKoreanLabel()}");

        if (result.nextStep == null)
        {
            SetHintText("이미 모든 단서를 찾았어.");
            return;
        }

        // LLM을 쓸 수 없으면(로드 실패/워밍업 미완료/연결 누락) 고정 문구로 대체.
        // 힌트 레벨 판단(HintEngine)은 위에서 이미 끝났으므로 표현만 hintByLevel 문구로 바뀜.
        if (llmClient == null || !llmClient.IsAvailable)
        {
            string reason = llmClient == null ? "llmClient null" : "LLM 사용 불가";
            ShowFallbackHint(result, reason);
            currentPlayerState.hintCount++;
            UpdateUsageUI();
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
                if (this == null) return; // 생성 도중 씬이 바뀌어 HintManager가 파괴된 경우
                stopwatch.Stop();
                isRequesting = false;
                if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);

                // 생성 중 오류로 아무 글자도 못 받았으면 고정 문구로 대체
                if (!firstChunkReceived || string.IsNullOrWhiteSpace(lastReply))
                {
                    ShowFallbackHint(result, $"LLM 응답 없음 ({stopwatch.ElapsedMilliseconds}ms)");
                    return;
                }

                Debug.Log($"[힌트 결과] fallback=false / 모델: {llmClient.ModelName} / 레벨: {result.hintLevel} / 상태: {result.playerStatus.ToKoreanLabel()} / 응답시간: {stopwatch.ElapsedMilliseconds}ms / 응답: {lastReply}");
            },
            hintDirection: PromptBuilder.GetStepHint(result.nextStep, result.hintLevel)
        );

        currentPlayerState.hintCount++;
        UpdateUsageUI();
    }

    // LLM 없이 hintByLevel 고정 문구를 그대로 출력. (베타 로그 비교용으로 fallback=true 남김)
    void ShowFallbackHint(HintResult result, string reason)
    {
        if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);

        string fixedHint = PromptBuilder.GetStepHint(result.nextStep, result.hintLevel);
        string message = string.IsNullOrEmpty(fixedHint)
            ? "치지직... 지금은 응답할 수 없어."
            : "치지직... " + fixedHint;

        Debug.LogWarning($"[힌트 결과] fallback=true / 사유: {reason} / 레벨: {result.hintLevel} / 상태: {result.playerStatus.ToKoreanLabel()} / 스텝: {result.nextStep?.id} / 응답: {message}");
        SetHintText(message);
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
        int remaining = maxHints - currentPlayerState.hintCount;
        if (usageCountText != null)
            usageCountText.text = $"{remaining}/{maxHints}";
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