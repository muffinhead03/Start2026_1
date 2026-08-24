using System;
using UnityEngine;


public class SpringRailStateManager : MonoBehaviour
{
    // =========================================================
    // 선로 데이터
    // =========================================================

    [Header("선로 데이터 Manager")]
    [SerializeField]
    private SpringRailDataManager railDataManager;


    // =========================================================
    // 정답 판정
    // =========================================================

    [Header("Rotation 허용 오차")]
    [SerializeField]
    private float rotationTolerance = 1f;


    // =========================================================
    // 상태
    // =========================================================

    private bool isInteractionLocked =
        false;


    private bool isRotationInProgress =
        false;


    // =========================================================
    // Property
    // =========================================================

    public bool IsAllRailsCorrect
    {
        get;
        private set;
    }


    public bool IsInteractionLocked =>
        isInteractionLocked;


    public bool IsRotationInProgress =>
        isRotationInProgress;


    // =========================================================
    // Event
    // =========================================================

    public event Action<bool> OnRailStateChanged;


    // =========================================================
    // Unity
    // =========================================================

    private void Start()
    {
        RefreshRailState();
    }


    // =========================================================
    // 버튼 회전 시작 가능 여부
    // =========================================================

    public bool TryBeginRotation()
    {
        // 기차 이동 중
        if (isInteractionLocked)
        {
            return false;
        }


        // 이미 다른 색상의 선로 회전 중
        if (isRotationInProgress)
        {
            return false;
        }


        // 이미 퍼즐 완료
        if (IsAllRailsCorrect)
        {
            return false;
        }


        isRotationInProgress =
            true;


        return true;
    }


    // =========================================================
    // 회전 종료
    // =========================================================

    public void EndRotation()
    {
        isRotationInProgress =
            false;


        RefreshRailState();
    }


    // =========================================================
    // 선로 조작 잠금
    //
    // 기차 이동 시작 시 호출
    // =========================================================

    public void LockInteraction()
    {
        isInteractionLocked =
            true;


        Debug.Log(
            "[SpringRailState] 선로 입력 잠금"
        );
    }


    // =========================================================
    // 선로 조작 잠금 해제
    // =========================================================

    public void UnlockInteraction()
    {
        isInteractionLocked =
            false;


        Debug.Log(
            "[SpringRailState] 선로 입력 잠금 해제"
        );
    }


    // =========================================================
    // 전체 선로 정답 검사
    // =========================================================

    public void RefreshRailState()
    {
        if (railDataManager == null)
        {
            Debug.LogWarning(
                "[SpringRailState] RailDataManager가 연결되지 않았습니다."
            );


            SetCorrectState(
                false
            );


            return;
        }


        SpringRailDataManager.RailData[] rails =
            railDataManager.Rails;


        if (rails == null ||
            rails.Length == 0)
        {
            SetCorrectState(
                false
            );


            return;
        }


        bool allCorrect =
            true;


        for (int i = 0; i < rails.Length; i++)
        {
            SpringRailDataManager.RailData data =
                rails[i];


            if (data == null)
            {
                allCorrect =
                    false;

                continue;
            }


            if (data.rail == null)
            {
                data.isCorrect =
                    false;


                allCorrect =
                    false;

                continue;
            }


            // =================================================
            // 현재 Rotation
            // =================================================

            Quaternion currentRotation =
                data.rail.localRotation;


            // =================================================
            // 정답 Rotation
            // =================================================

            Quaternion correctRotation =
                Quaternion.Euler(
                    data.correctRotation
                );


            // =================================================
            // 현재 / 정답 Rotation 차이
            // =================================================

            float difference =
                Quaternion.Angle(
                    currentRotation,
                    correctRotation
                );


            data.isCorrect =
                difference <=
                rotationTolerance;


            if (!data.isCorrect)
            {
                allCorrect =
                    false;
            }
        }


        SetCorrectState(
            allCorrect
        );
    }


    // =========================================================
    // 정답 상태 저장
    // =========================================================

    private void SetCorrectState(
        bool value
    )
    {
        bool changed =
            IsAllRailsCorrect !=
            value;


        IsAllRailsCorrect =
            value;


        if (!changed)
        {
            return;
        }


        Debug.Log(
            "[SpringRailState] 전체 선로 정답 : " +
            IsAllRailsCorrect
        );


        OnRailStateChanged?.Invoke(
            IsAllRailsCorrect
        );
    }


    // =========================================================
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (rotationTolerance < 0f)
        {
            rotationTolerance =
                0f;
        }
    }
}