using System;

/// <summary>
/// 힌트 생성을 담당하는 LLM 클라이언트가 구현해야 하는 인터페이스.
/// HintManager는 이 인터페이스만 알고, 실제 LLM 백엔드(LLMUnity 등)를 몰라도 된다.
/// </summary>
public interface IHintLLMClient
{
    /// <summary>현재 로드된 모델 이름 (로그/디버깅용).</summary>
    string ModelName { get; }

    /// <summary>
    /// 지금 LLM으로 힌트를 생성할 수 있는지 여부.
    /// false면 HintManager는 LLM을 호출하지 않고 hintByLevel 고정 문구로 대체한다.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// 힌트를 스트리밍으로 요청한다.
    /// onChunk는 생성 중 누적 텍스트로 여러 번, onComplete는 생성이 끝나면 한 번 호출된다.
    /// 생성 중 오류가 나도 onComplete는 반드시 한 번 호출된다 (onChunk 없이 호출될 수 있음).
    /// </summary>
    void RequestHintStream(
        string systemPrompt,
        string userPrompt,
        Action<string> onChunk,
        Action onComplete,
        string hintDirection);
}