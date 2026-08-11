using UnityEngine;

public class FiveDial_InputController : MonoBehaviour
{
    public enum InputType
    {
        Keyboard,
        Mouse
    }


    [Header("Input Type")]
    [SerializeField]
    private InputType inputType = InputType.Mouse;


    [Header("Dials")]
    [SerializeField]
    private FiveDial_Number[] dials;


    [Header("State")]
    [SerializeField]
    private bool isActive = false;


    public void SetActive(bool active)
    {
        isActive = active;
    }


    private void Update()
    {
        if (!isActive)
            return;


        switch (inputType)
        {
            case InputType.Keyboard:

                UpdateKeyboard();

                break;


            case InputType.Mouse:

                UpdateMouse();

                break;
        }
    }


    // ================================================
    // Keyboard
    // ================================================

    private void UpdateKeyboard()
    {
        /*
         * TODO
         *
         * 기획 확정 후 구현
         *
         * 예:
         * A / D = Dial 선택
         * W / S = Dial 회전
         */
    }


    // ================================================
    // Mouse
    // ================================================

    private void UpdateMouse()
    {
        /*
         * TODO
         *
         * 기획 확정 후 구현
         *
         * 예:
         * 클릭 = Dial 선택
         * Drag = 회전
         * Release = TrySnapCurrentAngle()
         */
    }
}