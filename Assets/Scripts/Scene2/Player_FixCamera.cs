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

    [Header("Fix Object")]
    public Object_FixCamera fixObject;

    Vector3 originalPos;
    Vector3 originalRot;

    Vector3 targetPos;
    Vector3 targetRot;


    void Start()
    {
        fixObject = null;
    }

    public bool isPlayerFix()
    {
        return (fixObject != null);
    }


    public void FixCamera(Vector3 targetPos, Vector3 targetRot, GameObject fixObject)
    {
        if (this.fixObject != null) return;

        this.targetPos = targetPos;
        this.targetRot = targetRot;

        StartCoroutine(MoveToFixPosition());

        this.fixObject = fixObject.GetComponent<Object_FixCamera>();
    }

    public void UnFixCamera()
    {
        if (fixObject == null) return;

        fixObject = null;

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
