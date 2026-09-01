using System.Collections.Generic;

/// <summary>
/// HintEngine의 판단 결과(HintResult)를 LLM에 보낼 실제 프롬프트 문자열로 조립한다.
/// "판단은 로직, 표현은 AI" 원칙의 표현 쪽 절반 — 여기서 답을 결정하지 않고, HintEngine이 내린 결정을
/// 자연어 프롬프트 형태로 옮기기만 한다.
/// </summary>
public static class PromptBuilder
{
    static readonly Dictionary<int, string> LevelGuide = new Dictionary<int, string>
    {
        { 1, "Give a vague nudge toward the general area or the current puzzle object the player already sees (e.g. the organ, the bookshelf). Do not name, describe, or hint at the existence of any hidden item." },
        // 해석: 플레이어가 이미 보고 있는 현재 오브젝트(오르간, 책장 등)나 대략적인 구역 쪽으로만 막연하게 유도해. 숨겨진 아이템의 이름/외형/존재 자체를 언급하지 마.

        { 2, "You may mention a rough direction or distance (e.g. 'something nearby'), but do not name or describe the hidden item itself." },
        // 해석: 대략적인 방향이나 거리("근처에 뭔가 있어" 정도)는 언급해도 되지만, 숨겨진 아이템의 이름이나 생김새는 말하지 마.

        { 3, "You may clearly name and describe the hidden item — its name, appearance, and exact location." },
        // 해석: 숨겨진 아이템의 이름, 생김새, 정확한 위치를 명확히 말해도 돼.

        { 4, "Clearly name the hidden item and describe the specific action the player should take with it to solve the puzzle." },
        // 해석: 숨겨진 아이템 이름을 명확히 말하고, 그걸로 퍼즐을 풀기 위해 플레이어가 취해야 할 구체적인 행동까지 설명해.
    };

    static readonly Dictionary<string, string> StatusGuide = new Dictionary<string, string>
    {
        { "단서 미발견",    "Player hasn't found the key clue yet. Guide them to explore." },
        { "단서 미이해",    "Player found the clue but doesn't understand it. Help them interpret it." },
        { "단서 연결 실패", "Player can't connect the clues. Hint at the relationship." },
        { "반복 실패",      "Player keeps repeating the same failed attempt. Be more direct." },
        { "포기 직전",      "Player is about to give up. Give a strong hint." },
    };

    // 씬별 배경 정보는 SceneContextProvider로 위임. 기본값 없음(Core가 게임 콘텐츠를 모르게 하기 위함) —
    // 반드시 HintManager.Start() 등에서 세팅해줘야 함.
    public static ISceneContextProvider SceneContextProvider { get; set; }

    // SystemPrompt 조립에 쓰이는 언어/톤/문장수 설정 (기본값 = 기존과 동일한 한국어/으스스한 톤/2문장).
    // 외부에서 다른 언어/톤으로 교체 가능.
    public static HintSystemConfig Config { get; set; } = new HintSystemConfig();

    public static string SystemPrompt =>
        $"You are a hint guide AI in a horror escape room game. " +
        $"CRITICAL: You MUST respond in {Config.language} language ONLY. Do NOT use any other language whatsoever. " +
        $"Do NOT add a translation in parentheses after your sentence. " +
        $"Do NOT explain or repeat your answer in another language in any form. " +
        $"Once your {Config.language} sentence ends, STOP writing immediately. " +
        $"Do NOT insert words or phrases from other languages in the middle of your sentence either. " +
        $"You must write exactly 1 to {Config.maxSentences} sentences, never more. " +
        "Never reveal the answer directly. " +
        "Never mention information the player has not yet discovered. " +
        "Only suggest actions the player can currently take. " +
        $"Always maintain a {Config.tone} tone. " +
        $"Again, {Config.language} ONLY. No other language at all. No parenthetical translations.";
        // 해석: 방탈출 게임 힌트 안내 AI. 반드시 설정된 언어(Config.language)만, 다른 언어 단어/괄호 번역/다른 언어 설명 전부 금지.
        // 문장 끝나면 즉시 멈춤. 문장 중간 다른 언어 삽입 금지. 정확히 1~Config.maxSentences 문장. 정답 직접 언급 금지.
        // 플레이어가 아직 발견 못한 정보 언급 금지. 지금 취할 수 있는 행동만 제안. 항상 설정된 톤(Config.tone) 유지.

    /// <summary>
    /// HintResult 하나를 받아 LLM에 보낼 유저 프롬프트 전체(씬 컨텍스트+플레이어 상태+힌트 방향)를 조립한다.
    /// </summary>
    public static string Build(HintResult result)
    {
        string typeEn      = result.hintType == "direct" ? "direct" : "indirect and atmospheric";
        string levelGuide  = LevelGuide.ContainsKey(result.hintLevel)    ? LevelGuide[result.hintLevel]    : "";
        string statusGuide = StatusGuide.ContainsKey(result.playerStatus) ? StatusGuide[result.playerStatus] : "";
        string sceneCtx    = SceneContextProvider.GetSceneContext(result.puzzleId, result.hintLevel);
        string stepHint    = GetStepHint(result.nextStep, result.hintLevel);

        return
            $"[Scene context]\n{sceneCtx}\n\n" +
            $"[Player state]\n" +
            $"Hint level: {result.hintLevel} out of 4 ({levelGuide})\n" +
            $"Hint style: {typeEn}\n" +
            $"Player status: {statusGuide}\n" +
            $"Hint direction: {stepHint}\n\n" +
            $"{Config.language} hint ({Config.language} language only, no other language):";
    }

    /// <summary>
    /// 특정 스텝의 hintByLevel 배열에서 현재 힌트 레벨에 맞는 문구 하나를 꺼낸다.
    /// 배열 범위를 넘어가는 레벨이 들어와도 가장 가까운 유효 인덱스로 클램프한다.
    /// </summary>
    public static string GetStepHint(PuzzleStep step, int hintLevel)
    {
        if (step?.hintByLevel == null || step.hintByLevel.Length == 0)
            return "";

        int idx = hintLevel < 1 ? 0 : (hintLevel > step.hintByLevel.Length ? step.hintByLevel.Length - 1 : hintLevel - 1);
        return step.hintByLevel[idx];
    }
}