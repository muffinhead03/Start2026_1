using System.Collections;
using UnityEngine;


public class SpringRailRotationController : MonoBehaviour
{
    // =========================================================
    // Manager
    // =========================================================

    [Header("선로 데이터 Manager")]
    [SerializeField]
    private SpringRailDataManager railDataManager;


    [Header("선로 상태 Manager")]
    [SerializeField]
    private SpringRailStateManager railStateManager;


    // =========================================================
    // Motion
    // =========================================================

    [Header("회전 시간")]
    [SerializeField]
    private float rotationDuration = 0.2f;


    // =========================================================
    // 버튼에서 호출
    // =========================================================

    public void PressRed()
    {
        RotateColor(
            SpringRailColor.Red
        );
    }


    public void PressYellow()
    {
        RotateColor(
            SpringRailColor.Yellow
        );
    }


    public void PressGreen()
    {
        RotateColor(
            SpringRailColor.Green
        );
    }


    // =========================================================
    // 색깔 회전 요청
    // =========================================================

    public void RotateColor(
        SpringRailColor color
    )
    {
        if (
            railDataManager == null ||
            railStateManager == null
        )
        {
            return;
        }


        if (!railStateManager.TryBeginRotation())
        {
            return;
        }


        StartCoroutine(
            RotateRails(
                color
            )
        );
    }


    // =========================================================
    // 실제 Motion
    // =========================================================

    private IEnumerator RotateRails(
        SpringRailColor color
    )
    {
        SpringRailDataManager.RailData[] rails =
            railDataManager.GetRailsByColor(
                color
            );


        if (rails == null || rails.Length == 0)
        {
            railStateManager.EndRotation();

            yield break;
        }


        Quaternion[] startRotations =
            new Quaternion[rails.Length];

        Quaternion[] targetRotations =
            new Quaternion[rails.Length];


        // =====================================================
        // 시작 / 목표 Rotation 계산
        // =====================================================

        for (int i = 0; i < rails.Length; i++)
        {
            SpringRailDataManager.RailData data =
                rails[i];

            if (
                data == null ||
                data.rail == null
            )
            {
                continue;
            }


            startRotations[i] =
                data.rail.localRotation;


            Vector3 startEuler =
                NormalizeEuler(
                    data.rail.localEulerAngles
                );

            Vector3 targetEuler =
                NormalizeEuler(
                    startEuler +
                    data.rotationStep
                );


            targetRotations[i] =
                Quaternion.Euler(
                    targetEuler
                );
        }


        // =====================================================
        // 회전
        // =====================================================

        if (rotationDuration <= 0f)
        {
            for (int i = 0; i < rails.Length; i++)
            {
                SpringRailDataManager.RailData data =
                    rails[i];

                if (
                    data == null ||
                    data.rail == null
                )
                {
                    continue;
                }


                data.rail.localRotation =
                    targetRotations[i];

                RemoveTinyRotationError(
                    data.rail
                );
            }


            railStateManager.EndRotation();

            yield break;
        }


        float elapsed = 0f;


        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    rotationDuration
                );


            for (int i = 0; i < rails.Length; i++)
            {
                SpringRailDataManager.RailData data =
                    rails[i];

                if (
                    data == null ||
                    data.rail == null
                )
                {
                    continue;
                }


                data.rail.localRotation =
                    Quaternion.Slerp(
                        startRotations[i],
                        targetRotations[i],
                        t
                    );
            }


            yield return null;
        }


        // =====================================================
        // 최종값 정확히 고정
        // =====================================================

        for (int i = 0; i < rails.Length; i++)
        {
            SpringRailDataManager.RailData data =
                rails[i];

            if (
                data == null ||
                data.rail == null
            )
            {
                continue;
            }


            data.rail.localRotation =
                targetRotations[i];

            RemoveTinyRotationError(
                data.rail
            );
        }


        // =====================================================
        // 정답 확인
        // =====================================================

        railStateManager.EndRotation();
    }


    // =========================================================
    // 아주 작은 각도 오차 제거
    // =========================================================

    private void RemoveTinyRotationError(
        Transform rail
    )
    {
        if (rail == null)
        {
            return;
        }


        Vector3 euler =
            NormalizeEuler(
                rail.localEulerAngles
            );


        euler.x =
            SnapTinyAngle(
                euler.x
            );

        euler.y =
            SnapTinyAngle(
                euler.y
            );

        euler.z =
            SnapTinyAngle(
                euler.z
            );


        rail.localRotation =
            Quaternion.Euler(
                euler
            );
    }


    private float SnapTinyAngle(
        float angle
    )
    {
        float rounded =
            Mathf.Round(angle * 1000f) /
            1000f;


        if (Mathf.Abs(rounded) < 0.001f)
        {
            return 0f;
        }


        if (Mathf.Abs(rounded - 360f) < 0.001f)
        {
            return 0f;
        }


        return rounded;
    }


    // =========================================================
    // Euler 정리
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
            angle = 0f;
        }


        return angle;
    }


    // =========================================================
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (rotationDuration < 0f)
        {
            rotationDuration = 0f;
        }
    }
}