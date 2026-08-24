using UnityEngine;
using System.Collections;

public class DollScene_GameMachine : MonoBehaviour
{
    [Header("Retro Game Manager")]
    [SerializeField]
    private RetroGameManager retroGameManager;


    [Header("Coin Manager")]
    [SerializeField]
    private GameMachineCoinManager coinManager;


    [Header("Game Screen")]
    [SerializeField]
    private GameObject gameScreen;


    [Header("Game State")]
    [SerializeField]
    private bool isGamePlaying = false;

    [SerializeField]
    private bool isGameCleared = false;


    public bool IsGamePlaying => isGamePlaying;
    public bool IsGameCleared => isGameCleared;


    private void Start()
    {
        if (retroGameManager == null)
        {
            retroGameManager =
                FindFirstObjectByType<RetroGameManager>();
        }


        if (coinManager == null)
        {
            coinManager =
                FindFirstObjectByType<GameMachineCoinManager>();
        }


        /*
         * GameScreen의 초기 ON/OFF는
         * 가능하면 Inspector에서 설정하는 것을 추천.
         */
    }


    // =============================================
    // GameMachineCoinManager에서 호출
    // =============================================

    public void StartGame()
    {
        if (isGamePlaying)
            return;


        isGamePlaying = true;


        if (retroGameManager != null)
        {
            retroGameManager.StartRetroGame();
        }


        if (gameScreen != null)
        {
            gameScreen.SetActive(true);
        }


        Debug.Log(
            "[GameMachine] Game Start"
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
         * - 게임 전용 카메라
         * - 캐릭터 입력
         * - 적 AI
         * - 체력
         * - 공격
         * - 승/패 판정
         *
         */


        // 현재 임시 구현
        StartCoroutine(
            PrototypeGame()
        );
    }


    // =============================================
    // 임시 게임
    // =============================================

    private IEnumerator PrototypeGame()
    {
        yield return new WaitForSeconds(2f);


        GameWin();
    }


    // =============================================
    // 게임 승리
    // =============================================

    public void GameWin()
    {
        if (!isGamePlaying)
            return;


        isGamePlaying = false;
        isGameCleared = true;


        Debug.Log(
            "[GameMachine] YOU WIN"
        );


        if (retroGameManager != null)
        {
            retroGameManager.GameWin();
        }


        if (coinManager != null)
        {
            coinManager.GameWin();
        }


        EndGame();
    }


    // =============================================
    // 게임 패배
    // =============================================

    public void GameLose()
    {
        if (!isGamePlaying)
            return;


        isGamePlaying = false;


        Debug.Log(
            "[GameMachine] YOU LOSE"
        );


        if (retroGameManager != null)
        {
            retroGameManager.GameLose();
        }


        EndGame();
    }


    // =============================================
    // 게임 종료
    // =============================================

    private void EndGame()
    {
        if (gameScreen != null)
        {
            gameScreen.SetActive(false);
        }


        Debug.Log(
            "[GameMachine] Game End"
        );
    }
}