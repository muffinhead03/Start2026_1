using UnityEngine;
using UnityEngine.InputSystem;

public class GoToNumberLockMode : MonoBehaviour
{
    // ================================================
    // Broken Leg Controller
    // ================================================

    [Header("Broken Leg Controller")]
    [SerializeField]
    private DollScene_ChangeBrokenLeg brokenLegController;


    // ================================================
    // Canvas
    // ================================================

    [Header("Canvas")]

    [Tooltip("자물쇠 조작용 Canvas")]
    [SerializeField]
    private GameObject numberLockCanvas;


    [Tooltip("평상시 Player UI Canvas")]
    [SerializeField]
    private GameObject playerCanvas;


    // ================================================
    // Mode Object
    // ================================================

    [Header("Mode Light Object")]
    [SerializeField]
    private GameObject modeLightObject;


    // ================================================
    // State
    // ================================================

    [Header("State")]
    [SerializeField]
    private bool isActive = false;


    /*
     * 자물쇠에 진입할 때 누른 E가
     * 같은 프레임/입력으로 종료까지 발생하는 것을 방지.
     *
     * 진입 후 E를 한번 떼어야
     * 다음 E 입력으로 나갈 수 있음.
     */
    private bool canExitWithE = false;


    // ================================================
    // Awake
    // ================================================

    private void Awake()
    {
        if (brokenLegController == null)
        {
            brokenLegController =
                GetComponentInParent<DollScene_ChangeBrokenLeg>();
        }


        if (brokenLegController == null)
        {
            brokenLegController =
                FindFirstObjectByType<DollScene_ChangeBrokenLeg>();
        }
    }


    // ================================================
    // Start
    // ================================================

    private void Start()
    {
        // 평상시에는 NumberLock Canvas 숨김
        if (numberLockCanvas != null)
        {
            numberLockCanvas.SetActive(false);
        }


        // 연출용 오브젝트도 평상시에는 OFF
        if (modeLightObject != null)
        {
            modeLightObject.SetActive(false);
        }
    }


    // ================================================
    // Update
    // ================================================

    private void Update()
    {
        if (!isActive)
            return;


        if (Keyboard.current == null)
            return;


        // --------------------------------
        // 처음 진입할 때 누른 E를
        // 한번 놓을 때까지 기다림
        // --------------------------------

        if (!canExitWithE)
        {
            if (Keyboard.current.eKey.wasReleasedThisFrame)
            {
                canExitWithE = true;
            }


            return;
        }


        // --------------------------------
        // E를 다시 누르면 NumberLock 종료
        // --------------------------------

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ExitByE();
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


        // --------------------------------
        // 기존 Player UI 숨기기
        // --------------------------------

        if (playerCanvas != null)
        {
            playerCanvas.SetActive(false);
        }


        // --------------------------------
        // NumberLock UI 표시
        // --------------------------------

        if (numberLockCanvas != null)
        {
            numberLockCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "[FiveDial] NumberLock Canvas가 연결되지 않았습니다."
            );
        }


        // --------------------------------
        // 조명 / 연출 오브젝트
        // --------------------------------

        if (modeLightObject != null)
        {
            modeLightObject.SetActive(true);
        }


        Debug.Log(
            "[FiveDial] NumberLock Mode ON"
        );
    }


    // ================================================
    // E 버튼으로 종료
    // ================================================

    private void ExitByE()
    {
        if (!isActive)
            return;


        Debug.Log(
            "[FiveDial] E → NumberLock Exit"
        );


        /*
         * 여기서 직접 ExitMode()만 하지 않고
         * DollScene_ChangeBrokenLeg를 통해서 종료.
         *
         * 그래야
         *
         * isCheckingNumberLock
         * Camera
         * Cursor
         * Pointer
         *
         * 상태까지 같이 정상 복구됨.
         */

        if (brokenLegController != null)
        {
            brokenLegController.ExitNumberLock();
        }
        else
        {
            Debug.LogWarning(
                "[FiveDial] DollScene_ChangeBrokenLeg가 없습니다."
            );


            // 비상용
            ExitMode();
        }
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


        // --------------------------------
        // NumberLock UI 숨기기
        // --------------------------------

        if (numberLockCanvas != null)
        {
            numberLockCanvas.SetActive(false);
        }


        // --------------------------------
        // 기존 Player UI 복구
        // --------------------------------

        if (playerCanvas != null)
        {
            playerCanvas.SetActive(true);
        }


        // --------------------------------
        // 조명 / 연출 오브젝트 OFF
        // --------------------------------

        if (modeLightObject != null)
        {
            modeLightObject.SetActive(false);
        }


        Debug.Log(
            "[FiveDial] NumberLock Mode OFF"
        );
    }


    // ================================================
    // State
    // ================================================

    public bool IsActive()
    {
        return isActive;
    }
}