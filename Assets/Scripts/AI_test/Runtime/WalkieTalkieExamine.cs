using System.Collections;
using UnityEngine;

public class WalkieTalkieExamine : MonoBehaviour
{
    [Header("무전기 3D 오브젝트")]
    [SerializeField] Transform walkieTalkieObject;   // 씬 안의 무전기 모델
    [SerializeField] Transform examinePoint;         // 카메라 자식, 화면에 뜰 위치/각도

    [Header("연출 설정")]
    [SerializeField] float moveDuration = 0.4f;      // 집어드는 이동 시간
    [SerializeField] float rotateSpeed  = 20f;       // 초당 회전 각도 (자동 회전)

    [Header("조사 중 정자세 각도 (모델 pivot이 이상하면 여기서 보정)")]
    [SerializeField] Vector3 targetLocalEuler = Vector3.zero;

    [Header("자동 회전축 (기본값 Y축 = 원래 동작, 씬마다 다르게 설정 가능)")]
    [SerializeField] Vector3 rotateAxis = Vector3.up;

    Vector3    originalPos;
    Quaternion originalRot;
    Transform  originalParent;
    Coroutine  moveCoroutine;
    bool       isRotating = false;

    void Update()
    {
        if (isRotating && walkieTalkieObject != null)
        {
            // Space.Self: 물체 자기 자신의 축 기준 회전 → targetLocalEuler로 삐딱하게 포즈 잡혀있어도
            // 흔들림 없이 깨끗하게 돎 (팽이 자기 축 회전과 같은 원리).
            // rotateAxis 값(예: (1,0,0), (0,1,0), (0,0,1))을 바꿔가며 원하는 방향 찾으면 됨.
            walkieTalkieObject.Rotate(rotateAxis, rotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void StartExamine()
    {
        if (walkieTalkieObject == null || examinePoint == null) return;

        walkieTalkieObject.gameObject.SetActive(true);   // ← 추가

        originalParent = walkieTalkieObject.parent;
        originalPos    = walkieTalkieObject.localPosition;
        originalRot    = walkieTalkieObject.localRotation;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToExamine());
    }

    public void EndExamine()
    {
        isRotating = false;
        if (walkieTalkieObject == null) return;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveBack());
    }

    IEnumerator MoveToExamine()
    {
        walkieTalkieObject.SetParent(examinePoint, true);

        float t = 0f;
        Vector3    startPos = walkieTalkieObject.localPosition;
        Quaternion startRot = walkieTalkieObject.localRotation;
        Quaternion endRot   = Quaternion.Euler(targetLocalEuler);   // ← 변경: identity 대신 지정한 각도

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            walkieTalkieObject.localPosition = Vector3.Lerp(startPos, Vector3.zero, p);
            walkieTalkieObject.localRotation = Quaternion.Slerp(startRot, endRot, p);   // ← 변경
            yield return null;
        }

        walkieTalkieObject.localPosition = Vector3.zero;
        walkieTalkieObject.localRotation = endRot;   // ← 변경
        isRotating = true;
    }

    IEnumerator MoveBack()
    {
        walkieTalkieObject.SetParent(originalParent, true);

        float t = 0f;
        Vector3    startPos = walkieTalkieObject.localPosition;
        Quaternion startRot = walkieTalkieObject.localRotation;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            walkieTalkieObject.localPosition = Vector3.Lerp(startPos, originalPos, p);
            walkieTalkieObject.localRotation = Quaternion.Slerp(startRot, originalRot, p);
            yield return null;
        }

        walkieTalkieObject.localPosition = originalPos;
        walkieTalkieObject.localRotation = originalRot;

        walkieTalkieObject.gameObject.SetActive(false);
    }
}