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
    // Rotation Setting
    // ================================================

    [Header("Door Rotation")]

    [Tooltip("문 회전축")]
    [SerializeField]
    private RotationAxis rotationAxis =
        RotationAxis.Y;


    [Tooltip("문이 열리는 각도")]
    [SerializeField]
    private float openAngle = 60f;


    [Tooltip(
        "열리는 방향\n" +
        "1 = 한 방향\n" +
        "-1 = 반대 방향"
    )]
    [SerializeField]
    private float openDirection = 1f;


    // ================================================
    // Animation
    // ================================================

    [Header("Animation")]

    [Tooltip("문 여닫는 시간")]
    [SerializeField]
    private float rotationDuration = 0.8f;


    [Tooltip("문 회전 애니메이션")]
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


    // ================================================
    // Rotation
    // ================================================

    private Quaternion closedRotation;

    private Quaternion openedRotation;


    private Coroutine moveCoroutine;


    // ================================================
    // Property
    // ================================================

    public bool IsOpen =>
        isOpen;


    public bool IsMoving =>
        isMoving;


    // ================================================
    // Start
    // ================================================

    private void Start()
    {
        // 현재 Scene에서 배치된 상태를
        // 닫힌 상태로 기억
        closedRotation =
            transform.localRotation;


        // 열린 상태 각도 계산
        openedRotation =
            CalculateOpenRotation();
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
    // ★ E 상호작용에서 호출
    //
    // 닫혀 있으면 → 열기
    // 열려 있으면 → 닫기
    // ================================================

    public void ToggleDoor()
    {
        // 움직이는 중에는 추가 입력 무시
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
    // 문 열기
    // ================================================

    public void OpenDoor()
    {
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
    // 문 닫기
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
    // 실제 문 회전
    // ================================================

    private IEnumerator RotateDoor(
        Quaternion targetRotation,
        bool targetOpenState)
    {
        isMoving = true;


        Quaternion startRotation =
            transform.localRotation;


        float elapsedTime = 0f;


        // 즉시 이동
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