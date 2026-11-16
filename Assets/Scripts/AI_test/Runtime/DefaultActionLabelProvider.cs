/// <summary>
/// 이 게임(와인씬/오르간씬)의 action id → 관찰 문장 라벨 매핑. 원래 HintEngine(Core)에 하드코딩돼 있던 문구를
/// Core 밖으로 옮긴 것 — DefaultSceneContextProvider와 같은 위치/역할.
///
/// action id는 접두사로 매칭한다. 새 씬 추가 시 Patterns에 줄만 추가하면 됨.
/// 문장 자체(한·영·일)는 HintLanguageData.Actions.cs에 있음 — 여기는 "어떤 행동 → 어떤 번역 키"만 연결 (문장을 고칠 땐 이 파일 안 건드림).
/// (와인씬: WineHintBridge가 "wine_..." 행동 키로 AddLastAction — 10월 리디자인 기준.
///  관찰 문장은 모든 레벨에 붙으므로 "글자/알파벳/숫자"처럼 풀이 방법을 알려주는 단어는 넣지 않음)
/// (인형씬: DollHintBridge가 스텝 완료 시 "doll_step_NN", 곰 인형 던질 때 "doll_bear_throw_{던진 횟수}")
/// (오르간씬: 손에 새 물건을 들면 "organ_hold_{종류}"도 기록)
/// (오르간씬: OrganHintBridge가 스텝 완료 시 "organ_step_NN"으로 AddLastAction 호출 —
///  두 자리 번호라 "organ_step_01"이 "organ_step_010" 같은 걸 잘못 잡을 일 없음.
///  01은 자동 보정 스텝, 17은 탈출이라 이후 힌트 요청이 없어서 제외)
/// </summary>
public class DefaultActionLabelProvider : IActionLabelProvider
{
    static readonly (string prefix, string key)[] Patterns =
    {
        // ── 와인씬 (WineHintBridge) ─────────────────────
        ("wine_view_hint_stain", HintLanguageDataKey.HintAction_WineViewHintStain),
        ("wine_view_floor_stain", HintLanguageDataKey.HintAction_WineViewFloorStain),
        ("wine_memo_solved",     HintLanguageDataKey.HintAction_WineMemoSolved),
        ("wine_memo_partial",    HintLanguageDataKey.HintAction_WineMemoPartial),
        ("wine_memo_written",    HintLanguageDataKey.HintAction_WineMemoWritten),
        ("wine_memo_opened",     HintLanguageDataKey.HintAction_WineMemoOpened),
        ("wine_view_bookshelf",  HintLanguageDataKey.HintAction_WineViewBookshelf),
        ("wine_bottle_grabbed",  HintLanguageDataKey.HintAction_WineBottleGrabbed),
        ("wine_bottle_placed",   HintLanguageDataKey.HintAction_WineBottlePlaced),
        ("wine_bottle_removed",  HintLanguageDataKey.HintAction_WineBottleRemoved),
        ("wine_cabinet_locked",  HintLanguageDataKey.HintAction_WineCabinetLocked),
        ("wine_rack_solved",     HintLanguageDataKey.HintAction_WineRackSolved),
        ("wine_key_grabbed",     HintLanguageDataKey.HintAction_WineKeyGrabbed),

        // ── 오르간씬 ───────────────────────────────────────
        ("organ_step_02", HintLanguageDataKey.HintAction_OrganStep02),
        ("organ_step_03", HintLanguageDataKey.HintAction_OrganStep03),
        ("organ_step_04", HintLanguageDataKey.HintAction_OrganStep04),
        ("organ_step_05", HintLanguageDataKey.HintAction_OrganStep05),
        ("organ_step_06", HintLanguageDataKey.HintAction_OrganStep06),
        ("organ_step_08", HintLanguageDataKey.HintAction_OrganStep08),
        ("organ_step_09", HintLanguageDataKey.HintAction_OrganStep09),
        ("organ_step_10", HintLanguageDataKey.HintAction_OrganStep10),
        ("organ_step_11", HintLanguageDataKey.HintAction_OrganStep11),
        ("organ_step_12", HintLanguageDataKey.HintAction_OrganStep12),
        ("organ_step_13", HintLanguageDataKey.HintAction_OrganStep13),
        ("organ_step_14", HintLanguageDataKey.HintAction_OrganStep14),
        ("organ_step_15", HintLanguageDataKey.HintAction_OrganStep15),
        ("organ_step_16", HintLanguageDataKey.HintAction_OrganStep16),

        // 오르간씬 — 손에 새 물건을 들었을 때 "organ_hold_{종류}" (OrganHintBridge가 LP_0 → "LP"처럼 번호를 떼서 넘김).
        // 구체적인 것부터 매칭하고, 목록에 없는 물건은 맨 아래 "organ_hold_" 공통 문구로.
        ("organ_hold_LP",      HintLanguageDataKey.HintAction_OrganHoldLP),
        ("organ_hold_pipe",    HintLanguageDataKey.HintAction_OrganHoldPipe),
        ("organ_hold_paper",   HintLanguageDataKey.HintAction_OrganHoldPaper),
        ("organ_hold_picture", HintLanguageDataKey.HintAction_OrganHoldPicture),
        ("organ_hold_",        HintLanguageDataKey.HintAction_OrganHoldAny),

        // ── 인형씬 ─────────────────────────────────────────
                // DollHintBridge가 스텝 완료 시 "doll_step_NN"으로 AddLastAction 호출 (DollRoomData 9단계 번호).
        // (곰 인형 던지기는 횟수가 들어가는 동적 문구라 아래 GetLabels()에서 따로 처리)
        // (09는 탈출 직전이라 이후 힌트 요청이 없어서 제외)
        ("doll_step_01",    HintLanguageDataKey.HintAction_DollBookViewed),
        ("doll_step_02",    HintLanguageDataKey.HintAction_DollLockOpened),
        ("doll_step_03",    HintLanguageDataKey.HintAction_DollLegFixed),
        ("doll_step_04",    HintLanguageDataKey.HintAction_DollCoinGot),
        ("doll_step_05",    HintLanguageDataKey.HintAction_DollArmGot),
        ("doll_step_06",    HintLanguageDataKey.HintAction_DollArmFixed),
        ("doll_step_07",    HintLanguageDataKey.HintAction_DollSpringGot),
        ("doll_step_08",    HintLanguageDataKey.HintAction_DollSpringFixed),
    };

    // 곰 인형 던지기 — "doll_bear_throw_3"처럼 횟수가 붙어서 들어옴
    const string BearThrowPrefix = "doll_bear_throw_";

    public string[] GetLabels(string actionId)
    {
        if (string.IsNullOrEmpty(actionId)) return null;

        if (actionId.StartsWith(BearThrowPrefix) &&
            int.TryParse(actionId.Substring(BearThrowPrefix.Length), out int throwCount))
            return BearThrowLabels(throwCount);

        foreach (var (prefix, key) in Patterns)
            if (actionId.StartsWith(prefix))
                return HintLanguageData.GetCandidates(key); // 현재 언어 문장 후보 (없으면 한국어)

        return null;
    }

    /// <summary>
    /// 곰 인형을 던진 횟수를 그대로 문장에 반영. 정답 횟수(5번)는 스포일러라 직접 말하지 않음
    /// (남은 횟수까지 알려주고 싶으면 여기서 5 - count를 문장에 넣으면 됨).
    /// 문장 틀은 HintLanguageData.Actions.cs — {0}에 던진 횟수.
    /// </summary>
    static string[] BearThrowLabels(int count)
    {
        string key = count <= 1 ? HintLanguageDataKey.HintAction_DollBearThrowOnce
                   : count < 3  ? HintLanguageDataKey.HintAction_DollBearThrowFew
                   :              HintLanguageDataKey.HintAction_DollBearThrowMany;

        string[] templates = HintLanguageData.GetCandidates(key);
        if (templates == null) return null;

        var labels = new string[templates.Length];
        for (int i = 0; i < templates.Length; i++)
            labels[i] = string.Format(templates[i], count);
        return labels;
    }
}