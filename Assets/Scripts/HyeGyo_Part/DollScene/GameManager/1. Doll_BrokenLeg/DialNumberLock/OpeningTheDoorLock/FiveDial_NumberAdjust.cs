using UnityEngine;
using UnityEngine.UI;

public class FiveDial_NumberAdjust : MonoBehaviour
{
    [Header("Target Dial")]
    [SerializeField]
    private FiveDial_Number dial;


    [Header("UI Button")]
    [SerializeField]
    private Button leftButton;

    [SerializeField]
    private Button rightButton;


    private void Awake()
    {
        // 같은 오브젝트에 있는 FiveDial_Number 자동 연결
        if (dial == null)
        {
            dial =
                GetComponent<FiveDial_Number>();
        }
    }


    private void OnEnable()
    {
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(
                PreviousNumber
            );
        }


        if (rightButton != null)
        {
            rightButton.onClick.AddListener(
                NextNumber
            );
        }
    }


    private void OnDisable()
    {
        if (leftButton != null)
        {
            leftButton.onClick.RemoveListener(
                PreviousNumber
            );
        }


        if (rightButton != null)
        {
            rightButton.onClick.RemoveListener(
                NextNumber
            );
        }
    }


    // ================================================
    // 왼쪽 버튼
    // ================================================

    public void PreviousNumber()
{
    Debug.Log(
        $"[FiveDialAdjust] LEFT 버튼 클릭됨 : {gameObject.name}"
    );

    if (dial == null)
    {
        Debug.LogWarning(
            $"[FiveDialAdjust] {gameObject.name} Dial 없음"
        );

        return;
    }

    int nextNumber =
        dial.CurrentNumber - 1;

    if (nextNumber < 0)
    {
        nextNumber = 9;
    }

    Debug.Log(
        $"[FiveDialAdjust] LEFT : " +
        $"{dial.CurrentNumber} → {nextNumber}"
    );

    dial.SetNumberAndSnap(nextNumber);
}


    // ================================================
    // 오른쪽 버튼
    // ================================================

    public void NextNumber()
    {
        if (dial == null)
        {
            Debug.LogWarning(
                $"[FiveDialAdjust] {gameObject.name} Dial 없음"
            );

            return;
        }


        int nextNumber =
            dial.CurrentNumber + 1;


        if (nextNumber > 9)
        {
            nextNumber = 0;
        }


        Debug.Log(
            $"[FiveDialAdjust] {gameObject.name} " +
            $"RIGHT : {dial.CurrentNumber} → {nextNumber}"
        );


        dial.SetNumberAndSnap(
            nextNumber
        );
    }
}