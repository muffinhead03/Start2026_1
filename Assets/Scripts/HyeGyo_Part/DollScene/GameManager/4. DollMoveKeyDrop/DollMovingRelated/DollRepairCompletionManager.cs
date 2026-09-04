using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DollRepairCompletionManager : MonoBehaviour
{
    // =========================================================
    // Doll State
    // =========================================================

    [Header("Doll State")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


    // =========================================================
    // Doll Motion
    // =========================================================

    [Header("Doll Motion")]

    [Tooltip("인형 수리 완료 시 실행할 이동 모션")]
    [SerializeField]
    private UnityEvent onRepairMotionStart;

    [Tooltip("인형 이동 모션 시간")]
    [SerializeField]
    private float repairMotionDuration = 2f;


    // =========================================================
    // Key Drop
    // =========================================================

    [Header("Key Drop")]
    [SerializeField]
    private DollFixedCheckManager fixedCheckManager;


    // =========================================================
    // State
    // =========================================================

    private bool sequenceStarted = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (changeDoll == null)
        {
            changeDoll =
                FindFirstObjectByType<DollScene_ChangeDoll>();
        }

        if (fixedCheckManager == null)
        {
            fixedCheckManager =
                FindFirstObjectByType<DollFixedCheckManager>();
        }


        if (changeDoll == null)
        {
            Debug.LogError(
                "[DollRepairCompletion] DollScene_ChangeDoll을 찾을 수 없습니다.",
                this
            );
        }

        if (fixedCheckManager == null)
        {
            Debug.LogError(
                "[DollRepairCompletion] DollFixedCheckManager를 찾을 수 없습니다.",
                this
            );
        }
    }


    // =========================================================
    // 전체 수리 체크
    // =========================================================

    public void CheckRepairCompleted()
    {
        if (sequenceStarted)
        {
            return;
        }

        if (changeDoll == null)
        {
            return;
        }


        bool allCompleted =
            changeDoll.IsBrokenLegFixed &&
            changeDoll.IsBrokenArmFixed &&
            changeDoll.IsSpringFixed;


        Debug.Log(
            $"[DollRepairCompletion] 전체 수리 상태 = {allCompleted}"
        );


        if (!allCompleted)
        {
            return;
        }


        sequenceStarted = true;


        Debug.Log(
            "[DollRepairCompletion] ★ 전체 수리 완료 → 수리 연출 시작 ★"
        );


        StartCoroutine(
            RepairSequence()
        );
    }


    // =========================================================
    // 전체 수리 연출
    //
    // 1. 수리 완료 상태 저장
    // 2. 인형 이동 모션
    // 3. 이동 완료까지 대기
    // 4. DollFixedCheckManager에 전달
    // 5. 그쪽에서 4초 후 Key Drop
    // =========================================================

    private IEnumerator RepairSequence()
    {
        // 수리 완료 상태 저장
        changeDoll.RepairDoll();


        // 인형 이동 모션 시작
        Debug.Log(
            "[DollRepairCompletion] 인형 이동 모션 시작"
        );

        onRepairMotionStart?.Invoke();


        // 이동 완료 대기
        yield return new WaitForSeconds(
            repairMotionDuration
        );


        Debug.Log(
            "[DollRepairCompletion] 인형 이동 모션 완료"
        );


        // 여기서부터 DollFixedCheckManager가
        // 추가로 4초 기다린 후 Key를 떨어뜨림
        if (fixedCheckManager != null)
        {
            fixedCheckManager.StartKeyDropAfterAnimation();
        }
    }
}