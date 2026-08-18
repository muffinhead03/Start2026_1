using System;
using UnityEngine;

public class FiveDial_Number : MonoBehaviour
{
    // 회전축
    public enum RotationAxis
    {   X,Y,Z    }

    // 숫자별 각도 설정
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

    // 현재 숫자 / 정답
    [Header("Number")]
    [Range(0, 9)]
    [SerializeField]
    private int currentNumber = 2;
    [Range(0, 9)]
    [SerializeField]
    private int correctNumber = 0;

    // 회전 설정
    [Header("Rotation Setting")]
    [SerializeField]
    private RotationAxis rotationAxis = RotationAxis.Y;

    // 기본 각도 생성 설정
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

    // 각도 → 숫자 설정
    [Header("Angle To Number Settings")]

    [SerializeField]
    private NumberAngleSetting[] angleSettings;

    // Number Lock Manager
    [Header("Number Lock Manager")]

    [SerializeField]
    private NumberLockManager numberLockManager;


    // Property
    public int CurrentNumber => currentNumber;
    public int CorrectNumber => correctNumber;
    public bool IsCorrect => currentNumber == correctNumber;


    // 시작 시 현재 숫자 확인
    private void Start()
    {
        DetectInitialNumber();
    }

    // Inspector용 기본 Angle Setting 생성
     [ContextMenu("Generate Angle Settings")]
    public void GenerateAngleSettings()
    {
        angleSettings =
            new NumberAngleSetting[10];


        for (int number = 0; number <= 9; number++)
        {
            float snapAngle = defaultAngle + ((number - defaultNumber) * angleStep);

            snapAngle = NormalizeAngle(snapAngle);

            float minAngle = NormalizeAngle(snapAngle - recognitionHalfRange);

            float maxAngle = NormalizeAngle(snapAngle + recognitionHalfRange);

            angleSettings[number] =
                new NumberAngleSetting
                {
                    number = number,
                    minAngle = minAngle,
                    maxAngle = maxAngle,
                    snapAngle = snapAngle
                };
        }

        Debug.Log($"[FiveDial] {gameObject.name} " + "Angle Settings 생성 완료");
    }

    // 현재 다이얼 각도 가져오기
    public float GetCurrentAngle()
    {
        Vector3 euler = transform.localEulerAngles;

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


    // 게임 시작 시 현재 숫자 판단
    private void DetectInitialNumber()
    {
        if (angleSettings == null || angleSettings.Length == 0)
        {return;}

        float currentAngle = GetCurrentAngle();

        foreach (NumberAngleSetting setting in angleSettings)
        {
            if (IsAngleInRange(
                    currentAngle,
                    setting.minAngle,
                    setting.maxAngle))
            {
                currentNumber =
                    setting.number;


                Debug.Log(
                    $"[FiveDial] {gameObject.name} / " + $"초기 각도 = {currentAngle} / " + $"현재 숫자 = {currentNumber}");
                return;
            }
        }
    }

    // 현재 숫자 변경
    public void SetCurrentNumber(int number)
    {
        currentNumber =
            Mathf.Clamp(number, 0, 9);
        Debug.Log($"[FiveDial] {gameObject.name} / " + $"현재 숫자 = {currentNumber} / " + $"정답 숫자 = {correctNumber} / " + $"정답 여부 = {IsCorrect}");

        if (numberLockManager != null)
        {
            numberLockManager.NotifyDialChanged();
        }
    }

    // 현재 각도 확인 후 Snap
    public void TrySnapCurrentAngle()
    {
        float currentAngle =
            GetCurrentAngle();


        Debug.Log($"[FiveDial] {gameObject.name} " +$"현재 각도 = {currentAngle}");

        if (angleSettings == null || angleSettings.Length == 0)
        {
            Debug.LogWarning($"[FiveDial] {gameObject.name} " +"Angle Setting이 없습니다.");
            return;
        }


        foreach (NumberAngleSetting setting in angleSettings)
        {
            if (!IsAngleInRange(currentAngle,setting.minAngle,setting.maxAngle))
            {
                continue;
            }

            SnapToAngle(setting.snapAngle);
            SetCurrentNumber(setting.number);


            Debug.Log(
                $"[FiveDial] {gameObject.name} → " +
                $"숫자 {setting.number} / " +
                $"Snap Angle = {setting.snapAngle}"
            );


            return;
        }


        Debug.Log(
            $"[FiveDial] {gameObject.name} " +
            $"현재 각도 {currentAngle}에 해당하는 숫자가 없습니다."
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
        // 일반 범위
        if (minAngle <= maxAngle)
        {
            return angle >= minAngle &&
                   angle <= maxAngle;
        }


        // -180 / 180 경계를 넘는 범위
        //
        // 예:
        // Min = 177
        // Max = -147
        return angle >= minAngle ||
               angle <= maxAngle;
    }


    // ================================================
    // 지정 각도로 Snap
    // ================================================

    private void SnapToAngle(float angle)
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

    private float NormalizeAngle(float angle)
    {
        return Mathf.DeltaAngle(
            0f,
            angle
        );
    }
}