using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


public class SpringRailDataManager : MonoBehaviour
{
    // =========================================================
    // 선로 하나의 데이터
    // =========================================================

    [Serializable]
    public class RailData
    {
        [Header("구분용 이름")]
        public string railName;


        [Header("선로")]
        public Transform rail;


        [Header("선로 색상")]
        public SpringRailColor color;


        [Header("초기 Rotation")]
        [Tooltip(
            "게임 시작 시 적용할 Local Rotation입니다."
        )]
        public Vector3 initialRotation;


        [Header("정답 Rotation")]
        [Tooltip(
            "퍼즐 정답 상태의 Local Rotation입니다."
        )]
        public Vector3 correctRotation;


        [Header("버튼 1회 회전량")]
        [Tooltip(
            "버튼 한 번 눌렀을 때 변경되는 Local Rotation입니다.\n" +
            "현재 선로는 Y축 -30도 회전을 기본으로 사용합니다.\n" +
            "예: (0, -30, 0)"
        )]
        public Vector3 rotationStep =
            new Vector3(
                0f,
                -30f,
                0f
            );


        // 런타임에서 계산되는 값이므로
        // Scene에 저장하지 않음
        [NonSerialized]
        public bool isCorrect;
    }


    // =========================================================
    // 전체 선로
    // =========================================================

    [Header("전체 선로 12개")]
    [SerializeField]
    private RailData[] rails =
        new RailData[12];


    // =========================================================
    // 기본값
    // =========================================================

    private static readonly Vector3
        DefaultRotationStep =
            new Vector3(
                0f,
                -30f,
                0f
            );


    // =========================================================
    // 외부 접근
    // =========================================================

    public RailData[] Rails =>
        rails;


    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        ApplyInitialRotations();
    }


    // =========================================================
    // 색상별 선로 가져오기
    // =========================================================

    public RailData[] GetRailsByColor(
        SpringRailColor color
    )
    {
        if (rails == null)
        {
            return Array.Empty<RailData>();
        }


        return Array.FindAll(
            rails,
            data =>
                data != null &&
                data.color == color
        );
    }


    // =========================================================
    // 초기 Rotation 적용
    //
    // Position / Scale은 변경하지 않음
    // =========================================================

    public void ApplyInitialRotations()
    {
        if (rails == null)
        {
            return;
        }


        for (int i = 0; i < rails.Length; i++)
        {
            RailData data =
                rails[i];


            if (
                data == null ||
                data.rail == null
            )
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
    // 모든 선로 Rotation Step을
    // Y축 -30도로 한번에 설정
    //
    // 기존 Inspector에 Z -30 값이 저장되어 있을 때 사용
    // =========================================================

    [ContextMenu(
        "모든 선로 Rotation Step을 Y -30으로 설정"
    )]
    private void SetAllRotationStepsToDefault()
    {
        if (rails == null)
        {
            return;
        }


        int changedCount =
            0;


        for (int i = 0; i < rails.Length; i++)
        {
            RailData data =
                rails[i];


            if (data == null)
            {
                continue;
            }


            data.rotationStep =
                DefaultRotationStep;


            changedCount++;
        }


        Debug.Log(
            "[SpringRailData] 모든 선로 Rotation Step 설정 완료 : " +
            changedCount +
            "개 / (0, -30, 0)"
        );


        MarkSceneDirty();
    }


    // =========================================================
    // 현재 Rotation을 정답 Rotation으로 저장
    // =========================================================

    [ContextMenu(
        "현재 Rotation을 정답 Rotation으로 저장"
    )]
    private void SaveCurrentRotationAsCorrect()
    {
        if (rails == null)
        {
            return;
        }


        int savedCount =
            0;


        for (int i = 0; i < rails.Length; i++)
        {
            RailData data =
                rails[i];


            if (
                data == null ||
                data.rail == null
            )
            {
                continue;
            }


            data.correctRotation =
                NormalizeEuler(
                    data.rail.localEulerAngles
                );


            savedCount++;
        }


        Debug.Log(
            "[SpringRailData] 정답 Rotation 저장 완료 : " +
            savedCount +
            "개"
        );


        MarkSceneDirty();
    }


    // =========================================================
    // 현재 Rotation을 초기 Rotation으로 저장
    // =========================================================

    [ContextMenu(
        "현재 Rotation을 초기 Rotation으로 저장"
    )]
    private void SaveCurrentRotationAsInitial()
    {
        if (rails == null)
        {
            return;
        }


        int savedCount =
            0;


        for (int i = 0; i < rails.Length; i++)
        {
            RailData data =
                rails[i];


            if (
                data == null ||
                data.rail == null
            )
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
            "[SpringRailData] 초기 Rotation 저장 완료 : " +
            savedCount +
            "개"
        );


        MarkSceneDirty();
    }


    // =========================================================
    // Inspector에 입력한 초기 Rotation 적용
    // =========================================================

    [ContextMenu(
        "입력한 초기 Rotation 적용"
    )]
    private void ApplyInitialRotationFromMenu()
    {
        ApplyInitialRotations();


        Debug.Log(
            "[SpringRailData] 초기 Rotation 적용 완료"
        );


        MarkSceneDirty();
    }


    // =========================================================
    // Euler 정리
    //
    // 330 → -30
    // 270 → -90
    // =========================================================

    private Vector3 NormalizeEuler(
        Vector3 euler
    )
    {
        return new Vector3(
            NormalizeAngle(euler.x),
            NormalizeAngle(euler.y),
            NormalizeAngle(euler.z)
        );
    }


    private float NormalizeAngle(
        float angle
    )
    {
        angle =
            Mathf.Repeat(
                angle + 180f,
                360f
            ) - 180f;


        if (Mathf.Abs(angle) < 0.001f)
        {
            angle =
                0f;
        }


        return angle;
    }


    // =========================================================
    // Editor에서 변경사항 저장 처리
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
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (rails == null)
        {
            rails =
                new RailData[12];

            return;
        }


        if (rails.Length != 12)
        {
            Debug.LogWarning(
                "[SpringRailData] 선로는 12개를 사용합니다. " +
                "현재 등록 개수 : " +
                rails.Length,
                this
            );
        }
    }
}