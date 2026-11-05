/// <summary>
/// PromptBuilder.SystemPrompt를 조립할 때 쓰이는 설정값.
/// 기본값은 기존 하드코딩과 동일(한국어, 으스스한 톤, 최대 2문장).
/// 다른 언어/톤으로 바꾸고 싶으면 PromptBuilder.Config를 교체하면 됨.
/// </summary>
public class HintSystemConfig
{
    public string language = "Korean";
    public string tone = "creepy and atmospheric";
    public int maxSentences = 2;

    // 한국어일 때 반말(무전기 너머 목소리)로 말할지. 관찰 문장·고정 문구(hintByLevel/진행 문장)가 전부 반말이라
    // LLM 답변도 맞춰야 한 줄 안에서 말투가 안 섞임. 영어 등 다른 언어에선 무시됨.
    public bool casualKorean = true;
}