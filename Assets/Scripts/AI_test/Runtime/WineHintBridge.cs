using UnityEngine;

/// <summary>
/// 와인씬 쪽지 퍼즐(2단계 쪽지 찾기 / 3단계 벽 종이에 붙이기)의 세부 진행도를 힌트 시스템에 연결하는 브릿지.
/// FloorStainManager / HintPaperNotesManager의 읽기 전용 개수를 매 프레임 폴링함 (DollHintBridge와 같은 패턴).
///
/// 역할 3가지:
/// 1) 진행 기록 — 진짜 쪽지를 하나라도 찾으면 step 1 자동 완료 (얼룩을 안 보고 쪽지부터 주운 경우),
///    쪽지 4장을 다 붙이면 step 3 완료 + clue_hint_paper
/// 2) 최근 행동 — 쪽지 찾음 / 붙임 / 가짜 시도를 "wine_note_..."로 AddLastAction
///    (DefaultActionLabelProvider가 "쪽지를 벌써 2장 모았지" 같은 관찰 문장으로 바꿈)
/// 3) 스텝 진행도 제공(IStepProgressProvider) — (구간 × 힌트 레벨)별 문장 풀에서 랜덤으로 골라 hintByLevel을 대신함
///    - step 2: 일부 찾음("partial") / 가짜 시도("decoy")
///    - step 3: 다 모았는데 안 붙임("ready") / 붙이는 중("attaching") / 가짜 시도("decoy")
///    가짜 시도 구간은 진짜 쪽지를 새로 찾거나 붙이면 풀림.
///    (남은 개수, 정답 알파벳은 어떤 문장에도 넣지 않음 — 정답 누설 방지)
///
/// 위치 정보는 기존 WineLocationProvider가 그대로 담당 (step 3의 "HintPaper"는 거기에 등록).
/// </summary>
public class WineHintBridge : MonoBehaviour, IStepProgressProvider
{
    [Header("힌트 시스템")]
    [SerializeField] private HintManager hintManager;

    [Header("와인씬 상태 참조 (읽기 전용, 비워두면 자동 탐색)")]
    [SerializeField] private FloorStainManager stainManager;
    [SerializeField] private HintPaperNotesManager notesManager;

    // WineGlassRoomData의 스텝 번호
    const int StepStains = 1;
    const int StepFindNotes = 2;
    const int StepAttachNotes = 3;

    int lastFound;
    int lastPlaced;
    int lastDecoyAttempts;
    bool decoyActive;     // 가짜 쪽지를 붙이려 한 뒤 아직 진짜로 진행한 게 없음
    bool step3Reported;

    void Start()
    {
        if (hintManager == null)
            hintManager = FindFirstObjectByType<HintManager>();

        if (stainManager == null)
            stainManager = FindFirstObjectByType<FloorStainManager>();

        if (notesManager == null)
            notesManager = FindFirstObjectByType<HintPaperNotesManager>();

        if (hintManager == null)
        {
            Debug.LogError("[WineHintBridge] HintManager를 찾을 수 없습니다.");
            return;
        }

        if (stainManager == null) Debug.LogWarning("[WineHintBridge] FloorStainManager를 못 찾음 — 쪽지 찾기 진행도가 빠집니다.");
        if (notesManager == null) Debug.LogWarning("[WineHintBridge] HintPaperNotesManager를 못 찾음 — 벽 종이 진행도가 빠집니다.");

        lastFound = stainManager != null ? stainManager.InspectedMidpointCount : 0;
        lastPlaced = notesManager != null ? notesManager.PlacedCount : 0;
        lastDecoyAttempts = notesManager != null ? notesManager.DecoyAttemptCount : 0;

        hintManager.ProgressProvider = this;
        Debug.Log("[WineHintBridge] HintManager.ProgressProvider로 등록됨");
    }


    // =========================================================
    // Update — 매 프레임 상태 폴링
    // =========================================================

    void Update()
    {
        if (hintManager == null) return;

        PollFoundNotes();
        PollPlacedNotes();
        PollDecoyAttempts(); // 진행 체크 뒤에 — 같은 프레임이면 가짜 시도가 마지막 행동으로 남음
    }

    void PollFoundNotes()
    {
        if (stainManager == null) return;

        int found = stainManager.InspectedMidpointCount;
        if (found <= lastFound) return;
        lastFound = found;
        decoyActive = false;

        // 얼룩을 안 보고 교차점 쪽지부터 주운 경우 — step 1의 목적(중점 쪽지 찾기)은 이미 달성한 셈이라 같이 완료
        var state = hintManager.currentPlayerState;
        if (!state.completedSteps.Contains(StepStains))
        {
            state.completedSteps.Add(StepStains);
            Debug.Log("[WineHintBridge] 진짜 쪽지 발견 → step 1 자동 완료");
        }

        hintManager.AddLastAction(found >= stainManager.TotalMidpoints
            ? "wine_note_found_all"
            : $"wine_note_found_{found}");
    }

    void PollPlacedNotes()
    {
        if (notesManager == null) return;

        int placed = notesManager.PlacedCount;
        if (placed <= lastPlaced) return;
        lastPlaced = placed;
        decoyActive = false;

        if (placed >= notesManager.TotalNotes)
        {
            hintManager.AddLastAction("wine_note_all_placed");
            ReportStep3();
        }
        else
        {
            hintManager.AddLastAction($"wine_note_placed_{placed}");
        }
    }

    void PollDecoyAttempts()
    {
        if (notesManager == null) return;

        int attempts = notesManager.DecoyAttemptCount;
        if (attempts <= lastDecoyAttempts) return;
        lastDecoyAttempts = attempts;
        decoyActive = true;

        hintManager.AddLastAction("wine_note_decoy");
    }

    void ReportStep3()
    {
        if (step3Reported) return;
        step3Reported = true;

        var state = hintManager.currentPlayerState;
        if (!state.foundClues.Contains("clue_hint_paper"))
            state.foundClues.Add("clue_hint_paper");
        if (!state.completedSteps.Contains(StepAttachNotes))
            state.completedSteps.Add(StepAttachNotes);

        Debug.Log($"[WineHintBridge] step 3 완료 (벽 종이에 쪽지 전부 부착) → 현재: [{string.Join(", ", state.completedSteps)}]");
    }


    // =========================================================
    // IStepProgressProvider 구현
    // =========================================================

    // [레벨 단계][후보] — 레벨 1~2(모호) / 3(분명) / 4~5(거의 정답).
    // 같은 칸 안에서 랜덤으로 골라서, 같은 상태·같은 레벨로 다시 물어도 방향 문장이 조금씩 달라짐.
    // 숫자(남은 개수)와 정답 알파벳은 넣지 않음.

    // step 2 — 진짜 쪽지를 일부 찾음
    static readonly string[][] PartialNotes =
    {
        new[] { "잘하고 있어요. 바닥 얼룩 색깔이 몇 가지였는지 떠올려보고, 남은 쪽지도 찾아보세요.",
                "좋아요, 제대로 찾고 있어요. 얼룩 쌍이 몇 개였는지 생각해보면 쪽지가 더 있을 거예요.",
                "잘 가고 있어요. 아직 쪽지를 못 찾은 얼룩 색깔이 남아 있을지도 몰라요." },
        new[] { "잘하고 있어요. 아직 중점을 확인하지 않은 얼룩 색깔이 남아 있어요. 그 색 얼룩 두 개의 가운데를 찾아보세요.",
                "좋아요. 얼룩 색깔마다 쪽지가 하나씩 있어요. 아직 안 찾은 색깔의 얼룩 쌍을 찾아보세요." },
        new[] { "얼룩 색깔마다 진짜 쪽지가 하나씩 있어요. 아직 쪽지를 못 찾은 색깔의 얼룩 두 개를 찾고, 그 정확한 중간 지점에 놓인 쪽지를 주우세요.",
                "남은 색깔의 얼룩 두 개를 찾아 그 사이 정확한 가운데를 보세요. 거기 놓인 쪽지만 진짜예요." },
    };

    // step 3 — 진짜 쪽지를 다 모았는데 아직 하나도 안 붙임
    static readonly string[][] ReadyNotes =
    {
        new[] { "쪽지를 다 모았는데 왜 망설여요? 이 쪽지들을 붙일 만한 종이가 어딘가 있을 거예요.",
                "쪽지는 다 모았어요. 이제 이걸 어디에 쓸지 찾아보세요. 또 다른 종이가 있을지도 몰라요.",
                "모은 쪽지를 들고만 있으면 소용없어요. 쪽지를 붙일 곳을 찾아보세요." },
        new[] { "벽 쪽을 살펴보세요. 쪽지를 붙일 수 있는 낡은 종이가 붙어 있어요.",
                "방 벽에 낡은 종이가 붙어 있어요. 모은 쪽지를 거기에 붙여보세요." },
        new[] { "모은 쪽지를 하나씩 손에 들고, 벽에 붙은 낡은 종이의 빈 칸에 붙이세요.",
                "벽의 낡은 종이로 가서 모은 쪽지 네 장을 하나씩 붙이면 종이에 뭔가 나타날 거예요." },
    };

    // step 3 — 일부 붙임
    static readonly string[][] AttachingNotes =
    {
        new[] { "좋아요, 종이에 쪽지가 붙기 시작했어요. 남은 쪽지도 마저 붙여보세요.",
                "잘하고 있어요. 종이가 아직 다 채워지지 않았어요." },
        new[] { "아직 종이에 빈자리가 남아 있어요. 모아둔 쪽지를 들고 가서 마저 붙이세요.",
                "남은 쪽지도 같은 종이에 붙이면 돼요. 빈 칸을 채워보세요." },
        new[] { "모아둔 나머지 쪽지를 하나씩 손에 들고 벽 종이의 빈 칸에 붙이면 종이에 뭔가 나타날 거예요.",
                "벽 종이의 빈 칸이 다 채워질 때까지 남은 쪽지를 하나씩 붙이세요." },
    };

    // step 2/3 — 가짜 쪽지를 붙이려 함
    static readonly string[][] DecoyNotes =
    {
        new[] { "그 쪽지가 아니에요. 같은 색 얼룩 사이 중점에 있던 쪽지가 필요해요.",
                "종이가 그 쪽지는 원하지 않는 것 같아요. 얼룩 중점에 놓여 있던 쪽지를 찾아보세요.",
                "그건 엉뚱한 쪽지예요. 얼룩 쌍 사이에 있던 쪽지를 떠올려보세요." },
        new[] { "바닥에 흩어진 쪽지 중엔 가짜도 섞여 있어요. 같은 색 얼룩 두 개의 정확한 가운데에 있던 쪽지만 붙일 수 있어요.",
                "아무 쪽지나 붙일 수 있는 게 아니에요. 같은 색 얼룩 쌍의 중점에 놓인 쪽지만 진짜예요." },
        new[] { "아무 데나 떨어진 쪽지는 소용없어요. 같은 색 얼룩 두 개를 찾고, 그 정확한 중간 지점에 놓인 쪽지만 주워서 붙이세요.",
                "그 쪽지는 버리고, 같은 색 얼룩 두 개 사이 정확한 가운데에 놓인 쪽지를 주워서 붙이세요." },
    };

    static string PickByLevel(string[][] table, int hintLevel)
    {
        int tier = hintLevel <= 2 ? 0 : (hintLevel == 3 ? 1 : 2);
        tier = Mathf.Clamp(tier, 0, table.Length - 1);
        var pool = table[tier];
        return pool[Random.Range(0, pool.Length)];
    }

    /// <summary>
    /// 진행 구간 이름. 같은 스텝 재요청 때 HintManager가 "지난 힌트 이후 진행이 있었는지" 보는 데 씀
    /// (구간이 바뀌었으면 잘 가고 있는 거라 레벨을 올리지 않음).
    /// </summary>
    public string GetProgressBand(PuzzleStep step)
    {
        if (step == null) return null;

        if (step.id == StepFindNotes && stainManager != null)
        {
            if (decoyActive) return "decoy";
            int found = stainManager.InspectedMidpointCount;
            if (found > 0 && found < stainManager.TotalMidpoints) return "partial";
            return null; // 아직 하나도 못 찾음 → hintByLevel 그대로
        }

        if (step.id == StepAttachNotes && notesManager != null)
        {
            if (decoyActive) return "decoy";
            return notesManager.PlacedCount == 0 ? "ready" : "attaching";
        }

        return null;
    }

    public int GetMinHintLevel(PuzzleStep step) => 1;

    /// <summary>
    /// step 2/3일 때만 현재 진행 구간 + 힌트 레벨에 맞는 문장을 돌려줌. 해당 없으면 null → hintByLevel 문구가 그대로 쓰임.
    /// (이 문장이 hintByLevel 문구를 대신하므로 할 행동까지 포함한 완결 문장이어야 함)
    /// </summary>
    public string GetProgressNote(PuzzleStep step, int hintLevel)
    {
        switch (GetProgressBand(step))
        {
            case "partial":   return PickByLevel(PartialNotes, hintLevel);
            case "ready":     return PickByLevel(ReadyNotes, hintLevel);
            case "attaching": return PickByLevel(AttachingNotes, hintLevel);
            case "decoy":     return PickByLevel(DecoyNotes, hintLevel);
            default:          return null;
        }
    }
}