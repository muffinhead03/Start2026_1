using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// 와인랙 라벨 확대 조사(줌인 + 마우스 드래그 회전)
// Event_On_Ray.OnClick → OnInspect() 연결
// ESC 누르면 원위치로 취소
// 같은 오브젝트의 WineRackLabel과 함께 사용
public class WineLabelInspect : MonoBehaviour
{
    [Header("Player")]
    public Player_Move player;

    [Header("연결 (같은 오브젝트의 WineRackLabel)")]
    public WineRackLabel wineLabel;

    [Header("Inspect Settings")]
    public float targetTime = 0.5f;
    public float inspectDistance = 0.6f;
    public float verticalOffset = 0.15f; // ← 새로 추가: 확대 시 화면상 세로 위치 보정 (+ 값 = 아래로, 라벨 잘림 방지)

    [Header("Rotate Settings")]
    public float rotateSpeed = 150f; // 마우스 감도

    [Header("UI Settings")]
    public Scene_UI_Manager SceneUI;

    Vector3 originalPosition;
    Quaternion originalRotation;

    Collider col;
    Rigidbody rigid;
    bool isInspecting = false;
    Coroutine moveCoroutine;

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        col = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isInspecting) return;

        // ESC로 취소
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelInspect();
            return;
        }

        // 마우스 움직임으로 회전
        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            transform.Rotate(Vector3.up, -delta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, delta.y * rotateSpeed * Time.deltaTime, Space.World);
        }
    }

    // Event_On_Ray.OnClick 에 연결할 함수
    public void OnInspect()
    {
        if (!isInspecting)
            StartCoroutine(MoveToInspectPosition());
    }

    IEnumerator MoveToInspectPosition()
    {
        isInspecting = true;

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (col != null) col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = true;
        if (player != null) player.SetMoveLock(true);

        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * inspectDistance;
        targetPosition -= mainCamera.transform.up * verticalOffset; // ← 새로 추가: 병 전체를 화면에서 살짝 아래로 내려서 윗부분(라벨) 여백 확보
        Quaternion targetRotation = Quaternion.LookRotation(mainCamera.transform.forward);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;
            transform.position = Vector3.Lerp(startPos, targetPosition, tp);
            transform.rotation = Quaternion.Lerp(startRot, targetRotation, tp);
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        if (col != null) col.isTrigger = false;

        if (SceneUI != null) SceneUI.SetActiveCursor(false);

        // 라벨 정보 수집 (텍스트 표시 + 진행상황 기록)
        wineLabel?.Collect();
    }

    void CancelInspect()
    {
        isInspecting = false;

        if (SceneUI != null)
        {
            wineLabel?.Hide();
            SceneUI.SetActiveCursor(true);
        }

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(ReturnToOriginal());
    }

    IEnumerator ReturnToOriginal()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;
            transform.position = Vector3.Lerp(startPos, originalPosition, tp);
            transform.rotation = Quaternion.Lerp(startRot, originalRotation, tp);
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (col != null) col.isTrigger = false;
        if (rigid != null) rigid.isKinematic = false;
        if (player != null) player.SetMoveLock(false);
    }
}