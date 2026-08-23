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
        [Header("구분용 이름")]
        public string railName;


        [Header("선로 오브젝트")]
        public Transform rail;


        [Header("선로 색상")]
        public SpringRailColor color;


        [Header("초기 각도")]
        [Tooltip("게임 시작 시 적용할 Z축 각도")]
        public float initialAngle;


        [Header("정답 각도")]
        [Tooltip("이 선로가 정답일 때의 Z축 각도")]
        public float correctAngle;


        [Header("버튼 1회 회전 각도")]
        [Tooltip("예: -30 또는 +30")]
        public float rotationStep = -30f;


        [HideInInspector]
        public bool isCorrect;
    }


    // =========================================================
    // 전체 선로
    // =========================================================

    [Header("전체 선로 12개")]
    [SerializeField]
    private RailAngleData[] rails =
        new RailAngleData[12];


    // =========================================================
    // 정답 판정
    // =========================================================

    [Header("각도 오차 허용 범위")]
    [SerializeField]
    private float angleTolerance = 1f;


    // =========================================================
    // 현재 상태
    // =========================================================

    private bool isInteractionLocked = false;

    private bool isRotationInProgress = false;


    // =========================================================
    // 외부 확인 Property
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

    private void Awake()
    {
        ApplyInitialAngles();
    }


    private void Start()
    {
        RefreshRailState();
    }


    // =========================================================
    // 초기 각도 적용
    // =========================================================

    private void ApplyInitialAngles()
    {
        if (rails == null)
        {
            return;
        }


        for (int i = 0; i < rails.Length; i++)
        {
            RailAngleData data =
                rails[i];


            if (data == null)
            {
                continue;
            }


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
        if (rails == null)
        {
            return
                Array.Empty<RailAngleData>();
        }


        return Array.FindAll(
            rails,
            data =>
                data != null &&
                data.color == color
        );
    }


    // =========================================================
    // 선로 회전 시작 요청
    //
    // SpringRailButtonMotion에서 사용
    // =========================================================

    public bool TryBeginRotation()
    {
        // 기차 이동 중 등으로
        // 퍼즐 전체 입력이 잠긴 상태
        if (isInteractionLocked)
        {
            return false;
        }


        // 이미 다른 버튼의 선로가 회전 중
        if (isRotationInProgress)
        {
            return false;
        }


        // 이미 정답 완료
        if (IsAllRailsCorrect)
        {
            return false;
        }


        isRotationInProgress =
            true;


        return true;
    }


    // =========================================================
    // 선로 회전 종료
    //
    // 회전 Motion이 완전히 끝난 후 호출
    // =========================================================

    public void EndRotation()
    {
        if (!isRotationInProgress)
        {
            return;
        }


        isRotationInProgress =
            false;


        // 회전이 끝난 순간에만
        // 전체 정답 검사
        RefreshRailState();
    }


    // =========================================================
    // 전체 선로 조작 잠금
    //
    // 기차 이동 시작 직전에 호출
    // =========================================================

    public void LockInteraction()
    {
        isInteractionLocked =
            true;


        Debug.Log(
            "[SpringRail] 선로 조작 잠금"
        );
    }


    // =========================================================
    // 선로 조작 잠금 해제
    //
    // 테스트 / 실패 처리 등이 필요할 경우 사용
    // =========================================================

    public void UnlockInteraction()
    {
        isInteractionLocked =
            false;


        Debug.Log(
            "[SpringRail] 선로 조작 잠금 해제"
        );
    }


    // =========================================================
    // 전체 선로 정답 검사
    // =========================================================

    public void RefreshRailState()
    {
        if (rails == null ||
            rails.Length == 0)
        {
            SetAllRailsCorrect(
                false
            );

            return;
        }


        bool allCorrect =
            true;


        for (int i = 0; i < rails.Length; i++)
        {
            RailAngleData data =
                rails[i];


            // 데이터 자체가 없음
            if (data == null)
            {
                allCorrect =
                    false;

                continue;
            }


            // Transform 연결 안 됨
            if (data.rail == null)
            {
                data.isCorrect =
                    false;


                allCorrect =
                    false;

                continue;
            }


            // 현재 각도
            float currentAngle =
                NormalizeAngle(
                    data.rail.localEulerAngles.z
                );


            // 정답 각도
            float correctAngle =
                NormalizeAngle(
                    data.correctAngle
                );


            // 0 / 360 문제까지 처리해서
            // 두 각도의 가장 짧은 차이 계산
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


        SetAllRailsCorrect(
            allCorrect
        );
    }


    // =========================================================
    // 전체 정답 상태 저장
    // =========================================================

    private void SetAllRailsCorrect(
        bool value
    )
    {
        bool stateChanged =
            IsAllRailsCorrect != value;


        IsAllRailsCorrect =
            value;


        if (!stateChanged)
        {
            return;
        }


        Debug.Log(
            "[SpringRail] 전체 선로 정답 상태 : " +
            IsAllRailsCorrect
        );


        // 상태 변경을 다른 Manager에 전달
        OnRailStateChanged?.Invoke(
            IsAllRailsCorrect
        );
    }


    // =========================================================
    // 특정 선로 현재 정답 여부 확인
    // =========================================================

    public bool IsRailCorrect(
        int index
    )
    {
        if (rails == null)
        {
            return false;
        }


        if (index < 0 ||
            index >= rails.Length)
        {
            return false;
        }


        if (rails[index] == null)
        {
            return false;
        }


        return
            rails[index].isCorrect;
    }


    // =========================================================
    // 각도 0 ~ 360 정규화
    // =========================================================

    private float NormalizeAngle(
        float angle
    )
    {
        return Mathf.Repeat(
            angle,
            360f
        );
    }


    // =========================================================
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (angleTolerance < 0f)
        {
            angleTolerance =
                0f;
        }


        if (rails != null &&
            rails.Length != 12)
        {
            // 반드시 12개여야 실행이 막히는 것은 아니지만
            // 현재 기획상 12개이므로 확인용
        }
    }
}