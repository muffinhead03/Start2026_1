using System.Collections;
using UnityEngine;

// 사용법:
// 1. 빈 오브젝트 하나 만들고 이 스크립트 추가
// 2. cameraTransform = Main Camera
// 3. focusPoint = 빈 칸(슬롯 영역)을 바라보는 위치/각도로 미리 배치한 빈 오브젝트
// 4. player = 씬의 Player_Move
public class BookShelfFocus : MonoBehaviour
{
    [Header("카메라 연결")]
    public Transform cameraTransform;

    [Header("포커스 위치 (빈 칸을 바라보는 각도)")]
    public Transform focusPoint;

    [Header("연출 설정")]
    public float moveDuration = 0.5f;

    [Header("플레이어 이동 잠금")]
    public Player_Move player;

    Vector3 originalPos;
    Quaternion originalRot;
    Coroutine moveCoroutine;

    public void StartFocus()
    {
        if (cameraTransform == null || focusPoint == null) return;

        originalPos = cameraTransform.position;
        originalRot = cameraTransform.rotation;

        player?.SetMoveLock(true);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveCamera(focusPoint.position, focusPoint.rotation));
    }

    public void EndFocus()
    {
        player?.SetMoveLock(false);

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveCamera(originalPos, originalRot));
    }

    IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot)
    {
        float t = 0f;
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            cameraTransform.position = Vector3.Lerp(startPos, targetPos, p);
            cameraTransform.rotation = Quaternion.Slerp(startRot, targetRot, p);
            yield return null;
        }

        cameraTransform.position = targetPos;
        cameraTransform.rotation = targetRot;
    }
}