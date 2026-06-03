using UnityEngine;
using System.Collections;
using TMPro;

public class Object_Inspecatable : MonoBehaviour
{
    [Header("Player Character")]
    public EX_InputSystem_PC_V2 player;

    [Header("Inspect Settings")]
    public float moveSpeed = 5f;
    public float inspectDistance = 1f;

    [Header("UI Settings")]
    public string disc;
    public GameObject panel;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    private bool isInspecting = false;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
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
            panel.SetActive(false);
            player.SetMoveLock(false);
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

        // 플레이어 이동 잠금
        player.SetMoveLock(true);

        // 목표 위치 (카메라 앞 1m)
        Vector3 targetPosition =
            mainCamera.transform.position +
            mainCamera.transform.forward * inspectDistance;

        Quaternion targetRotation =
            Quaternion.LookRotation(mainCamera.transform.forward);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            transform.position =
                Vector3.Lerp(transform.position, targetPosition, t);

            transform.rotation =
                Quaternion.Lerp(transform.rotation, targetRotation, t*10);

            yield return null;
        }

        panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = disc;
        panel.SetActive(true);
    }

    IEnumerator ReturnToOriginalPosition()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            transform.position =
                Vector3.Lerp(transform.position, originalPosition, t);

            transform.rotation =
                Quaternion.Lerp(transform.rotation, originalRotation, t);

            yield return null;
        }

        // 플레이어 이동 해제
        //PlayerController.Instance.SetMoveLock(false);

        isInspecting = false;
    }
}