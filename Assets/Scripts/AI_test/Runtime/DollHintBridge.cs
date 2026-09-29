using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인형씬 게임 상태(DollScene_GameManager / DollScene_ChangeDoll 등)를 읽기 전용으로 폴링해서
/// 힌트 시스템(HintManager)에 연결하는 브릿지. 혜교님 파트 스크립트는 수정하지 않고 public 프로퍼티만 참조함.
///
/// 역할 5가지:
/// 1) 진행 기록 — completedSteps / foundClues
/// 2) 최근 행동 — 스텝 완료 시 "doll_step_NN", 곰 인형 던질 때 "doll_bear_throw_{던진 횟수}"를 AddLastAction으로 넘김
///    (DefaultActionLabelProvider가 "인형에 다리를 붙여줬지" 같은 관찰 문장으로 바꿈)
/// 3) 손/인벤토리 제공(IPossessionProvider) — 들고 있는 부품 기준으로 HintEngine이 스텝 override
/// 4) 위치 제공(ILocationAwareProvider) — 씬의 번호 붙은 구역 오브젝트를 자동으로 찾아 거리 문장에 사용
/// 5) 스텝 진행도 제공(IStepProgressProvider) — 곰 인형 던진 횟수를 구간으로 판단해 "조금만 더" 같은 문장으로 넘김
///    (정답 횟수 5는 넘기지 않음 — 기획상 플레이어는 곰 인형 반응이 약해지는 걸 보고 계속 던지는 구조)
///    (Inspector에서 직접 넣으면 그걸 우선 사용, 비워두면 경로로 자동 탐색)
/// </summary>
public class DollHintBridge : MonoBehaviour, IPossessionProvider, ILocationAwareProvider, IStepProgressProvider
{
    // =========================================================
    // 힌트 시스템
    // =========================================================

    [Header("힌트 시스템")]
    [SerializeField] private HintManager hintManager;


    // =========================================================
    // 인형씬 상태 참조 (읽기 전용)
    // =========================================================

    [Header("인형씬 상태 참조 (읽기 전용)")]
    [SerializeField] private DollScene_GameManager gameManager;
    [SerializeField] private DollScene_ChangeDoll changeDoll;
    [SerializeField] private NumberLockManager numberLockManager;
    [SerializeField] private RetroGameManager retroGameManager;
    [SerializeField] private SpeakingBearDoll speakingBearDoll;


    // =========================================================
    // 손/인벤토리 참조 (IPossessionProvider 구현용, 읽기 전용)
    // =========================================================

    [Header("손/인벤토리 참조 (읽기 전용)")]
    [Tooltip("플레이어가 현재 손에 들고 있는 오브젝트를 실시간으로 스캔하는 컴포넌트")]
    [SerializeField] private PerceiveObjectHandPivot handPerception;

    [Tooltip("8슬롯 인벤토리")]
    [SerializeField] private InventoryData inventoryData;


    // =========================================================
    // 위치 참조 (ILocationAwareProvider 구현용) — 비워두면 Start()에서 경로로 자동 탐색
    // =========================================================

    [Header("위치 (비워두면 자동 탐색)")]
    [Tooltip("거리 계산 기준점. 비우면 Player_Move 오브젝트")]
    [SerializeField] private Transform player;
    [Tooltip("step 1 자물쇠 — 기본: 1. DollFencePart/NumberLock")]
    [SerializeField] private Transform lockArea;
    [Tooltip("step 2 다리 찾는 곳 — 기본: 1. DollFencePart/WoodenHorse")]
    [SerializeField] private Transform horseArea;
    [Tooltip("step 3 곰 인형 — 기본: 2. BrokenTeddyBear/TeddyBear")]
    [SerializeField] private Transform teddyBear;
    [Tooltip("step 4 오락기 — 기본: 3. RetroGame")]
    [SerializeField] private Transform retroGame;
    [Tooltip("step 6 기차 선로 — 기본: 4. TrainPart/Trainset")]
    [SerializeField] private Transform trainArea;
    [Tooltip("step 2/5/7/8 부품 장착하는 인형 — 기본: 5. MarioNet/DollStandHere")]
    [SerializeField] private Transform dollStand;

    const string LockAreaPath  = "1. DollFencePart/NumberLock";
    const string HorseAreaPath = "1. DollFencePart/WoodenHorse";
    const string TeddyBearPath = "2. BrokenTeddyBear/TeddyBear";
    const string RetroGamePath = "3. RetroGame";
    const string TrainAreaPath = "4. TrainPart/Trainset";
    const string DollStandPath = "5. MarioNet/DollStandHere";


    // =========================================================
    // 스텝 완료 여부 (id1~id8, 중복 보고 방지용)
    // DollRoomData.cs의 실제 8-step 번호에 맞춤
    // =========================================================

    bool step1Reported, step2Reported, step3Reported, step4Reported,
         step5Reported, step6Reported, step7Reported, step8Reported;

    int lastBearHitCount = 0;


    // =========================================================
    // Start
    // =========================================================

    void Start()
    {
        if (hintManager == null)
            hintManager = FindFirstObjectByType<HintManager>();

        if (gameManager == null)
            gameManager = FindFirstObjectByType<DollScene_GameManager>();

        if (changeDoll == null)
            changeDoll = FindFirstObjectByType<DollScene_ChangeDoll>();

        if (numberLockManager == null)
            numberLockManager = FindFirstObjectByType<NumberLockManager>();

        if (retroGameManager == null)
            retroGameManager = FindFirstObjectByType<RetroGameManager>();

        if (speakingBearDoll == null)
            speakingBearDoll = FindFirstObjectByType<SpeakingBearDoll>();

        if (handPerception == null)
            handPerception = FindFirstObjectByType<PerceiveObjectHandPivot>();

        if (inventoryData == null)
            inventoryData = FindFirstObjectByType<InventoryData>();

        if (player == null)
        {
            var move = FindFirstObjectByType<Player_Move>();
            if (move != null) player = move.transform;
        }

        lockArea  = lockArea  != null ? lockArea  : FindByPath(LockAreaPath);
        horseArea = horseArea != null ? horseArea : FindByPath(HorseAreaPath);
        teddyBear = teddyBear != null ? teddyBear : FindByPath(TeddyBearPath);
        retroGame = retroGame != null ? retroGame : FindByPath(RetroGamePath);
        trainArea = trainArea != null ? trainArea : FindByPath(TrainAreaPath);
        dollStand = dollStand != null ? dollStand : FindByPath(DollStandPath);

        if (speakingBearDoll != null)
            lastBearHitCount = speakingBearDoll.HitCount;


        if (hintManager == null)
        {
            Debug.LogError("[DollHintBridge] HintManager를 찾을 수 없습니다.");
            return;
        }

        // 힌트 매니저에게 이 브릿지가 손/인벤토리 + 위치 정보 제공자라고 등록
        // (해석: HintManager는 인터페이스를 통해서만 알 수 있고,
        //  DollHintBridge/혜교님 클래스에 직접 접근하지 않음 — PuzzleDataProvider와 같은 패턴)
        hintManager.PossessionProvider = this;
        hintManager.LocationProvider = this;
        hintManager.ProgressProvider = this;
    }

    static Transform FindByPath(string path)
    {
        var go = GameObject.Find(path);
        if (go == null)
        {
            Debug.LogWarning($"[DollHintBridge] 위치 오브젝트를 못 찾음: \"{path}\" — 이 구역 거리 문장만 생략됩니다. (이름이 바뀌었으면 Inspector에 직접 연결)");
            return null;
        }
        return go.transform;
    }


    // =========================================================
    // Update — 매 프레임 상태 폴링
    // =========================================================

    void Update()
    {
        if (hintManager == null) return;

        // id1 : 마트료시카 순서로 자물쇠 번호를 알아내 목마 구역을 연다
        if (!step1Reported && numberLockManager != null && numberLockManager.IsSolved)
        {
            step1Reported = true;
            ReportStep(1, "clue_leg_lock_code");
        }

        // id2 : 목마 구역에서 부러진 다리를 찾아 교체한다
        if (!step2Reported && changeDoll != null && changeDoll.IsBrokenLegFixed)
        {
            step2Reported = true;
            ReportStep(2, "clue_doll_leg");
        }

        // id3 : 말하는 곰 인형을 반복해서 던져 동전을 얻는다
        // (해석: TeddyBear_Grab.wasHeld는 private이라 상호작용 자체의 별도 신호는 없어서,
        //  실질적 결과물인 동전 획득(IsCoinFound)으로 판정)
        if (!step3Reported && gameManager != null && gameManager.IsCoinFound)
        {
            step3Reported = true;
            ReportStep(3, "clue_coin");
        }

        // id4 : 오락기에 동전을 넣고 게임에서 승리해 팔을 얻는다
        if (!step4Reported && retroGameManager != null && retroGameManager.IsArmRewardUnlocked)
        {
            step4Reported = true;
            ReportStep(4, "clue_doll_arm");
        }

        // id5 : 획득한 팔을 인형에 교체한다
        if (!step5Reported && changeDoll != null && changeDoll.IsBrokenArmFixed)
        {
            step5Reported = true;
            ReportStep(5, null);
        }

        // id6 : 버튼으로 선로를 맞춰 검은 기차를 밀어내고 태엽을 얻는다
        // (주의: changeDoll.IsSpringFixed(= gameManager.IsSpringFound)는 현재 씬에서 태엽을 "인형에 장착"할 때
        //  켜짐 — SpringPickupState.CollectSpring()을 부르는 곳이 없어서 획득 시점엔 안 켜짐.
        //  그래서 "태엽을 손/인벤토리에 들고 있음"을 획득 신호로 쓰고, 장착 완료도 획득한 걸로 같이 인정)
        if (!step6Reported && (IsHolding("Spring") || (changeDoll != null && changeDoll.IsSpringFixed)))
        {
            step6Reported = true;
            ReportStep(6, "clue_spring");
        }

        // id7 : 얻은 태엽을 인형에 장착한다
        if (!step7Reported && step6Reported && changeDoll != null && changeDoll.IsSpringFixed)
        {
            step7Reported = true;
            ReportStep(7, null);
        }

        // id8 : 완성된 인형의 태엽을 돌려 수리를 완료하고 열쇠를 얻어 탈출한다
        // (해석: 다리+팔+태엽이 전부 붙어야만 true가 되도록 게임 로직 자체가
        //  DollRepairCompletionManager에서 가드하고 있어서, 별도 순서 체크 없이
        //  이 프로퍼티 하나만 봐도 안전함)
        if (!step8Reported && changeDoll != null && changeDoll.IsDollRepairCompleted)
        {
            step8Reported = true;
            ReportStep(8, null);
        }

        // 곰인형 HitCount(던진 횟수) 동적 반영
        SyncTeddyBearHitCount();
    }


    // =========================================================
    // 곰인형 HitCount → repeatedInspections 동기화 + 던질 때마다 최근 행동 기록
    // =========================================================

    private void SyncTeddyBearHitCount()
    {
        if (speakingBearDoll == null) return;

        int hitCount = speakingBearDoll.HitCount;

        var list = hintManager.currentPlayerState.repeatedInspections;
        var entry = list.Find(r => r.objectName == "TeddyBear");

        if (entry == null)
        {
            entry = new RepeatedInspection { objectName = "TeddyBear", count = 0 };
            list.Add(entry);
        }

        entry.count = hitCount;

        // 던진 횟수를 action id에 그대로 실어 보냄 → DefaultActionLabelProvider가 "곰 인형을 벌써 2번 던졌지"처럼 숫자를 문장에 반영
        if (hitCount > lastBearHitCount)
            hintManager.AddLastAction($"doll_bear_throw_{hitCount}");

        lastBearHitCount = hitCount;
    }


    // =========================================================
    // IPossessionProvider 구현
    // =========================================================

    /// <summary>
    /// 플레이어가 현재 손에 들고 있는 오브젝트의 objectName. 없으면 null.
    /// </summary>
    public string GetHandObjectName()
    {
        if (handPerception == null || handPerception.CurrentObject == null)
            return null;

        return ResolveObjectName(handPerception.CurrentObject);
    }

    /// <summary>
    /// 인벤토리 8슬롯에 들어있는 오브젝트들의 objectName 목록.
    /// InventoryData.GetObjectAt(index)로 실제 Object_Grabbable을 받아서
    /// 그 objectName 필드를 그대로 사용함
    /// (해석: SlotEntry.ObjectName은 표시용 InventoryObjectName을 쓰기 때문에
    ///  relatedObjectName 매칭 기준인 Object_Grabbable.objectName과 다를 수 있어서
    ///  일부러 슬롯의 표시 이름이 아니라 원본 오브젝트에서 직접 읽음)
    /// </summary>
    public List<string> GetInventoryObjectNames()
    {
        var names = new List<string>();

        if (inventoryData == null)
            return names;

        for (int i = 0; i < inventoryData.SlotCount; i++)
        {
            Object_Grabbable obj = inventoryData.GetObjectAt(i);

            if (obj != null)
                names.Add(ResolveObjectName(obj));
        }

        // 곰 인형은 "던지는" 퍼즐이라 던지는 순간 손을 떠남 → 그대로 두면 던지는 중인데도
        // 손/인벤토리 override가 풀려서 체크리스트 순서(예: 아직 안 푼 step 1 자물쇠)로 힌트가 돌아감.
        // 한 번이라도 던졌고 아직 동전을 못 얻었으면 "곰 인형 퍼즐 진행 중"으로 보고 인벤토리에 있는 것처럼 취급
        // (HintEngine 2순위 inventory-override로 step 3 유지 — Core 수정 없이 브릿지에서만 처리)
        if (speakingBearDoll != null && speakingBearDoll.HitCount > 0 && !step3Reported && !names.Contains("TeddyBear"))
            names.Add("TeddyBear");

        return names;
    }

    /// <summary>
    /// Object_Grabbable.objectName을 그대로 쓰되, 씬에서 objectName이 비어있는 곰 인형만 보정.
    /// (DollSceneUpdate의 TeddyBear 오브젝트는 Object_Grabbable.objectName이 빈 문자열이라
    ///  그대로 두면 손에 들어도 HintEngine이 "아무것도 안 들고 있음"으로 판단함 —
    ///  혜교님 씬 값을 건드리지 않으려고 SpeakingBearDoll 컴포넌트 유무로 식별해서 "TeddyBear"로 치환)
    /// </summary>
    private static string ResolveObjectName(Object_Grabbable obj)
    {
        if (!string.IsNullOrEmpty(obj.objectName))
            return obj.objectName;

        if (obj.GetComponent<SpeakingBearDoll>() != null)
            return "TeddyBear";

        return obj.objectName;
    }

    private bool IsHolding(string objectName)
    {
        if (GetHandObjectName() == objectName)
            return true;

        return GetInventoryObjectNames().Contains(objectName);
    }


    // =========================================================
    // ILocationAwareProvider 구현
    // =========================================================

    public Vector3? GetPlayerPosition()
        => player != null ? player.position : (Vector3?)null;

    public Vector3? GetObjectPosition(string objectName)
    {
        var t = ResolveTarget(objectName);
        return t != null ? t.position : (Vector3?)null;
    }

    /// <summary>
    /// DollRoomData의 relatedObjectName → "플레이어가 가야 할 곳" Transform.
    /// 부품(다리/팔/태엽)은 아직 안 주웠으면 그 부품이 있는 구역, 이미 들고 있으면 장착할 인형 쪽을 가리킴
    /// (들고 있는 물건과의 거리는 항상 0이라 의미가 없어서).
    /// </summary>
    Transform ResolveTarget(string key)
    {
        switch (key)
        {
            case "LockArea":  return lockArea;
            case "DollLeg":   return IsHolding("DollLeg") ? dollStand : horseArea;
            case "TeddyBear": return teddyBear;
            case "동전":       return retroGame;
            case "DollArm":   return IsHolding("DollArm") ? dollStand : retroGame;
            case "TrainArea": return trainArea;
            case "Spring":    return dollStand; // step 7은 태엽을 이미 얻은 뒤라 항상 인형 쪽
            case "DollStand": return dollStand;
            default:          return null;
        }
    }

    // 구역 시스템은 아직 없음 (와인/오르간씬과 동일) — null/0이면 HintEngine이 구역 문장만 생략함
    public string GetCurrentZoneName() => null;
    public float GetCurrentZoneStaySeconds() => 0f;


    // =========================================================
    // IStepProgressProvider 구현
    // =========================================================

    // 곰 인형이 찢어지는 횟수는 SpeakingBearDoll 안에 5로 하드코딩돼 있음 (hitCount >= 5).
    // 여기선 "거의 다 왔다" 판단 기준으로만 씀 — 바뀌면 이 값만 맞춰주면 됨.
    const int BearAlmostThreshold = 3;

    /// <summary>
    /// step 3(곰 인형 던지기)일 때만 던진 횟수를 구간으로 판단해서 문장을 돌려줌.
    /// 1~2번: 반응이 있으니 계속 / 3번 이상: 거의 한계, 조금만 더. 0번이거나 다른 스텝이면 null.
    /// (이 문장이 step 3의 hintByLevel 문구를 대신하므로, "계속 던져보세요"처럼 할 행동까지 포함해야 함)
    /// </summary>
    public string GetProgressNote(PuzzleStep step)
    {
        if (step == null || step.id != 3 || speakingBearDoll == null)
            return null;

        int hitCount = speakingBearDoll.HitCount;

        if (hitCount <= 0)
            return null;

        if (hitCount < BearAlmostThreshold)
            return "곰 인형이 던질 때마다 반응하고 있어요. 멈추지 말고 계속 던져보세요.";

        return "곰 인형이 이제 거의 버티지 못하는 것 같아요. 조금만 더 던져보세요.";
    }


    // =========================================================
    // 스텝 완료 보고
    // =========================================================

    void ReportStep(int stepId, string clueId)
    {
        var state = hintManager.currentPlayerState;

        if (clueId != null && !state.foundClues.Contains(clueId))
            state.foundClues.Add(clueId);

        if (!state.completedSteps.Contains(stepId))
            state.completedSteps.Add(stepId);

        hintManager.AddLastAction($"doll_step_{stepId:D2}");

        Debug.Log($"[DollHintBridge] step {stepId} 완료 기록 → 현재: [{string.Join(", ", state.completedSteps)}]");
    }
}