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

        { 3, "Clearly name the item and place written in the Hint direction. Use only the details written there — never add a color, material, or location of your own." },
        // 해석: 힌트 방향에 적힌 물건과 장소를 분명하게 말해. 거기 적힌 내용만 쓰고, 색깔·재질·위치를 지어내서 덧붙이지 마.
        // (예전 문구 "생김새(appearance)까지 말해도 돼"가 오히려 지어내기를 유도함 — 베타: "금장식 오르골", "붉은색 LP", "상자 아래".
        //  재요청 시 레벨이 올라가면서 레벨 3에 더 자주 도달하므로 같이 수정)

        { 4, "Clearly name the hidden item and describe the specific action the player should take with it to solve the puzzle." },
        // 해석: 숨겨진 아이템 이름을 명확히 말하고, 그걸로 퍼즐을 풀기 위해 플레이어가 취해야 할 구체적인 행동까지 설명해.

        { 5, "Clearly name the hidden item, describe exactly what to do with it, and if needed, walk through the solution step by step." },
        // 해석: 숨겨진 아이템 이름과 행동을 명확히 말하고, 필요하면 해결 과정을 단계별로 짚어줘.
    };

    static readonly Dictionary<HintStatus, string> StatusGuide = new Dictionary<HintStatus, string>
    {
        { HintStatus.ClueNotFound, "Player hasn't found the key clue yet. Guide them to explore." },
        // 해석: 플레이어가 아직 핵심 단서를 못 찾음. 탐색하도록 유도해.

        { HintStatus.ClueMisunderstood, "Player found the clue but doesn't understand it. Help them interpret it." },
        // 해석: 단서는 찾았는데 이해를 못 하고 있음. 해석하는 걸 도와줘.

        { HintStatus.ClueConnectFailed, "Player can't connect the clues. Hint at the relationship." },
        // 해석: 단서들을 서로 연결 짓지 못함. 단서들 사이의 관계를 넌지시 알려줘.

        { HintStatus.RepeatedFailure, "Player keeps repeating the same failed attempt. Be more direct." },
        // 해석: 같은 실패를 반복 중. 더 직접적으로 말해줘.

        { HintStatus.AboutToGiveUp, "Player is about to give up. Give a strong hint." },
        // 해석: 포기하기 직전. 강한 힌트를 줘.

        { HintStatus.PossessedItemInProgress, "Player already has an item related to this step in hand or inventory and is actively working with it. Do NOT assume earlier steps are finished." },
        // 해석: 플레이어가 이 스텝과 관련된 물건을 이미 손이나 인벤토리에 들고 있고 그걸로 진행 중임. 이전 스텝들이 끝났다고 가정하지 마.
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
    /// previousHint: 같은 스텝·같은 레벨에서 힌트를 다시 요청한 경우에만 직전 답변을 넘김 →
    /// "같은 뜻, 다른 표현"으로 말하게 해서 똑같은 문장이 반복되지 않게 한다. 그 외엔 null.
    /// </summary>
    public static string Build(HintResult result, string previousHint = null)
    {
        string typeEn      = result.hintType == "direct" ? "direct" : "indirect and atmospheric";
        string levelGuide  = LevelGuide.ContainsKey(result.hintLevel)    ? LevelGuide[result.hintLevel]    : "";
        string statusGuide = StatusGuide.ContainsKey(result.playerStatus) ? StatusGuide[result.playerStatus] : "";
        string sceneCtx    = SceneContextProvider.GetSceneContext(result.puzzleId);
        string stepHint    = GetEffectiveHint(result);

        // nextStep이 체크리스트 순서를 따라 나온 게 아니라 손/인벤토리 보유 물품 기준으로 override된 경우,
        // "이전 스텝을 이미 지나쳤다"는 전제를 깔면 안 됨 — 실제로는 안 지나쳤을 수 있어서 LLM이 사실과 다른 멘트를 할 위험이 있음
        string orderNote = result.isOverride
            ? "the player may not have completed earlier puzzle steps yet — do not assume any prior step is done, focus only on the item they currently have."
            // 해석: 플레이어가 이전 퍼즐 스텝을 아직 안 끝냈을 수 있음 — 이전 스텝이 끝났다고 가정하지 말고, 지금 들고 있는 물건에만 집중해.
            : "the player has already moved past earlier parts of the puzzle.";
            // 해석: 플레이어는 이미 퍼즐의 앞부분을 지나쳤음.

        // 최근 행동/근접도("플레이어를 지켜보고 있다"는 느낌)는 여기서 LLM에 맡기지 않는다.
        // HintEngine.BuildWatchingLine()이 이미 자연어 문장(result.watchingLine)으로 만들어뒀고,
        // HintManager가 힌트 본문 앞에 코드로 직접 붙인다 — 로컬 소형 모델이 "선택 반영" 지시를
        // 안정적으로 안 따르는 문제 때문에, 이 부분만큼은 판단(로직)이 표현까지 확정해서 내려보낸다.
        // 스텝 진행 상황(예: 곰 인형을 3번 던짐 → "거의 버티지 못하는 것 같아요. 조금만 더 던져보세요.")이 있으면
        // hintByLevel 문구 대신 그 문장 하나를 [Hint direction]으로 넘긴다 (GetEffectiveHint).
        // 처음엔 둘을 같이 넘겼더니 4B 모델이 한쪽을 빼먹거나(1차), 둘 다 베낀 뒤 덧붙여 4문장이 됨(2차) — 베타 로그 확인.
        // 전할 내용을 하나로 줄이는 게 소형 모델에선 가장 안정적이었음.
        bool hasProgress = !string.IsNullOrEmpty(result.progressNote);
        string baseOn = hasProgress
            ? "Base your hint ONLY on the [Hint direction] above. Never mention any numbers or counts. "
            // 해석: 힌트 방향만 근거로 삼아. 숫자나 횟수는 절대 말하지 마.
            : "Base your hint ONLY on the [Hint direction] above. ";

        return
            $"[Scene context]\n{sceneCtx}\n\n" +
            $"[Player state]\n" +
            $"Hint level: {result.hintLevel} out of 5 ({levelGuide})\n" +
            $"Hint style: {typeEn}\n" +
            $"Player status: {statusGuide}\n" +
            $"Hint direction: {stepHint}\n\n" +
            $"IMPORTANT: {baseOn}" +
            $"Do NOT invent objects, places, lights, or actions that are not in the [Hint direction]. " +
            // 해석: 힌트 방향에 없는 물건·장소·빛·행동을 지어내지 마. (베타 로그에서 "빛을 향해", "그림 뒤에 숨겨진 비밀",
            // "묘한 빛을 따라" 같은 없는 단서를 지어내서 플레이어를 엉뚱한 곳으로 보낼 위험이 반복 확인됨)
            $"Do NOT mention any other puzzle mechanic, object, or step that isn't part of it — " +
            $"{orderNote}\n\n" +
            BuildRepeatNote(previousHint) +
            $"{Config.language} hint ({Config.language} language only, no other language):";
    }

    // 같은 레벨 재요청일 때만 붙는 한 줄. 방향 문장은 그대로 하나만 두고(1·2차 실험에서 둘 이상 주면 퇴화),
    // "이 문장은 이미 들었으니 같은 뜻을 다른 말로"만 덧붙인다.
    // 예전엔 프롬프트 맨 끝(답변 시작 신호 뒤)에 붙었는데, 답변 신호 앞으로 옮김.
    static string BuildRepeatNote(string previousHint)
    {
        if (string.IsNullOrWhiteSpace(previousHint))
            return "";

        return $"[Previous hint for this step]\n{previousHint.Trim()}\n" +
               "The player already heard this. Give the same direction in different words. Do not repeat the previous sentence.\n\n";
        // 해석: 플레이어가 이미 이 힌트를 들었음. 같은 방향을 다른 말로 전해. 이전 문장을 반복하지 마.
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

    /// <summary>
    /// 실제로 전달할 힌트 문구. 스텝 진행 상황(progressNote)이 있으면 그 문장이 hintByLevel 문구를 대신한다.
    /// LLM 프롬프트의 [Hint direction], LLM 요청 맨 끝의 [힌트 방향], LLM이 없을 때의 고정 문구(폴백) 모두 이 값을 쓴다.
    /// </summary>
    public static string GetEffectiveHint(HintResult result)
    {
        return !string.IsNullOrEmpty(result.progressNote)
            ? result.progressNote
            : GetStepHint(result.nextStep, result.hintLevel);
    }

    /// <summary>
    /// LLM 응답을 앞에서부터 maxSentences 문장까지만 잘라낸다 (SystemPrompt로 문장 수를 지시해도 소형 모델이 자주 넘겨서 코드로 보장).
    /// 문장 끝 판정: '.', '?', '!' 뒤에 공백이 오거나 텍스트가 끝날 때.
    /// 말줄임표("...", "…")는 문장 끝으로 안 셈 — LLM이 "한계인 것 같아요… 조금만 더"처럼 문장 중간에 자주 써서.
    /// 문장이 maxSentences개보다 적으면 그대로 반환 (스트리밍 중간 결과에도 매번 적용해도 안전).
    /// </summary>
    public static string LimitSentences(string text, int maxSentences)
    {
        if (string.IsNullOrEmpty(text) || maxSentences <= 0)
            return text;

        int count = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c != '.' && c != '?' && c != '!')
                continue;

            // "..." 같은 연속 마침표는 말줄임표로 보고 건너뜀
            if (c == '.' && ((i > 0 && text[i - 1] == '.') || (i + 1 < text.Length && text[i + 1] == '.')))
                continue;

            bool atEnd = i + 1 >= text.Length;
            if (!atEnd && !char.IsWhiteSpace(text[i + 1]))
                continue;

            count++;
            if (count >= maxSentences)
                return text.Substring(0, i + 1);
        }

        return text;
    }
}