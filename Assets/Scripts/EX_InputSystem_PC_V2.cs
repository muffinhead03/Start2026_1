using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class EX_InputSystem_PC_V2 : MonoBehaviour
{
    CharacterController Character;

    [Header("Camera")]
    public Transform CameraPivot;
    public float mouseSensitivity = 0.1f;
    float yaw;
    float pitch;

    [Header("Move")]
    public float walkSpeed = 1.5f;
    public float friction = 0.9f;

    Vector3 Velocity;
    Vector2 MoveInput;
    Vector2 LookInput;

    InputAction lookAction;
    InputAction moveAction;

    void Start()
    {
        Character = GetComponent<CharacterController>();

        if (CameraPivot == null)
            CameraPivot = transform.Find("CameraRig/CameraPivot");

        lookAction = InputSystem.actions.FindAction("Look");
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Look();

        Move();
        Character.Move(Velocity * Time.deltaTime);

        LookInput = lookAction.ReadValue<Vector2>();
        MoveInput = moveAction.ReadValue<Vector2>();
    }

    void Look()
    {
        yaw += LookInput.x * mouseSensitivity;
        pitch -= LookInput.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        CameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void Move()
    {
        Vector3 inputDir = transform.forward * MoveInput.y + transform.right * MoveInput.x;

        if (inputDir.sqrMagnitude > 1)
            inputDir.Normalize();

        float speed = walkSpeed;

        // Move 키 눌렀을 때
        if (inputDir.magnitude > 0)
        {
            Velocity.x = inputDir.x * speed;
            Velocity.z = inputDir.z * speed;
        }
        // Move 키 땠을 때
        else
        {
            Velocity.x *= friction;
            Velocity.z *= friction;
        }

        //Velocity.y = -2f;
    }

}