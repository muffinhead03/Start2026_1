using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// 쪽지 조사(확대 + 마우스 회전) → 두 번째 상호작용 시 실제로 손에 잡히도록 처리
// BookInspectGrab과 동일한 구조이지만, 텍스트 UI 패널 없이 쪽지 자체 머티리얼(텍스처)에
// 미리 입혀둔 알파벳을 확대해서 눈으로 직접 확인하는 방식입니다.
// E: 조사 → E 한 번 더: 손에 들어감 → 손에 든 상태에서 E: 떨어뜨림(Player_Grab.Release, 기존 동작)
// 같은 오브젝트의 Object_Grabbable과 함께 사용합니다.
// 처음엔 비활성 상태로 배치해두고, StainIntersection.notePickup에 연결해 조사 시 활성화합니다.
public class StainNoteInspectGrab : MonoBehaviour, IInspectInteractHandler
{
    [Header("Player")]
    public Player_Move player;

    [Header("연결 (같은 오브젝트의 Object_Grabbable)")]
    public Object_Grabbable grabbable;

    [Header("Sound")]
    public Play_Audio audioPlayer;     // 이 쪽지 오브젝트의 Play_Audio
    public AudioClip paperClip;        // Sounds > paper 에 있는 종이 소리

    [Header("Inspect Settings")]
    public float targetTime = 0.5f;
    public float inspectDistance = 0.6f;

    [Header("Rotate Settings")]
    public float rotateSpeed = 150f;

    [Header("UI Settings")]
    [Tooltip("확대 중 커서만 숨깁니다. 텍스트 패널은 사용하지 않습니다.")]
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

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            transform.Rotate(Vector3.up, -delta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, delta.y * rotateSpeed * Time.deltaTime, Space.World);
        }
    }

    // Event_On_Ray.OnClick 에 연결할 함수
    public void OnInspectOrGrab()
    {
        if (!isInspecting)
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveToInspectPosition());
        }
        else
        {
            GrabNow();
        }
    }

    // 조사 중 E 입력 (Player_Interaction이 레이와 상관없이 호출) → 손에 넣기
    public void OnInteractWhileInspecting()
    {
        if (isInspecting) GrabNow();
    }

    void OnDisable()
    {
        InspectInput.End(this);
    }

    IEnumerator MoveToInspectPosition()
    {
        isInspecting = true;
        InspectInput.Begin(this);

        if (audioPlayer != null && paperClip != null)
        audioPlayer.PlayAudio(paperClip);

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (col != null) col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = true;
        if (player != null) player.SetMoveLock(true);

        InputManager.EnableUIInput();

        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * inspectDistance;
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

        if (SceneUI != null)
            SceneUI.SetActiveCursor(false);
    }

    void GrabNow()
    {
        isInspecting = false;
        InspectInput.End(this);

        // 확대 애니메이션 도중 E를 눌러도 이동 코루틴이 계속 돌지 않도록 정지
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = null;

        if (SceneUI != null)
            SceneUI.SetActiveCursor(true);

        if (player != null) player.SetMoveLock(false);

        InputManager.EnablePlayerInput();

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (rigid != null) rigid.isKinematic = false;
        if (col != null) col.isTrigger = false;

        if (grabbable != null)
            grabbable.OnGrab();
    }

    // (현재 E 흐름에서는 쓰이지 않음 — 나중에 '잡지 않고 취소' 키가 필요하면 사용)
    void CancelInspect()
    {
        isInspecting = false;
        InspectInput.End(this);

        if (SceneUI != null)
            SceneUI.SetActiveCursor(true);

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

        InputManager.EnablePlayerInput();
    }
}