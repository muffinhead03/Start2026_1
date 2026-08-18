using UnityEngine;
using UnityEngine.Events;

public class NumberLockManager : MonoBehaviour
{
    [Header("Number Dials")]
    [SerializeField] private FiveDial_Number[] dials;


    [Header("Puzzle State")]
    [SerializeField] private bool isSolved = false;


    [Header("Puzzle Event")]
    public UnityEvent OnSolved;


    // ================================
    // 상태 확인
    // ================================

    public bool IsSolved => isSolved;


    // ================================
    // Dial 숫자가 변경될 때 호출
    // ================================

    public void NotifyDialChanged()
    {
        if (isSolved)
            return;

        CheckAnswer();
    }


    // ================================
    // 전체 정답 확인
    // ================================

    public void CheckAnswer()
    {
        if (isSolved)
            return;


        if (dials == null || dials.Length == 0)
        {
            Debug.LogWarning(
                "[NumberLock] Dial이 연결되지 않았습니다."
            );

            return;
        }


        // 모든 Dial 확인
        foreach (FiveDial_Number dial in dials)
        {
            if (dial == null)
            {
                Debug.LogWarning(
                    "[NumberLock] 연결되지 않은 Dial이 있습니다."
                );

                return;
            }


            // 하나라도 틀렸으면 정답 아님
            if (!dial.IsCorrect)
            {
                return;
            }
        }


        Solve();
    }


    // ================================
    // Puzzle Solve
    // ================================

    private void Solve()
    {
        if (isSolved)
            return;


        isSolved = true;


        Debug.Log(
            "[NumberLock] Puzzle Complete"
        );


        OnSolved?.Invoke();
    }
}