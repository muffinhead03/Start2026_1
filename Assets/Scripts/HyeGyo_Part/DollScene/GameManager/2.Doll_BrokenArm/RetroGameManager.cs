using UnityEngine;

public class RetroGameManager : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    [Header("Game State")]
    [SerializeField]
    private bool isPlaying = false;

    [SerializeField]
    private bool isCoinInserted = false;


    [Header("Reward State")]
    [Tooltip("인형 팔 보상이 이미 한 번 등장했는지")]
    [SerializeField]
    private bool isArmRewardUnlocked = false;


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


        if (gameManager == null)
        {
            Debug.LogError(
                "[RetroGame] DollScene_GameManager를 찾을 수 없습니다.",
                this
            );
        }


        Debug.Log(
            "[RetroGame] RetroGameManager 초기화 완료"
        );
    }


    // =============================================
    // 게임 시작 상태 설정
    // =============================================

    public void StartRetroGame()
    {
        if (isPlaying)
        {
            Debug.Log(
                "[RetroGame] 이미 게임이 진행 중입니다."
            );

            return;
        }


        isPlaying = true;


        Debug.Log(
            "[RetroGame] 게임 시작 상태 저장"
        );


        /*
         * =============================================
         *
         *              철권게임파트
         *
         * =============================================
         *
         * 실제 게임 구현 시
         * 여기 또는 별도의 게임 스크립트에서
         * 게임 시작 처리를 연결합니다.
         *
         */
    }


    // =============================================
    // 동전 투입 상태
    // =============================================

    public void SetCoinInserted(bool inserted)
    {
        isCoinInserted = inserted;


        Debug.Log(
            $"[RetroGame] 동전 투입 상태 : {isCoinInserted}"
        );
    }


    // =============================================
    // 게임 승리
    // =============================================

    public void GameWin()
    {
        if (!isPlaying)
        {
            Debug.LogWarning(
                "[RetroGame] 게임 중이 아닙니다."
            );

            return;
        }


        Debug.Log(
            "[RetroGame] 게임 승리"
        );


        EndGame();
    }


    // =============================================
    // 게임 패배
    // =============================================

    public void GameLose()
    {
        if (!isPlaying)
        {
            Debug.LogWarning(
                "[RetroGame] 게임 중이 아닙니다."
            );

            return;
        }


        Debug.Log(
            "[RetroGame] 게임 패배"
        );


        EndGame();
    }


    // =============================================
    // 인형 팔 보상 상태 기록
    // GameMachineCoinManager에서 호출
    // =============================================

    public void UnlockArmReward()
    {
        if (isArmRewardUnlocked)
        {
            Debug.Log(
                "[RetroGame] 인형 팔 보상은 이미 등장한 상태입니다."
            );

            return;
        }


        isArmRewardUnlocked = true;


        Debug.Log(
            "[RetroGame] 인형 팔 보상 등장 상태 저장"
        );
    }


    // =============================================
    // 게임 종료
    // =============================================

    public void EndGame()
    {
        if (!isPlaying)
            return;


        isPlaying = false;


        Debug.Log(
            "[RetroGame] 게임 종료 상태 저장"
        );
    }
}