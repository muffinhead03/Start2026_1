using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using LLMUnity;

public class LLMClient : MonoBehaviour, IHintLLMClient
{
    [SerializeField] LLMCharacter llmCharacter;
    public LLMCharacter LlmCharacter => llmCharacter;

    public string ModelName => llmCharacter != null && llmCharacter.llm != null ? llmCharacter.llm.model : "(none)";

    // LoadingSceneController가 이 이벤트들을 구독해서 워밍업이 끝나면(성공이든 실패든) 씬 전환
    public event Action OnWarmupComplete;
    public event Action OnWarmupFailed;

    public bool IsWarmedUp { get; private set; } = false;
    public bool IsFailed   { get; private set; } = false;

    // 워밍업 성공 + 실패 이력 없음일 때만 LLM 사용. 아니면 HintManager가 고정 문구로 대체
    public bool IsAvailable => IsWarmedUp && !IsFailed;

    [Header("테스트용 — 체크하면 LLM 로드 실패를 흉내냄 (에디터/Development Build에서만 동작)")]
    [SerializeField] bool simulateLLMFailure = false;

    async void Start()
    {
        Debug.Log($"[LLMClient] Start 호출됨. llmCharacter 연결됨? {(llmCharacter != null)}");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (simulateLLMFailure)
        {
            MarkFailed("simulateLLMFailure 체크됨 (테스트용 강제 실패)");
            return;
        }
#endif

        if (llmCharacter == null)
        {
            MarkFailed("llmCharacter가 연결 안 되어 있음! Warmup 진행 불가.");
            return;
        }

        Debug.Log("[LLMClient] Warmup 시작...");
        try
        {
            // LLM 로드 실패 시 LLMUnity가 Warmup 안에서 LLMUnityException을 던짐
            // (예: Windows에서 VC++ 런타임 누락 → runtime.dll 로드 실패 → "Unable to load symbol 'Has_GPU_Layers'")
            await llmCharacter.Warmup(WarmupCompleted);
        }
        catch (Exception e)
        {
            MarkFailed($"Warmup 중 예외: {e.Message}");
            return;
        }

        // 예외 없이 끝났는데 LLM이 실패 상태인 경우 방어
        if (!IsWarmedUp && !IsFailed)
        {
            if (llmCharacter.llm != null && llmCharacter.llm.failed)
                MarkFailed("Warmup은 끝났지만 LLM이 failed 상태");
            else
                MarkFailed("Warmup은 끝났지만 완료 콜백이 호출되지 않음");
            return;
        }

        Debug.Log("[LLMClient] Warmup await 완료 (WarmupCompleted 콜백 호출 이후 시점)");
    }

    void WarmupCompleted()
    {
        Debug.Log("[LLMUnity] 모델 워밍업 완료 콜백 실행됨");
        IsWarmedUp = true;
        Debug.Log($"[LLMClient] OnWarmupComplete 구독자 수: {(OnWarmupComplete?.GetInvocationList().Length ?? 0)}");
        OnWarmupComplete?.Invoke();
        Debug.Log("[LLMClient] OnWarmupComplete 이벤트 Invoke 완료");
    }

    void MarkFailed(string reason)
    {
        if (IsFailed) return;
        IsFailed = true;
        Debug.LogError($"[LLMClient] LLM 사용 불가 → 힌트는 고정 문구로 대체됨. 사유: {reason}");
        OnWarmupFailed?.Invoke();
    }

    // 스트리밍: onChunk는 생성되는 동안 누적 텍스트로 여러 번 호출, onComplete는 끝나면 한 번
    // 실패해도 onComplete는 반드시 한 번 호출 → HintManager의 isRequesting이 풀리고 고정 문구로 대체됨
    public async void RequestHintStream(string systemPrompt, string userPrompt,
        Action<string> onChunk, Action onComplete, string hintDirection = null)
    {
        if (!IsAvailable)
        {
            Debug.LogWarning("[LLMClient] LLM 사용 불가 상태에서 RequestHintStream 호출됨 → 바로 onComplete");
            onComplete?.Invoke();
            return;
        }

        string combined = systemPrompt + "\n\n" + userPrompt;

        if (!string.IsNullOrEmpty(hintDirection))
            combined += "\n\n[힌트 방향: " + hintDirection + "]";

        bool completed = false;
        try
        {
            await llmCharacter.Chat(combined, onChunk, () =>
            {
                completed = true;
                onComplete?.Invoke();
            }, false);
        }
        catch (Exception e)
        {
            Debug.LogError($"[LLMClient] 힌트 생성 중 예외: {e.Message}");
        }

        // 예외 등으로 완료 콜백이 안 불린 경우 여기서 한 번 호출
        if (!completed)
            onComplete?.Invoke();
    }

    // HintManager가 부르는 시그니처는 OllamaClient랑 똑같이 유지 + hintDirection 추가
    public IEnumerator RequestHint(string systemPrompt, string userPrompt, Action<string> onComplete, string hintDirection = null)
    {
        bool done = false;
        string result = null;

        // PromptBuilder가 매 요청마다 다르게 만들기 때문에
        // LLMCharacter의 고정 Prompt 대신 매번 시스템+유저를 합쳐서 보낸다
        string combined = systemPrompt + "\n\n" + userPrompt;

        // 힌트 방향성이 있으면 프롬프트에 명시적으로 덧붙임
        if (!string.IsNullOrEmpty(hintDirection))
            combined += "\n\n[힌트 방향: " + hintDirection + "]";

        _ = SendChat(combined, (reply) =>
        {
            result = reply;
            done = true;
        });

        yield return new WaitUntil(() => done);
        onComplete?.Invoke(result);
    }

    async Task SendChat(string prompt, Action<string> callback)
    {
        string reply = null;
        try
        {
            if (IsAvailable)
            {
                // addToHistory: false — 힌트 요청은 매번 독립적인 질문이라 대화 맥락 쌓을 필요 없음
                reply = await llmCharacter.Chat(prompt, null, null, false);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[LLMClient] SendChat 예외: {e.Message}");
        }
        callback?.Invoke(reply);
    }
}