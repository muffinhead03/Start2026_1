using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인형씬 게임 상태(DollScene_GameManager / DollScene_ChangeDoll 등)를 읽기 전용으로 폴링해서
/// 힌트 시스템(HintManager)에 completedSteps / foundClues를 기록하고,
/// IPossessionProvider를 구현해서 손/인벤토리 정보도 함께 제공하는 브릿지.
/// 혜교님 파트 스크립트는 수정하지 않고 public 프로퍼티만 참조함.
/// (해석: 씬 소유자의 스크립트를 직접 건드리지 않고, 밖에서 상태만 읽어와
///  힌트 매니저에게 "지금 어디까지 진행됐는지" / "지금 뭘 들고 있는지"를 알려주는 역할)
/// </summary>
public class DollHintBridge : MonoBehaviour, IPossessionProvider
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
    // 스텝 완료 여부 (id1~id8, 중복 보고 방지용)
    // DollRoomData.cs의 실제 8-step 번호에 맞춤
    // =========================================================

    bool step1Reported, step2Reported, step3Reported, step4Reported,
         step5Reported, step6Reported, step7Reported, step8Reported;


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


        if (hintManager == null)
        {
            Debug.LogError("[DollHintBridge] HintManager를 찾을 수 없습니다.");
            return;
        }

        // 힌트 매니저에게 이 브릿지가 손/인벤토리 정보 제공자라고 등록
        // (해석: HintManager는 이 인터페이스를 통해서만 손/인벤토리를 알 수 있고,
        //  DollHintBridge/혜교님 클래스에 직접 접근하지 않음 — PuzzleDataProvider와 같은 패턴)
        hintManager.PossessionProvider = this;
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
        if (!step6Reported && changeDoll != null && changeDoll.IsSpringFixed)
        {
            step6Reported = true;
            ReportStep(6, "clue_spring");
        }

        // id7 : 얻은 태엽을 인형에 장착한다
        // (해석: gameManager에는 "획득"과 "장착"을 구분하는 플래그가 따로 없어서,
        //  "획득은 됐는데(IsSpringFixed) 더 이상 손/인벤토리에 태엽이 없다"
        //  = 실제로 인형에 장착해서 소모된 것으로 판단함)
        if (!step7Reported && step6Reported && !IsHoldingSpring())
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
    // 태엽 소지 여부 확인 (id7 판정용)
    // =========================================================

    private bool IsHoldingSpring()
    {
        string handObjectName = GetHandObjectName();
        if (handObjectName == "Spring")
            return true;

        List<string> inventoryObjectNames = GetInventoryObjectNames();
        return inventoryObjectNames.Contains("Spring");
    }


    // =========================================================
    // 곰인형 HitCount → repeatedInspections 동기화
    // =========================================================

    private void SyncTeddyBearHitCount()
    {
        if (speakingBearDoll == null) return;

        var list = hintManager.currentPlayerState.repeatedInspections;
        var entry = list.Find(r => r.objectName == "TeddyBear");

        if (entry == null)
        {
            entry = new RepeatedInspection { objectName = "TeddyBear", count = 0 };
            list.Add(entry);
        }

        entry.count = speakingBearDoll.HitCount;
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

        return handPerception.CurrentObject.objectName;
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
                names.Add(obj.objectName);
        }

        return names;
    }


    // =========================================================
    // 스텝 완료 보고
    // =========================================================

    void ReportStep(int stepId, string clueId)
    {
        if (clueId != null)
            hintManager.currentPlayerState.foundClues.Add(clueId);

        if (!hintManager.currentPlayerState.completedSteps.Contains(stepId))
            hintManager.currentPlayerState.completedSteps.Add(stepId);

        Debug.Log($"[DollHintBridge] step {stepId} 완료 기록");
    }
}