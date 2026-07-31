using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PerceiveObjectHandPivot : MonoBehaviour
{
    [Header("Player Hand")]
    [SerializeField] private Transform handPivot;

    [Tooltip("Player_Grab.Grab()이 물체를 Hand의 직접 자식으로 넣으므로 기본값은 체크합니다.")]
    [SerializeField] private bool directChildrenOnly = true;

    private Object_Grabbable currentObject;

    public event Action<Object_Grabbable> HandObjectChanged;

    public Object_Grabbable CurrentObject => currentObject;
    public Transform HandPivot => handPivot;

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
        Object_Grabbable detected = FindHandObject();

        if (!forceNotify && detected == currentObject)
            return;

        currentObject = detected;
        HandObjectChanged?.Invoke(currentObject);
    }

    private Object_Grabbable FindHandObject()
    {
        if (handPivot == null)
            return null;

        for (int i = 0; i < handPivot.childCount; i++)
        {
            Transform child = handPivot.GetChild(i);

            Object_Grabbable grabbable =
                child.GetComponent<Object_Grabbable>();

            if (grabbable != null)
                return grabbable;

            if (!directChildrenOnly)
            {
                grabbable =
                    child.GetComponentInChildren<Object_Grabbable>(true);

                if (grabbable != null)
                    return grabbable;
            }
        }

        return null;
    }
}
