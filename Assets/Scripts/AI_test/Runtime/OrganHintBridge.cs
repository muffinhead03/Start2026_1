using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오르간씬과 힌트 시스템(HintManager)을 잇는 브릿지. 네 가지 역할을 한다.
///
/// 1) 진행 기록 — OrganSceneManager.OnStepCompleted 이벤트를 받아 실제로 완료된 스텝 번호만 completedSteps에 기록.
///    (예전 폴링 방식은 "가장 큰 번호 이하를 전부 채워서" 순서를 건너뛰면 파이프 힌트가 사라지는 문제가 있었음)
///    별도 완료 신호가 없는 스텝만 예외로 같이 채움:
///    - 1 (오르간 고장 확인): 아무 스텝이나 진행되면 이미 확인한 걸로 간주
///    - 7 (파이프 4개 교체): 오르간 UnlockEvent가 CompleteStep(8)을 호출하므로 8이 오면 7도 완료
///
/// 2) 최근 행동 — 스텝이 완료될 때 "organ_step_NN", 손에 새 물건을 들었을 때 "organ_hold_{종류}"를 AddLastAction으로 넘김.
///    DefaultActionLabelProvider가 "오르골 소리를 듣고 있었지", "LP판을 집어 들었지" 같은 관찰 문장으로 바꿈.
///    (예전엔 스텝 완료만 기록해서, LP를 집어도 관찰 문장이 계속 "파이프를 찾아냈지"로 남는 문제가 있었음)
///
/// 3) 손/인벤토리 제공(IPossessionProvider) — Scene2의 PerceiveObjectHandPivot / InventoryData를 읽기 전용으로 참조.
///    objectName을 종류별로 묶어서(LP_0~4 → "LP", pipe_0~3 → "pipe") OrganRoomData의 relatedObjectName과 매칭
///    → 예: LP를 "손에" 들고 있으면 체크리스트 순서와 상관없이 step 6(LP 퍼즐) 힌트로 override.
///    (LP는 인벤토리에만 있을 땐 override 안 함 — HandOnlyKinds 참고)
///
/// 4) 위치 제공(ILocationAwareProvider) — OrganRoomData의 relatedObjectName 키를
///    OrganSceneManager가 이미 들고 있는 배열(pipes/papers/clues/puzzle_obj)의 Transform으로 매핑.
///    배열에 없는 환풍구만 Inspector에서 직접 연결.
///    → HintEngine이 "가까이 와 있구나 / 좀 멀리 떨어진 곳에 있는 것 같은데" 문장을 만듦.
/// </summary>
public class OrganHintBridge : MonoBehaviour, ILocationAwareProvider, IPossessionProvider
{
    [Header("힌트 시스템")]
    [SerializeField] private HintManager hintManager;

    [Header("오르간룸 상태 참조 (읽기 전용)")]
    [SerializeField] private OrganSceneManager organSceneManager;

    [Header("손/인벤토리 참조 (읽기 전용) — 비워두면 자동으로 찾음")]
    [SerializeField] private PerceiveObjectHandPivot handPerception;
    [SerializeField] private InventoryData inventoryData;

    [Header("플레이어 (거리 계산 기준점) — 비워두면 Player_Move를 자동으로 찾음")]
    [SerializeField] private Transform player;

    [Header("OrganSceneManager 배열에 없는 위치")]
    [Tooltip("step 10 (비밀 통로) — Vent_EventTrigger 오브젝트 연결")]
    [SerializeField] private Transform vent;

    const int TotalSteps = 17;

    static readonly (int stepId, string clueId)[] ClueMap =
    {
        (9,  "clue_torn_sheet_pieces"),
        (15, "clue_completed_sheet"),
    };

    // (이 스텝이 완료되면, 이 스텝도 같이 완료)
    static readonly (int trigger, int implied)[] ImpliedSteps =
    {
        (8, 7),
    };

    // 손에 든 물건 변화 감지용 (같은 물건을 계속 들고 있으면 행동을 반복 기록하지 않음)
    string lastHandName = null;

    // 스텝 완료 행동을 기록한 시각 — 파이프를 집는 순간처럼 "스텝 완료"와 "손에 듦"이 동시에 일어나면
    // 더 구체적인 스텝 문장("거실에서 파이프를 하나 찾아냈지")을 남기기 위해, 직후의 손 변화 기록은 건너뜀
    float lastStepActionTime = -999f;
    const float HoldActionSuppressSeconds = 1f;

    // OrganSceneManager 배열 인덱스 — Scene2.unity Inspector 설정 기준 (2026-09-25 main 확인)
    // pipes      : [0] Pipe 1(step 2)  [1] Pipe 2(step 3)  [2] Pipe 3(step 5)  [3] Pipe 4(step 6)
    // papers     : [0] pivot_0(step 11)  [1] pivot_1(step 9)  [2] pivot_2(step 14)  [3] pivot_3(step 13)
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
            case "MusicBox":    return At(organSceneManager.Clues, 1);
            case "PwdDoor":     return At(organSceneManager.PuzzleObjects, 0);
            case "LP":          return At(organSceneManager.PuzzleObjects, 2); // step 6 — LP를 올릴 턴테이블(GramophoneA)
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
        if (inventoryData == null)
            inventoryData = FindFirstObjectByType<InventoryData>();
        if (player == null)
        {
            var move = FindFirstObjectByType<Player_Move>();
            if (move != null) player = move.transform;
        }

        if (hintManager == null)
        {
            Debug.LogError("[OrganHintBridge] HintManager를 찾을 수 없습니다.");
        }
        else
        {
            hintManager.LocationProvider = this;
            hintManager.PossessionProvider = this;
        }

        if (organSceneManager == null)
            Debug.LogError("[OrganHintBridge] OrganSceneManager를 찾을 수 없습니다.");
        else
            organSceneManager.OnStepCompleted += HandleStepCompleted;

        if (player == null)
            Debug.LogWarning("[OrganHintBridge] 플레이어 Transform이 없어 거리 문장은 생략됩니다.");
        if (vent == null)
            Debug.LogWarning("[OrganHintBridge] vent 미연결 — step 10 거리 문장만 생략됩니다.");
        if (handPerception == null)
            Debug.LogWarning("[OrganHintBridge] PerceiveObjectHandPivot을 찾지 못함 — 손에 든 물건 기준 힌트/행동 기록이 생략됩니다.");
    }

    void OnDestroy()
    {
        if (organSceneManager != null)
            organSceneManager.OnStepCompleted -= HandleStepCompleted;
    }

    void Update()
    {
        if (hintManager == null) return;
        TrackHandObject();
    }

    // =========================================================
    // 1) 진행 기록 + 2) 최근 행동
    // =========================================================

    void HandleStepCompleted(int stepId)
    {
        if (hintManager == null) return;
        if (stepId < 1 || stepId > TotalSteps) return;

        // 같은 스텝이 두 번 들어와도(예: MusicBoxPlay가 먼저 4를 넣은 경우) 행동 기록은 매번 남김 — 최근 행동이니까
        hintManager.AddLastAction($"organ_step_{stepId:D2}");
        lastStepActionTime = Time.time;

        ReportStep(1); // 오르간 확인 스텝은 별도 신호가 없어서 첫 진행 시 같이 처리 (중복 호출 안전)

        foreach (var (trigger, implied) in ImpliedSteps)
            if (trigger == stepId)
                ReportStep(implied);

        ReportStep(stepId);
    }

    /// <summary>
    /// 손에 든 물건이 바뀌면 "organ_hold_{종류}"를 최근 행동으로 기록.
    /// 스텝 완료 직후(파이프/악보 조각을 집는 순간 등)는 스텝 문장이 더 구체적이라 건너뜀.
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

    void ReportStep(int stepId)
    {
        var state = hintManager.currentPlayerState;
        if (state.completedSteps.Contains(stepId)) return;

        foreach (var (id, clueId) in ClueMap)
            if (id == stepId && !state.foundClues.Contains(clueId))
                state.foundClues.Add(clueId);

        state.completedSteps.Add(stepId);
        Debug.Log($"[OrganHintBridge] step {stepId} 완료 기록 → 현재: [{string.Join(", ", state.completedSteps)}]");
    }

    // =========================================================
    // 3) IPossessionProvider
    // =========================================================

    /// <summary>
    /// objectName을 종류 단위로 묶음. Scene2의 LP는 LP_0~LP_4, 파이프는 pipe_0~pipe_3, 악보는 paper_0~paper_3처럼
    /// "종류_번호" 형식이라 마지막 '_' 앞부분만 사용 → "LP", "pipe", "paper".
    /// (relatedObjectName 하나로 같은 종류 여러 개를 매칭하기 위함. '_'가 없으면 그대로)
    /// </summary>
    static string NormalizeName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return objectName;
        int idx = objectName.LastIndexOf('_');
        return idx > 0 ? objectName.Substring(0, idx) : objectName;
    }

    public string GetHandObjectName()
    {
        if (handPerception == null || handPerception.CurrentObject == null)
            return null;

        return NormalizeName(handPerception.CurrentObject.objectName);
    }

    // 인벤토리에 들어 있어도 override 근거로 쓰지 않을 종류.
    // LP는 5장이라 한 번 주우면 퍼즐을 풀 때까지 인벤토리에 계속 남아서, 다른 걸 하는 중에도 힌트가 LP로 고정됨
    // (베타 로그: 파이프를 새로 주웠는데 힌트는 계속 그림/LP 얘기) → LP는 "손에 들고 있을 때"만 step 6으로 override.
    static readonly HashSet<string> HandOnlyKinds = new HashSet<string> { "LP" };

    public List<string> GetInventoryObjectNames()
    {
        var names = new List<string>();
        if (inventoryData == null) return names;

        for (int i = 0; i < inventoryData.SlotCount; i++)
        {
            Object_Grabbable obj = inventoryData.GetObjectAt(i);
            if (obj == null) continue;

            string kind = NormalizeName(obj.objectName);
            if (!HandOnlyKinds.Contains(kind))
                names.Add(kind);
        }

        return names;
    }

    // =========================================================
    // 4) ILocationAwareProvider
    // =========================================================

    public Vector3? GetPlayerPosition()
        => player != null ? player.position : (Vector3?)null;

    public Vector3? GetObjectPosition(string objectName)
    {
        var t = ResolveTarget(objectName);
        return t != null ? t.position : (Vector3?)null;
    }

    // 구역 시스템은 아직 없음 (와인씬과 동일) — null/0이면 HintEngine이 구역 문장만 생략함
    public string GetCurrentZoneName() => null;
    public float GetCurrentZoneStaySeconds() => 0f;
}