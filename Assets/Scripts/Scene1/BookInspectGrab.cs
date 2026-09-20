using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// 책 조사(확대 + 마우스 회전 + 제목 표시) → 두 번째 상호작용 시 실제로 손에 잡히도록 처리
// ESC를 누르면 잡지 않고 원위치로 취소됩니다.
// 같은 오브젝트의 Object_Grabbable과 함께 사용합니다.
public class BookInspectGrab : MonoBehaviour
{
    [Header("Player")]
    public Player_Move player;

    [Header("연결 (같은 오브젝트의 Object_Grabbable)")]
    public Object_Grabbable grabbable;

    [Header("Inspect Settings")]
    public float targetTime = 0.5f;
    public float inspectDistance = 0.6f;

    [Header("Obstruction Check")]
    [Tooltip("책장 등 인스펙트 위치와 충돌 검사할 레이어 (BookShelf 콜라이더가 속한 레이어로 설정)")]
    public LayerMask obstructionMask;
    [Tooltip("장애물 표면에서 얼마나 앞에서 멈출지")]
    public float safetyMargin = 0.05f;
    [Tooltip("플레이어가 아주 가까이 있어도 최소 이 거리는 확보")]
    public float minInspectDistance = 0.15f;

    [Header("Rotate Settings")]
    public float rotateSpeed = 150f;

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

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelInspect();
            return;
        }

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            transform.Rotate(Vector3.up, -delta.x * rotateSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, delta.y * rotateSpeed * Time.deltaTime, Space.World);
        }
    }

    public void OnInspectOrGrab()
    {
        if (!isInspecting)
            StartCoroutine(MoveToInspectPosition());
        else
            GrabNow();
    }

    // 카메라 앞 목표 지점 사이에 장애물(책장 등)이 있으면 그 앞에서 멈추도록 거리 보정
    float GetSafeInspectDistance()
    {
        float safeDistance = inspectDistance;

        bool hitSomething = Physics.Raycast(
                mainCamera.transform.position,
                mainCamera.transform.forward,
                out RaycastHit hit,
                inspectDistance,
                obstructionMask,
                QueryTriggerInteraction.Ignore);

        Debug.Log($"[BookInspectGrab] hit={hitSomething}, target={(hitSomething ? hit.collider.name : "none")}, dist={(hitSomething ? hit.distance.ToString("F2") : "-")}");

        if (hitSomething)
            safeDistance = hit.distance - safetyMargin;

        return Mathf.Max(safeDistance, minInspectDistance);
    }

    IEnumerator MoveToInspectPosition()
    {
        isInspecting = true;

        if (BookSoundManager.Instance != null)
            BookSoundManager.Instance.PlayPickup();

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        if (col != null) col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = true;
        if (player != null) player.SetMoveLock(true);

        float safeDistance = GetSafeInspectDistance();
        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * safeDistance;
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

        if (SceneUI != null && grabbable != null)
        {
            string letter = grabbable.objectName.Replace("book_", "");
            SceneUI.ChangeText(0, $"책 제목: {letter}");
            SceneUI.SetActivePanel(2, true);
            SceneUI.SetActiveCursor(false);
        }
    }

    void GrabNow()
    {
        isInspecting = false;

        if (SceneUI != null)
        {
            SceneUI.SetActivePanel(2, false);
            SceneUI.SetActiveCursor(true);
        }

        if (player != null) player.SetMoveLock(false);

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (rigid != null) rigid.isKinematic = false;
        if (col != null) col.isTrigger = false;

        if (grabbable != null)
            grabbable.OnGrab();
    }

    void CancelInspect()
    {
        isInspecting = false;

        if (SceneUI != null)
        {
            SceneUI.SetActivePanel(2, false);
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