using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// 여러 번 다시 읽을 수 있는 조사 오브젝트 (힌트 종이 전용)
// Object_Inspecatable 구조를 참고했지만, 두 번째 상호작용 시
// 사라지지 않고 원래 위치로 돌아옵니다. (수집되지 않음)
public class HintPaperInspect : MonoBehaviour
{
    [Header("Player Character")]
    public Player_Move player;

    [Header("Inspect Settings")]
    public float targetTime = 0.5f;
    public float inspectDistance = 0.6f;

    [Header("UI Settings")]
    public string discBefore = "낡은 종이가 붙어있다. 뭔가 더 필요해 보인다.";
    public string discAfter;
    public Scene_UI_Manager SceneUI;

    [Header("사운드")]
    public AudioClip audio_inspect;

    Vector3 originalPosition;
    Quaternion originalRotation;

    Collider col;
    Rigidbody rigid;
    bool isInspecting = false;
    bool notesComplete = false;

    Camera mainCamera;
    Play_Audio audio_player;

    Collider[] childColliders;
    Coroutine moveCoroutine;

    void Start()
    {
        mainCamera = Camera.main;
        audio_player = GetComponent<Play_Audio>();
        col = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        childColliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (!isInspecting) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveBackToOriginal());
        }
    }

    // HintPaperNotesManager.onAllNotesPlaced 에 연결
    public void RevealText()
    {
        notesComplete = true;
    }

    // OnClick 에 연결할 함수
    public void OnInspect()
    {
        if (!isInspecting)
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveToInspectPosition());
        }
        else
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveBackToOriginal());
        }
    }

    IEnumerator MoveToInspectPosition()
    {
        isInspecting = true;

        foreach (var c in childColliders)
            if (c != col) c.enabled = false;

        if (col != null) col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = true;
        if (player != null) player.SetMoveLock(true);

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

        if (SceneUI != null)
        {
            SceneUI.ChangeText(0, notesComplete ? discAfter : discBefore);
            SceneUI.SetActivePanel(2, true);
            SceneUI.SetActiveCursor(false);
        }

        if (col != null) col.isTrigger = false;
        audio_player?.PlayAudio(audio_inspect);
    }

    IEnumerator MoveBackToOriginal()
    {
        isInspecting = false;

        if (SceneUI != null)
        {
            SceneUI.SetActivePanel(2, false);
            SceneUI.SetActiveCursor(true);
        }

        if (col != null) col.isTrigger = true;

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

        foreach (var c in childColliders)
            if (c != col) c.enabled = true;
    }
}