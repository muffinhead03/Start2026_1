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
    private bool showDebugLog = false;

    private Object_Grabbable currentObject;

    public event Action<Object_Grabbable>
        HandObjectChanged;

    /*
     * 중요:
     * public field가 아니라 현재 실제 값을 돌려주는 property입니다.
     */
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

    private void Scan(
        bool forceNotify)
    {
        Object_Grabbable detected =
            FindHandObject();

        if (showDebugLog)
        {
            Debug.Log(
                "[HandPerception] Scan: " +
                $"Pivot=" +
                $"{(handPivot != null ? handPivot.name : "null")}, " +
                $"ChildCount=" +
                $"{(handPivot != null ? handPivot.childCount : 0)}, " +
                $"Detected={GetObjectName(detected)}, " +
                $"Force={forceNotify}",
                this
            );
        }

        if (!forceNotify &&
            detected == currentObject)
        {
            return;
        }

        currentObject =
            detected;

        if (showDebugLog)
        {
            Debug.Log(
                "[HandPerception] Hand Object 변경: " +
                $"Current={GetObjectName(currentObject)}",
                this
            );
        }

        /*
         * 구독자 한 곳에서 예외가 나더라도
         * 나머지 구독자/감지기 자체가 끊기지 않도록
         * 각 delegate를 개별 호출합니다.
         */
        InvokeHandObjectChangedSafely(
            currentObject
        );
    }

    private Object_Grabbable FindHandObject()
    {
        if (handPivot == null)
        {
            if (showDebugLog)
            {
                Debug.LogWarning(
                    "[HandPerception] HandPivot이 연결되지 않았습니다.",
                    this
                );
            }

            return null;
        }

        /*
         * 마지막 자식부터 검사합니다.
         * 일반적인 SetParent() Grab은 새 물체가 마지막 자식이 되므로
         * 새로 잡은 물체를 우선 감지합니다.
         *
         * 비활성 오브젝트는 손에 들고 있는 것으로 취급하지 않습니다.
         */
        for (int i = handPivot.childCount - 1;
             i >= 0;
             i--)
        {
            Transform child =
                handPivot.GetChild(i);

            if (child == null ||
                !child.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (showDebugLog)
            {
                Debug.Log(
                    "[HandPerception] 자식 검사: " +
                    $"Index={i}, Name={child.name}",
                    child
                );
            }

            Object_Grabbable grabbable =
                child.GetComponent<Object_Grabbable>();

            if (grabbable != null &&
                grabbable.gameObject.activeInHierarchy)
            {
                return grabbable;
            }

            if (!directChildrenOnly)
            {
                /*
                 * false:
                 * 비활성 자식의 Object_Grabbable은 감지하지 않습니다.
                 */
                grabbable =
                    child.GetComponentInChildren
                        <Object_Grabbable>(false);

                if (grabbable != null &&
                    grabbable.gameObject.activeInHierarchy)
                {
                    return grabbable;
                }
            }
        }

        return null;
    }

    private void InvokeHandObjectChangedSafely(
        Object_Grabbable value)
    {
        Action<Object_Grabbable> handlers =
            HandObjectChanged;

        if (handlers == null)
        {
            return;
        }

        Delegate[] invocationList =
            handlers.GetInvocationList();

        for (int i = 0;
             i < invocationList.Length;
             i++)
        {
            Action<Object_Grabbable> handler =
                invocationList[i] as Action<Object_Grabbable>;

            if (handler == null)
            {
                continue;
            }

            try
            {
                handler.Invoke(
                    value
                );
            }
            catch (Exception exception)
            {
                /*
                 * Hand 감지 이벤트의 한 구독자가 실패해도
                 * Input/Inventory 전체로 예외가 전파되지 않게 합니다.
                 */
                Debug.LogWarning(
                    "[HandPerception] HandObjectChanged 구독자 예외를 복구했습니다. " +
                    exception.GetType().Name + ": " +
                    exception.Message,
                    this
                );
            }
        }
    }

    private static string GetObjectName(
        Object_Grabbable target)
    {
        return target != null
            ? target.gameObject.name
            : "null";
    }
}