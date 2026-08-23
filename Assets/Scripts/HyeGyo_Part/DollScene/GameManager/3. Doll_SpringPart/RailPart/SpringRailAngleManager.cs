using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


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


        // =====================================================
        // 초기 Rotation
        // =====================================================

        [Header("초기 Rotation")]
        [Tooltip(
            "게임 시작 시 적용할 Local Rotation입니다.\n" +
            "X / Y / Z 값을 직접 입력하세요."
        )]
        public Vector3 initialRotation;


        // =====================================================
        // 정답 Rotation
        // =====================================================

        [Header("정답 Rotation")]
        [Tooltip(
            "정답 상태의 Local Rotation입니다.\n" +
            "컴포넌트의 ⋮ 메뉴에서 현재 Rotation을 자동 저장할 수 있습니다."
        )]
        public Vector3 correctRotation;


        // =====================================================
        // 버튼 1회 회전량
        // =====================================================

        [Header("버튼 1회 회전량")]
        [Tooltip(
            "버튼 한 번 눌렀을 때 회전시킬 X / Y / Z 값입니다.\n" +
            "예: Z축 시계방향 30도 = (0, 0, -30)"
        )]
        public Vector3 rotationStep =
            new Vector3(
                0f,
                0f,
                -30f
            );


        // =====================================================
        // 현재 정답 여부
        // =====================================================

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

    [Header("정답 Rotation 허용 오차")]
    [Tooltip(
        "현재 Rotation과 정답 Rotation 사이의 전체 각도 차이입니다."
    )]
    [SerializeField]
    private float rotationTolerance = 1f;


    // =========================================================
    // 현재 상태
    // =========================================================

    private bool isInteractionLocked =
        false;


    private bool isRotationInProgress =
        false;


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
        // 게임 시작 시
        // Inspector에 입력된 초기 Rotation 적용
        ApplyInitialRotations();
    }


    private void Start()
    {
        RefreshRailState();
    }


    // =========================================================
    // 초기 Rotation 적용
    //
    // Position / Scale은 건드리지 않음
    // Rotation만 변경
    // =========================================================

    private void ApplyInitialRotations()
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


            data.rail.localRotation =
                Quaternion.Euler(
                    data.initialRotation
                );
        }
    }


    // =========================================================
    // ⋮ 메뉴
    //
    // 입력한 Initial Rotation을
    // 실제 선로에 적용
    // =========================================================

    [ContextMenu("입력한 초기 Rotation 적용")]
    private void ApplyInitialRotationsFromMenu()
    {
        ApplyInitialRotations();


        RefreshRailState();


        Debug.Log(
            "[SpringRail] 입력한 Initial Rotation을 적용했습니다."
        );


        MarkSceneDirty();
    }


    // =========================================================
    // ⋮ 메뉴
    //
    // 현재 선로의 X / Y / Z Rotation 전체를
    // Correct Rotation에 저장
    // =========================================================

    [ContextMenu("현재 Rotation을 정답 Rotation으로 저장")]
    private void SaveCurrentRotationsAsCorrect()
    {
        if (rails == null)
        {
            return;
        }


        int savedCount =
            0;


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


            // 현재 Inspector에 표시되는
            // Local Rotation X/Y/Z를 저장
            data.correctRotation =
                NormalizeEuler(
                    data.rail.localEulerAngles
                );


            savedCount++;
        }


        RefreshRailState();


        Debug.Log(
            "[SpringRail] 현재 Rotation을 정답으로 저장 완료. " +
            "저장 개수 : " +
            savedCount
        );


        MarkSceneDirty();
    }


    // =========================================================
    // ⋮ 메뉴
    //
    // 현재 Rotation을 Initial Rotation으로 저장
    //
    // 필요할 때 편하게 사용
    // =========================================================

    [ContextMenu("현재 Rotation을 초기 Rotation으로 저장")]
    private void SaveCurrentRotationsAsInitial()
    {
        if (rails == null)
        {
            return;
        }


        int savedCount =
            0;


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


            data.initialRotation =
                NormalizeEuler(
                    data.rail.localEulerAngles
                );


            savedCount++;
        }


        Debug.Log(
            "[SpringRail] 현재 Rotation을 초기값으로 저장 완료. " +
            "저장 개수 : " +
            savedCount
        );


        MarkSceneDirty();
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
    // =========================================================

    public bool TryBeginRotation()
    {
        // 기차 이동 중
        if (isInteractionLocked)
        {
            return false;
        }


        // 이미 다른 선로가 회전 중
        if (isRotationInProgress)
        {
            return false;
        }


        // 이미 퍼즐 정답
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
    // =========================================================

    public void EndRotation()
    {
        if (!isRotationInProgress)
        {
            return;
        }


        isRotationInProgress =
            false;


        // 회전 완료 후 정답 검사
        RefreshRailState();
    }


    // =========================================================
    // 선로 조작 잠금
    //
    // 기차 이동 시작 시 사용
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
            // 두 Rotation 사이의 실제 각도 차이
            //
            // Euler X/Y/Z를 각각 비교하는 것보다
            // Quaternion.Angle이 훨씬 안전함
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


        SetAllRailsCorrect(
            allCorrect
        );
    }


    // =========================================================
    // 전체 정답 상태 변경
    // =========================================================

    private void SetAllRailsCorrect(
        bool value
    )
    {
        bool stateChanged =
            IsAllRailsCorrect !=
            value;


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


        OnRailStateChanged?.Invoke(
            IsAllRailsCorrect
        );
    }


    // =========================================================
    // 특정 선로 정답 여부
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
    // Euler 정리
    //
    // Unity의 0~360 값을 보기 편하게
    // -180 ~ 180 범위로 변경
    //
    // 예:
    // 330 → -30
    // 270 → -90
    // =========================================================

    private Vector3 NormalizeEuler(
        Vector3 euler
    )
    {
        return new Vector3(
            NormalizeSingleAngle(
                euler.x
            ),
            NormalizeSingleAngle(
                euler.y
            ),
            NormalizeSingleAngle(
                euler.z
            )
        );
    }


    private float NormalizeSingleAngle(
        float angle
    )
    {
        angle =
            Mathf.Repeat(
                angle + 180f,
                360f
            ) - 180f;


        // -0 방지
        if (Mathf.Abs(angle) < 0.001f)
        {
            angle =
                0f;
        }


        return angle;
    }


    // =========================================================
    // Editor 변경 저장
    // =========================================================

    private void MarkSceneDirty()
    {
#if UNITY_EDITOR

        if (Application.isPlaying)
        {
            return;
        }


        EditorUtility.SetDirty(
            this
        );


        if (gameObject.scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(
                gameObject.scene
            );
        }

#endif
    }


    // =========================================================
    // Inspector 값 검사
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