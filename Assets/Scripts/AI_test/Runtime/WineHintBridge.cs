using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 와인씬(10월 리디자인)을 힌트 시스템에 연결하는 브릿지. DollHintBridge처럼 씬 상태를 읽기 전용으로 폴링함
/// (팀원 스크립트 수정 없음 — LetterMemoPanel / Object_PutOn.putOn / Object_Puzzle.pwd / Object_Door.isLocked / 손·인벤토리만 읽음).
///
/// 역할 4가지:
/// 1) 진행 기록 — completedSteps 1~3 (WineGlassRoomData 주석 참고). 뒤 단계가 진행되면 앞 단계도 같이 완료(back-fill).
/// 2) 최근 행동 — "wine_..." 행동 키를 AddLastAction (DefaultActionLabelProvider가 관찰 문장으로 바꿈)
///    - 폴링으로 자동: 메모 열고 닫음(고침/그냥 봄), 병 집음/꽂음/뺌, 열쇠 집음, 렉 완성
///    - 씬 이벤트에 연결: ReportHintStainViewed / ReportBookshelfViewed (Object_FixCamera.Fixed),
///                        ReportCabinetTried (장식장 문 Event_On_Ray.OnClick)
/// 3) 손/인벤토리 제공(IPossessionProvider) — 병은 "wine", 열쇠는 "key1"로 넘겨서 HintEngine이 스텝을 override
///    (병은 글자를 알아낸 뒤(step 1 완료)부터만 넘김 — 처음부터 병을 집으면 앞 단계를 건너뛴 힌트가 나와서)
/// 4) 스텝 진행도 제공(IStepProgressProvider) — 진행 구간 × 레벨별 문장으로 hintByLevel을 대신함
///    - step 1: 벽 그림만 봄 / 메모 일부 맞음
///    - step 2: 글자 알아낸 뒤 책장 확대 봄
///    - step 3: 다른 병 자리에 꽂음(색 혼동) / 정답 아닌 칸에 꽂음(숫자 혼동) / 일부 맞음
///    - step 4: 열쇠 보유
///    정답 글자·숫자·칸 번호, 맞은 개수는 어떤 문장에도 넣지 않음.
///
/// 위치 정보는 WineLocationProvider가 그대로 담당 (trackedObjects에 HintStain / BookShelf / wine / key1 등록).
/// </summary>
public class WineHintBridge : MonoBehaviour, IStepProgressProvider, IPossessionProvider
{
    [Header("힌트 시스템 (비워두면 자동 탐색)")]
    [SerializeField] private HintManager hintManager;

    [Header("1단계 — 메모 종이 (Note_Alphabet의 LetterMemoPanel)")]
    [SerializeField] private LetterMemoPanel memo;

    [Header("3단계 — 와인렉")]
    [Tooltip("Room_2/WineRackPuzzle의 Object_Puzzle (정답 pwd를 여기서 읽음)")]
    [SerializeField] private Object_Puzzle rackPuzzle;
    [Tooltip("wine_slots 부모 — 아래 slot_1~26의 Object_Puzzle_Piece를 자동으로 모음")]
    [SerializeField] private Transform slotsRoot;
    [Tooltip("병 objectName 접두사 (wine_1 ~ wine_4)")]
    [SerializeField] private string bottlePrefix = "wine_";
    [Tooltip("틀린 칸에 꽂으면 failCount를 올릴지 (힌트 레벨이 빨리 오름)")]
    [SerializeField] private bool registerFailOnWrongSlot = true;

    [Header("3·4단계 — 장식장 / 열쇠")]
    [Tooltip("DisplayModularDoorMid의 Object_Door — 잠긴 상태에서 열어보려 했는지 판단용")]
    [SerializeField] private Object_Door cabinetDoor;
    [SerializeField] private string keyObjectName = "key1";

    [Header("손/인벤토리 (읽기 전용, 비워두면 자동 탐색)")]
    [SerializeField] private PerceiveObjectHandPivot handPerception;
    [SerializeField] private InventoryData inventoryData;

    [Header("디버그")]
    [SerializeField] private bool verboseLog = true;

    // WineGlassRoomData 스텝 번호
    const int StepLetters = 1;
    const int StepBooks   = 2;
    const int StepRack    = 3;
    const int StepKey     = 4;

    // relatedObjectName과 맞춘 이름
    const string BottleKind = "wine";

    // 슬롯 상태
    readonly List<Object_Puzzle_Piece> slots = new List<Object_Puzzle_Piece>();
    readonly Dictionary<Object_Puzzle_Piece, GameObject> lastSlotObject = new Dictionary<Object_Puzzle_Piece, GameObject>();

    // 폴링 상태
    bool memoWasOpen;
    string memoLettersAtOpen;
    string lastHandName;
    bool keyAcquired;
    bool rackSolvedReported;

    // 진행 구간 판단용 기록
    bool hintStainViewed;
    bool bookshelfViewedWithLetters; // 글자를 알아낸 뒤(step 1 완료 후) 책장을 확대해 봤는지
    string lastWrongKind;            // 가장 최근 틀린 꽂기 종류 ("color" / "number") — 맞는 꽂기를 하면 풀림

    // 같은 프레임에 "칸에서 뺌"과 "손에 듦"이 같이 일어나면 뺌만 기록
    float lastRemoveTime = -999f;
    const float GrabSuppressSeconds = 0.5f;

    // =========================================================
    // 초기화
    // =========================================================

    void Start()
    {
        if (hintManager == null)    hintManager    = FindFirstObjectByType<HintManager>();
        if (memo == null)           memo           = FindFirstObjectByType<LetterMemoPanel>(FindObjectsInactive.Include);
        if (handPerception == null) handPerception = FindFirstObjectByType<PerceiveObjectHandPivot>();
        if (inventoryData == null)  inventoryData  = FindFirstObjectByType<InventoryData>();

        if (hintManager == null)
        {
            Debug.LogError("[WineHintBridge] HintManager를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        if (slotsRoot != null)
            slotsRoot.GetComponentsInChildren(true, slots);

        if (memo == null)           Debug.LogWarning("[WineHintBridge] LetterMemoPanel 없음 — 1단계 진행도가 빠집니다.");
        if (rackPuzzle == null)     Debug.LogWarning("[WineHintBridge] Rack Puzzle 칸이 비어 있음 — 3단계 정답 판단이 빠집니다.");
        if (slots.Count == 0)       Debug.LogWarning("[WineHintBridge] Slots Root 아래 Object_Puzzle_Piece가 없음 — 병 꽂기 판단이 빠집니다.");
        if (cabinetDoor == null)    Debug.LogWarning("[WineHintBridge] Cabinet Door 칸이 비어 있음 — 잠긴 장식장 시도 기록이 항상 남습니다.");
        if (handPerception == null) Debug.LogWarning("[WineHintBridge] PerceiveObjectHandPivot 없음 — 손 기준 override가 빠집니다.");

        foreach (var s in slots)
            lastSlotObject[s] = CurrentObjectIn(s);

        hintManager.ProgressProvider   = this;
        hintManager.PossessionProvider = this;
        Debug.Log($"[WineHintBridge] 등록 완료 (Progress/Possession) — 슬롯 {slots.Count}개, 정답 칸 {AnswerSlotCount()}개");
    }

    // =========================================================
    // Update — 상태 폴링
    // =========================================================

    void Update()
    {
        PollMemo();
        PollSlots();   // 손보다 먼저 — 칸에서 뺀 병이 손에 들리는 걸 "뺌"으로 기록하려고
        PollHand();
        PollKey();
        PollRackSolved();
    }

    void PollMemo()
    {
        if (memo == null) return;

        bool open = memo.IsOpen;
        if (open && !memoWasOpen)
        {
            memoLettersAtOpen = memo.CurrentLetters;
        }
        else if (!open && memoWasOpen)
        {
            bool changed = memo.CurrentLetters != memoLettersAtOpen;
            hintManager.AddLastAction(changed ? "wine_memo_written" : "wine_memo_opened");
            Log($"메모 닫음 ({(changed ? "고침" : "그냥 봄")}) — 정답 {memo.CorrectCount}/4");

            if (memo.IsSolved)
                CompleteStep(StepLetters, "clue_memo_letters", "메모 정답 기록");
        }
        memoWasOpen = open;
    }

    void PollSlots()
    {
        foreach (var slot in slots)
        {
            GameObject now = CurrentObjectIn(slot);
            lastSlotObject.TryGetValue(slot, out GameObject before);
            if (now == before) continue;
            lastSlotObject[slot] = now;

            if (now == null)
            {
                lastRemoveTime = Time.time;
                hintManager.AddLastAction("wine_bottle_removed");
                Log($"병 뺌: {slot.name}");
                continue;
            }

            string verdict = Classify(slot, now);
            hintManager.AddLastAction("wine_bottle_placed");
            Log($"병 꽂음: {NameOf(now)} → {slot.name} ({verdict})");

            if (verdict == "correct")
            {
                lastWrongKind = null;
            }
            else
            {
                lastWrongKind = verdict;
                if (registerFailOnWrongSlot) hintManager.RegisterFail();
            }
        }

        // 병 2개 이상이 정답 칸 → 글자·숫자는 이미 알아낸 것
        if (CorrectPlacedCount() >= 2)
        {
            CompleteStep(StepLetters, "clue_memo_letters", "병 2개 이상 정답 칸 (back-fill)");
            CompleteStep(StepBooks,   "clue_book_numbers", "병 2개 이상 정답 칸 (back-fill)");
        }
    }

    void PollHand()
    {
        string hand = RawHandName();
        if (hand == lastHandName) return;
        lastHandName = hand;

        if (string.IsNullOrEmpty(hand)) return;
        if (Time.time - lastRemoveTime < GrabSuppressSeconds) return; // 칸에서 뺀 거면 "뺌"으로 이미 기록

        if (IsBottle(hand))
        {
            hintManager.AddLastAction("wine_bottle_grabbed");
            Log($"병 집음: {hand}");
        }
    }

    void PollKey()
    {
        if (keyAcquired) return;
        if (RawHandName() != keyObjectName && !InventoryHas(keyObjectName)) return;

        keyAcquired = true;
        hintManager.AddLastAction("wine_key_grabbed");
        // 열쇠를 손에 넣었으면 렉은 풀린 것 (렉 판단이 어긋났을 때 대비)
        CompleteStep(StepLetters, "clue_memo_letters", "열쇠 획득 (back-fill)");
        CompleteStep(StepBooks,   "clue_book_numbers", "열쇠 획득 (back-fill)");
        CompleteStep(StepRack,    "clue_wine_rack",    "열쇠 획득 (back-fill)");
    }

    void PollRackSolved()
    {
        if (rackSolvedReported) return;

        // 장식장 잠금 상태로는 판단하지 않음 (Inspector에서 Is Locked를 안 켜면 처음부터 풀려 있어서 오판)
        bool solved = AnswerSlotCount() > 0 && CorrectPlacedCount() == AnswerSlotCount();
        if (!solved) return;

        rackSolvedReported = true;
        lastWrongKind = null;
        hintManager.AddLastAction("wine_rack_solved");
        CompleteStep(StepLetters, "clue_memo_letters", "렉 완성 (back-fill)");
        CompleteStep(StepBooks,   "clue_book_numbers", "렉 완성 (back-fill)");
        CompleteStep(StepRack,    "clue_wine_rack",    "렉 완성");
    }

    // =========================================================
    // 씬 이벤트에 연결하는 메서드 (UnityEvent)
    // =========================================================

    /// <summary>Room_1/Hint_Stain의 Object_FixCamera.Fixed에 연결</summary>
    public void ReportHintStainViewed()
    {
        hintStainViewed = true;
        hintManager?.AddLastAction("wine_view_hint_stain");
        Log("벽 그림 확대");
    }

    /// <summary>Room_3/BookShelfView의 Object_FixCamera.Fixed에 연결</summary>
    public void ReportBookshelfViewed()
    {
        hintManager?.AddLastAction("wine_view_bookshelf");

        if (IsDone(StepLetters))
        {
            bookshelfViewedWithLetters = true;
            CompleteStep(StepBooks, "clue_book_numbers", "글자 안 뒤 책장 확대");
        }
        Log($"책장 확대 (글자 앎: {IsDone(StepLetters)})");
    }

    /// <summary>장식장 문 Event_On_Ray.OnClick에 연결 (Object_Door.OpenCloseDoor 뒤에 추가)</summary>
    public void ReportCabinetTried()
    {
        if (cabinetDoor != null && !cabinetDoor.isLocked) return; // 열린 뒤엔 그냥 여닫기라 기록 안 함
        hintManager?.AddLastAction("wine_cabinet_locked");
        Log("잠긴 장식장 열어보려 함");
    }

    // =========================================================
    // 슬롯 판정
    // =========================================================

    static GameObject CurrentObjectIn(Object_Puzzle_Piece slot)
        => slot != null && slot.putOn != null ? slot.putOn.putOn : null;

    static string NameOf(GameObject go)
    {
        if (go == null) return null;
        var g = go.GetComponent<Object_Grabbable>();
        return g != null ? g.objectName : go.name;
    }

    char AnswerAt(Object_Puzzle_Piece slot)
    {
        if (rackPuzzle == null || string.IsNullOrEmpty(rackPuzzle.pwd)) return '-';
        if (slot.id < 0 || slot.id >= rackPuzzle.pwd.Length) return '-';
        return rackPuzzle.pwd[slot.id];
    }

    /// <summary>
    /// "correct" — 맞는 칸
    /// "color"   — 다른 병의 정답 칸 (숫자는 맞게 찾았는데 병 색↔글자 연결이 틀림)
    /// "number"  — 어떤 병의 정답도 아닌 칸 (책 숫자를 잘못 찾았거나 아무 데나 꽂음)
    /// </summary>
    string Classify(Object_Puzzle_Piece slot, GameObject bottle)
    {
        char answer = AnswerAt(slot);
        if (answer == '-') return "number";

        string n = NameOf(bottle);
        char id = string.IsNullOrEmpty(n) ? '?' : n[n.Length - 1]; // Object_PutOn.GetKeyId와 같은 규칙
        return id == answer ? "correct" : "color";
    }

    int AnswerSlotCount()
    {
        if (rackPuzzle == null || string.IsNullOrEmpty(rackPuzzle.pwd)) return 0;
        int c = 0;
        foreach (char ch in rackPuzzle.pwd) if (ch != '-') c++;
        return c;
    }

    int CorrectPlacedCount()
    {
        int c = 0;
        foreach (var s in slots)
        {
            var go = CurrentObjectIn(s);
            if (go != null && Classify(s, go) == "correct") c++;
        }
        return c;
    }

    // =========================================================
    // 진행 기록
    // =========================================================

    bool IsDone(int stepId) => hintManager != null && hintManager.currentPlayerState.completedSteps.Contains(stepId);

    void CompleteStep(int stepId, string clueId, string why)
    {
        var state = hintManager.currentPlayerState;
        if (state.completedSteps.Contains(stepId)) return;

        state.completedSteps.Add(stepId);
        if (!string.IsNullOrEmpty(clueId) && !state.foundClues.Contains(clueId))
            state.foundClues.Add(clueId);

        Debug.Log($"[WineHintBridge] step {stepId} 완료 ({why}) → 현재: [{string.Join(", ", state.completedSteps)}]");
    }

    // =========================================================
    // IPossessionProvider
    // =========================================================

    bool IsBottle(string objectName)
        => !string.IsNullOrEmpty(objectName) && objectName.StartsWith(bottlePrefix);

    string RawHandName()
        => handPerception != null && handPerception.CurrentObject != null
            ? handPerception.CurrentObject.objectName
            : null;

    bool InventoryHas(string objectName)
    {
        if (inventoryData == null) return false;
        for (int i = 0; i < inventoryData.SlotCount; i++)
        {
            var obj = inventoryData.GetObjectAt(i);
            if (obj != null && obj.objectName == objectName) return true;
        }
        return false;
    }

    // 병 → "wine" (step 3), 열쇠 → "key1" (step 4). 병은 글자를 알아낸 뒤부터만 override 근거로 씀.
    string ToKind(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return null;
        if (IsBottle(objectName)) return IsDone(StepLetters) ? BottleKind : null;
        return objectName;
    }

    public string GetHandObjectName() => ToKind(RawHandName());

    public List<string> GetInventoryObjectNames()
    {
        var names = new List<string>();
        if (inventoryData == null) return names;

        for (int i = 0; i < inventoryData.SlotCount; i++)
        {
            var obj = inventoryData.GetObjectAt(i);
            if (obj == null) continue;
            string kind = ToKind(obj.objectName);
            if (kind != null && !names.Contains(kind)) names.Add(kind);
        }
        return names;
    }

    // =========================================================
    // IStepProgressProvider
    // =========================================================

    /// <summary>
    /// 진행 구간 이름. 같은 스텝 재요청 때 HintManager가 "지난 힌트 이후 진행이 있었는지" 보는 데 씀.
    /// null이면 hintByLevel 문구가 그대로 쓰임.
    /// </summary>
    public string GetProgressBand(PuzzleStep step)
    {
        if (step == null) return null;

        switch (step.id)
        {
            case StepLetters:
                if (memo != null && memo.CorrectCount > 0) return $"memo_partial_{memo.CorrectCount}";
                if (hintStainViewed) return "stain_viewed";
                return null;

            case StepBooks:
                return bookshelfViewedWithLetters ? "shelf_viewed" : null;

            case StepRack:
                if (lastWrongKind == "color")  return "wrong_color";
                if (lastWrongKind == "number") return "wrong_number";
                int correct = CorrectPlacedCount();
                if (correct > 0) return $"rack_partial_{correct}";
                return null;

            case StepKey:
                return keyAcquired ? "has_key" : null;
        }
        return null;
    }

    public int GetMinHintLevel(PuzzleStep step) => 1;

    public string GetProgressNote(PuzzleStep step, int hintLevel)
    {
        string band = GetProgressBand(step);
        if (band == null) return null;

        if (band.StartsWith("memo_partial_")) return PickByLevel(MemoPartialNotes, hintLevel);
        if (band.StartsWith("rack_partial_")) return PickByLevel(RackPartialNotes, hintLevel);

        switch (band)
        {
            case "stain_viewed": return PickByLevel(StainViewedNotes, hintLevel);
            case "shelf_viewed": return PickByLevel(ShelfViewedNotes, hintLevel);
            case "wrong_color":  return PickByLevel(WrongColorNotes, hintLevel);
            case "wrong_number": return PickByLevel(WrongNumberNotes, hintLevel);
            case "has_key":      return PickByLevel(HasKeyNotes, hintLevel);
        }
        return null;
    }

    // [레벨 단계][후보] — 레벨 1~2(모호) / 3(분명) / 4~5(거의 정답).
    // 이 문장이 hintByLevel 문구를 대신하므로 "할 행동"까지 들어간 완결 문장이어야 함. 정답 글자·숫자·개수는 넣지 않음.

    // step 1 — 벽 그림은 봤는데 메모는 아직 하나도 안 맞음
    static readonly string[][] StainViewedNotes =
    {
        new[] { "그림을 봤으면 이제 바닥을 볼 차례예요.",
                "그림 속 색깔을 기억해두고 방을 다시 둘러보세요." },
        new[] { "그림 속 얼룩과 같은 색의 얼룩이 바닥에 있어요. 하나씩 가까이 가서 살펴보세요.",
                "바닥 얼룩을 그림 속 색깔과 하나씩 맞춰보세요. 얼룩마다 숨은 게 있어요." },
        new[] { "그림과 같은 색의 바닥 얼룩에서 글자를 찾고, 1번 방 액자 옆 메모 종이의 같은 색 칸에 적으세요.",
                "바닥 얼룩마다 글자가 숨어 있어요. 색깔별로 찾은 글자를 메모 종이 같은 색 칸에 적어보세요." },
    };

    // step 1 — 메모에 일부만 맞게 적음
    static readonly string[][] MemoPartialNotes =
    {
        new[] { "잘하고 있어요. 메모에 적은 것 중에 아직 맞지 않는 게 있어요.",
                "거의 왔어요. 메모를 다시 한번 확인해보세요." },
        new[] { "메모 일부는 맞았어요. 나머지 색깔의 얼룩을 다시 찾아가 글자를 확인해보세요.",
                "아직 틀린 칸이 남아 있어요. 그 칸 색깔과 같은 바닥 얼룩을 다시 보세요." },
        new[] { "메모 칸 색깔과 바닥 얼룩 색깔이 정확히 같은지 확인하고, 그 얼룩의 글자로 틀린 칸을 고쳐 적으세요.",
                "틀린 칸은 색깔을 헷갈렸을 가능성이 커요. 벽 그림과 바닥 얼룩 색을 다시 비교해서 그 칸 글자를 고치세요." },
    };

    // step 2 — 글자를 안 뒤 책장을 봤음 (보통 바로 완료되지만, 완료 조건이 바뀌었을 때 대비)
    static readonly string[][] ShelfViewedNotes =
    {
        new[] { "책장에서 메모 글자와 관련된 걸 찾아보세요.",
                "책 제목들을 메모와 비교해보세요." },
        new[] { "메모 글자로 시작하는 책에 숫자가 적혀 있어요.",
                "메모의 각 글자로 시작하는 책을 찾고 그 숫자를 확인해보세요." },
        new[] { "메모의 색깔별 글자로 시작하는 책을 하나씩 찾아서, 그 책의 숫자를 색깔별로 기억해두세요.",
                "책 네 권의 숫자를 색깔과 함께 기억한 뒤 와인렉으로 가보세요." },
    };

    // step 3 — 다른 병의 정답 칸에 꽂음 (숫자는 찾았는데 병 색 연결이 틀림)
    static readonly string[][] WrongColorNotes =
    {
        new[] { "칸은 잘 찾았는데 병이 바뀐 것 같아요.",
                "그 칸에 들어갈 병은 따로 있어요." },
        new[] { "병마다 해당하는 색이 있어요. 그 색 글자의 숫자 칸에 꽂아야 해요.",
                "숫자 칸은 맞게 찾았어요. 그 칸에 맞는 색의 병으로 바꿔 꽂아보세요." },
        new[] { "꽂은 병의 색을 메모 칸 색과 맞춰보세요. 그 색 글자로 찾은 책의 숫자 칸이 그 병의 자리예요.",
                "병을 빼서, 병 색과 같은 메모 칸의 글자를 확인하고 그 글자 책의 숫자 칸에 다시 꽂으세요." },
    };

    // step 3 — 어떤 병의 정답도 아닌 칸에 꽂음 (숫자를 모르거나 아무 데나 꽂음)
    static readonly string[][] WrongNumberNotes =
    {
        new[] { "그 칸은 아닌 것 같아요. 칸을 고르는 기준이 있어요.",
                "아무 칸에나 꽂으면 안 돼요. 어디서 숫자를 봤는지 떠올려보세요." },
        new[] { "와인렉 칸 번호는 책에서 본 숫자와 맞춰야 해요.",
                "책장에서 메모 글자로 시작하는 책을 다시 보고 그 숫자를 확인하세요." },
        new[] { "병 색에 맞는 메모 글자로 시작하는 책을 찾고, 그 책 숫자와 같은 번호 칸에 꽂으세요.",
                "꽂은 병을 빼고, 책장에서 그 병 색 글자의 책 숫자를 확인한 뒤 같은 번호 칸에 꽂으세요." },
    };

    // step 3 — 일부 맞게 꽂음 (틀린 꽂기 없음)
    static readonly string[][] RackPartialNotes =
    {
        new[] { "좋아요, 제대로 꽂고 있어요. 남은 병도 같은 방법으로 해보세요.",
                "잘 가고 있어요. 나머지 병의 자리도 찾아보세요." },
        new[] { "맞게 꽂은 병이 있어요. 남은 병도 색깔 → 글자 → 책 숫자 순서로 자리를 찾아보세요.",
                "같은 방법으로 남은 병의 칸을 찾으면 돼요." },
        new[] { "남은 병마다 색에 맞는 메모 글자를 확인하고, 그 글자 책의 숫자 칸에 꽂으세요.",
                "아직 자리를 못 찾은 병을 들고, 그 색 글자의 책 숫자와 같은 번호 칸에 꽂아보세요." },
    };

    // step 4 — 열쇠를 가지고 있음
    static readonly string[][] HasKeyNotes =
    {
        new[] { "손에 넣은 걸 쓸 곳이 있을 거예요.",
                "그 열쇠가 맞는 곳을 찾아보세요." },
        new[] { "열쇠 구멍이 있는 걸 찾아보세요.",
                "방 안의 상자를 살펴보세요." },
        new[] { "열쇠를 들고 상자로 가서 넣어보세요.",
                "상자에 열쇠를 넣으면 나갈 수 있을 거예요." },
    };

    static string PickByLevel(string[][] table, int hintLevel)
    {
        int tier = hintLevel <= 2 ? 0 : (hintLevel == 3 ? 1 : 2);
        tier = Mathf.Clamp(tier, 0, table.Length - 1);
        var pool = table[tier];
        return pool[Random.Range(0, pool.Length)];
    }

    void Log(string msg)
    {
        if (verboseLog) Debug.Log($"[WineHintBridge] {msg}");
    }
}