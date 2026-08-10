using UnityEngine;

public class DollScene_ChangeBrokenLeg : MonoBehaviour
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


    // ================================
    // 자물쇠 검사
    // ================================

    public void CheckNumberLock()
    {
        /*
         * TODO : 자물쇠 숫자 판정
         *
         * 정답 : 3 - 4 - 1 - 5 - 2
         *
         * 각각의 자물쇠 숫자가
         * 원하는 위치에 배치되어 있는지 검사한다.
         *
         * 예시:
         *
         * if (number1 == 3 &&
         *     number2 == 4 &&
         *     number3 == 1 &&
         *     number4 == 5 &&
         *     number5 == 2)
         * {
         *     UnlockHorseArea();
         * }
         */
    }


    // ================================
    // 목마 구역 해제
    // ================================

    private void UnlockHorseArea()
    {
        /*
         * TODO
         *
         * 울타리 자물쇠 해제
         * 울타리 Open
         *
         * 이후 플레이어가
         * 두 번째 / 세 번째 목마 사이에서
         * 인형 다리를 획득할 수 있음.
         */
    }


    // ================================
    // 다리 교체 완료
    // ================================

    public void CompleteLegChange()
    {
        /*
         * TODO
         *
         * 실제 인형 다리를 획득한 뒤
         * 최종 인형에 다리를 장착하는 시점에 호출할지,
         *
         * 혹은 다리를 획득하는 시점에 호출할지는
         * 추후 인벤토리 시스템 확인 후 결정.
         */

        if (gameManager != null)
        {
            gameManager.CompleteBrokenLeg();
        }
    }
}