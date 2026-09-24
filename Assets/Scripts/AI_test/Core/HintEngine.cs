using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 힌트 요청 시점의 플레이어 상태를 5+1가지로 분류한 값.
/// 문자열 대신 enum으로 둬서 HintEngine↔PromptBuilder 사이 매직 스트링 오타를 컴파일 타임에 잡는다.
/// </summary>
public enum HintStatus
{
    ClueNotFound,           // 단서 미발견
    ClueMisunderstood,      // 단서 미이해
    ClueConnectFailed,      // 단서 연결 실패
    RepeatedFailure,        // 반복 실패
    AboutToGiveUp,          // 포기 직전
    PossessedItemInProgress // 보유 물품 진행중 — 손/인벤토리 보유 물품 기준으로 순서를 건너뛰고 override된 경우
}

/// <summary>
/// HintEngine.Calculate()의 결과물. 어떤 레벨의 힌트를, 어떤 스텝에 대해,
/// 어떤 톤(direct/indirect)으로 보여줄지에 대한 판단 결과를 담는다.
/// LLM은 이 결과를 자연어로 옮기기만 할 뿐, 이 판단 자체에는 관여하지 않는다.
/// </summary>
public class HintResult
{
    public int hintLevel;        // 1~5
    public HintStatus playerStatus; // 상태
    public PuzzleStep nextStep;  // 다음 안내할 단계
    public string hintType;      // direct / indirect
    public string puzzleId;      // 현재 씬 퍼즐 ID — PromptBuilder 씬 컨텍스트 조회용
    public string debugReason;   // nextStep이 어떤 경로(손/인벤토리/체크리스트)로 선택됐는지 — 콘솔 로그용, 프롬프트엔 안 넣음
    public bool isOverride;      // true면 손/인벤토리 보유 물품 기준으로 순서를 건너뛰고 선택된 것 — PromptBuilder가 "이미 지나쳤다" 문구를 뺄지 판단하는 데 씀

    // 아래 3개는 ILocationAwareProvider가 등록된 씬에서만 채워짐. 등록 안 된 씬은 전부 null —
    // PromptBuilder가 null이면 그냥 그 줄을 프롬프트에서 뺀다 (판단 실패가 아니라 "정보 없음"으로 처리).
    public string proximityNote; // "player-very-close" / "player-nearby" / "player-far" 중 하나. 좌표 자체는 절대 안 담음
    public string zoneNote;      // 예: "GateArea (오래 머무는 중)" — 구역 이름 + 체류시간 판단 결과
    public string recentAction;  // state.lastActions의 가장 최근 항목 하나. 없으면 null

    // recentAction/proximityNote를 자연스러운 한국어 한 문장으로 미리 조립해둔 것.
    // LLM한테 "참고해서 녹여봐" 맡기지 않고 코드에서 직접 만들어서 100% 확실하게 노출시키기 위함
    // (로컬 소형 모델은 "선택적으로 반영" 지시를 잘 안 따름 — 그래서 로직 쪽에서 보장함).
    // HintManager가 힌트 본문 앞에 그대로 붙여서 보여줌. 둘 다 없으면 null.
    public string watchingLine;
}

/// <summary>
/// 힌트 레벨/상태/다음 스텝을 코드로 결정하는 판단 엔진.
/// "판단은 로직, 표현은 AI" 원칙의 로직 쪽 절반을 담당한다 — LLM이나 Unity 특정 기능에
/// 의존하지 않는 순수 계산 로직이라, PlayerState/PuzzleConfig만 맞춰주면 어느 프로젝트에도 그대로 재사용 가능하다.
/// </summary>
public static class HintEngine
{
    // 점수 계산 임계값 모음. 기본값 = 기존 하드코딩 값과 동일.
    // HintManager 등 외부에서 Inspector로 받은 값을 여기에 주입해서 코드 수정 없이 튜닝 가능.
    public static HintTuningConfig Tuning { get; set; } = new HintTuningConfig();

    // 근접도 판단 거리 임계값(미터). 나중에 HintTuningConfig로 옮기고 싶으면 그쪽으로 이전 가능 —
    // 지금은 별도 파일 의존 늘리지 않으려고 여기 상수로 둠.
    const float ProximityVeryCloseDistance = 2f;
    const float ProximityNearbyDistance    = 6f;

    // 구역 체류시간이 이 값(초)을 넘으면 "오래 머무는 중"으로 판단.
    const float ZoneLongStaySeconds = 25f;

    // 콘솔 로그 등에서 사람이 읽기 좋은 한국어 라벨이 필요할 때 쓰는 매핑.
    // 판단 로직 자체와는 무관 — 순수 표시용이라 Unity 의존성 없음.
    static readonly Dictionary<HintStatus, string> KoreanLabel = new Dictionary<HintStatus, string>
    {
        { HintStatus.ClueNotFound,             "단서 미발견" },
        { HintStatus.ClueMisunderstood,        "단서 미이해" },
        { HintStatus.ClueConnectFailed,        "단서 연결 실패" },
        { HintStatus.RepeatedFailure,          "반복 실패" },
        { HintStatus.AboutToGiveUp,            "포기 직전" },
        { HintStatus.PossessedItemInProgress,  "보유 물품 진행중" },
    };

    public static string ToKoreanLabel(this HintStatus status)
        => KoreanLabel.TryGetValue(status, out var label) ? label : status.ToString();

    /// <summary>
    /// 플레이어 상태와 퍼즐 설정을 기반으로 힌트 레벨(1~5), 플레이어 상태, 다음 안내 스텝을 계산한다.
    /// 5개 가중치 요소(체류·힌트 요청 이력 35%, 요청강도 25%, 정체시간 20%, 퍼즐진행 15%, 반복조사 5%)를 합산해
    /// 점수를 내고, 그 점수를 5단계로 환산한다.
    /// handObjectName/inventoryObjectNames가 주어지면, 체크리스트 순서보다 "지금 실제로 보유 중인 것"을 우선해서
    /// nextStep을 override한다 (물리적 배치상 스텝 순서 보장이 안 되는 씬 대응용 — 예: 좁은 방에 여러 상호작용 오브젝트가 동시에 노출되는 경우).
    /// locationProvider가 주어지면, nextStep이 정해진 뒤에 플레이어-오브젝트 거리 / 현재 구역 체류시간을
    /// 판단해서 proximityNote/zoneNote로 남긴다 (좌표 자체는 절대 HintResult에 담지 않음 — "가깝다/멀다"류의
    /// 판단된 결과만 전달해서 PromptBuilder/LLM이 자연스럽게 문장으로 풀게 한다).
    /// </summary>
    public static HintResult Calculate(PlayerState state, PuzzleConfig config,
        string handObjectName = null, List<string> inventoryObjectNames = null,
        ILocationAwareProvider locationProvider = null)
    {
        float score = 0f;
        score += CalcHintHistoryScore(state)  * 0.35f;
        score += CalcRequestScore(state)      * 0.25f;
        score += CalcStagnationScore(state)   * 0.20f;
        score += CalcProgressScore(state, config) * 0.15f;
        score += CalcMisunderstandScore(state) * 0.05f;

        var nextStep = GetNextStep(state, config, handObjectName, inventoryObjectNames, out string reason, out bool isOverride);

        string proximityNote = null;
        string zoneNote      = null;

        if (locationProvider != null && nextStep != null)
        {
            proximityNote = BuildProximityNote(locationProvider, nextStep);
            zoneNote      = BuildZoneNote(locationProvider);
        }

        string recentAction = (state.lastActions != null && state.lastActions.Count > 0)
            ? state.lastActions[state.lastActions.Count - 1]
            : null;

        string watchingLine = BuildWatchingLine(recentAction, proximityNote);

        return new HintResult
        {
            hintLevel     = ScoreToLevel(score),
            playerStatus  = DetermineStatus(state, config, handObjectName, inventoryObjectNames),
            nextStep      = nextStep,
            hintType      = state.hintType,
            puzzleId      = config.puzzleId,
            debugReason   = reason,
            isOverride    = isOverride,
            proximityNote = proximityNote,
            zoneNote      = zoneNote,
            recentAction  = recentAction,
            watchingLine  = watchingLine
        };
    }

    static float CalcHintHistoryScore(PlayerState s)
    {
        var t = Tuning;
        float score = 0f;

        if      (s.staySeconds > t.staySecondsHighThreshold) score += t.staySecondsHighBonus;
        else if (s.staySeconds > t.staySecondsMidThreshold)  score += t.staySecondsMidBonus;

        if      (s.hintCount >= 3)    score += 3f;
        else if (s.hintCount >= 2)    score += 1.5f;
        if      (s.failCount >= 5)    score += 2f;
        else if (s.failCount >= 3)    score += 1f;

        return Mathf.Min(score, 8f);
    }

    static float CalcRequestScore(PlayerState s)
        => s.hintType == "direct" ? 8f : 3f;

    static float CalcStagnationScore(PlayerState s)
        => Mathf.Min(s.failCount * 0.8f + (s.staySeconds / Tuning.stagnationStaySecondsDivisor), 8f);

    static float CalcProgressScore(PlayerState s, PuzzleConfig c)
    {
        float ratio = (float)s.completedSteps.Count / Mathf.Max(c.totalSteps, 1);
        return (1f - ratio) * 8f;
    }

    static float CalcMisunderstandScore(PlayerState s)
    {
        int count = 0;
        foreach (var obj in s.repeatedInspections)
            if (obj.count >= 3) count++;
        return Mathf.Min(count * 2.5f, 8f);
    }

    static int ScoreToLevel(float score)
    {
        var t = Tuning;
        if (score < t.level2Cutline) return 1;
        if (score < t.level3Cutline) return 2;
        if (score < t.level4Cutline) return 3;
        if (score < t.level5Cutline) return 4;
        return 5;
    }

    /// <summary>
    /// 힌트 톤/방향을 정하는 상태값을 판정한다.
    /// override로 선택된 스텝이면(previousStepDone 체크가 의미 없어지므로) PossessedItemInProgress로 별도 분기한다.
    /// </summary>
    static HintStatus DetermineStatus(PlayerState s, PuzzleConfig c, string handObjectName, List<string> inventoryObjectNames)
    {
        if (s.failCount >= 5) return HintStatus.RepeatedFailure;
        if (s.hintCount >= 3) return HintStatus.AboutToGiveUp;

        var nextStep = GetNextStep(s, c, handObjectName, inventoryObjectNames, out _, out bool isOverride);
        if (nextStep == null) return HintStatus.ClueConnectFailed; // 모든 스텝 완료된 경우 (힌트 요청 자체가 막히긴 함)

        if (isOverride) return HintStatus.PossessedItemInProgress; // 순서 뒤섞인 채 손/인벤토리 기준으로 뽑힌 경우 — "이전 스텝 완료 여부" 체크가 무의미해서 별도 처리

        bool previousStepDone = nextStep.id == 1 || s.completedSteps.Contains(nextStep.id - 1);
        if (!previousStepDone) return HintStatus.ClueNotFound;

        if (s.completedSteps.Count == 0) return HintStatus.ClueMisunderstood;
        return HintStatus.ClueConnectFailed;
    }

    /// <summary>
    /// 다음 안내할 스텝을 3단계 우선순위로 결정한다:
    /// 1순위 - 지금 손에 쥔 오브젝트와 연결된 미완료 스텝 (가장 확실한 "지금 이거 하는 중" 신호)
    /// 2순위 - 인벤토리에 보관 중인 오브젝트와 연결된 미완료 스텝
    /// 3순위 - 기존 체크리스트 방식(가장 낮은 id의 미완료 스텝)
    /// reason은 콘솔 로그용, isOverride는 PromptBuilder/DetermineStatus가 순서 전제를 깔지 말지 판단하는 데 씀.
    /// </summary>
    static PuzzleStep GetNextStep(PlayerState s, PuzzleConfig c,
        string handObjectName, List<string> inventoryObjectNames,
        out string reason, out bool isOverride)
    {
        // 1순위: 손
        if (!string.IsNullOrEmpty(handObjectName))
        {
            var held = c.steps.Find(st => st.relatedObjectName == handObjectName && !s.completedSteps.Contains(st.id));
            if (held != null)
            {
                reason = $"hand-override: step {held.id} ({handObjectName})";
                isOverride = true;
                return held;
            }
        }

        // 2순위: 인벤토리
        if (inventoryObjectNames != null)
        {
            foreach (var step in c.steps)
                if (!s.completedSteps.Contains(step.id) &&
                    !string.IsNullOrEmpty(step.relatedObjectName) &&
                    inventoryObjectNames.Contains(step.relatedObjectName))
                {
                    reason = $"inventory-override: step {step.id} ({step.relatedObjectName})";
                    isOverride = true;
                    return step;
                }
        }

        // 3순위: 기존 체크리스트 (가장 낮은 id의 미완료 스텝)
        foreach (var step in c.steps)
            if (!s.completedSteps.Contains(step.id))
            {
                reason = $"checklist-fallback: step {step.id}";
                isOverride = false;
                return step;
            }

        reason = "all-completed";
        isOverride = false;
        return null;
    }

    /// <summary>
    /// 플레이어-오브젝트 거리를 3단계로 판단한다. 좌표는 계산에만 쓰고 결과에는 절대 안 남김.
    /// nextStep에 relatedObjectName이 없거나(구역형 퍼즐), provider가 위치를 못 찾으면 null 반환
    /// (해석: "정보 없음"과 "멀리 있음"을 구분 — 모르면 아예 언급 안 하는 게 맞음)
    /// </summary>
    static string BuildProximityNote(ILocationAwareProvider provider, PuzzleStep nextStep)
    {
        if (string.IsNullOrEmpty(nextStep.relatedObjectName))
            return null;

        Vector3? playerPos = provider.GetPlayerPosition();
        Vector3? objectPos = provider.GetObjectPosition(nextStep.relatedObjectName);

        if (!playerPos.HasValue || !objectPos.HasValue)
            return null;

        float dist = Vector3.Distance(playerPos.Value, objectPos.Value);

        if (dist <= ProximityVeryCloseDistance) return "player-very-close";
        if (dist <= ProximityNearbyDistance)    return "player-nearby";
        return "player-far";
    }

    /// <summary>
    /// 구역 이름 + 체류시간을 판단해서 "오래 머무는 중"인지 표시한다.
    /// 구역 시스템이 없는 씬(GetCurrentZoneName()이 null 반환)은 그냥 null.
    /// </summary>
    static string BuildZoneNote(ILocationAwareProvider provider)
    {
        string zoneName = provider.GetCurrentZoneName();
        if (string.IsNullOrEmpty(zoneName))
            return null;

        float staySeconds = provider.GetCurrentZoneStaySeconds();
        return staySeconds >= ZoneLongStaySeconds
            ? $"{zoneName} (오래 머무는 중)"
            : zoneName;
    }

    // recentAction의 접두사(action id 규칙)별 자연어 라벨. 씬마다 AddLastAction()에 넘기는 문자열
    // 규칙이 다를 수 있으니, 새 씬 추가 시 여기에 패턴만 추가하면 됨. 매칭 안 되면 GenericActionLabels로 폴백.
    static readonly (string prefix, string[] labels)[] ActionLabelPatterns =
    {
        ("inspect_wine_stain_",      new[] { "바닥 얼룩을 들여다보고 있었지", "바닥 얼룩 색깔을 살펴보고 있었네" }),
        ("inspect_alphabet_marker_", new[] { "바닥의 알파벳 표시를 확인하고 있었지", "얼룩 사이 알파벳을 들여다보고 있었네" }),
        ("inspect_wine_label_",      new[] { "와인 라벨을 확인하고 있었지", "병 라벨을 살펴보고 있었네" }),
    };

    static readonly string[] GenericActionLabels =
    {
        "방금 뭔가를 조사하고 있었지",
        "조금 전에 뭔가 살펴보고 있었네",
    };

    static readonly Dictionary<string, string[]> ProximityLabels = new Dictionary<string, string[]>
    {
        { "player-very-close", new[] { "바로 근처에 있네", "가까이 와 있구나" } },
        { "player-nearby",     new[] { "근처에 있는 것 같은데", "가까운 곳에 있나 보네" } },
        { "player-far",        new[] { "좀 멀리 떨어진 곳에 있는 것 같은데", "다른 데 가 있나 보네" } },
    };

    static string PickRandom(string[] options)
        => options[Random.Range(0, options.Length)];

    static string LabelForAction(string actionId)
    {
        foreach (var (prefix, labels) in ActionLabelPatterns)
            if (actionId.StartsWith(prefix))
                return PickRandom(labels);

        return PickRandom(GenericActionLabels); // 매칭 안 되는 action id도 그냥 생략하지 않고 일반 문구로 커버
    }

    /// <summary>
    /// recentAction/proximityNote를 조합해서 자연스러운 한국어 "관찰 문장" 한 줄을 만든다.
    /// 프롬프트로 LLM에 맡기지 않고 여기서 직접 조립하는 이유: 로컬 소형 모델은 "선택 반영" 지시를
    /// 안정적으로 안 따라서, "플레이어를 지켜보고 있다"는 느낌이 매번 확실히 나오게 하려면 로직에서 보장해야 함.
    /// 둘 다 없으면 null (아무것도 안 붙임).
    /// </summary>
    static string BuildWatchingLine(string recentAction, string proximityNote)
    {
        string actionPart = !string.IsNullOrEmpty(recentAction)
            ? LabelForAction(recentAction)
            : null;

        string proximityPart = (!string.IsNullOrEmpty(proximityNote) &&
                                 ProximityLabels.TryGetValue(proximityNote, out var labels))
            ? PickRandom(labels)
            : null;

        if (actionPart != null && proximityPart != null)
            return $"{actionPart}, 지금은 {proximityPart}.";
        if (actionPart != null)
            return $"{actionPart}.";
        if (proximityPart != null)
            return $"{proximityPart}.";

        return null;
    }
}