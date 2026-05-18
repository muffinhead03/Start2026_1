using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class EX_InputSystem_PC_V2 : MonoBehaviour
{
    CharacterController Character;

    [Header("Camera")]
    public Transform CameraPivot;
    public float mouseSensitivity = 0.1f;
    float yaw;
    float pitch;

    [Header("Body")]
    public Transform bodyMesh;
    public float crouchMeshScaleY = 0.5f;

    [Header("Move")]
    public float walkSpeed = 1.5f;    // 걷기 속도
    public float runSpeed = 4.0f;     // 달리기 속도
    public float crouchSpeed = 0.8f;  // 웅크리기 속도
    public float jumpHeight = 1f;   // 점프 높이 설정
    public float gravity = -9.81f;    // 중력 값
    public float friction = 0.9f;

    [Header("Crouch")]
    public float crouchHeight = 1.0f; // 웅크렸을 때의 콜라이더 높이
    public float crouchCameraY = 0.5f; // 웅크렸을 때의 카메라 위치(Y)
    private float originalHeight;
    private Vector3 originalCameraPos;
    private Vector3 originalMeshScale;

    [Header("Settings")]
    public GameObject panelSettings;    // 설정창 UI

    Vector3 Velocity;
    Vector2 MoveInput;
    Vector2 LookInput;

    InputAction lookAction;
    InputAction moveAction;
    InputAction runAction;
    InputAction hideAction;
    InputAction jumpAction;
    InputAction settingsAction;

    bool isPanelOpened = false;

    void Start()
    {
        LockPointer();

        Character = GetComponent<CharacterController>();

        // 서 있을 때의 원래 높이와 카메라 위치 저장
        originalHeight = Character.height;

        if (CameraPivot == null)
            CameraPivot = transform.Find("CameraRig/CameraPivot");

        if (CameraPivot != null)
            originalCameraPos = CameraPivot.localPosition;

        if (bodyMesh != null)
            originalMeshScale = bodyMesh.localScale;

        lookAction = InputSystem.actions.FindAction("Look");
        moveAction = InputSystem.actions.FindAction("Move");
        runAction = InputSystem.actions.FindAction("Run");
        hideAction = InputSystem.actions.FindAction("Hide");
        jumpAction = InputSystem.actions.FindAction("Jump");
        settingsAction = InputSystem.actions.FindAction("Settings");

        settingsAction.performed += OpenCloseSettings;
    }

    void Update()
    {
        if (isPanelOpened) return;

        Look();

        Move();
        Character.Move(Velocity * Time.deltaTime);

        if (lookAction != null) LookInput = lookAction.ReadValue<Vector2>();
        if (moveAction != null) MoveInput = moveAction.ReadValue<Vector2>();
    }

    void Look()
    {
        yaw += LookInput.x * mouseSensitivity;
        pitch -= LookInput.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        if (CameraPivot != null)
            CameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void Move()
    {
        bool isRunning = (runAction != null && runAction.IsPressed());
        bool isHiding = (hideAction != null && hideAction.IsPressed());
        bool isJumping = (jumpAction != null && jumpAction.triggered);

        float currentSpeed = walkSpeed;
        float targetHeight = originalHeight;
        float targetCameraY = originalCameraPos.y;
        float targetMeshScaleY = originalMeshScale.y;

        if (isHiding)
        {
            currentSpeed = crouchSpeed;
            targetHeight = crouchHeight;
            targetCameraY = crouchCameraY;
            targetMeshScaleY = crouchMeshScaleY;
        }
        else if (isRunning)
        {
            currentSpeed = runSpeed;
        }

        // 콜라이더와 카메라 높이 설정
        Character.height = Mathf.Lerp(Character.height, targetHeight, Time.deltaTime * 10f);
        Character.center = new Vector3(0, Character.height / 2f, 0);

        if (CameraPivot != null)
        {
            Vector3 camPos = CameraPivot.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * 10f);
            CameraPivot.localPosition = camPos;
        }

        if (bodyMesh != null)
        {
            Vector3 meshScale = bodyMesh.localScale;
            meshScale.y = Mathf.Lerp(meshScale.y, targetMeshScaleY, Time.deltaTime * 10f);
            bodyMesh.localScale = meshScale;
            bodyMesh.localPosition = new Vector3(0, meshScale.y, 0);
        }

        Vector3 inputDir = transform.forward * MoveInput.y + transform.right * MoveInput.x;

        if (inputDir.sqrMagnitude > 1)
            inputDir.Normalize();

        if (inputDir.magnitude > 0)
        {
            Velocity.x = inputDir.x * currentSpeed;
            Velocity.z = inputDir.z * currentSpeed;
        }
        // Move 키 뗐을 때
        else
        {
            Velocity.x *= friction;
            Velocity.z *= friction;
        }

        // 땅에 닿아있을 때 수직 속도 초기화 (경사면에서 튀지 않게 약간의 아래쪽 힘 유지)
        if (Character.isGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }

        // 스페이스바를 눌렀고, 바닥에 닿아있을 때 점프 실행
        if (!isHiding && isJumping && Character.isGrounded)
        {
            // 물리 공식: v = sqrt(h * -2 * g)
            Velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 매 프레임마다 중력 적용
        Velocity.y += gravity * Time.deltaTime;
    }

    void OpenCloseSettings(InputAction.CallbackContext context)
    {
        isPanelOpened = !isPanelOpened;
        panelSettings.SetActive(isPanelOpened);

        if (isPanelOpened) UnlockPointer();
        else LockPointer();
    }

    public void LockPointer()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockPointer()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}