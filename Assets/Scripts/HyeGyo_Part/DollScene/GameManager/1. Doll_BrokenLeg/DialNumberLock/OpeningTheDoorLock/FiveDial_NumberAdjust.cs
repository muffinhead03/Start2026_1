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


    private void Start()
    {
        // Dial 자동 연결
        if (dial == null)
        {
            dial =
                GetComponent<FiveDial_Number>();
        }


        // ============================================
        // Left Button 연결
        // ============================================

        if (leftButton != null)
        {
            leftButton.onClick.RemoveListener(
                PreviousNumber
            );

            leftButton.onClick.AddListener(
                PreviousNumber
            );


            Debug.Log(
                $"[FiveDialAdjust] {gameObject.name} " +
                $"LEFT 등록 완료 : {leftButton.gameObject.name}"
            );
        }
        else
        {
            Debug.LogError(
                $"[FiveDialAdjust] {gameObject.name} " +
                $"LEFT BUTTON 연결 안 됨"
            );
        }


        // ============================================
        // Right Button 연결
        // ============================================

        if (rightButton != null)
        {
            rightButton.onClick.RemoveListener(
                NextNumber
            );

            rightButton.onClick.AddListener(
                NextNumber
            );


            Debug.Log(
                $"[FiveDialAdjust] {gameObject.name} " +
                $"RIGHT 등록 완료 : {rightButton.gameObject.name}"
            );
        }
        else
        {
            Debug.LogError(
                $"[FiveDialAdjust] {gameObject.name} " +
                $"RIGHT BUTTON 연결 안 됨"
            );
        }
    }


    private void OnDestroy()
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
    // 왼쪽
    // ================================================

    public void PreviousNumber()
    {
        Debug.Log(
            $"[FiveDialAdjust] ★ LEFT CLICK : {gameObject.name}"
        );


        if (dial == null)
        {
            Debug.LogError(
                $"[FiveDialAdjust] {gameObject.name} Dial 없음"
            );

            return;
        }


        int currentNumber =
            dial.CurrentNumber;


        int nextNumber =
            currentNumber - 1;


        if (nextNumber < 0)
        {
            nextNumber = 9;
        }


        Debug.Log(
            $"[FiveDialAdjust] {gameObject.name} " +
            $"LEFT : {currentNumber} → {nextNumber}"
        );


        dial.SetNumberAndSnap(
            nextNumber
        );
    }


    // ================================================
    // 오른쪽
    // ================================================

    public void NextNumber()
    {
        Debug.Log(
            $"[FiveDialAdjust] ★ RIGHT CLICK : {gameObject.name}"
        );


        if (dial == null)
        {
            Debug.LogError(
                $"[FiveDialAdjust] {gameObject.name} Dial 없음"
            );

            return;
        }


        int currentNumber =
            dial.CurrentNumber;


        int nextNumber =
            currentNumber + 1;


        if (nextNumber > 9)
        {
            nextNumber = 0;
        }


        Debug.Log(
            $"[FiveDialAdjust] {gameObject.name} " +
            $"RIGHT : {currentNumber} → {nextNumber}"
        );


        dial.SetNumberAndSnap(
            nextNumber
        );
    }
}