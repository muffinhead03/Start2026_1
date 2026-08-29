using System.Collections;
using UnityEngine;

public class Door_OpenClose : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }


    // ================================================
    // Lock State
    // ================================================

    [Header("Lock State")]

    [Tooltip("게임 시작 시 문이 잠겨있는지")]
    [SerializeField]
    private bool isLocked = true;


    // ================================================
    // Door Rotation
    // ================================================

    [Header("Door Rotation")]

    [SerializeField]
    private RotationAxis rotationAxis =
        RotationAxis.Y;


    [Tooltip("문이 열리는 각도")]
    [SerializeField]
    private float openAngle = 60f;


    [Tooltip("열리는 방향. 1 또는 -1")]
    [SerializeField]
    private float openDirection = 1f;


    // ================================================
    // Animation
    // ================================================

    [Header("Animation")]

    [SerializeField]
    private float rotationDuration = 0.8f;


    [SerializeField]
    private AnimationCurve rotationCurve =
        AnimationCurve.EaseInOut(
            0f,
            0f,
            1f,
            1f
        );


    // ================================================
    // State
    // ================================================

    [Header("State")]

    [SerializeField]
    private bool isOpen = false;


    [SerializeField]
    private bool isMoving = false;


    private Quaternion closedRotation;

    private Quaternion openedRotation;

    private Coroutine moveCoroutine;


    // ================================================
    // Property
    // ================================================

    public bool IsLocked => isLocked;

    public bool IsOpen => isOpen;

    public bool IsMoving => isMoving;


    // ================================================
    // Start
    // ================================================

    private void Start()
    {
        // 현재 DoorPivot 방향을 닫힌 상태로 기억
        closedRotation =
            transform.localRotation;


        openedRotation =
            CalculateOpenRotation();


        Debug.Log(
            $"[Door] 초기화 / " +
            $"Locked = {isLocked} / " +
            $"Open = {isOpen}"
        );
    }


    // ================================================
    // 자물쇠 퍼즐 해결 시 호출
    // ================================================

    public void UnlockDoor()
    {
        isLocked = false;


        Debug.Log(
            $"[Door] {gameObject.name} 잠금 해제 / " +
            $"IsLocked = {isLocked}"
        );
    }


    // ================================================
    // 필요하면 다시 잠그기
    // ================================================

    public void LockDoor()
    {
        isLocked = true;


        Debug.Log(
            $"[Door] {gameObject.name} 잠금"
        );
    }


    // ================================================
    // E 상호작용
    // ================================================

    public void ToggleDoor()
    {
        Debug.Log(
            $"[Door] ToggleDoor 호출 / " +
            $"Locked = {isLocked} / " +
            $"Open = {isOpen} / " +
            $"Moving = {isMoving}"
        );


        // 잠겨있으면 열 수 없음
        if (isLocked)
        {
            Debug.Log(
                $"[Door] {gameObject.name} 문이 잠겨있습니다."
            );

            return;
        }


        if (isMoving)
            return;


        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }


    // ================================================
    // Open
    // ================================================

    public void OpenDoor()
    {
        if (isLocked)
            return;


        if (isMoving)
            return;


        if (isOpen)
            return;


        StartRotation(
            openedRotation,
            true
        );
    }


    // ================================================
    // Close
    // ================================================

    public void CloseDoor()
    {
        if (isMoving)
            return;


        if (!isOpen)
            return;


        StartRotation(
            closedRotation,
            false
        );
    }


    // ================================================
    // 열린 상태 Rotation 계산
    // ================================================

    private Quaternion CalculateOpenRotation()
    {
        float angle =
            openAngle *
            openDirection;


        Vector3 rotation =
            Vector3.zero;


        switch (rotationAxis)
        {
            case RotationAxis.X:

                rotation.x = angle;

                break;


            case RotationAxis.Y:

                rotation.y = angle;

                break;


            case RotationAxis.Z:

                rotation.z = angle;

                break;
        }


        return
            closedRotation *
            Quaternion.Euler(rotation);
    }


    // ================================================
    // Rotation 시작
    // ================================================

    private void StartRotation(
        Quaternion targetRotation,
        bool targetOpenState)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );
        }


        moveCoroutine =
            StartCoroutine(
                RotateDoor(
                    targetRotation,
                    targetOpenState
                )
            );
    }


    // ================================================
    // 실제 회전
    // ================================================

    private IEnumerator RotateDoor(
        Quaternion targetRotation,
        bool targetOpenState)
    {
        isMoving = true;


        Quaternion startRotation =
            transform.localRotation;


        float elapsedTime = 0f;


        if (rotationDuration <= 0f)
        {
            transform.localRotation =
                targetRotation;


            isOpen =
                targetOpenState;


            isMoving = false;

            moveCoroutine = null;

            yield break;
        }


        while (elapsedTime < rotationDuration)
        {
            elapsedTime +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsedTime /
                    rotationDuration
                );


            t =
                rotationCurve.Evaluate(t);


            transform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );


            yield return null;
        }


        transform.localRotation =
            targetRotation;


        isOpen =
            targetOpenState;


        isMoving = false;

        moveCoroutine = null;


        Debug.Log(
            $"[Door] {gameObject.name} → " +
            $"{(isOpen ? "OPEN" : "CLOSE")}"
        );
    }
}