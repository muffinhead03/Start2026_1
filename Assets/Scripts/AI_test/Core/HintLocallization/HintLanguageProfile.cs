/// <summary>
/// 힌트 AI가 한 언어로 말할 때 필요한 정보 묶음 (프롬프트에 넣을 언어 이름, 말투 규칙 등).
///
/// Core는 게임의 언어 설정(LanguageType, LanguageCurrentStatus 등)을 전혀 모름 —
/// 게임 쪽 어댑터(IHintLanguageSource 구현체)가 현재 언어에 맞는 프로필을 골라서 넘겨줌.
/// 새 언어를 추가할 때는 여기에 프로필 하나를 더 만들고, 어댑터에서 그 언어를 연결하면 됨
/// (PromptBuilder 등 기존 코드는 안 고쳐도 됨).
/// </summary>
public sealed class HintLanguageProfile
{
    /// <summary>언어 코드 (예: "ko", "en", "ja") — 이후 언어별 힌트 문장 테이블을 고를 때 키로 씀</summary>
    public string Code { get; }

    /// <summary>프롬프트에 그대로 들어가는 언어 이름 (예: "Korean") — "You MUST respond in {이름} language ONLY"</summary>
    public string PromptLanguageName { get; }

    /// <summary>
    /// 반말/タメ口처럼 무전기 너머 친구 말투를 지시하는 규칙 문장 (영어 프롬프트).
    /// 관찰 문장·고정 문구가 전부 친구 말투라 LLM 답변도 맞춰야 한 줄 안에서 말투가 안 섞임. 비우면 규칙 없음.
    /// </summary>
    public string CasualStyleRule { get; }

    /// <summary>답변 시작 신호 뒤에 붙는 짧은 말투 표시 (예: "casual 반말 only"). 비우면 안 붙음</summary>
    public string CasualStyleLabel { get; }

    public HintLanguageProfile(string code, string promptLanguageName, string casualStyleRule = "", string casualStyleLabel = "")
    {
        Code               = code;
        PromptLanguageName = promptLanguageName;
        CasualStyleRule    = casualStyleRule ?? "";
        CasualStyleLabel   = casualStyleLabel ?? "";
    }

    // 소형 모델은 "casual"만 주면 존댓말/정중체로 돌아가는 경우가 있어서 어미 예시까지 박아둠.

    public static readonly HintLanguageProfile Korean = new HintLanguageProfile(
        "ko", "Korean",
        "Speak in casual Korean (반말), like a friend talking over a walkie-talkie. " +
        "End sentences with casual endings such as -아, -어, -야, -지, -봐. " +
        "NEVER use polite endings such as -요, -세요, -습니다, -니다.",
        // 해석: 무전기 너머 친구처럼 반말로. 어미는 -아/-어/-야/-지/-봐, -요/-세요/-습니다/-니다는 절대 금지.
        "casual 반말 only");

    public static readonly HintLanguageProfile English = new HintLanguageProfile(
        "en", "English",
        "Speak casually, like a friend talking over a walkie-talkie. Use short, plain sentences.",
        // 해석: 무전기 너머 친구처럼 편하게. 짧고 쉬운 문장으로.
        "casual tone only");

    public static readonly HintLanguageProfile Japanese = new HintLanguageProfile(
        "ja", "Japanese",
        "Speak in casual Japanese (タメ口), like a friend talking over a walkie-talkie. " +
        "End sentences with plain casual forms such as -だ, -よ, -ね, -て, -な. " +
        "NEVER use polite forms such as -です, -ます, -ください.",
        // 해석: 무전기 너머 친구처럼 タメ口(반말)로. 어미는 -だ/-よ/-ね/-て/-な, -です/-ます/-ください는 절대 금지.
        "casual タメ口 only");
}