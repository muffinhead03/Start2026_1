using UnityEngine;
using System.Collections;

public class DollScene_GameMachine : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField] private DollScene_GameManager gameManager;


    [Header("Game Machine")]
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject dollArm;


    [Header("Game State")]
    [SerializeField] private bool isCoinInserted = false;
    [SerializeField] private bool isGamePlaying = false;
    [SerializeField] private bool isGameCleared = false;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                GetComponentInParent<DollScene_GameManager>();
        }

        if (gameScreen != null)
            gameScreen.SetActive(false);

        if (dollArm != null)
            dollArm.SetActive(false);
    }


    // ================================
    // 동전 투입
    // ================================

    public void InsertCoin()
    {
        if (isCoinInserted)
            return;

        if (isGameCleared)
            return;

        /*
         * TODO
         *
         * Player Inventory에
         * Coin이 있는지 확인
         *
         * Coin이 있다면
         * Inventory에서 Coin 제거
         *
         * 현재는 인벤토리 코드 확인 후 연결
         */

        isCoinInserted = true;

        Debug.Log("[GameMachine] Coin Inserted");

        StartGame();
    }


    // ================================
    // 게임 시작
    // ================================

    private void StartGame()
    {
        if (isGamePlaying)
            return;

        isGamePlaying = true;

        if (gameScreen != null)
            gameScreen.SetActive(true);

        Debug.Log("[GameMachine] Game Start");


        /*
         * TODO
         *
         * 추후 철권 스타일 미니게임 구현
         *
         * 현재 프로토타입에서는
         * 일정 시간 뒤 자동 승리 처리 가능
         */

        StartCoroutine(PrototypeGame());
    }


    private IEnumerator PrototypeGame()
    {
        yield return new WaitForSeconds(2f);

        GameWin();
    }


    // ================================
    // 게임 승리
    // ================================

    public void GameWin()
    {
        if (!isGamePlaying)
            return;

        isGamePlaying = false;
        isGameCleared = true;

        Debug.Log("[GameMachine] YOU WIN");

        OpenReward();
    }


    // ================================
    // 보상
    // ================================

    private void OpenReward()
    {
        /*
         * TODO
         *
         * 오락기 하단 작은 문 Open Animation
         */

        if (dollArm != null)
            dollArm.SetActive(true);

        Debug.Log("[GameMachine] Doll Arm Open");
    }
}