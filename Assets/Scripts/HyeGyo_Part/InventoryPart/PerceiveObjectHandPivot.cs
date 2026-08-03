using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PerceiveObjectHandPivot : MonoBehaviour
{
    [Header("Player Hand")]
    [SerializeField]
    private Transform handPivot;

    [Tooltip(
        "체크하면 HandPivot의 직접 자식에 붙은 " +
        "Object_Grabbable만 검사합니다. " +
        "오브젝트 안쪽 자식에 컴포넌트가 있다면 체크를 해제하세요."
    )]
    [SerializeField]
    private bool directChildrenOnly = false;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;

    private Object_Grabbable currentObject;

    public event Action<Object_Grabbable>
        HandObjectChanged;

    public Object_Grabbable CurrentObject =>
        currentObject;

    public Transform HandPivot =>
        handPivot;

    private void OnEnable()
    {
        ForceScan();
    }

    private void LateUpdate()
    {
        Scan(false);
    }

    public void ForceScan()
    {
        Scan(true);
    }

    private void Scan(bool forceNotify)
{
    Object_Grabbable detected =
        FindHandObject();

    Debug.Log(
        $"[HandPerception] Scan: " +
        $"Pivot=" +
        $"{(handPivot != null ? handPivot.name : "null")}, " +
        $"ChildCount=" +
        $"{(handPivot != null ? handPivot.childCount : 0)}, " +
        $"Detected=" +
        $"{(detected != null ? detected.name : "null")}, " +
        $"Force={forceNotify}",
        this
    );

    if (!forceNotify &&
        detected == currentObject)
    {
        return;
    }

    currentObject = detected;

    HandObjectChanged?.Invoke(
        currentObject
    );
}

    private Object_Grabbable FindHandObject()
{
    if (handPivot == null)
        return null;

    for (int i = handPivot.childCount - 1;
         i >= 0;
         i--)
    {
        Transform child =
            handPivot.GetChild(i);

        Debug.Log(
            $"[HandPerception] 자식 검사: " +
            $"Index={i}, Name={child.name}",
            child
        );

        Object_Grabbable grabbable =
            child.GetComponent<Object_Grabbable>();

        if (grabbable != null)
            return grabbable;

        if (!directChildrenOnly)
        {
            grabbable =
                child.GetComponentInChildren
                <Object_Grabbable>(true);

            if (grabbable != null)
                return grabbable;
        }
    }

    return null;
}

    private static string GetObjectName(
        Object_Grabbable target)
    {
        return target != null
            ? target.gameObject.name
            : "null";
    }
}