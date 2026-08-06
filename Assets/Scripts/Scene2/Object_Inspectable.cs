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
    public string disc;
    public Scene_UI_Manager SceneUI;

    [Header("사운드")]
    public AudioClip audio_inspect;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

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
            gameObject.SetActive(false);
            isInspecting = false;

            SceneUI.SetActivePanel(2, false);
            SceneUI.SetActiveCursor(true);

            player.SetMoveLock(false);

            Player_Inventory.AddItem(objectName);
            //StartCoroutine(ReturnToOriginalPosition());
        }
    }

    IEnumerator MoveToInspectPosition()
    {
        isInspecting = true;

        // 원래 상태 저장
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = true;

        // 플레이어 이동 잠금
        player.SetMoveLock(true);

        // 목표 위치 (카메라 앞 1m)
        Vector3 targetPosition =
            mainCamera.transform.position +
            mainCamera.transform.forward * inspectDistance;

        Quaternion targetRotation =
            Quaternion.LookRotation(mainCamera.transform.forward);

        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;

            transform.position =
                Vector3.Lerp(transform.position, targetPosition, tp);

            transform.rotation =
                Quaternion.Lerp(transform.rotation, targetRotation, tp);

            yield return null;
        }

        SceneUI.ChangeText(0, disc);
        SceneUI.SetActivePanel(2, true);
        SceneUI.SetActiveCursor(false);

        col.isTrigger = false;

        audio_player?.PlayAudio(audio_inspect);
    }
}