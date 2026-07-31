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

    Vector3    originalPos;
    Quaternion originalRot;
    Transform  originalParent;
    Coroutine  moveCoroutine;
    bool       isRotating = false;

    void Update()
    {
        if (isRotating && walkieTalkieObject != null)
            walkieTalkieObject.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);
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

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            walkieTalkieObject.localPosition = Vector3.Lerp(startPos, Vector3.zero, p);
            walkieTalkieObject.localRotation = Quaternion.Slerp(startRot, Quaternion.identity, p);
            yield return null;
        }

        walkieTalkieObject.localPosition = Vector3.zero;
        walkieTalkieObject.localRotation = Quaternion.identity;
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
