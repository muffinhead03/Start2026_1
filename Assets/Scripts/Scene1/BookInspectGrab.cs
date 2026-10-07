using System.Collections;
using System.Collections.Generic;                 // ★ 추가
using System.Text.RegularExpressions;             // ★ 추가
using UnityEngine;
using UnityEngine.InputSystem;

// 책 조사(확대 + 마우스 회전 + 제목 표시) → 두 번째 상호작용 시 실제로 손에 잡히도록 처리
// ESC를 누르면 잡지 않고 원위치로 취소됩니다.
// 책을 든 채 허공에 E → 책장 근처면 원래 자리로 돌아감 (Player_Interaction에서 호출)
// 같은 오브젝트의 Object_Grabbable과 함께 사용합니다.
public class BookInspectGrab : MonoBehaviour, IInspectInteractHandler
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

    // ★ 추가: 제자리 돌려놓기
    [Header("제자리 돌려놓기")]
    [Tooltip("책을 든 채 허공에 E를 누를 때, 원래 자리에서 이 거리 안이면 제자리로 돌아감 (멀면 기존처럼 내려놓기)")]
    public float returnRange = 3f;

    static readonly List<BookInspectGrab> allBooks = new List<BookInspectGrab>();
    Vector3 homePosition;
    Quaternion homeRotation;
    Transform homeParent;
    bool homeKinematic;

    Vector3 originalPosition;
    Quaternion originalRotation;

    Collider col;
    Rigidbody rigid;
    bool isInspecting = false;
    Coroutine moveCoroutine;

    Camera mainCamera;

    // ★ 추가: 게임 시작 시 책장 원래 자리 기억
    void Awake()
    {
        allBooks.Add(this);
        homePosition = transform.position;
        homeRotation = transform.rotation;
        homeParent = transform.parent;
        var rb = GetComponent<Rigidbody>();
        homeKinematic = rb != null && rb.isKinematic;
    }

    // ★ 추가
    void OnDestroy()
    {
        allBooks.Remove(this);
    }

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

    public void OnInteractWhileInspecting()
    {
        if (isInspecting) GrabNow();
    }

    public void OnInspectOrGrab()
    {
        if (!isInspecting)
            StartCoroutine(MoveToInspectPosition());
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

        InspectInput.Begin(this);
        InputManager.EnableUIInput();

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

        InputManager.EnablePlayerInput();
        InspectInput.End(this);

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

        InputManager.EnablePlayerInput();
        InspectInput.End(this);
    }

    // ───────── ★ 추가: 제자리 돌려놓기 ─────────

    // Player_Interaction에서 허공에 E를 눌렀을 때 호출. 책을 제자리로 돌렸으면 true
    public static bool TryReturnHeldBook(Player_Grab grab)
    {
        if (grab == null || !grab.isGrab()) return false;

        foreach (var book in allBooks)
        {
            if (book == null || book.grabbable == null) continue;

            // hasKey가 부분 일치라 ^...$로 정확히 같은 이름만
            string exact = "^" + Regex.Escape(book.grabbable.objectName) + "$";
            if (!grab.hasKey(exact)) continue;

            // 책장에서 너무 멀면 기존처럼 내려놓기
            float dist = Vector3.Distance(grab.transform.position, book.homePosition);
            if (dist > book.returnRange) return false;

            book.ReturnHome(grab);
            return true;
        }
        return false; // 책이 아니면 기존 동작
    }

    void ReturnHome(Player_Grab grab)
    {
        if (grab.PutOn(homePosition) == null) return;
        StartCoroutine(SnapHome(grab.targetTime));
    }

    IEnumerator SnapHome(float delay)
    {
        // PutOn 이동이 끝난 뒤 원래 회전·부모·물리 상태로 복구
        yield return new WaitForSeconds(delay + 0.05f);

        transform.SetParent(homeParent, true);
        transform.SetPositionAndRotation(homePosition, homeRotation);

        if (rigid != null) rigid.isKinematic = homeKinematic;
        if (col != null)
        {
            col.enabled = true;   // PutOn이 콜라이더를 꺼두므로 다시 켬
            col.isTrigger = false;
        }

        if (BookSoundManager.Instance != null)
            BookSoundManager.Instance.PlayPickup();
    }
}