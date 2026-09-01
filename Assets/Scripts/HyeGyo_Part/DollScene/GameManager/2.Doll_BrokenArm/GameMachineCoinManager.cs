using UnityEngine;

public class GameMachineCoinManager : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    [Header("Retro Game Manager")]
    [SerializeField]
    private RetroGameManager retroGameManager;


    [Header("Reward")]
    [SerializeField]
    private GameObject brokenArmObject;


    [Header("State")]
    [SerializeField]
    private bool isCoinInserted = false;


    public bool IsCoinInserted => isCoinInserted;


    private void Start()
    {
        // ================================
        // GameManager 자동 탐색
        // ================================

        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }


        // ================================
        // RetroGameManager 자동 탐색
        // ================================

        if (retroGameManager == null)
        {
            retroGameManager =
                FindFirstObjectByType<RetroGameManager>();
        }


        // ================================
        // 팔 보상 초기 상태
        // ================================

        /*
         * 이미 보상이 나온 상태가 아니라면
         * 시작할 때 BrokenArm을 숨깁니다.
         */

        if (brokenArmObject != null)
        {
            bool alreadyUnlocked =
                retroGameManager != null &&
                retroGameManager.IsArmRewardUnlocked;

            if (!alreadyUnlocked)
            {
                brokenArmObject.SetActive(false);
            }
        }
    }


    // =============================================
    // 동전 투입구 E 상호작용
    // Event_On_Ray.OnClick에 연결
    // =============================================

    public void InteractCoinSlot()
    {
        Debug.Log(
            "[GameMachine] 동전 투입구 상호작용"
        );


        // ================================
        // GameManager 확인
        // ================================

        if (gameManager == null)
        {
            Debug.LogError(
                "[GameMachine] DollScene_GameManager가 없습니다."
            );

            return;
        }


        // ================================
        // 팔 보상이 이미 나온 경우
        // ================================

        if (retroGameManager != null &&
            retroGameManager.IsArmRewardUnlocked)
        {
            Debug.Log(
                "[GameMachine] 인형 팔 보상은 이미 등장했습니다."
            );

            return;
        }


        // ================================
        // 동전 확인
        // ================================

        if (!gameManager.IsCoinOwned)
        {
            Debug.Log(
                "[GameMachine] 동전이 필요합니다."
            );

            return;
        }


        InsertCoin();
    }


    // =============================================
    // 동전 투입
    // =============================================

    private void InsertCoin()
    {
        if (isCoinInserted)
        {
            Debug.Log(
                "[GameMachine] 이미 동전이 투입되어 있습니다."
            );

            return;
        }


        // 실제 동전 소비
        if (!gameManager.UseCoin())
        {
            Debug.Log(
                "[GameMachine] 동전 사용에 실패했습니다."
            );

            return;
        }


        isCoinInserted = true;


        Debug.Log(
            "[GameMachine] 동전을 투입했습니다."
        );


        StartGame();
    }


    // =============================================
    // 게임 시작
    // =============================================

    private void StartGame()
    {
        Debug.Log(
            "[GameMachine] 게임 시작"
        );


        /*
         * =============================================
         *
         *              철권게임파트
         *
         * =============================================
         *
         * TODO
         *
         * - 오락기 화면 활성화
         * - 플레이어 이동 제한
         * - 격투 게임 시작
         * - 입력 처리
         * - 적 AI
         * - 승리 / 패배 판정
         *
         */


        // =============================================
        // 임시 구현
        // =============================================
        //
        // 지금은 철권 게임이 없으므로
        // 동전을 넣으면 바로 승리 처리
        //

        TemporaryGameWin();
    }


    // =============================================
    // 임시 게임 승리
    // =============================================

    private void TemporaryGameWin()
    {
        Debug.Log(
            "[GameMachine] 임시 게임 승리"
        );


        UnlockBrokenArm();


        /*
         * 지금은 동전 투입 즉시 승리하기 때문에
         * isCoinInserted를 바로 해제합니다.
         *
         * 실제 게임이 구현되면 게임 종료 시점에서
         * 처리하면 됩니다.
         */

        isCoinInserted = false;
    }


    // =============================================
    // 인형 팔 보상 등장
    // =============================================

    private void UnlockBrokenArm()
    {
        // 이미 팔이 등장했다면 중복 처리 X
        if (retroGameManager != null &&
            retroGameManager.IsArmRewardUnlocked)
        {
            Debug.Log(
                "[GameMachine] BrokenArm은 이미 등장했습니다."
            );

            return;
        }


        // BrokenArm 활성화
        if (brokenArmObject != null)
        {
            brokenArmObject.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "[GameMachine] BrokenArm Object가 연결되지 않았습니다."
            );

            return;
        }


        // RetroGameManager에는
        // "팔이 이미 나왔다"는 상태만 전달
        if (retroGameManager != null)
        {
            retroGameManager.UnlockArmReward();
        }


        Debug.Log(
            "[GameMachine] BrokenArm 등장"
        );
    }
    public void GameWin()
{
    // 이미 보상이 나온 경우 중복 지급 방지
    if (retroGameManager != null &&
        retroGameManager.IsArmRewardUnlocked)
    {
        Debug.Log(
            "[GameMachineCoin] 인형 팔 보상은 이미 등장했습니다."
        );

        isCoinInserted = false;
        return;
    }


    // 팔 보상 등장
    if (brokenArmObject != null)
    {
        brokenArmObject.SetActive(true);
    }
    else
    {
        Debug.LogError(
            "[GameMachineCoin] BrokenArm Object가 연결되지 않았습니다."
        );

        return;
    }


    // RetroGameManager에는 상태만 저장
    if (retroGameManager != null)
    {
        retroGameManager.UnlockArmReward();
    }


    isCoinInserted = false;


    Debug.Log(
        "[GameMachineCoin] 게임 승리 - BrokenArm 등장"
    );
}
}