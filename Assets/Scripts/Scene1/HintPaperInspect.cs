using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// 여러 번 다시 읽을 수 있는 조사 오브젝트 (힌트 종이 전용)
// 종이는 원래 위치에 고정되어 있고, 카메라가 종이 앞으로 다가갔다가
// 다시 원래 위치로 돌아오는 방식입니다. (종이 자체는 움직이지 않음)
public class HintPaperInspect : MonoBehaviour
{
    [Header("Player Character")]
    public Player_Move player;

    [Header("Inspect Settings")]
    public float targetTime = 0.5f;
    public float inspectDistance = 0.6f;

    [Header("UI Settings")]
    [TextArea(2, 5)]
    public string discBefore = "낡은 종이가 붙어있다. 뭔가 더 필요해 보인다.";
    [TextArea(2, 5)]
    public string discAfter;
    public Scene_UI_Manager SceneUI;

    [Header("사운드")]
    public AudioClip audio_inspect;

    Vector3 originalCameraPosition;
    Quaternion originalCameraRotation;

    bool isInspecting = false;
    bool notesComplete = false;

    Camera mainCamera;
    Play_Audio audio_player;

    Coroutine moveCoroutine;

    void Start()
    {
        mainCamera = Camera.main;
        audio_player = GetComponent<Play_Audio>();
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

        if (player != null) player.SetMoveLock(true);

        // 복귀용으로 카메라의 원래 위치/회전 저장
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;

        // 종이는 고정, 카메라가 종이 정면 inspectDistance 만큼 앞으로 이동
        Vector3 targetPosition = transform.position - transform.forward * inspectDistance;
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward);

        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, tp);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetRotation, tp);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;

        if (SceneUI != null)
        {
            SceneUI.ChangeText(0, notesComplete ? discAfter : discBefore);
            SceneUI.SetActivePanel(2, true);
            SceneUI.SetActiveCursor(false);
        }

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

        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;
            mainCamera.transform.position = Vector3.Lerp(startPos, originalCameraPosition, tp);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, originalCameraRotation, tp);
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
        mainCamera.transform.rotation = originalCameraRotation;

        if (player != null) player.SetMoveLock(false);
    }
}