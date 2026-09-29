/// <summary>
/// 이 게임(와인씬/오르간씬)의 action id → 관찰 문장 라벨 매핑. 원래 HintEngine(Core)에 하드코딩돼 있던 문구를
/// Core 밖으로 옮긴 것 — DefaultSceneContextProvider와 같은 위치/역할.
///
/// action id는 접두사로 매칭한다. 새 씬 추가 시 Patterns에 줄만 추가하면 됨.
/// (와인씬: 각 조사 스크립트가 "inspect_xxx_" + 색/글자로 AddLastAction 호출)
/// (인형씬: DollHintBridge가 스텝 완료 시 "doll_step_NN", 곰 인형 던질 때 "doll_bear_throw_{던진 횟수}")
/// (오르간씬: 손에 새 물건을 들면 "organ_hold_{종류}"도 기록)
/// (오르간씬: OrganHintBridge가 스텝 완료 시 "organ_step_NN"으로 AddLastAction 호출 —
///  두 자리 번호라 "organ_step_01"이 "organ_step_010" 같은 걸 잘못 잡을 일 없음.
///  01은 자동 보정 스텝, 17은 탈출이라 이후 힌트 요청이 없어서 제외)
/// </summary>
public class DefaultActionLabelProvider : IActionLabelProvider
{
    static readonly (string prefix, string[] labels)[] Patterns =
    {
        // ── 와인씬 ─────────────────────────────────────────
        ("inspect_wine_stain_",      new[] { "바닥 얼룩을 들여다보고 있었지", "바닥 얼룩 색깔을 살펴보고 있었네" }),
        ("inspect_alphabet_marker_", new[] { "바닥의 알파벳 표시를 확인하고 있었지", "얼룩 사이 알파벳을 들여다보고 있었네" }),
        ("inspect_wine_label_",      new[] { "와인 라벨을 확인하고 있었지", "병 라벨을 살펴보고 있었네" }),

        // ── 오르간씬 ───────────────────────────────────────
        ("organ_step_02", new[] { "거실에서 파이프를 하나 찾아냈지", "파이프 하나를 손에 넣었네" }),
        ("organ_step_03", new[] { "옆방에서 파이프를 또 찾아냈지", "파이프를 하나 더 챙겼네" }),
        ("organ_step_04", new[] { "오르골 소리를 듣고 있었지", "오르골 멜로디에 귀 기울이고 있었네" }),
        ("organ_step_05", new[] { "잠긴 방에서 파이프를 꺼내왔지", "비밀번호 방을 열어냈네" }),
        ("organ_step_06", new[] { "LP를 틀어서 파이프를 찾아냈지", "턴테이블을 돌려보고 있었네" }),
        ("organ_step_08", new[] { "오르간에 파이프를 끼워 넣었지", "오르간을 고치고 있었네" }),
        ("organ_step_09", new[] { "거실 바닥에서 종이 조각을 주웠지", "바닥에 떨어진 악보 조각을 챙겼네" }),
        ("organ_step_10", new[] { "좁은 통로로 기어 들어갔지", "숨겨진 길을 찾아냈네" }),
        ("organ_step_11", new[] { "메트로놈 박자를 맞추고 있었지", "캐비닛 안의 악보 조각을 꺼냈네" }),
        ("organ_step_12", new[] { "책장을 만지작거리고 있었지", "책장 속 책들을 움직이고 있었네" }),
        ("organ_step_13", new[] { "액자 뒤를 뒤져보고 있었지", "가족사진 액자를 살펴보고 있었네" }),
        ("organ_step_14", new[] { "책상 위를 뒤지고 있었지", "촛불 켜진 책상 위를 살펴보고 있었네" }),
        ("organ_step_15", new[] { "악보 조각을 맞추고 있었지", "찢어진 악보를 이어 붙이고 있었네" }),
        ("organ_step_16", new[] { "오르간 건반을 누르고 있었지", "오르간을 연주하고 있었네" }),

        // 오르간씬 — 손에 새 물건을 들었을 때 "organ_hold_{종류}" (OrganHintBridge가 LP_0 → "LP"처럼 번호를 떼서 넘김).
        // 구체적인 것부터 매칭하고, 목록에 없는 물건은 맨 아래 "organ_hold_" 공통 문구로.
        ("organ_hold_LP",      new[] { "LP판을 집어 들었지", "LP 한 장을 손에 들고 있었네" }),
        ("organ_hold_pipe",    new[] { "파이프를 손에 들고 있었지", "파이프를 챙겨 들고 있었네" }),
        ("organ_hold_paper",   new[] { "악보 조각을 들여다보고 있었지", "찢어진 악보를 손에 들고 있었네" }),
        ("organ_hold_picture", new[] { "그림을 들고 살펴보고 있었지", "그림을 손에 들고 있었네" }),
        ("organ_hold_",        new[] { "뭔가를 집어 들었지", "손에 뭔가를 들고 있었네" }),

        // ── 인형씬 ─────────────────────────────────────────
        // DollHintBridge가 스텝 완료 시 "doll_step_NN"으로 AddLastAction 호출.
        // (곰 인형 던지기는 횟수가 들어가는 동적 문구라 아래 GetLabels()에서 따로 처리)
        // (08은 탈출 직전이라 이후 힌트 요청이 없어서 제외)
        ("doll_step_01",    new[] { "자물쇠 번호를 맞춰 열었지", "마트료시카 순서를 알아냈네" }),
        ("doll_step_02",    new[] { "인형에 다리를 붙여줬지", "부러진 다리를 갈아 끼웠네" }),
        ("doll_step_03",    new[] { "곰 인형한테서 동전을 얻어냈지", "곰 인형이 동전을 떨어뜨렸네" }),
        ("doll_step_04",    new[] { "오락기 게임에서 이겼지", "오락기에서 팔을 얻어냈네" }),
        ("doll_step_05",    new[] { "인형에 팔을 붙여줬지", "인형 팔을 갈아 끼웠네" }),
        ("doll_step_06",    new[] { "기차 선로를 맞춰서 태엽을 얻었지", "태엽을 손에 넣었네" }),
        ("doll_step_07",    new[] { "인형에 태엽을 끼워 넣었지", "인형 등에 태엽을 달아줬네" }),
    };

    // 곰 인형 던지기 — "doll_bear_throw_3"처럼 횟수가 붙어서 들어옴
    const string BearThrowPrefix = "doll_bear_throw_";

    public string[] GetLabels(string actionId)
    {
        if (string.IsNullOrEmpty(actionId)) return null;

        if (actionId.StartsWith(BearThrowPrefix) &&
            int.TryParse(actionId.Substring(BearThrowPrefix.Length), out int throwCount))
            return BearThrowLabels(throwCount);

        foreach (var (prefix, labels) in Patterns)
            if (actionId.StartsWith(prefix))
                return labels;

        return null;
    }

    /// <summary>
    /// 곰 인형을 던진 횟수를 그대로 문장에 반영. 정답 횟수(5번)는 스포일러라 직접 말하지 않음
    /// (남은 횟수까지 알려주고 싶으면 여기서 5 - count를 문장에 넣으면 됨).
    /// </summary>
    static string[] BearThrowLabels(int count)
    {
        if (count <= 1)
            return new[] { "곰 인형을 한 번 던져봤지", "곰 인형을 한 번 집어던졌네" };

        if (count < 3)
            return new[] { $"곰 인형을 벌써 {count}번 던졌지", $"곰 인형을 {count}번째 던지고 있었네" };

        // 관찰 문장은 HintEngine에서 "{행동}, 지금은 {거리}." 형태로 이어붙여지므로 라벨 안에는 쉼표를 넣지 않음
        return new[] { $"곰 인형을 벌써 {count}번이나 던졌지", $"곰 인형을 {count}번이나 집어던졌네" };
    }
}