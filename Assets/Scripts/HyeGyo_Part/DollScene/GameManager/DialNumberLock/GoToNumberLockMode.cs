using UnityEngine;
using UnityEngine.InputSystem;

public class GoToNumberLockMode : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;


    [Header("Number Dials")]
    [SerializeField] private FiveDial_Number dial1;
    [SerializeField] private FiveDial_Number dial2;
    [SerializeField] private FiveDial_Number dial3;
    [SerializeField] private FiveDial_Number dial4;
    [SerializeField] private FiveDial_Number dial5;


    [Header("Dial Setting")]
    [SerializeField] private LayerMask dialLayerMask = ~0;
    [SerializeField] private float rayDistance = 2f;


    [Header("State")]
    [SerializeField] private bool isActive = false;


    private InputAction click;


    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        click =
            InputSystem.actions.FindAction("Click");


        if (click != null)
        {
            click.performed += OnClickPerformed;

            click.Disable();
        }
    }


    private void OnDestroy()
    {
        if (click != null)
        {
            click.performed -= OnClickPerformed;
        }
    }


    // ================================================
    // FiveDial 조작 모드 ON/OFF
    //
    // Object_Pwd.SetActive와 같은 역할
    // ================================================

    public void SetActive(bool active)
    {
        isActive = active;


        if (click == null)
            return;


        if (active)
        {
            click.Enable();

            Debug.Log(
                "[FiveDial] Dial Mode ON"
            );
        }
        else
        {
            click.Disable();

            Debug.Log(
                "[FiveDial] Dial Mode OFF"
            );
        }
    }


    // UnityEvent 연결을 쉽게 하기 위한 함수
    public void EnterMode()
    {
        SetActive(true);
    }


    public void ExitMode()
    {
        SetActive(false);
    }


    // ================================================
    // Click Input
    // ================================================

    private void OnClickPerformed(
        InputAction.CallbackContext context)
    {
        PressDial();
    }


    // ================================================
    // 마우스로 Dial 선택
    // ================================================

    private void PressDial()
    {
        if (!isActive)
            return;


        if (mainCamera == null)
            return;


        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        Ray ray =
            mainCamera.ScreenPointToRay(
                mousePosition
            );


        RaycastHit hit;


        if (!Physics.Raycast(
                ray,
                out hit,
                rayDistance,
                dialLayerMask))
        {
            return;
        }


        FiveDial_Number dial =
            hit.collider.GetComponentInParent
                <FiveDial_Number>();


        if (dial == null)
            return;


        if (!IsRegisteredDial(dial))
            return;


        Debug.Log(
            "[FiveDial] Click : " +
            dial.gameObject.name
        );


        // TODO
        //
        // 여기서 앞으로
        // Drag 시작 또는 Dial 회전 기능 연결
    }


    private bool IsRegisteredDial(
        FiveDial_Number dial)
    {
        return dial == dial1 ||
               dial == dial2 ||
               dial == dial3 ||
               dial == dial4 ||
               dial == dial5;
    }


    public bool IsActive()
    {
        return isActive;
    }
}