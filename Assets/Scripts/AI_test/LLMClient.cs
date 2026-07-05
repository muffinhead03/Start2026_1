using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using LLMUnity;

public class LLMClient : MonoBehaviour
{
    [SerializeField] LLMCharacter llmCharacter;

    // HintManager가 부르는 시그니처는 OllamaClient랑 똑같이 유지
    public IEnumerator RequestHint(string systemPrompt, string userPrompt, Action<string> onComplete)
    {
        bool done = false;
        string result = null;

        // PromptBuilder가 매 요청마다 다르게 만들기 때문에
        // LLMCharacter의 고정 Prompt 대신 매번 시스템+유저를 합쳐서 보낸다
        string combined = systemPrompt + "\n\n" + userPrompt;

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