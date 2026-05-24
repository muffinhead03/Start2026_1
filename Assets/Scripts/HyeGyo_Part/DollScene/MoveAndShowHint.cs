using System.Collections;
using UnityEngine;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MoveAndShowHint : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header("Move Settings")]
    public float moveSpeed = 0.5f;
    public float moveDuration = 1f;
    public Vector3 moveDirection = Vector3.right;

    [Header("UI")]
    public GameObject hintPanel;
    public TMP_Text hintText;
    [TextArea]
    public string hintMessage = "힌트입니다.";

    [Header("Raycast Debug")]
    public float rayDistance = 5f;
    public bool debugLog = true;

    private Vector3 originalPosition;
    private bool isRunning = false;
    private bool panelOpen = false;

    void Start()
    {
        originalPosition = transform.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (hintPanel != null)
            hintPanel.SetActive(false);

        if (playerCamera == null)
            Debug.LogError($"[{name}] Player Camera가 비어 있습니다. Inspector에 플레이어 카메라를 넣어주세요.");

        if (GetComponent<Collider>() == null)
            Debug.LogError($"[{name}] Collider가 없습니다. 우클릭 감지를 위해 Collider가 필요합니다.");
    }

    void Update()
    {
        // Panel이 열려 있을 때는 버튼 클릭을 위해 커서를 보이게 함
        if (panelOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (RightClickPressed() && !isRunning && !panelOpen)
        {
            if (debugLog)
                Debug.Log($"[{name}] 우클릭 입력 감지됨");

            TryCenterRaycast();
        }
    }

    bool RightClickPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            return true;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetMouseButtonDown(1))
            return true;
#endif

        return false;
    }

    void TryCenterRaycast()
    {
        if (playerCamera == null)
        {
            Debug.LogError($"[{name}] playerCamera가 없어서 Raycast 불가");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (debugLog)
                Debug.Log($"[{name}] 중앙 Raycast Hit: {hit.transform.name}");

            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (debugLog)
                    Debug.Log($"[{name}] 대상 맞음. 이동 시작");

                StartCoroutine(MoveThenShowHint());
            }
            else
            {
                if (debugLog)
                    Debug.LogWarning($"[{name}] 다른 오브젝트를 보고 있음: {hit.transform.name}");
            }
        }
        else
        {
            if (debugLog)
                Debug.LogWarning($"[{name}] 중앙 Raycast가 아무것도 맞추지 못함");
        }
    }

    IEnumerator MoveThenShowHint()
    {
        isRunning = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + moveDirection.normalized * moveSpeed * moveDuration;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        transform.position = targetPosition;

        if (hintText != null)
            hintText.text = hintMessage;

        if (hintPanel != null)
            hintPanel.SetActive(true);

        panelOpen = true;
        isRunning = false;

        if (debugLog)
            Debug.Log($"[{name}] 이동 완료 + 힌트 표시 완료");
    }

    // UI 뒤로가기 버튼에서 이 함수를 연결하세요.
    public void ClosePanelAndReturn()
    {
        if (isRunning)
            return;

        if (hintPanel != null)
            hintPanel.SetActive(false);

        panelOpen = false;

        StopAllCoroutines();
        StartCoroutine(ReturnToOriginalPosition());
    }

    IEnumerator ReturnToOriginalPosition()
    {
        isRunning = true;

        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            transform.position = Vector3.Lerp(startPosition, originalPosition, t);

            yield return null;
        }

        transform.position = originalPosition;
        isRunning = false;

        if (debugLog)
            Debug.Log($"[{name}] 원위치 복귀 완료");
    }
}