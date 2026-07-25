using System.Collections.Generic;

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

    // 씬별, 레벨별 누적 배경 정보 — 레벨이 낮으면 정답 로직 자체를 LLM에 전달하지 않음
    // 엔티티 삭제 + 4단계 기준, 0719 기획 반영. 씬이 바뀌면 여기에 추가
    static readonly Dictionary<string, string[]> SceneContextByLevel = new Dictionary<string, string[]>
    {
        { "wine_glass_room", new[]
            {
                "Scene: A room themed around wine, with wine stains and alphabet letters scattered across the floor, leading into a room with an old bookshelf.",
                // 해석: 와인 테마 방. 바닥에 와인 얼룩과 알파벳이 흩어져 있고, 오래된 책장이 있는 방으로 이어짐.

                "Look closely at the wine stains on the floor and the wine rack in this room.",
                // 해석: 바닥의 와인 얼룩과 이 방의 와인렉을 잘 살펴봐.

                "Each wine stain on the floor has a slightly different color, with two stains of each color, and an alphabet letter at the midpoint between each pair. The wine rack holds 4 numbered wine bottles — match each stain's color to the wine of the same color to get a number, and shift the alphabet letter forward by that number to find the true letter.",
                // 해석: 바닥 얼룩은 색깔이 조금씩 다르고 같은 색이 2개씩 있으며, 같은 색 얼룩 사이 중점에 알파벳이 있음. 와인렉엔 번호가 매겨진 와인 4병이 있음 — 얼룩 색과 같은 색 와인을 매치해서 숫자를 얻고, 그 숫자만큼 알파벳을 밀면 진짜 알파벳이 나옴.

                "The books on the shelf are ordered A-Z by the first letter of their titles. Pull out the 4 books matching the true letters, then arrange them in the same order as the wine bottles are arranged on the rack — this reveals a hidden key to escape."
                // 해석: 책장의 책은 제목 첫 글자 기준 A-Z 순서. 알아낸 알파벳에 해당하는 책 4권을 꺼내서 와인렉의 와인 배치 순서대로 꽂으면 → 숨겨진 열쇠가 나와서 탈출 가능.
            }
        },
        { "organ_room", new[]
            {
                "Scene: A large old mansion themed around a broken organ. Goal: repair the organ and perform the completed sheet music to reveal a hidden key.",
                // 해석: 낡은 저택, 고장난 오르간 테마. 목표: 오르간을 고치고 완성된 악보를 연주해서 열쇠를 찾는 것.

                "Phase 1: search the mansion for the organ's missing pipes. Phase 2: gather scattered pieces of torn sheet music. Phase 3: play the completed score on the organ.",
                // 해석: 1단계는 저택 곳곳에서 사라진 파이프를 찾는 것. 2단계는 흩어진 악보 조각을 모으는 것. 3단계는 완성된 악보를 오르간에서 연주하는 것.

                "Pipes 1 and 2 are in the living room and organ room. Pipes 3 and 4 are behind a locked door — its passcode can be found from a music box melody in the living room, and a painting hints at which LP to play to reveal each pipe. Sheet music pieces are found in the living room, a metronome room, and a large room unlocked via a bookshelf hint.",
                // 해석: 파이프 1,2는 거실과 오르간방에 있음. 파이프 3,4는 잠긴 문 너머에 있는데, 비밀번호는 거실의 오르골 멜로디로 알아낼 수 있고, 그림이 어떤 색 LP를 재생해야 하는지 힌트를 줌. 악보 조각들은 거실, 메트로놈 방, 책장 힌트로 열리는 큰 방에서 찾을 수 있음.

                "Match the metronome's rhythm to open a cabinet with sheet music piece 2, follow the cabinet's hint picture on the bookshelf to unlock the large room and find piece 3 in a photo frame and piece 4 in a trash can. Combine all pieces with the one on the organ, then press the keys in the order shown on the completed score to reveal the key."
                // 해석: 메트로놈 박자를 맞추면 캐비닛이 열리며 악보 조각 2가 나옴. 캐비닛 안 힌트 그림대로 책장을 조작하면 큰 방이 열리고, 그 안 사진액자에서 조각 3을, 쓰레기통에서 조각 4를 찾을 수 있음. 모든 조각을 기존 조각과 합쳐서 완성된 악보의 순서대로 건반을 누르면 열쇠가 나옴.
            }
        }
    };

    static string GetSceneContext(string puzzleId, int hintLevel)
    {
        if (!SceneContextByLevel.TryGetValue(puzzleId, out var chunks))
            return "Scene context not available.";

        int level = hintLevel < 1 ? 1 : (hintLevel > chunks.Length ? chunks.Length : hintLevel);
        return string.Join(" ", chunks, 0, level);
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
        // 해석: 한국어 방탈출 게임 힌트 안내 AI. 반드시 한국어만, 영어 단어/괄호 번역/영어 설명 전부 금지.
        // 한국어 문장 끝나면 즉시 멈춤. 문장 중간 영어 삽입 금지. 정확히 1~2문장. 정답 직접 언급 금지.
        // 플레이어가 아직 발견 못한 정보 언급 금지. 지금 취할 수 있는 행동만 제안. 항상 으스스한 톤 유지.

    public static string Build(HintResult result)
    {
        string typeEn      = result.hintType == "direct" ? "direct" : "indirect and atmospheric";
        string levelGuide  = LevelGuide.ContainsKey(result.hintLevel)    ? LevelGuide[result.hintLevel]    : "";
        string statusGuide = StatusGuide.ContainsKey(result.playerStatus) ? StatusGuide[result.playerStatus] : "";
        string sceneCtx    = GetSceneContext(result.puzzleId, result.hintLevel);
        string stepHint    = GetStepHint(result.nextStep, result.hintLevel);

        return
            $"[Scene context]\n{sceneCtx}\n\n" +
            $"[Player state]\n" +
            $"Hint level: {result.hintLevel} out of 4 ({levelGuide})\n" +
            $"Hint style: {typeEn}\n" +
            $"Player status: {statusGuide}\n" +
            $"Hint direction: {stepHint}\n\n" +
            "Korean hint (Korean language only, no English):";
    }

    public static string GetStepHint(PuzzleStep step, int hintLevel)
    {
        if (step?.hintByLevel == null || step.hintByLevel.Length == 0)
            return "";

        int idx = hintLevel < 1 ? 0 : (hintLevel > step.hintByLevel.Length ? step.hintByLevel.Length - 1 : hintLevel - 1);
        return step.hintByLevel[idx];
    }
}