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

    // questionType별 질문 의도 + 씬별 힌트 방향
    // 씬이 바뀌면 여기에 else if 추가
    static readonly Dictionary<string, string> TypeGuide = new Dictionary<string, string>
    {
        { "puzzle", "Player is asking about the current puzzle. Focus on the next step goal and hint direction." },
        { "exit",   "Player is asking where the exit is. Hint at the exit location without revealing it directly." },
        { "entity", "Player is asking how to avoid the entity. Hint at the entity's detection mechanic and how to evade it." },
    };

    // 씬별 배경 정보 — LLM이 맥락을 알고 힌트를 생성하도록
    // 씬이 바뀌면 여기에 추가
    static string GetSceneContext(string puzzleId) => puzzleId switch
    {
        "wine_glass_room" =>
            "Scene: A dark Victorian dining room connected to a wine cellar and a secret passage. " +
            "Exit: A locked iron door at the end of the secret passage (room C). " +
            "Entity: An invisible ghost. It can only detect the player within a 90-degree forward view range of about 10 meters. " +
            "Moving behind it is safe. Hiding in a wardrobe breaks line of sight. " +
            "Puzzle: Spilled wine stains on the table have colors. " +
            "Wine rack labels match each color to an alphabet name. " +
            "Tracing two stains of the same color reveals an intersection marked with a number on the floor or wall. " +
            "Each wine's first letter shifted forward by that number gives one character. " +
            "All characters combined spell a furniture name. Inspect that furniture to find the escape item.",
        _ => "Scene context not available."
    };

    // 시스템 프롬프트 (고정 - AI 역할/말투 정의)
    public static string SystemPrompt =>
        "You are a hint guide AI in a Korean horror escape room game. " +
        "You must always respond in Korean only. " +
        "You must write exactly 1 to 2 sentences, never more. " +
        "Never reveal the answer directly. " +
        "Never mention information the player has not yet discovered. " +
        "Only suggest actions the player can currently take. " +
        "Always maintain a creepy and atmospheric tone.";

    // 유저 프롬프트 (매 요청마다 상황에 맞게 조립)
    // questionType: "puzzle" | "exit" | "entity"
    public static string Build(HintResult result, string questionType = "puzzle")
    {
        string typeEn      = result.hintType == "direct" ? "direct" : "indirect and atmospheric";
        string levelGuide  = LevelGuide.ContainsKey(result.hintLevel)     ? LevelGuide[result.hintLevel]     : "";
        string statusGuide = StatusGuide.ContainsKey(result.playerStatus)  ? StatusGuide[result.playerStatus]  : "";
        string typeGuide   = TypeGuide.ContainsKey(questionType)           ? TypeGuide[questionType]           : TypeGuide["puzzle"];
        string sceneCtx    = GetSceneContext(result.puzzleId);

        return
            $"[Scene context]\n{sceneCtx}\n\n" +
            $"[Player state]\n" +
            $"Hint level: {result.hintLevel} out of 5 ({levelGuide})\n" +
            $"Hint style: {typeEn}\n" +
            $"Player status: {statusGuide}\n" +
            $"Next step goal: {result.nextStep.goal}\n" +
            $"Hint direction: {result.nextStep.hintDirection}\n\n" +
            $"[Player question type]\n{typeGuide}\n\n" +
            "Korean hint:";
    }
}