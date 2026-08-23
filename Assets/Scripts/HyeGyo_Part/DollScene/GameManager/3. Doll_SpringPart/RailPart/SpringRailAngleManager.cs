using System;
using UnityEngine;


public class SpringRailAngleManager : MonoBehaviour
{
    // =========================================================
    // 선로 하나의 정보
    // =========================================================

    [Serializable]
    public class RailAngleData
    {
        [Header("선로 오브젝트")]
        public Transform rail;


        [Header("선로 색상")]
        public SpringRailColor color;


        [Header("초기 각도")]
        public float initialAngle;


        [Header("정답 각도")]
        public float correctAngle;


        [Header("버튼 1회 회전 각도")]
        public float rotationStep = -30f;


        [HideInInspector]
        public bool isCorrect;
    }


    // =========================================================
    // 전체 선로
    // =========================================================

    [Header("전체 선로 12개")]
    [SerializeField]
    private RailAngleData[] rails;


    // =========================================================
    // 정답 판정 설정
    // =========================================================

    [Header("각도 오차 허용 범위")]
    [SerializeField]
    private float angleTolerance = 1f;


    // =========================================================
    // 현재 퍼즐 정답 여부
    // =========================================================

    public bool IsAllRailsCorrect
    {
        get;
        private set;
    }


    // =========================================================
    // 상태 변경 Event
    // =========================================================

    public event Action<bool> OnRailStateChanged;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        ApplyInitialAngles();

        RefreshRailState();
    }


    // =========================================================
    // 초기 각도 적용
    // =========================================================

    private void ApplyInitialAngles()
    {
        foreach (RailAngleData data in rails)
        {
            if (data.rail == null)
            {
                continue;
            }


            Vector3 euler =
                data.rail.localEulerAngles;


            euler.z =
                NormalizeAngle(
                    data.initialAngle
                );


            data.rail.localEulerAngles =
                euler;
        }
    }


    // =========================================================
    // 특정 색깔 선로 가져오기
    // =========================================================

    public RailAngleData[] GetRailsByColor(
        SpringRailColor color
    )
    {
        return Array.FindAll(
            rails,
            data => data.color == color
        );
    }


    // =========================================================
    // 전체 선로 정답 검사
    // =========================================================

    public void RefreshRailState()
    {
        bool allCorrect =
            true;


        foreach (RailAngleData data in rails)
        {
            if (data.rail == null)
            {
                allCorrect =
                    false;

                continue;
            }


            float currentAngle =
                NormalizeAngle(
                    data.rail.localEulerAngles.z
                );


            float correctAngle =
                NormalizeAngle(
                    data.correctAngle
                );


            float difference =
                Mathf.Abs(
                    Mathf.DeltaAngle(
                        currentAngle,
                        correctAngle
                    )
                );


            data.isCorrect =
                difference <= angleTolerance;


            if (!data.isCorrect)
            {
                allCorrect =
                    false;
            }
        }


        bool stateChanged =
            IsAllRailsCorrect != allCorrect;


        IsAllRailsCorrect =
            allCorrect;


        if (stateChanged)
        {
            OnRailStateChanged?.Invoke(
                IsAllRailsCorrect
            );
        }
    }


    // =========================================================
    // 각도 0 ~ 360 정규화
    // =========================================================

    private float NormalizeAngle(
        float angle
    )
    {
        angle %=
            360f;


        if (angle < 0f)
        {
            angle +=
                360f;
        }


        return angle;
    }
}