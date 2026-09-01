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
}