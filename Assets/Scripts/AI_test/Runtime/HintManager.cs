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

    // (ILocationAwareProvider는 선택 사항: 씬에 구역/좌표 추적 구현이 없으면 null로 둬도
    //  HintEngine.Calculate()가 알아서 위치/구역 관련 필드만 비우고 나머진 정상 동작함)
    public ILocationAwareProvider LocationProvider { get; set; }

    // (IStepProgressProvider도 선택 사항: 스텝 안에 "몇 번 했는지" 같은 세부 진행도가 있는 씬만 등록.
    //  null이면 progressNote만 비고 나머진 기존과 동일하게 동작 — 예: DollHintBridge가 곰 인형 던진 횟수로 판단)
    public IStepProgressProvider ProgressProvider { get; set; }

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

    // 관찰 문장("치지직... ~했지") 타이핑 연출 + LLM 요청 병렬 처리용 상태.
    // introTypingActive: 관찰 문장이 아직 타이핑되는 중인지.
    // llmTarget: 지금까지 도착한 LLM 응답(문장 수 제한 적용)을 전부 쌓아두는 버퍼 — 타자기 큐.
    //            onChunk는 화면을 직접 안 건드리고 여기만 갱신하고, RevealLLM 코루틴이 한 글자씩 꺼내서 타이핑함.
    // llmStreamDone: LLM 생성이 끝났는지 (끝났고 버퍼도 다 타이핑했으면 RevealLLM 종료).
    // llmFlowActive: 이번 요청이 LLM 경로인지 (fallback 경로면 RevealLLM을 안 띄움).
    bool introTypingActive = false;
    string llmTarget = "";
    bool llmStreamDone = false;
    bool llmFlowActive = false;

    Coroutine typingCoroutine;
    Coroutine loadingCoroutine;
    Coroutine revealCoroutine;

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
        // 관찰 문장("~하고 있었지")의 씬별 문구 — Core(HintEngine)가 게임 내용을 모르도록 여기서 주입
        HintEngine.ActionLabelProvider = new DefaultActionLabelProvider();
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
            // 힌트가 아직 생성/타이핑 중이면(패널을 닫았다 다시 연 경우) 인사말로 덮어쓰지 않고 이어서 보여줌
            if (!isRequesting && revealCoroutine == null)
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

        var result = HintEngine.Calculate(
            currentPlayerState,     // 플레이어 상태 (힌트 요청 이력, 정체 시간 등)
            config,                 // 현재 씬의 퍼즐 체크리스트
            handObjectName,         // 1순위: 손에 쥔 오브젝트 이름 (기존)
            inventoryObjectNames,   // 2순위: 인벤토리 보유 오브젝트 이름 목록 (기존)
            LocationProvider,       // 위치/구역 정보 제공자 — null이면 proximityNote/zoneNote만 안 채워짐
            ProgressProvider        // 스텝 진행도 제공자 — null이면 progressNote만 안 채워짐
        );

        Debug.Log($"[힌트 판단] {result.debugReason} / override={result.isOverride} / 레벨: {result.hintLevel} / 상태: {result.playerStatus.ToKoreanLabel()} / 관찰: {result.watchingLine ?? "(없음)"} / 진행: {result.progressNote ?? "(없음)"}");

        if (result.nextStep == null)
        {
            SetHintText("이미 모든 단서를 찾았어.");
            return;
        }

        // "플레이어를 지켜보고 있다"는 느낌을 주는 관찰 문장 — HintEngine이 이미 완성된 한국어 문장으로 만들어둔 것을
        // 힌트 본문 앞에 붙인다. LLM 사용 여부와 상관없이(fallback이든 스트리밍이든) 100% 붙어서 나가도록
        // 여기서 코드로 직접 처리함 (로컬 소형 모델이 프롬프트만으로는 이 부분을 안정적으로 반영 못 해서).
        //
        // 병렬 처리: 관찰 문장이 타이핑되는 동안 LLM 요청은 이미 먼저 쏴놓는다(순차 X).
        // 타이핑이 끝나는 시점엔 LLM이 이미 일부 응답을 만들어놨을 수도 있어서, 그 뒤에 뜨는
        // "교신 중..." 로딩 시간이 줄어들거나 아예 없어짐 — 체감 대기시간을 실제로 줄이기 위함.
        // 타이핑 중에 도착한 응답은 llmTarget 버퍼에 전부 쌓아뒀다가, 관찰 문장이 끝나면
        // RevealLLM이 같은 속도로 한 글자씩 이어서 타이핑한다 (타자기 큐).
        bool hasWatchingLine = !string.IsNullOrEmpty(result.watchingLine);
        string introText = hasWatchingLine ? "치지직... " + result.watchingLine : null;
        string displayPrefix = hasWatchingLine ? introText + " " : "";

        StopReveal();
        introTypingActive = hasWatchingLine;
        llmTarget = "";
        llmStreamDone = false;
        llmFlowActive = false;

        if (hasWatchingLine)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(PlayIntroThenReveal(introText, displayPrefix));
        }

        ContinueHintFlow(result, displayPrefix, hasWatchingLine);
    }

    // 관찰 문장(치지직... 포함)을 타이핑 효과로 보여준다. 그동안 LLM 요청은 ContinueHintFlow에서
    // 이미 병렬로 진행 중이고 응답은 llmTarget 버퍼에 계속 쌓임 — 타이핑이 끝나면 RevealLLM이
    // 버퍼에서 같은 속도로 한 글자씩 이어서 타이핑한다 (한 번에 확 뜨는 끊김 없이).
    IEnumerator PlayIntroThenReveal(string introText, string displayPrefix)
    {
        yield return StartCoroutine(TypeText(introText));
        introTypingActive = false;

        if (llmFlowActive)
            revealCoroutine = StartCoroutine(RevealLLM(displayPrefix));
    }

    // 타자기 큐: llmTarget 버퍼에 쌓인 LLM 응답을 typingSpeed로 한 글자씩 꺼내 보여준다.
    // - 버퍼가 앞서 있으면(LLM이 타이핑보다 빠름) 쌓인 걸 매끄럽게 이어서 타이핑
    // - 버퍼를 다 따라잡았는데 생성이 안 끝났으면 다음 글자가 올 때까지 대기
    // - 아직 한 글자도 안 왔으면 관찰 문장 뒤에 "교신 중..." 점 애니메이션
    IEnumerator RevealLLM(string displayPrefix)
    {
        int shown = 0;
        bool showingLoading = false;

        while (true)
        {
            string target = llmTarget ?? "";

            // 문장 수 제한으로 버퍼가 줄어든 경우(예: 스트리밍 중 "..."이 완성되며 판정이 바뀜) 화면도 맞춰줌
            if (shown > target.Length)
            {
                shown = target.Length;
                hintText.text = displayPrefix + target;
            }

            if (shown < target.Length)
            {
                if (showingLoading)
                {
                    if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
                    showingLoading = false;
                }

                shown++;
                hintText.text = displayPrefix + target.Substring(0, shown);
                yield return new WaitForSecondsRealtime(typingSpeed);
                continue;
            }

            if (llmStreamDone)
                break;

            if (shown == 0 && !showingLoading)
            {
                if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
                loadingCoroutine = StartCoroutine(LoadingDots(displayPrefix));
                showingLoading = true;
            }

            yield return null;
        }

        if (showingLoading && loadingCoroutine != null) StopCoroutine(loadingCoroutine);
        revealCoroutine = null;
    }

    void StopReveal()
    {
        if (revealCoroutine != null) StopCoroutine(revealCoroutine);
        revealCoroutine = null;
        if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
    }

    // 관찰 문장 타이핑이 끝날 때까지 기다렸다가(이미 끝났으면 즉시) fallback 문구를 이어붙인다.
    // 인트로를 다시 타이핑하지 않고 그 뒤에 바로 덧붙이기만 함 — 같은 문장을 두 번 타이핑하는 걸 방지.
    IEnumerator AppendFallbackAfterIntro(HintResult result, string displayPrefix, string reason)
    {
        while (introTypingActive)
            yield return null;

        string fixedHint = PromptBuilder.GetEffectiveHint(result); // hintByLevel 문구 (스텝 진행 상황이 있으면 그 문장)
        string appendText = string.IsNullOrEmpty(fixedHint) ? "지금은 응답할 수 없어." : fixedHint;

        hintText.text = displayPrefix + appendText;

        Debug.LogWarning($"[힌트 결과] fallback=true / 사유: {reason} / 레벨: {result.hintLevel} / 상태: {result.playerStatus.ToKoreanLabel()} / 스텝: {result.nextStep?.id} / 응답: {hintText.text}");
    }

    // 관찰 문장 타이핑과 별개로(병렬로) 실제 힌트를 가져오는 부분.
    // displayPrefix는 힌트 텍스트를 다시 세팅할 때마다(스트리밍 중간 갱신 포함) 맨 앞에 유지하기 위해 넘김.
    void ContinueHintFlow(HintResult result, string displayPrefix, bool hasWatchingLine)
    {
        // LLM을 쓸 수 없으면(로드 실패/워밍업 미완료/연결 누락) 고정 문구로 대체.
        // 힌트 레벨 판단(HintEngine)은 위에서 이미 끝났으므로 표현만 hintByLevel 문구로 바뀜.
        if (llmClient == null || !llmClient.IsAvailable)
        {
            string reason = llmClient == null ? "llmClient null" : "LLM 사용 불가";

            if (hasWatchingLine)
                StartCoroutine(AppendFallbackAfterIntro(result, displayPrefix, reason));
            else
                ShowFallbackHint(result, reason);

            currentPlayerState.hintCount++;
            UpdateUsageUI();
            return;
        }

        string systemPrompt = PromptBuilder.SystemPrompt;
        string userPrompt   = PromptBuilder.Build(result);

        // 진행 상황이 붙는 요청만 실제 프롬프트를 로그로 남김 — LLM이 진행 상황을 반영했는지 비교하기 위함 (베타 확인용)
        if (!string.IsNullOrEmpty(result.progressNote))
            Debug.Log($"[프롬프트] {userPrompt}\n[힌트 방향: {PromptBuilder.GetEffectiveHint(result)}]");

        isRequesting = true;
        firstChunkReceived = false;
        llmFlowActive = true;
        string lastReply = "";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 인트로(관찰 문장)가 없으면 바로 타자기 큐 시작. 인트로가 있으면 PlayIntroThenReveal이
        // 인트로 타이핑이 끝난 직후에 시작함 (그동안 온 응답은 llmTarget에 쌓여 있다가 이어서 타이핑됨).
        if (!hasWatchingLine)
            revealCoroutine = StartCoroutine(RevealLLM(""));

        llmClient.RequestHintStream(
            systemPrompt,
            userPrompt,
            onChunk: (partial) =>
            {
                firstChunkReceived = true;
                lastReply = partial;

                // 화면은 직접 안 건드리고 버퍼만 갱신 — RevealLLM이 같은 속도로 이어서 타이핑함.
                // 문장 수 제한(Config.maxSentences)을 매번 적용해서, 넘치는 문장은 처음부터 화면에 안 나옴.
                llmTarget = PromptBuilder.LimitSentences(partial, PromptBuilder.Config.maxSentences);
            },
            onComplete: () =>
            {
                if (this == null) return; // 생성 도중 씬이 바뀌어 HintManager가 파괴된 경우
                stopwatch.Stop();
                isRequesting = false;
                llmStreamDone = true;

                // 생성 중 오류로 아무 글자도 못 받았으면 고정 문구로 대체
                if (!firstChunkReceived || string.IsNullOrWhiteSpace(lastReply))
                {
                    StopReveal();
                    if (hasWatchingLine)
                        StartCoroutine(AppendFallbackAfterIntro(result, displayPrefix, $"LLM 응답 없음 ({stopwatch.ElapsedMilliseconds}ms)"));
                    else
                        ShowFallbackHint(result, $"LLM 응답 없음 ({stopwatch.ElapsedMilliseconds}ms)");
                    return;
                }

                string shownReply = PromptBuilder.LimitSentences(lastReply, PromptBuilder.Config.maxSentences);
                // 끝 공백/줄바꿈 차이는 잘린 걸로 치지 않음 (LLM 응답 끝에 줄바꿈이 붙어 오는 경우가 있어서)
                bool truncated = shownReply.TrimEnd().Length < lastReply.TrimEnd().Length;

                // 원문과 화면 표시가 다르면(문장 수 제한으로 잘림) 둘 다 남김 — 잘린 부분에 중요한 내용이 있었는지 베타에서 확인용
                Debug.Log($"[힌트 결과] fallback=false / 모델: {llmClient.ModelName} / 레벨: {result.hintLevel} / 상태: {result.playerStatus.ToKoreanLabel()} / 응답시간: {stopwatch.ElapsedMilliseconds}ms / 응답: {displayPrefix}{shownReply}" +
                          (truncated ? $" / 잘림=true / 원문: {lastReply}" : ""));
            },
            hintDirection: PromptBuilder.GetEffectiveHint(result) // 프롬프트 맨 끝에 붙는 방향 문구 (진행 상황이 있으면 그 문장)
        );

        currentPlayerState.hintCount++;
        UpdateUsageUI();
    }

    // LLM 없이 hintByLevel 고정 문구를 그대로 출력. (베타 로그 비교용으로 fallback=true 남김)
    void ShowFallbackHint(HintResult result, string reason)
    {
        StopReveal();

        string fixedHint = PromptBuilder.GetEffectiveHint(result); // hintByLevel 문구 (스텝 진행 상황이 있으면 그 문장)

        // 관찰 문장(result.watchingLine)을 고정 힌트 문구 앞에 붙인다 — LLM 사용 여부와 무관하게 항상 노출.
        string watchingPart = string.IsNullOrEmpty(result.watchingLine) ? "" : result.watchingLine + " ";

        string message = string.IsNullOrEmpty(fixedHint)
            ? "치지직... 지금은 응답할 수 없어."
            : "치지직... " + watchingPart + fixedHint;

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
        StopReveal(); // 이전 LLM 응답이 아직 타이핑 중이어도 새 문구로 확실히 교체
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    // displayPrefix(관찰 문장)가 있으면 그 뒤에 점만 찍고, 없으면 "치지직.. 교신 중" + 점
    IEnumerator LoadingDots(string displayPrefix = "")
    {
        string baseText = string.IsNullOrEmpty(displayPrefix) ? "치지직.. 교신 중" : displayPrefix.TrimEnd() + " ";
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