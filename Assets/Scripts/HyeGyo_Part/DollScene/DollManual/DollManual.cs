using System.Collections;
using UnityEngine;

/// <summary>
/// DollSceneの本を開閉するための専用コンポーネント。
///
/// Book_dollsceneにアタッチし、Event_On_Ray.OnClickから
/// ToggleBook()を呼び出して使用します。
/// </summary>
public class DollManual : MonoBehaviour
{
    [Header("Book Rotation")]
    [Tooltip("本を閉じた状態のローカル回転。未設定の場合はAwake時の回転を使用します。")]
    [SerializeField] private Vector3 closedLocalEulerAngles;

    [Tooltip("本を開いた状態のローカル回転。Inspectorで見た目に合わせて調整してください。")]
    [SerializeField] private Vector3 openLocalEulerAngles;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float openDuration = 0.5f;

    [SerializeField] private AnimationCurve rotationCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("ゲーム開始時に本を開いた状態にする場合に有効にします。")]
    [SerializeField] private bool startOpened;

    private Coroutine rotationCoroutine;
    private bool isOpen;

    private void Awake()
    {
        if (closedLocalEulerAngles == Vector3.zero)
        {
            closedLocalEulerAngles = transform.localEulerAngles;
        }

        isOpen = startOpened;
        transform.localRotation = Quaternion.Euler(
            isOpen ? openLocalEulerAngles : closedLocalEulerAngles);
    }

    /// <summary>
    /// 本を開きます。
    /// Event_On_Ray.OnClickから呼び出せます。
    /// </summary>
    public void OpenBook()
    {
        StartRotation(true);
    }

    /// <summary>
    /// 本を閉じます。
    /// </summary>
    public void CloseBook()
    {
        StartRotation(false);
    }

    /// <summary>
    /// 本の開閉状態を切り替えます。
    /// </summary>
    public void ToggleBook()
    {
        StartRotation(!isOpen);
    }

    public bool IsOpen => isOpen;

    private void StartRotation(bool open)
    {
        isOpen = open;

        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        rotationCoroutine = StartCoroutine(RotateBook(open));
    }

    private IEnumerator RotateBook(bool open)
    {
        Quaternion from = transform.localRotation;
        Quaternion to = Quaternion.Euler(
            open ? openLocalEulerAngles : closedLocalEulerAngles);

        if (openDuration <= 0f)
        {
            transform.localRotation = to;
            rotationCoroutine = null;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / openDuration);
            float curveTime = rotationCurve == null
                ? normalizedTime
                : rotationCurve.Evaluate(normalizedTime);

            transform.localRotation = Quaternion.Slerp(from, to, curveTime);
            yield return null;
        }

        transform.localRotation = to;
        rotationCoroutine = null;
    }
}