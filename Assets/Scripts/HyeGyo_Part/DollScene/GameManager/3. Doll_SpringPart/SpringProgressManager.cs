using UnityEngine;


public class SpringProgressManager : MonoBehaviour
{
    // =========================================================
    // Main Manager
    // =========================================================

    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    // =========================================================
    // Spring Puzzle
    // =========================================================

    [Header("선로")]
    [SerializeField]
    private SpringRailStateManager railStateManager;


    [Header("기차 Sequence")]
    [SerializeField]
    private SpringTrainSequenceManager trainSequenceManager;


    [Header("태엽")]
    [SerializeField]
    private SpringPickupState springPickupState;


    // =========================================================
    // 상태
    // =========================================================

    private bool isResultSent =
        false;


    // =========================================================
    // Event 연결
    // =========================================================

    private void OnEnable()
    {
        if (railStateManager != null)
        {
            railStateManager.OnRailStateChanged +=
                HandleRailStateChanged;
        }


        if (trainSequenceManager != null)
        {
            trainSequenceManager.OnSequenceCompleted +=
                CheckProgress;
        }


        if (springPickupState != null)
        {
            springPickupState.OnSpringCollected +=
                CheckProgress;
        }
    }


    private void OnDisable()
    {
        if (railStateManager != null)
        {
            railStateManager.OnRailStateChanged -=
                HandleRailStateChanged;
        }


        if (trainSequenceManager != null)
        {
            trainSequenceManager.OnSequenceCompleted -=
                CheckProgress;
        }


        if (springPickupState != null)
        {
            springPickupState.OnSpringCollected -=
                CheckProgress;
        }
    }


    private void Start()
    {
        if (gameManager != null &&
            gameManager.IsSpringFound)
        {
            isResultSent =
                true;

            return;
        }


        CheckProgress();
    }


    // =========================================================
    // Rail Event용
    // =========================================================

    private void HandleRailStateChanged(
        bool isCorrect
    )
    {
        CheckProgress();
    }


    // =========================================================
    // 전체 진행 상태 확인
    // =========================================================

    private void CheckProgress()
    {
        if (isResultSent)
        {
            return;
        }


        if (gameManager == null)
        {
            return;
        }


        if (railStateManager == null)
        {
            return;
        }


        if (trainSequenceManager == null)
        {
            return;
        }


        if (springPickupState == null)
        {
            return;
        }


        // 선로 정답
        if (!railStateManager.IsAllRailsCorrect)
        {
            return;
        }


        // 기차 이동 완료
        if (!trainSequenceManager.IsSequenceComplete)
        {
            return;
        }


        // 태엽 실제 획득
        if (!springPickupState.IsSpringCollected)
        {
            return;
        }


        // =====================================================
        // 최종 완료
        // =====================================================

        isResultSent =
            true;


        Debug.Log(
            "[SpringProgress] 태엽 파트 최종 완료"
        );


        gameManager.CompleteFindSpring();
    }
}