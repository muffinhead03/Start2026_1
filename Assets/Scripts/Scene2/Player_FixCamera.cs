using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class Player_FixCamera : MonoBehaviour
{
    [Header("Camera")]
    public Transform CameraPivot;
    public float targetTime;

    [Header("UI Settings")]
    public Scene_UI_Manager SceneUI;    // UI 매니저

    Vector3 originalPos;
    Vector3 originalRot;

    Vector3 targetPos;
    Vector3 targetRot;

    bool isFixing;

    void Start()
    {
        isFixing = false;
    }

    public void FixCamera(Vector3 targetPos, Vector3 targetRot)
    {
        if (isFixing) return;

        isFixing = true;

        this.targetPos = targetPos;
        this.targetRot = targetRot;

        StartCoroutine(MoveToFixPosition());
    }

    public void UnFixCamera()
    {
        if (!isFixing) return;

        isFixing = false;

        StartCoroutine(MoveToOriginalPosition());
    }

    IEnumerator MoveToFixPosition()
    {
        // 플레이어 이동 잠금
        GetComponent<Player_Move>().SetMoveLock(true);


        originalPos = CameraPivot.position;
        originalRot = CameraPivot.forward;

        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;

            CameraPivot.position = Vector3.Lerp(originalPos, targetPos, tp);

            CameraPivot.forward = Vector3.Lerp(originalRot, targetRot, tp);

            yield return null;
        }

        //SceneUI.UnlockPointer();
    }

    IEnumerator MoveToOriginalPosition()
    {
        //SceneUI.LockPointer();

        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;

            CameraPivot.position = Vector3.Lerp(targetPos, originalPos, tp);

            CameraPivot.forward = Vector3.Lerp(targetRot, originalRot, tp);

            yield return null;
        }

        GetComponent<Player_Move>().SetMoveLock(false);
    }
}
