using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using LLMUnity;

public class LLMClient : MonoBehaviour
{
    [SerializeField] LLMCharacter llmCharacter;
    public LLMCharacter LlmCharacter => llmCharacter;

    async void Start()
    {
        await llmCharacter.Warmup(WarmupCompleted);
    }

    void WarmupCompleted() => Debug.Log("[LLMUnity] 모델 워밍업 완료");

    // 스트리밍: onChunk는 생성되는 동안 누적 텍스트로 여러 번 호출, onComplete는 끝나면 한 번
    public async void RequestHintStream(string systemPrompt, string userPrompt,
        Action<string> onChunk, Action onComplete, string hintDirection = null)
    {
        string combined = systemPrompt + "\n\n" + userPrompt;

        if (!string.IsNullOrEmpty(hintDirection))
            combined += "\n\n[힌트 방향: " + hintDirection + "]";

        await llmCharacter.Chat(combined, onChunk, onComplete, false);
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
        // addToHistory: false — 힌트 요청은 매번 독립적인 질문이라 대화 맥락 쌓을 필요 없음
        string reply = await llmCharacter.Chat(prompt, null, null, false);
        callback?.Invoke(reply);
    }
}