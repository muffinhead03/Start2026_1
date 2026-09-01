using System;
using UnityEngine;

public class FiveDial_Number : MonoBehaviour
{
    // ================================================
    // 회전축
    // ================================================

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }


    // ================================================
    // 숫자별 각도 설정
    // 기존 각도 감지 / Snap 기능용
    // ================================================

    [Serializable]
    public class NumberAngleSetting
    {
        [Header("숫자")]
        [Range(0, 9)]
        public int number = 0;


        [Header("인식 각도 범위")]
        public float minAngle = 0f;

        public float maxAngle = 0f;


        [Header("자동 보정 각도")]
        public float snapAngle = 0f;
    }


    // ================================================
    // 현재 숫자 / 정답
    // ================================================

    [Header("Number")]

    [Range(0, 9)]
    [SerializeField]
    private int currentNumber = 2;


    [Range(0, 9)]
    [SerializeField]
    private int correctNumber = 0;


    // ================================================
    // 회전 설정
    // ================================================

    [Header("Rotation Setting")]

    [SerializeField]
    private RotationAxis rotationAxis =
        RotationAxis.Y;


    // ================================================
    // 기본 각도
    // ================================================

    [Header("Default Angle Generator")]

    [Tooltip("기준이 되는 숫자")]
    [Range(0, 9)]
    [SerializeField]
    private int defaultNumber = 2;


    [Tooltip("기준 숫자가 정중앙일 때의 각도")]
    [SerializeField]
    private float defaultAngle = -93f;


    [Tooltip("숫자 하나당 회전 각도")]
    [SerializeField]
    private float angleStep = 36f;


    [Tooltip("Snap 각도를 기준으로 ± 몇 도까지 해당 숫자로 인정할지")]
    [SerializeField]
    private float recognitionHalfRange = 18f;


    // ================================================
    // 각도 → 숫자 설정
    // ================================================

    [Header("Angle To Number Settings")]

    [SerializeField]
    private NumberAngleSetting[] angleSettings;


    // ================================================
    // Number Lock Manager
    // ================================================

    [Header("Number Lock Manager")]

    [SerializeField]
    private NumberLockManager numberLockManager;


    // ================================================
    // Property
    // ================================================

    public int CurrentNumber =>
        currentNumber;


    public int CorrectNumber =>
        correctNumber;


    public bool IsCorrect =>
        currentNumber == correctNumber;


    // ================================================
    // Start
    // ================================================

    private void Start()
    {
        /*
         * Angle Settings가 비어 있어도
         * 자동으로 생성해 준다.
         *
         * 따라서 Inspector에서 List is empty 상태여도
         * 게임 실행 시 정상 작동한다.
         */

        if (angleSettings == null ||
            angleSettings.Length != 10)
        {
            GenerateAngleSettings();
        }


        DetectInitialNumber();
    }


    // ================================================
    // Inspector용 Angle Setting 생성
    // ================================================

    [ContextMenu("Generate Angle Settings")]
    public void GenerateAngleSettings()
    {
        angleSettings =
            new NumberAngleSetting[10];


        for (int number = 0;
             number <= 9;
             number++)
        {
            float snapAngle =
                CalculateAngle(number);


            float minAngle =
                NormalizeAngle(
                    snapAngle -
                    recognitionHalfRange
                );


            float maxAngle =
                NormalizeAngle(
                    snapAngle +
                    recognitionHalfRange
                );


            angleSettings[number] =
                new NumberAngleSetting
                {
                    number = number,

                    minAngle = minAngle,

                    maxAngle = maxAngle,

                    snapAngle = snapAngle
                };
        }


        Debug.Log(
            $"[FiveDial] {gameObject.name} " +
            $"Angle Settings 생성 완료"
        );
    }


    // ================================================
    // 숫자 → 각도 계산
    // ================================================

    private float CalculateAngle(int number)
    {
        float angle =
            defaultAngle +
            (
                (number - defaultNumber) *
                angleStep
            );


        return NormalizeAngle(angle);
    }


    // ================================================
    // 현재 Dial 각도
    // ================================================

    public float GetCurrentAngle()
    {
        Vector3 euler =
            transform.localEulerAngles;


        float angle = 0f;


        switch (rotationAxis)
        {
            case RotationAxis.X:

                angle = euler.x;

                break;


            case RotationAxis.Y:

                angle = euler.y;

                break;


            case RotationAxis.Z:

                angle = euler.z;

                break;
        }


        return NormalizeAngle(angle);
    }


    // ================================================
    // 게임 시작 시 현재 숫자 감지
    // ================================================

    private void DetectInitialNumber()
    {
        if (angleSettings == null ||
            angleSettings.Length == 0)
        {
            return;
        }


        float currentAngle =
            GetCurrentAngle();


        foreach (
            NumberAngleSetting setting
            in angleSettings)
        {
            if (!IsAngleInRange(
                    currentAngle,
                    setting.minAngle,
                    setting.maxAngle))
            {
                continue;
            }


            currentNumber =
                setting.number;


            Debug.Log(
                $"[FiveDial] {gameObject.name} / " +
                $"초기 각도 = {currentAngle} / " +
                $"현재 숫자 = {currentNumber}"
            );


            return;
        }


        Debug.LogWarning(
            $"[FiveDial] {gameObject.name} / " +
            $"초기 각도 {currentAngle}에 " +
            $"해당하는 숫자를 찾지 못했습니다."
        );
    }


    // ================================================
    // 현재 숫자 변경
    // ================================================

    public void SetCurrentNumber(
        int number)
    {
        currentNumber =
            Mathf.Clamp(
                number,
                0,
                9
            );


        Debug.Log(
            $"[FiveDial] {gameObject.name} / " +
            $"현재 숫자 = {currentNumber} / " +
            $"정답 숫자 = {correctNumber} / " +
            $"정답 여부 = {IsCorrect}"
        );


        // 숫자가 변경될 때마다
        // NumberLock 정답 확인
        if (numberLockManager != null)
        {
            numberLockManager.NotifyDialChanged();
        }
    }


    // ================================================
    // ★ UI 버튼에서 사용하는 핵심 함수
    //
    // FiveDial_NumberAdjust가
    // 이 함수만 호출하면 됨.
    // ================================================

    public void SetNumberAndSnap(
        int number)
    {
        // --------------------------------
        // 0 ~ 9 순환
        //
        // -1 → 9
        // 10 → 0
        // --------------------------------

        number =
            (number + 10) % 10;


        // --------------------------------
        // 숫자에 해당하는 각도 계산
        // --------------------------------

        float snapAngle =
            CalculateAngle(number);


        // --------------------------------
        // 실제 Dial 회전
        // --------------------------------

        SnapToAngle(
            snapAngle
        );


        // --------------------------------
        // 숫자 갱신
        // 정답 검사도 여기서 실행됨
        // --------------------------------

        SetCurrentNumber(
            number
        );


        Debug.Log(
            $"[FiveDial] {gameObject.name} → " +
            $"숫자 {number} / " +
            $"Snap Angle = {snapAngle}"
        );
    }


    // ================================================
    // 기존 방식
    // 현재 각도 확인 후 가장 가까운 숫자로 Snap
    // ================================================

    public void TrySnapCurrentAngle()
    {
        float currentAngle =
            GetCurrentAngle();


        Debug.Log(
            $"[FiveDial] {gameObject.name} / " +
            $"현재 각도 = {currentAngle}"
        );


        if (angleSettings == null ||
            angleSettings.Length == 0)
        {
            GenerateAngleSettings();
        }


        foreach (
            NumberAngleSetting setting
            in angleSettings)
        {
            if (!IsAngleInRange(
                    currentAngle,
                    setting.minAngle,
                    setting.maxAngle))
            {
                continue;
            }


            SnapToAngle(
                setting.snapAngle
            );


            SetCurrentNumber(
                setting.number
            );


            Debug.Log(
                $"[FiveDial] {gameObject.name} → " +
                $"숫자 {setting.number} / " +
                $"Snap Angle = {setting.snapAngle}"
            );


            return;
        }


        Debug.LogWarning(
            $"[FiveDial] {gameObject.name} / " +
            $"현재 각도 {currentAngle}에 " +
            $"해당하는 숫자가 없습니다."
        );
    }


    // ================================================
    // 각도 범위 검사
    // ================================================

    private bool IsAngleInRange(
        float angle,
        float minAngle,
        float maxAngle)
    {
        // 일반적인 범위
        if (minAngle <= maxAngle)
        {
            return
                angle >= minAngle &&
                angle <= maxAngle;
        }


        // -180 / 180 경계를 넘어가는 경우
        return
            angle >= minAngle ||
            angle <= maxAngle;
    }


    // ================================================
    // 지정 각도로 실제 Transform 회전
    // ================================================

    private void SnapToAngle(
        float angle)
    {
        Vector3 euler =
            transform.localEulerAngles;


        switch (rotationAxis)
        {
            case RotationAxis.X:

                euler.x = angle;

                break;


            case RotationAxis.Y:

                euler.y = angle;

                break;


            case RotationAxis.Z:

                euler.z = angle;

                break;
        }


        transform.localEulerAngles =
            euler;
    }


    // ================================================
    // 각도 -180 ~ 180 정규화
    // ================================================

    private float NormalizeAngle(
        float angle)
    {
        return Mathf.DeltaAngle(
            0f,
            angle
        );
    }
}