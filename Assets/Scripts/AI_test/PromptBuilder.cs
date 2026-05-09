using System.Collections.Generic;

public static class PromptBuilder
{
    static readonly Dictionary<int, string> LevelGuide = new Dictionary<int, string>
    {
        { 1, "Give only a vague directional hint. Never mention specific locations or objects." },
        { 2, "You may mention a general area or room." },
        { 3, "You may mention a specific object by name." },
        { 4, "Clearly describe what action the player should take." },
        { 5, "Give a very strong hint, almost revealing the answer. But do NOT state the answer directly." },
    };

    static readonly Dictionary<string, string> StatusGuide = new Dictionary<string, string>
    {
        { "단서 미발견",    "Player hasn't found the key clue yet. Guide them to explore." },
        { "단서 미이해",    "Player found the clue but doesn't understand it. Help them interpret it." },
        { "단서 연결 실패", "Player can't connect the clues. Hint at the relationship." },
        { "반복 실패",      "Player keeps repeating the same failed attempt. Be more direct." },
        { "포기 직전",      "Player is about to give up. Give a strong hint." },
    };

    // 시스템 프롬프트 (고정 - AI 역할/말투 정의)
    public static string SystemPrompt =>
        "You are a hint guide AI in a Korean horror escape room game. " +
        "You must always respond in Korean only. " +
        "You must write exactly 1 to 2 sentences, never more. " +
        "Never reveal the answer directly. " +
        "Always maintain a creepy and atmospheric tone.";

    // 유저 프롬프트 (매 요청마다 상황에 맞게 조립)
    public static string Build(HintResult result)
    {
        string typeEn = result.hintType == "direct" ? "direct" : "indirect and atmospheric";

        string levelGuide  = LevelGuide.ContainsKey(result.hintLevel)    ? LevelGuide[result.hintLevel]    : "";
        string statusGuide = StatusGuide.ContainsKey(result.playerStatus) ? StatusGuide[result.playerStatus] : "";

        return $"Hint level: {result.hintLevel} out of 5 ({levelGuide})\n" +
               $"Hint style: {typeEn}\n" +
               $"Player state: {statusGuide}\n" +
               $"Guide goal: {result.nextStep.goal}\n" +
               $"Guide direction: {result.nextStep.hintDirection}\n\n" +
               "Korean hint:";
    }
}
