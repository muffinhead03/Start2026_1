using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인형씬 게임 상태(DollScene_GameManager / DollScene_ChangeDoll 등)를 읽기 전용으로 폴링해서
/// 힌트 시스템(HintManager)에 연결하는 브릿지. 혜교님 파트 스크립트는 수정하지 않고 public 프로퍼티만 참조함.
///
/// 2026-11: 인형씬 기획 변경으로 9단계 (1 그림책 봄 → 2 자물쇠 → 3 다리 → 4 곰 인형 → 5 오락기 → 6 팔 → 7 기차 → 8 태엽 → 9 탈출).
///  1단계는 폴링할 상태가 없어서 씬 이벤트로 받음: Book_Matryo(Book_Inspectable).OnInspectPerformed → ReportBookViewed()
///  스텝 번호는 아래 Step* 상수로만 씀 — 다시 바뀌면 상수만 고치면 됨.
///
/// 역할 5가지:
/// 1) 진행 기록 — completedSteps / foundClues
/// 2) 최근 행동 — 스텝 완료 시 "doll_step_NN", 곰 인형 던질 때 "doll_bear_throw_{던진 횟수}"를 AddLastAction으로 넘김
///    (DefaultActionLabelProvider가 "인형에 다리를 붙여줬지" 같은 관찰 문장으로 바꿈)
/// 3) 손/인벤토리 제공(IPossessionProvider) — 들고 있는 부품 기준으로 HintEngine이 스텝 override
/// 4) 위치 제공(ILocationAwareProvider) — 씬의 번호 붙은 구역 오브젝트를 자동으로 찾아 거리 문장에 사용
/// 5) 스텝 진행도 제공(IStepProgressProvider) — 곰 인형 던진 횟수 / 인형 수리 후 상태를 구간으로 판단해
///    (구간 × 힌트 레벨)별 문장 풀에서 랜덤으로 골라 넘김 → 같은 상태에서 다시 물어도 표현이 달라짐
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
    [Tooltip("step 1 마트료시카 그림책 — 기본: 1. DollFencePart/Book_Matryo")]
    [SerializeField] private Transform matryoBook;
    [Tooltip("step 2 자물쇠 — 기본: 1. DollFencePart/Fence/NumberLock")]
    [SerializeField] private Transform lockArea;
    [Tooltip("step 3 다리 찾는 곳 — 기본: 1. DollFencePart/WoodenHorse")]
    [SerializeField] private Transform horseArea;
    [Tooltip("step 4 곰 인형 — 기본: 2. BrokenTeddyBear/TeddyBear")]
    [SerializeField] private Transform teddyBear;
    [Tooltip("step 5 오락기 — 기본: 3. RetroGame")]
    [SerializeField] private Transform retroGame;
    [Tooltip("step 7 기차 선로 — 기본: 4. TrainPart/Trainset")]
    [SerializeField] private Transform trainArea;
    [Tooltip("step 3/6/8/9 부품 장착하는 인형 — 기본: 5. MarioNet/DollStandHere")]
    [SerializeField] private Transform dollStand;

    const string MatryoBookPath = "1. DollFencePart/Book_Matryo";
    const string LockAreaPath  = "1. DollFencePart/Fence/NumberLock";
    const string HorseAreaPath = "1. DollFencePart/WoodenHorse";
    const string TeddyBearPath = "2. BrokenTeddyBear/TeddyBear";
    const string RetroGamePath = "3. RetroGame";
    const string TrainAreaPath = "4. TrainPart/Trainset";
    const string DollStandPath = "5. MarioNet/DollStandHere";


    // =========================================================
    // 스텝 번호 (DollRoomData 9단계와 1:1)
    // =========================================================

    const int StepBook   = 1; // 그림책 봄
    const int StepLock   = 2; // 자물쇠 열기
    const int StepLeg    = 3; // 다리 교체
    const int StepBear   = 4; // 곰 인형 → 동전
    const int StepArcade = 5; // 오락기 → 팔
    const int StepArm    = 6; // 팔 교체
    const int StepTrain  = 7; // 선로 → 태엽
    const int StepSpring = 8; // 태엽 장착
    const int StepEscape = 9; // 태엽 돌림 → 열쇠 → 탈출

    // 스텝 완료 여부 (중복 보고 방지용)
    bool bookReported, lockReported, legReported, bearReported,
         arcadeReported, armReported, trainReported, springReported, escapeReported;

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

        matryoBook = matryoBook != null ? matryoBook : FindByPath(MatryoBookPath);
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

        // id1 : 그림책을 보고 마트료시카의 무게를 알아낸다
        // → 상태가 없어서 ReportBookViewed()(씬 이벤트)에서 처리. 책을 안 보고 자물쇠를 먼저 풀었으면 아래에서 같이 완료(back-fill)

        // id2 : 책을 보고난 후 자물쇠 번호를 알아내 목마 구역을 연다
        if (!lockReported && numberLockManager != null && numberLockManager.IsSolved)
        {
            lockReported = true;
            CompleteBook("자물쇠 열림 (back-fill)");
            ReportStep(StepLock, "clue_leg_lock_code");
        }

        // id3 : 목마 구역에서 부러진 다리를 찾아 교체한다
        if (!legReported && changeDoll != null && changeDoll.IsBrokenLegFixed)
        {
            legReported = true;
            ReportStep(StepLeg, "clue_doll_leg");
        }

        // id4 : 말하는 곰 인형을 반복해서 던져 동전을 얻는다
        // (해석: TeddyBear_Grab.wasHeld는 private이라 상호작용 자체의 별도 신호는 없어서,
        //  실질적 결과물인 동전 획득으로 판정)
        // IsCoinFound를 켜는 SpeakingBearDoll.CollectCoin()을 코드에서 부르는 곳이 없음(인스펙터 이벤트로만 연결됐을 수 있음).
        // 안 켜지면 곰 인형 단계가 영원히 미완료로 남아서, 동전을 주운 뒤에도 곰 인형 힌트가 계속 나옴 (테스트 표 D10, D12).
        // 그래서 동전을 들고 있거나 / 오락기에 넣었거나 / 오락기 보상을 받았으면 완료로 같이 인정
        if (!bearReported && IsCoinObtained())
        {
            bearReported = true;
            ReportStep(StepBear, "clue_coin");
        }

        // id5 : 오락기에 동전을 넣고 화면 연출이 끝나면 나오는 팔을 얻는다
        if (!arcadeReported && retroGameManager != null && retroGameManager.IsArmRewardUnlocked)
        {
            arcadeReported = true;
            ReportStep(StepArcade, "clue_doll_arm");
        }

        // id6 : 획득한 팔을 인형에 교체한다
        if (!armReported && changeDoll != null && changeDoll.IsBrokenArmFixed)
        {
            armReported = true;
            ReportStep(StepArm, null);
        }

        // id7 : 버튼으로 선로를 맞춰 검은 기차를 밀어내고 태엽을 얻는다
        // (주의: changeDoll.IsSpringFixed(= gameManager.IsSpringFound)는 현재 씬에서 태엽을 "인형에 장착"할 때
        //  켜짐 — SpringPickupState.CollectSpring()을 부르는 곳이 없어서 획득 시점엔 안 켜짐.
        //  그래서 "태엽을 손/인벤토리에 들고 있음"을 획득 신호로 쓰고, 장착 완료도 획득한 걸로 같이 인정)
        if (!trainReported && (IsHolding("Spring") || (changeDoll != null && changeDoll.IsSpringFixed)))
        {
            trainReported = true;
            ReportStep(StepTrain, "clue_spring");
        }

        // id8 : 얻은 태엽을 인형에 장착한다
        if (!springReported && trainReported && changeDoll != null && changeDoll.IsSpringFixed)
        {
            springReported = true;
            ReportStep(StepSpring, null);
        }

        // id9 : 완성된 인형의 태엽을 돌려 수리를 완료하고 열쇠를 얻어 탈출한다
        // 예전엔 수리 완료(IsDollRepairCompleted) 순간 완료로 쳤는데, 태엽을 붙이면 수리가 자동으로 돼서
        // 태엽 장착과 탈출 단계가 동시에 끝나버림 → 열쇠를 줍기 전에 힌트를 누르면 "이미 모든 단서를 찾았어"가 나옴.
        // 그래서 탈출(IsStageCleared) 시점에 완료로 바꾸고, 그 전엔 GetProgressNote가
        // "인형 이동 중 / 열쇠 떨어짐" 진행 구간에 맞는 문장을 넘김
        if (!escapeReported && gameManager != null && gameManager.IsStageCleared)
        {
            escapeReported = true;
            ReportStep(StepEscape, null);
        }

        // 곰인형 HitCount(던진 횟수) 동적 반영
        SyncTeddyBearHitCount();
    }


    // =========================================================
    // 씬 이벤트에 연결하는 메서드 (UnityEvent)
    // =========================================================

    /// <summary>
    /// 1. DollFencePart/Book_Matryo의 Book_Inspectable.OnInspectPerformed에 연결 (책을 펼쳐 본 순간).
    /// 여러 번 펼쳐도 완료 기록은 한 번만, 관찰 문장(최근 행동)은 처음 한 번만 남김.
    /// </summary>
    public void ReportBookViewed()
    {
        if (hintManager == null) return;
        CompleteBook("그림책 조사");
    }

    void CompleteBook(string why)
    {
        if (bookReported) return;
        bookReported = true;
        ReportStep(StepBook, "clue_matryoshka_book");
        Debug.Log($"[DollHintBridge] 그림책 단계 완료 ({why})");
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
        // 손/인벤토리 override가 풀려서 체크리스트 순서(예: 아직 안 푼 자물쇠)로 힌트가 돌아감.
        // 한 번이라도 던졌고 아직 동전을 못 얻었으면 "곰 인형 퍼즐 진행 중"으로 보고 인벤토리에 있는 것처럼 취급
        // (HintEngine 2순위 inventory-override로 곰 인형 단계 유지 — Core 수정 없이 브릿지에서만 처리)
        if (speakingBearDoll != null && speakingBearDoll.HitCount > 0 && !bearReported && !names.Contains("TeddyBear"))
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

    // DollRoomData step 5(오락기)의 relatedObjectName — 곰 인형에서 나온 동전의 Object_Grabbable.objectName
    const string CoinObjectName = "동전";

    private bool IsCoinObtained()
    {
        if (gameManager != null && gameManager.IsCoinFound) return true;
        if (retroGameManager != null && (retroGameManager.IsCoinInserted || retroGameManager.IsArmRewardUnlocked)) return true;
        return IsHolding(CoinObjectName);
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
            case "MatryoBook": return matryoBook;
            case "LockArea":  return lockArea;
            case "DollLeg":   return IsHolding("DollLeg") ? dollStand : horseArea;
            case "TeddyBear": return teddyBear;
            case "동전":       return retroGame;
            case "DollArm":   return IsHolding("DollArm") ? dollStand : retroGame;
            case "TrainArea": return trainArea;
            case "Spring":    return dollStand; // 태엽 장착 단계는 태엽을 이미 얻은 뒤라 항상 인형 쪽
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

    // [레벨 단계] 키 — 레벨 1~2(모호) / 3(분명) / 4~5(거의 정답).
    // 키마다 후보 문장이 여러 개라 랜덤으로 골라서, 같은 상태·같은 레벨로 다시 물어도 방향 문장 자체가 조금씩 달라짐
    // (LLM은 방향 문장을 거의 그대로 옮기는 경향이 있어서, 베끼는 걸 막는 대신 베낄 재료를 다양하게 줌).
    // 숫자(던진 횟수, 정답 횟수)는 어떤 문장에도 넣지 않음 — 정답 누설 방지.
    // 문장(한·영·일)은 HintLanguageData.DollScene.cs의 HintDoll_Note{구간}_T0~T2.

    static readonly string[] BearEarlyNotes =
    {
        HintLanguageDataKey.HintDoll_NoteBearEarly_T0,
        HintLanguageDataKey.HintDoll_NoteBearEarly_T1,
        HintLanguageDataKey.HintDoll_NoteBearEarly_T2,
    };

    static readonly string[] BearAlmostNotes =
    {
        HintLanguageDataKey.HintDoll_NoteBearAlmost_T0,
        HintLanguageDataKey.HintDoll_NoteBearAlmost_T1,
        HintLanguageDataKey.HintDoll_NoteBearAlmost_T2,
    };

    // 곰 인형이 찢어졌는데 아직 동전을 안 주운 상태 — 동전이 이미 눈앞에 보이는 상태라 모호하게 말하면 쓸모없음.
    // 그래서 최소 레벨 3(GetMinHintLevel)으로 올리고, 모든 문장에 "동전"을 넣음.
    // (T0은 레벨 1~2용인데 최소 레벨 때문에 실제로는 안 쓰임 — 모양을 다른 구간과 맞추려고 둠)
    static readonly string[] BearBrokenNotes =
    {
        HintLanguageDataKey.HintDoll_NoteBearBroken_T0,
        HintLanguageDataKey.HintDoll_NoteBearBroken_T1,
        HintLanguageDataKey.HintDoll_NoteBearBroken_T2,
    };

    // 탈출 단계 — 수리가 끝나고 인형이 움직이는 중 (레벨 상관없이 "지켜봐", 표현만 랜덤)
    static readonly string[] DollMovingNotes =
    {
        HintLanguageDataKey.HintDoll_NoteDollMoving_T0,
    };

    // 탈출 단계 — 열쇠가 떨어진 뒤 (곰 인형 찢어진 뒤와 같은 이유로 최소 레벨 3)
    static readonly string[] KeyDroppedNotes =
    {
        HintLanguageDataKey.HintDoll_NoteKeyDropped_T0,
        HintLanguageDataKey.HintDoll_NoteKeyDropped_T1,
        HintLanguageDataKey.HintDoll_NoteKeyDropped_T2,
    };

    // 레벨에 맞는 단계 키 → 현재 언어 후보 중 하나를 랜덤으로 (WineHintBridge와 같은 방식)
    static string PickByLevel(string[] tierKeys, int hintLevel)
    {
        int tier = hintLevel <= 2 ? 0 : (hintLevel == 3 ? 1 : 2);
        tier = Mathf.Clamp(tier, 0, tierKeys.Length - 1);
        var pool = HintLanguageData.GetCandidates(tierKeys[tier]);
        if (pool == null || pool.Length == 0) return null; // 번역 키가 빠졌으면 hintByLevel 문구로 대체됨
        return pool[Random.Range(0, pool.Length)];
    }

    /// <summary>
    /// 진행 구간 이름. 같은 스텝 재요청 때 HintManager가 "지난 힌트 이후 진행이 있었는지" 보는 데 씀
    /// (구간이 바뀌었으면 잘 가고 있는 거라 레벨을 올리지 않음).
    /// </summary>
    public string GetProgressBand(PuzzleStep step)
    {
        if (step == null) return null;

        if (step.id == StepBear && speakingBearDoll != null)
        {
            if (speakingBearDoll.IsBroken) return "broken";
            int hit = speakingBearDoll.HitCount;
            if (hit <= 0) return null;
            return hit < BearAlmostThreshold ? "early" : "almost";
        }

        if (step.id == StepEscape && gameManager != null)
        {
            if (gameManager.IsExitKeyDropped) return "key-dropped";
            if (gameManager.IsDollRepaired)   return "doll-moving";
        }

        return null;
    }

    /// <summary>
    /// 답이 이미 눈앞에 보이는 구간(곰 인형이 찢어져 동전이 나옴 / 열쇠가 떨어짐)은 최소 레벨 3.
    /// 레벨 1~2면 LLM이 "주변을 살펴보세요"처럼 물건 이름을 빼고 말해서 쓸모없는 힌트가 됨 (베타 로그 확인).
    /// </summary>
    public int GetMinHintLevel(PuzzleStep step)
    {
        switch (GetProgressBand(step))
        {
            case "broken":
            case "key-dropped":
                return 3;
            default:
                return 1;
        }
    }

    /// <summary>
    /// 곰 인형 던지기 / 탈출(수리 완료 → 열쇠) 단계일 때만, 현재 진행 구간 + 힌트 레벨에 맞는 문장을 돌려줌.
    /// 해당 없으면 null → hintByLevel 문구가 그대로 쓰임.
    /// (이 문장이 hintByLevel 문구를 대신하므로, "계속 던져보세요"처럼 할 행동까지 포함해야 함)
    /// </summary>
    public string GetProgressNote(PuzzleStep step, int hintLevel)
    {
        switch (GetProgressBand(step))
        {
            case "early":       return PickByLevel(BearEarlyNotes, hintLevel);
            case "almost":      return PickByLevel(BearAlmostNotes, hintLevel);
            case "broken":      return PickByLevel(BearBrokenNotes, hintLevel);
            case "doll-moving": return PickByLevel(DollMovingNotes, hintLevel);
            case "key-dropped": return PickByLevel(KeyDroppedNotes, hintLevel);
            default:            return null;
        }
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