using System.Collections;
using UnityEngine;


public class SpringRailButtonMotion : MonoBehaviour
{
    [Header("버튼 종류")]
    [SerializeField]
    private SpringRailColor railColor;


    [Header("회전 시간")]
    [SerializeField]
    private float rotationDuration = 0.2f;


    [Header("선로 상태 Manager")]
    [SerializeField]
    private SpringRailAngleManager railAngleManager;


    private bool isRotating = false;


    // =========================================================
    // 버튼 입력
    // =========================================================

    public void PressButton()
    {
        if (isRotating)
        {
            return;
        }


        if (railAngleManager == null)
        {
            Debug.LogWarning(
                "[SpringRailButtonMotion] RailAngleManager가 연결되지 않았습니다."
            );

            return;
        }


        if (railAngleManager.IsAllRailsCorrect)
        {
            return;
        }


        StartCoroutine(
            RotateRails()
        );
    }


    // =========================================================
    // 선로 회전
    // =========================================================

    private IEnumerator RotateRails()
    {
        isRotating =
            true;


        SpringRailAngleManager.RailAngleData[] rails =
            railAngleManager.GetRailsByColor(
                railColor
            );


        Quaternion[] startRotations =
            new Quaternion[rails.Length];


        Quaternion[] targetRotations =
            new Quaternion[rails.Length];


        // =====================================================
        // 목표 각도 계산
        // =====================================================

        for (int i = 0; i < rails.Length; i++)
        {
            Transform rail =
                rails[i].rail;


            if (rail == null)
            {
                continue;
            }


            startRotations[i] =
                rail.localRotation;


            targetRotations[i] =
                startRotations[i] *
                Quaternion.Euler(
                    0f,
                    0f,
                    rails[i].rotationStep
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
        // 최종 각도 고정
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


        isRotating =
            false;


        // =====================================================
        // 선로 정답 검사
        // =====================================================

        railAngleManager.RefreshRailState();
    }
}