using UnityEngine;
using UnityEngine.InputSystem;

public class GoToNumberLockMode : MonoBehaviour
{
    [Header("Fix Camera")]
    [SerializeField]
    private Object_FixCamera fixCamera;


    [Header("Dial Input")]
    [SerializeField]
    private FiveDial_InputController inputController;


    [Header("State")]
    [SerializeField]
    private bool isActive = false;


    private bool canExitWithE = false;


    private void Update()
    {
        if (!isActive)
            return;

        if (Keyboard.current == null)
            return;


        // 처음 진입할 때 사용한 E가
        // 바로 Exit 입력으로 들어오는 것 방지
        if (!canExitWithE)
        {
            if (Keyboard.current.eKey.wasReleasedThisFrame)
            {
                canExitWithE = true;
            }

            return;
        }


        // NumberLock 모드에서 다시 E
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (fixCamera != null)
            {
                fixCamera.UnFixCamera();
            }
        }
    }


    // ================================================
    // NumberLock Mode Enter
    // ================================================

    public void EnterMode()
    {
        if (isActive)
            return;


        isActive = true;
        canExitWithE = false;


        if (inputController != null)
        {
            inputController.SetActive(true);
        }


        Debug.Log(
            "[FiveDial] Dial Mode ON"
        );
    }


    // ================================================
    // NumberLock Mode Exit
    // ================================================

    public void ExitMode()
    {
        if (!isActive)
            return;


        isActive = false;
        canExitWithE = false;


        if (inputController != null)
        {
            inputController.SetActive(false);
        }


        Debug.Log(
            "[FiveDial] Dial Mode OFF"
        );
    }


    public bool IsActive()
    {
        return isActive;
    }
}