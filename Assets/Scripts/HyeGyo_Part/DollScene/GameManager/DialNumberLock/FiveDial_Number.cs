using UnityEngine;

public class FiveDial_Number : MonoBehaviour
{
    [Header("Number")]
    [Range(0, 9)]
    [SerializeField] private int currentNumber = 0;

    [Range(0, 9)]
    [SerializeField] private int correctNumber = 0;


    [Header("Number Lock Manager")]
    [SerializeField]
    private DollScene_ChangeBrokenLeg numberLockManager;


    public int CurrentNumber => currentNumber;

    public int CorrectNumber => correctNumber;

    public bool IsCorrect =>
        currentNumber == correctNumber;


    // ================================================
    // 현재 숫자 변경
    // ================================================

    public void SetCurrentNumber(int number)
    {
        currentNumber =
            Mathf.Clamp(number, 0, 9);

        if (numberLockManager != null)
        {
            numberLockManager.CheckNumberLock();
        }
    }


    // ================================================
    // 각도 Snap
    // ================================================

    public void TrySnapCurrentAngle()
    {
        /*
         * TODO
         *
         * 나중에 실제 모델의 각도를 확인하고 구현
         *
         * 예:
         *
         * 30 ~ 40도
         *      ↓
         * 35도로 Snap
         *      ↓
         * currentNumber = 3
         *
         * 각 숫자별 정확한 각도는 추후 입력
         */


        if (numberLockManager != null)
        {
            numberLockManager.CheckNumberLock();
        }
    }
}