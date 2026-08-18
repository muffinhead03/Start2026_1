using UnityEngine;

public class RetroGameManager : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField] private DollScene_GameManager gameManager;


    [Header("Retro Game")]
    [SerializeField] private GameObject gameScreen;


    [Header("Reward")]
    [SerializeField] private GameObject dollArmObject;

    [Tooltip("인형 팔 보상이 이미 한 번 등장했는지")]
    [SerializeField] private bool isArmRewardUnlocked = false;


    [Header("Game State")]
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private bool isCoinInserted = false;


    public bool IsPlaying => isPlaying;
    public bool IsCoinInserted => isCoinInserted;
    public bool IsArmRewardUnlocked => isArmRewardUnlocked;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }


        if (gameScreen != null)
            gameScreen.SetActive(false);


        // 팔을 아직 획득하지 않았다면 처음에는 숨김
        if (dollArmObject != null && !isArmRewardUnlocked)
        {
            dollArmObject.SetActive(false);
        }
    }


    // =============================================
    // RetroGame E 상호작용
    // =============================================

    public void Interact()
    {
        // 게임이 이미 진행 중이면 다시 실행하지 않음
        if (isPlaying)
        {
            Debug.Log(
                "[RetroGame] 이미 게임이 진행 중입니다."
            );

            return;
        }


        // GameManager 확인
        if (gameManager == null)
        {
            Debug.LogError(
                "[RetroGame] DollScene_GameManager가 없습니다."
            );

            return;
        }


        // 동전 확인
        if (!gameManager.IsCoinOwned)
        {
            Debug.Log(
                "[RetroGame] 동전이 필요합니다."
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
        if (!gameManager.UseCoin())
            return;


        isCoinInserted = true;


        Debug.Log(
            "[RetroGame] 동전을 넣었습니다."
        );


        StartGame();
    }


    // =============================================
    // 게임 시작
    // =============================================

    private void StartGame()
    {
        if (isPlaying)
            return;


        isPlaying = true;


        if (gameScreen != null)
            gameScreen.SetActive(true);


        Debug.Log(
            "[RetroGame] 게임 시작"
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
         * - 플레이어 이동 제한
         * - 격투 게임 시작
         * - 입력 처리
         * - 적 AI
         * - 승 / 패 판정
         *
         */


        // =============================================
        // 임시 구현
        // =============================================
        //
        // 현재는 철권 게임이 구현되지 않았으므로
        // 동전을 넣으면 바로 승리한 것으로 처리
        //

        GameWin();
    }


    // =============================================
    // 게임 승리
    // =============================================

    public void GameWin()
    {
        if (!isPlaying)
            return;


        Debug.Log(
            "[RetroGame] 게임 승리"
        );


        // 인형 팔 보상은 최초 1회만
        if (!isArmRewardUnlocked)
        {
            UnlockArmReward();
        }
        else
        {
            Debug.Log(
                "[RetroGame] 인형 팔 보상은 이미 등장했습니다."
            );
        }


        EndGame();
    }


    // =============================================
    // 게임 패배
    // =============================================

    public void GameLose()
    {
        if (!isPlaying)
            return;


        Debug.Log(
            "[RetroGame] 게임 패배"
        );


        EndGame();
    }


    // =============================================
    // 인형 팔 보상
    // =============================================

    private void UnlockArmReward()
    {
        if (isArmRewardUnlocked)
            return;


        isArmRewardUnlocked = true;


        if (dollArmObject != null)
        {
            dollArmObject.SetActive(true);
        }


        Debug.Log(
            "[RetroGame] 인형 팔 등장"
        );


        /*
         * TODO
         *
         * 오락기 하단 작은 문 열기
         * 문 Open Animation 실행
         *
         */
    }


    // =============================================
    // 게임 종료
    // =============================================

    private void EndGame()
    {
        isPlaying = false;


        if (gameScreen != null)
        {
            gameScreen.SetActive(false);
        }


        ReturnCoin();


        /*
         * TODO
         *
         * 플레이어 이동 제한 해제
         *
         */


        Debug.Log(
            "[RetroGame] 게임 종료"
        );
    }


    // =============================================
    // 동전 반환
    // =============================================

    private void ReturnCoin()
    {
        if (!isCoinInserted)
            return;


        isCoinInserted = false;


        if (gameManager != null)
        {
            gameManager.ReturnCoin();
        }


        Debug.Log(
            "[RetroGame] 동전이 반환되었습니다."
        );
    }
}