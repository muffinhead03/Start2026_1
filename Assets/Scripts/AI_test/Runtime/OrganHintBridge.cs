using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 오르간씬과 힌트 시스템(HintManager)을 잇는 브릿지 — scene02(민주) 힌트 트리 기준.
///
/// 1) 진행 기록 — OrganSceneManager.GetCurrentStep()(= 트리 리프 + 1)을 주기적으로 읽어서
///    LeafToStep으로 OrganRoomData id(진행 순서 1~23)로 바꾼 뒤,
///    "그 id보다 작은 스텝 = 전부 완료"로 completedSteps를 맞춤.
///    트리는 뒤로도 갈 수 있으니(LP를 잘못 틀면 조건이 false로 돌아감) 매번 통째로 다시 맞춤.
///    → HintEngine의 체크리스트(가장 낮은 미완료 id)가 곧 트리가 가리키는 스텝이 됨.
///
/// 2) 최근 행동 — 스텝이 앞으로 넘어가면 방금 끝낸 스텝에 맞는 "organ_step_NN" 문구를 AddLastAction.
///    (NN은 DefaultActionLabelProvider에 이미 있는 옛 번호 문구를 의미가 맞는 것만 재사용 — StepActionLabel 참고)
///    손에 새 물건을 들면 "organ_hold_{종류}"도 기록 (기존과 동일).
///
/// 3) 위치 제공(ILocationAwareProvider) — relatedObjectName 키 → OrganSceneManager 배열의 Transform. (기존과 동일)
///
/// 손/인벤토리 override(IPossessionProvider)는 일부러 등록하지 않음.
/// 트리가 진행 판정을 다 하는데, LP를 손에 들었다고 다른 스텝으로 튀면 트리랑 어긋나기 때문.
///
/// GetCurrentStep()은 scene02의 OrganSceneManager에만 있어서 리플렉션으로 호출함.
/// → scene02가 develop에 머지되기 전에도 develop에서 컴파일 에러가 나지 않음 (그땐 경고만 찍고 1단계 힌트로 동작).
/// </summary>
public class OrganHintBridge : MonoBehaviour, ILocationAwareProvider
{
    [Header("힌트 시스템")]
    [SerializeField] private HintManager hintManager;

    [Header("오르간룸 상태 참조 (읽기 전용)")]
    [SerializeField] private OrganSceneManager organSceneManager;

    [Header("손 참조 (최근 행동 기록용) — 비워두면 자동으로 찾음")]
    [SerializeField] private PerceiveObjectHandPivot handPerception;

    [Header("플레이어 (거리 계산 기준점) — 비워두면 Player_Move를 자동으로 찾음")]
    [SerializeField] private Transform player;

    [Header("OrganSceneManager 배열에 없는 위치")]
    [Tooltip("step 15 (비밀 통로) — Vent_EventTrigger 오브젝트 연결")]
    [SerializeField] private Transform vent;

    [Header("디버그")]
    [SerializeField] private float pollInterval = 0.25f;

    const int TotalSteps = 23;

    // 트리 리프(hint_id, 0~22) → OrganRoomData id(진행 순서).
    // LP 구간만 민주 트리의 리프 번호와 진행 순서가 달라서 표로 둠.
    //   리프 7 = 1층 LP 찾기, 8 = 2층 LP 찾기, 9 = 축음기 책 보기, 10 = LP 하나라도 찾기
    static readonly int[] LeafToStep =
    {
        1, 2, 3, 4, 5, 6, 7,   // 리프 0~6 : 설명서 → 파이프 → 오르골 → 키패드 → 비밀번호 방 → 파이프 3
        10, 11, 8, 9,          // 리프 7~10: LP 구간 (위 주석 참고)
        12,                    // 리프 11  : 파이프 4개 교체
        13, 14, 15, 16, 17,    // 리프 12~16: 악보 → 떨어진 악보 조각 → 비밀 통로 → 메트로놈 → 캐비닛 악보
        18, 19, 20, 21,        // 리프 17~20: 책장 → 큰 방 → 책상 악보 → 액자 악보
        22, 23,                // 리프 21~22: 조합 → 연주
    };

    // 스텝을 끝냈을 때 남길 최근 행동 키 (DefaultActionLabelProvider의 기존 오르간 문구 재사용).
    // 맞는 문구가 없는 스텝은 생략 → 손에 든 물건 기록(organ_hold_*)이 대신 관찰 문장을 채움.
    static readonly Dictionary<int, string> StepActionLabel = new Dictionary<int, string>
    {
        { 2,  "organ_step_02" }, // 파이프를 처음 찾음 — "거실에서 파이프를 하나 찾아냈지"
        { 3,  "organ_step_03" }, // 파이프 1·2 — "옆방에서 파이프를 또 찾아냈지"
        { 4,  "organ_step_04" }, // 오르골 — "오르골 소리를 듣고 있었지"
        { 7,  "organ_step_05" }, // 파이프 3 — "잠긴 방에서 파이프를 꺼내왔지"
        { 11, "organ_step_06" }, // LP로 파이프 4 — "LP를 틀어서 파이프를 찾아냈지"
        { 12, "organ_step_08" }, // 파이프 교체 — "오르간에 파이프를 끼워 넣었지"
        { 14, "organ_step_09" }, // 떨어진 악보 조각 — "바닥에 떨어진 악보 조각을 챙겼네"
        { 15, "organ_step_10" }, // 비밀 통로 — "좁은 통로로 기어 들어갔지"
        { 16, "organ_step_11" }, // 메트로놈 — "메트로놈 박자를 맞추고 있었지"
        { 19, "organ_step_12" }, // 책장 — "책장을 만지작거리고 있었지"
        { 20, "organ_step_14" }, // 책상 악보 — "책상 위를 뒤지고 있었지"
        { 21, "organ_step_13" }, // 액자 악보 — "액자 뒤를 뒤져보고 있었지"
        { 22, "organ_step_15" }, // 조합 — "악보 조각을 맞추고 있었지"
    };

    static readonly (int stepId, string clueId)[] ClueMap =
    {
        (4,  "clue_music_box_melody"),
        (14, "clue_torn_sheet_pieces"),
        (22, "clue_completed_sheet"),
    };

    MethodInfo getCurrentStepMethod;
    int currentStepId = 0;      // 트리가 가리키는 지금 스텝 (0 = 아직 못 읽음)
    float nextPollTime = 0f;

    string lastHandName = null;
    float lastStepActionTime = -999f;
    const float HoldActionSuppressSeconds = 1f;

    // OrganSceneManager 배열 인덱스 — Scene2.unity Inspector 설정 기준 (2026-09-25 main 확인)
    // pipes      : [0] Pipe 1  [1] Pipe 2  [2] Pipe 3  [3] Pipe 4
    // papers     : [0] pivot_0  [1] pivot_1  [2] pivot_2  [3] pivot_3
    // clues      : [0] Organ_Manual  [1] Musicbox
    // puzzle_obj : [0] pwd  [1] organ  [2] GramophoneA  [3] Metronome  [4] BookShelf  [5] organ  [6] chestbox
    // 배열 순서가 바뀌면 여기만 고치면 됨.
    Transform ResolveTarget(string key)
    {
        if (organSceneManager == null) return null;

        switch (key)
        {
            case "Organ":       return At(organSceneManager.PuzzleObjects, 1);
            case "Pipe1":       return At(organSceneManager.Pipes, 0);
            case "Pipe2":       return At(organSceneManager.Pipes, 1);
            case "Pipe3":       return At(organSceneManager.Pipes, 2);
            case "MusicBox":    return At(organSceneManager.Clues, 1);
            case "PwdDoor":     return At(organSceneManager.PuzzleObjects, 0);
            case "LP":          return At(organSceneManager.PuzzleObjects, 2); // LP 구간 — 턴테이블(GramophoneA) 기준
            case "SheetPiece2": return At(organSceneManager.Papers, 1);
            case "Vent":        return vent;
            case "Metronome":   return At(organSceneManager.PuzzleObjects, 3);
            case "BookShelf":   return At(organSceneManager.PuzzleObjects, 4);
            case "SheetPiece3": return At(organSceneManager.Papers, 3);
            case "SheetPiece4": return At(organSceneManager.Papers, 2);
            case "Chest":       return At(organSceneManager.PuzzleObjects, 6);
            default:            return null;
        }
    }

    static Transform At(GameObject[] arr, int index)
        => (arr != null && index >= 0 && index < arr.Length && arr[index] != null) ? arr[index].transform : null;

    void Start()
    {
        if (hintManager == null)
            hintManager = FindFirstObjectByType<HintManager>();
        if (organSceneManager == null)
            organSceneManager = FindFirstObjectByType<OrganSceneManager>();
        if (handPerception == null)
            handPerception = FindFirstObjectByType<PerceiveObjectHandPivot>();
        if (player == null)
        {
            var move = FindFirstObjectByType<Player_Move>();
            if (move != null) player = move.transform;
        }

        if (hintManager == null)
            Debug.LogError("[OrganHintBridge] HintManager를 찾을 수 없습니다.");
        else
        {
            hintManager.LocationProvider = this;
            hintManager.PossessionProvider = null; // 트리 기준으로만 진행 (위 설명 참고)
        }

        if (organSceneManager == null)
            Debug.LogError("[OrganHintBridge] OrganSceneManager를 찾을 수 없습니다.");
        else
        {
            getCurrentStepMethod = organSceneManager.GetType().GetMethod(
                "GetCurrentStep", BindingFlags.Public | BindingFlags.Instance, null, System.Type.EmptyTypes, null);
            if (getCurrentStepMethod == null)
                Debug.LogWarning("[OrganHintBridge] OrganSceneManager.GetCurrentStep()이 없음 — scene02 머지 전 버전. 1단계 힌트로만 동작합니다.");
        }

        if (player == null)
            Debug.LogWarning("[OrganHintBridge] 플레이어 Transform이 없어 거리 문장은 생략됩니다.");
        if (vent == null)
            Debug.LogWarning("[OrganHintBridge] vent 미연결 — 비밀 통로 스텝 거리 문장만 생략됩니다.");
    }

    void Update()
    {
        if (hintManager == null || hintManager.currentPlayerState == null) return;

        if (Time.time >= nextPollTime)
        {
            nextPollTime = Time.time + pollInterval;
            PollTree();
        }

        TrackHandObject();
    }

    // =========================================================
    // 1) 진행 기록 + 2) 최근 행동
    // =========================================================

    void PollTree()
    {
        int stepId = ReadTreeStep();
        if (stepId <= 0 || stepId == currentStepId) return;

        int prev = currentStepId;
        currentStepId = stepId;

        SyncCompletedSteps(stepId);

        // 앞으로 넘어갔을 때만 "방금 끝낸 스텝" 행동 기록 (여러 칸 건너뛰면 가장 최근 것 하나만)
        if (prev > 0 && stepId > prev)
        {
            for (int done = stepId - 1; done >= prev; done--)
            {
                if (StepActionLabel.TryGetValue(done, out var key))
                {
                    hintManager.AddLastAction(key);
                    lastStepActionTime = Time.time;
                    break;
                }
            }
        }

        Debug.Log($"[OrganHintBridge] 트리 스텝 {prev} → {stepId}");
    }

    /// <summary>트리 리프를 OrganRoomData id로. 못 읽으면 1(첫 스텝).</summary>
    int ReadTreeStep()
    {
        if (organSceneManager == null || getCurrentStepMethod == null) return 1;

        int leaf;
        try
        {
            leaf = (int)getCurrentStepMethod.Invoke(organSceneManager, null) - 1; // GetCurrentStep() = hint_id + 1
        }
        catch (System.Exception e)
        {
            // 트리 초기화(Start) 전에 읽으면 NullReference — 다음 폴링에서 다시 시도
            if (currentStepId == 0) Debug.Log($"[OrganHintBridge] 트리 아직 준비 안 됨: {e.InnerException?.GetType().Name ?? e.GetType().Name}");
            return 0;
        }

        if (leaf < 0 || leaf >= LeafToStep.Length)
        {
            Debug.LogWarning($"[OrganHintBridge] 알 수 없는 트리 리프 {leaf} — LeafToStep 표 확인 필요");
            return 0;
        }
        return LeafToStep[leaf];
    }

    /// <summary>completedSteps를 [1 .. stepId-1]로 맞춤 (트리가 뒤로 가면 줄어듦).</summary>
    void SyncCompletedSteps(int stepId)
    {
        var state = hintManager.currentPlayerState;

        state.completedSteps.Clear();
        for (int id = 1; id < stepId && id <= TotalSteps; id++)
            state.completedSteps.Add(id);

        foreach (var (id, clueId) in ClueMap)
            if (id < stepId && !state.foundClues.Contains(clueId))
                state.foundClues.Add(clueId);

        Debug.Log($"[OrganHintBridge] 완료 스텝 → [{string.Join(", ", state.completedSteps)}]");
    }

    /// <summary>
    /// 손에 든 물건이 바뀌면 "organ_hold_{종류}"를 최근 행동으로 기록.
    /// 스텝이 막 넘어간 직후(파이프/악보를 집는 순간 등)는 스텝 문장이 더 구체적이라 건너뜀.
    /// </summary>
    void TrackHandObject()
    {
        string hand = GetHandObjectName();
        if (hand == lastHandName) return;

        lastHandName = hand;
        if (string.IsNullOrEmpty(hand)) return;
        if (Time.time - lastStepActionTime < HoldActionSuppressSeconds) return;

        hintManager.AddLastAction($"organ_hold_{hand}");
    }

    /// <summary>LP_0~4 → "LP", pipe_0~3 → "pipe"처럼 마지막 '_' 뒤 번호를 뗌.</summary>
    static string NormalizeName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return objectName;
        int idx = objectName.LastIndexOf('_');
        return idx > 0 ? objectName.Substring(0, idx) : objectName;
    }

    string GetHandObjectName()
    {
        if (handPerception == null || handPerception.CurrentObject == null)
            return null;

        return NormalizeName(handPerception.CurrentObject.objectName);
    }

    // =========================================================
    // 3) ILocationAwareProvider
    // =========================================================

    public Vector3? GetPlayerPosition()
        => player != null ? player.position : (Vector3?)null;

    public Vector3? GetObjectPosition(string objectName)
    {
        var t = ResolveTarget(objectName);
        return t != null ? t.position : (Vector3?)null;
    }

    public string GetCurrentZoneName() => null;
    public float GetCurrentZoneStaySeconds() => 0f;
}