/// <summary>
/// PromptBuilder.SystemPrompt를 조립할 때 쓰이는 설정값.
/// 기본값은 기존 하드코딩과 동일(한국어, 으스스한 톤, 최대 2문장).
/// 언어는 HintLanguageProfile로 바꿔 끼움 — HintManager가 힌트 요청 때마다 IHintLanguageSource에서 받아 넣어줌.
/// </summary>
public class HintSystemConfig
{
    /// <summary>현재 응답 언어 (프롬프트 언어 이름 + 말투 규칙). null이면 한국어로 취급</summary>
    public HintLanguageProfile languageProfile = HintLanguageProfile.Korean;

    public string tone = "creepy and atmospheric";
    public int maxSentences = 2;

    /// <summary>
    /// 무전기 너머 친구 말투(한국어 반말 / 일본어 タメ口 등)로 말하게 할지. 관찰 문장·고정 문구가 전부 친구 말투라
    /// LLM 답변도 맞춰야 한 줄 안에서 말투가 안 섞임. 실제 규칙 문장은 languageProfile.CasualStyleRule.
    /// (예전 이름: casualKorean — 언어별 규칙으로 일반화하면서 이름 변경)
    /// </summary>
    public bool casualSpeech = true;

    /// <summary>프롬프트에 들어가는 언어 이름 (예: "Korean"). 예전 language 필드와 같은 값 — 읽기 전용</summary>
    public string language => Profile.PromptLanguageName;

    public HintLanguageProfile Profile => languageProfile ?? HintLanguageProfile.Korean;
}