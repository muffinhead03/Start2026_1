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
        if (railDataManager == null ||
            railStateManager == null)
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


        if (rails.Length == 0)
        {
            railStateManager.EndRotation();

            yield break;
        }


        Quaternion[] startRotations =
            new Quaternion[rails.Length];


        Quaternion[] targetRotations =
            new Quaternion[rails.Length];


        // =====================================================
        // 시작 / 목표 Rotation 저장
        // =====================================================

        for (int i = 0; i < rails.Length; i++)
        {
            if (rails[i].rail == null)
            {
                continue;
            }


            startRotations[i] =
                rails[i].rail.localRotation;


            Vector3 currentEuler =
                rails[i].rail.localEulerAngles;


            Vector3 targetEuler =
                currentEuler +
                rails[i].rotationStep;


            targetRotations[i] =
                Quaternion.Euler(
                    targetEuler
                );
        }


        // =====================================================
        // 회전
        // =====================================================

        float elapsed =
            0f;


        while (elapsed < rotationDuration)
        {
            elapsed +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    rotationDuration
                );


            for (int i = 0; i < rails.Length; i++)
            {
                if (rails[i].rail == null)
                {
                    continue;
                }


                rails[i].rail.localRotation =
                    Quaternion.Slerp(
                        startRotations[i],
                        targetRotations[i],
                        t
                    );
            }


            yield return null;
        }


        // =====================================================
        // 최종값 고정
        // =====================================================

        for (int i = 0; i < rails.Length; i++)
        {
            if (rails[i].rail == null)
            {
                continue;
            }


            rails[i].rail.localRotation =
                targetRotations[i];
        }


        // =====================================================
        // 정답 확인
        // =====================================================

        railStateManager.EndRotation();
    }
}