using UnityEngine;

public class DollScene_ChangeDoll : MonoBehaviour
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
     * TODO : 인형 수리
     * =============================================
     *
     * 조건
     *
     * 1. 부러진 다리 교체 완료
     * 2. 부러진 팔 교체 완료
     * 3. 태엽 획득 완료
     *
     *
     * 모든 조건 만족
     *
     * ↓
     *
     * 인형 수리 상호작용 활성화
     *
     * ↓
     *
     * 인형 수리 완료
     *
     * ↓
     *
     * 태엽 부분 상호작용 가능
     *
     * ↓
     *
     * 태엽 돌리기
     *
     * ↓
     *
     * 인형 Animation
     *
     * 팔을 벌림
     * 다리를 들어올림
     * 날아가는 자세
     *
     * ↓
     *
     * 인형이 들고 있던 열쇠 Drop
     *
     * ↓
     *
     * Exit Key 획득
     *
     * ↓
     *
     * 상자 사용
     *
     * ↓
     *
     * Stage Clear
     *
     */


    public void RepairDoll()
    {
        if (gameManager == null)
            return;

        if (!gameManager.CanRepairDoll())
        {
            Debug.Log(
                "[ChangeDoll] 아직 필요한 부품이 부족합니다."
            );

            return;
        }

        gameManager.CompleteDollRepair();


        /*
         * TODO
         *
         * Broken Doll Model 변경
         * Fixed Doll Model 활성화
         *
         * 태엽 Interaction 활성화
         */
    }


    public void DropExitKey()
    {
        /*
         * TODO
         *
         * 인형 Animation 종료 시
         *
         * ExitKey Parent 해제
         * Rigidbody 활성화
         *
         * 열쇠가 바닥에 떨어지도록 처리
         */
    }
}