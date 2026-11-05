using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Object_Inspectable : MonoBehaviour, IInspectInteractHandler, IRayInteractable
{
    [Header("이름")]
    public string objectName;

    [Header("Player Character")]
    public Player_Move player;

    [Header("Inspect Settings")]
    public float targetTime = 1f;
    public float inspectDistance = 1f;

    [SerializeField] protected Scene_UI_Manager SceneUI;

    [Header("Inspect Events")]
    public UnityEvent OnInspectStarted;
    public UnityEvent OnInspectPerformed;
    public UnityEvent OnInspectCanceled;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Collider col;
    private Rigidbody rigid;

    private bool isInspecting = false;

    private Camera mainCamera;

    public void OnRayEnter() { }
    public void OnRayStay() { }
    public void OnRayExit() { }
    public void OnRayClick() { OnInspect(); }

    private void Start()
    {
        mainCamera = Camera.main;

        col = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
    }

    // OnClick 에 연결할 함수
    public virtual void OnInspect()
    {
        if (isInspecting) return;

        StartCoroutine(MoveToInspectPosition());

        SceneUI.SetActiveCursor(false);

        OnInspectStarted?.Invoke();
    }

    public virtual void OnInspectPosition()
    {
        OnInspectPerformed?.Invoke();
    }

    // 조사 마침 상호작용이 들어왔을 때 실행할 함수
    public virtual void OnInteractWhileInspecting()
    {
        if (!isInspecting) return;

        StartCoroutine(ReturnToOriginalPosition());

        SceneUI.SetActiveCursor(true);

        OnInspectCanceled?.Invoke();
    }

    IEnumerator MoveToInspectPosition()
    {
        isInspecting = true;

        // 원래 상태 저장
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        col.enabled = false;
        if (rigid != null) rigid.isKinematic = true;

        // 플레이어 이동 잠금
        player.SetMoveLock(true);
        InputManager.DisableInput();

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

        InputManager.EnableUIInput();
        InspectInput.Begin(this);

        OnInspectPosition();

        //col.isTrigger = false;
    }

    IEnumerator ReturnToOriginalPosition()
    {
        InspectInput.End(this);
        InputManager.DisableInput();

        //col.isTrigger = true;
        if (rigid != null) rigid.isKinematic = false;

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

        col.enabled = true;

        isInspecting = false;
        player.SetMoveLock(false);

        InputManager.EnablePlayerInput();
    }
}