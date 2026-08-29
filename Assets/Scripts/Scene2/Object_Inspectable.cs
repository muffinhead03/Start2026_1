using System.Collections;
using TMPro;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Object_Inspecatable : MonoBehaviour
{
    [Header("이름")]
    public string objectName;

    [Header("Player Character")]
    public Player_Move player;

    [Header("Inspect Settings")]
    public float targetTime = 1f;
    public float inspectDistance = 1f;

    [Header("UI Settings")]
    [TextArea(3, 8)]
    public string disc;
    public Scene_UI_Manager SceneUI;

    [Header("사운드")]
    public AudioClip audio_inspect;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Collider col;
    private Rigidbody rigid;

    private bool isInspecting = false;

    private Camera mainCamera;

    private Play_Audio audio_player;

    private void Start()
    {
        mainCamera = Camera.main;
        audio_player = GetComponent<Play_Audio>();

        col = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
    }

    // OnClick 에 연결할 함수
    public void OnInspect()
    {
        if (!isInspecting)
        {
            StartCoroutine(MoveToInspectPosition());
        }
        else
        {
            //gameObject.SetActive(false);

            //Player_Inventory.AddItem(objectName);
            StartCoroutine(ReturnToOriginalPosition());
        }
    }

    IEnumerator MoveToInspectPosition()
    {
        isInspecting = true;

        // 원래 상태 저장
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = true;

        // 플레이어 이동 잠금
        player.SetMoveLock(true);

        // 목표 위치 (카메라 앞 1m)
        Vector3 targetPosition =
            mainCamera.transform.position +
            mainCamera.transform.forward * inspectDistance;

        Vector3 startPos = originalPosition;
        Quaternion startRot = originalRotation;

        Vector3 targetPos = mainCamera.transform.position + mainCamera.transform.forward * inspectDistance;
        Quaternion targetRot = Quaternion.LookRotation(mainCamera.transform.forward);

        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;

            transform.position =
                Vector3.Lerp(startPos, targetPos, tp);

            transform.rotation =
                Quaternion.Lerp(startRot, targetRot, tp);

            yield return null;
        }

        // 조사 UI 활성화
        SceneUI.ChangeText(0, disc);
        SceneUI.SetActivePanel(2, true);
        SceneUI.SetActiveCursor(false);

        col.isTrigger = false;

        audio_player?.PlayAudio(audio_inspect);
    }

    IEnumerator ReturnToOriginalPosition()
    {
        col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = false;

        // 조사 UI 비활성화
        SceneUI.SetActivePanel(2, false);
        SceneUI.SetActiveCursor(true);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = originalPosition;
        Quaternion targetRot = originalRotation;

        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;

            transform.position =
                Vector3.Lerp(startPos, targetPos, tp);

            transform.rotation =
                Quaternion.Lerp(startRot, targetRot, tp);

            yield return null;
        }

        col.isTrigger = false;

        isInspecting = false;
        player.SetMoveLock(false);
    }
}