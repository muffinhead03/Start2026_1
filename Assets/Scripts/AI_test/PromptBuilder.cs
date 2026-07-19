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

    static readonly Dictionary<string, string> TypeGuide = new Dictionary<string, string>
    {
        { "puzzle", "Player is asking about the current puzzle. Focus on the next step goal and hint direction." },
        { "exit",   "Player is asking where the exit is. Hint at the exit location without revealing it directly." },
        { "entity", "Player is asking how to avoid the entity. Hint at the entity's detection mechanic and how to evade it." },
    };

    static string GetSceneContext(string puzzleId)
    {
        switch (puzzleId)
        {
            case "wine_glass_room":
                return
                    "Scene: A dark Victorian dining room connected to a wine cellar and a secret passage. " +
                    "Exit: A locked iron door at the end of the secret passage (room C). " +
                    "Entity: An invisible ghost. It can only detect the player within a 90-degree forward view range of about 10 meters. " +
                    "Moving behind it is safe. Hiding in a wardrobe breaks line of sight. " +
                    "Puzzle: Spilled wine stains on the table have colors. " +
                    "Wine rack labels match each color to an alphabet name. " +
                    "Tracing two stains of the same color reveals an intersection marked with a number on the floor or wall. " +
                    "Each wine's first letter shifted forward by that number gives one character. " +
                    "All characters combined spell a furniture name. Inspect that furniture to find the escape item.";

            case "organ_room":
                return
                    "Scene: A large old mansion with an organ room on the first floor and several locked rooms on both floors. " +
                    "The organ is broken — no sound comes out because dried blood has clogged the pipes. " +
                    "Phase 1 - Repair: Collect 4 pipes hidden around the mansion and install them in the organ. " +
                    "Pipes 1 and 2 are on the first floor (near the stairs and under the organ). " +
                    "A key on the living room floor unlocks Room 2. " +
                    "A music box on the shelf plays a melody (Sol-Do-Mi-La) that maps to the Room 3 keypad code (4-0-2-5). " +
                    "There is an LP hint clue in Room 3 that tells which LP to use. " +
                    "Pipe 3 is in Room 3. Pipe 4 appears when the green LP number 7 is inserted into the LP player in Room 3.";
                    // Phase 2~4 구현 후 위 줄 끝에 " +" 붙이고 아래 주석 해제
                    // "Phase 2 - Entity: After repair, lights go out and an invisible entity appears. " +
                    // "It detects the player by sound within 5 meters and follows a pattern: playing → patrolling → chasing → returning. " +
                    // "The LP player in Room 1 can lure it away. The Room 4 key falls in the second-floor corridor. " +
                    // "Phase 3 - Sheet Music: Find 4 torn sheet music pieces. " +
                    // "Piece 1 is in Room 4 (match the metronome rhythm). Piece 2 is on the living room floor. " +
                    // "A hidden switch on the second shelf of the first bookcase unlocks Room 5. " +
                    // "Piece 3 is among the books in Room 5. " +
                    // "Book numbers on the second shelf form the drawer combination. Piece 4 is in the locked drawer. " +
                    // "Phase 4 - Performance: Bring the completed sheet music to the organ and perform it. " +
                    // "Keys are numbered; press them in the order shown on the sheet music. " +
                    // "Exit: After the performance the entity disappears and a key appears in the living room. Use it on the box to escape.";

            default:
                return "Scene context not available.";
        }
    }

    public static string SystemPrompt =>
        "You are a hint guide AI in a Korean horror escape room game. " +
        "CRITICAL: You MUST respond in Korean language ONLY. Do NOT use any English words whatsoever. " +
        "Do NOT add an English translation in parentheses after your Korean sentence. " +
        "Do NOT explain or repeat your answer in English in any form. " +
        "Once your Korean sentence ends, STOP writing immediately. " +
        "Do NOT insert any English words or phrases in the middle of your Korean sentence either. " +
        "You must write exactly 1 to 2 sentences, never more. " +
        "Never reveal the answer directly. " +
        "Never mention information the player has not yet discovered. " +
        "Only suggest actions the player can currently take. " +
        "Always maintain a creepy and atmospheric tone. " +
        "Again, Korean ONLY. No English at all. No parenthetical translations.";

    public static string Build(HintResult result, string questionType = "puzzle")
    {
        string typeEn      = result.hintType == "direct" ? "direct" : "indirect and atmospheric";
        string levelGuide  = LevelGuide.ContainsKey(result.hintLevel)    ? LevelGuide[result.hintLevel]    : "";
        string statusGuide = StatusGuide.ContainsKey(result.playerStatus) ? StatusGuide[result.playerStatus] : "";
        string typeGuide   = TypeGuide.ContainsKey(questionType)          ? TypeGuide[questionType]          : TypeGuide["puzzle"];
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
            "Korean hint (Korean language only, no English):";
    }
}