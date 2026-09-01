using UnityEngine;

public class DollScene_BrokenArmChange : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField] private DollScene_GameManager gameManager;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                GetComponentInParent<DollScene_GameManager>();
        }
    }


    /*
     * =============================================
     * TODO : 부러진 팔 퍼즐
     * =============================================
     *
     * 1. 말하는 곰 인형 발견
     *
     * 2. 곰 인형을 집어서 던질 수 있음
     *
     * 3. 충돌할 때마다 음성 출력
     *
     *    1회 : 아야!
     *    2회 : 아파..
     *    3회 : 이제 그만해..
     *    4회 : 제발 부탁이야
     *    5회 : 소리 없음
     *
     * 4. 5번째 충돌 시
     *    곰 인형 파손 상태로 변경
     *
     * 5. 곰 안에서 동전 등장
     *
     * 6. 동전 획득
     *
     * 이후 DollScene_GameMachine으로 진행
     *
     */


    public void CompleteArmChange()
    {
        /*
         * TODO
         *
         * 오락기에서 인형 팔 획득 후
         * 최종 인형에 팔을 장착했을 때
         * 호출 예정
         */

        if (gameManager != null)
        {
            gameManager.CompleteBrokenArm();
        }
    }
}