
using UnityEngine;

public class RayInteractionDisabler : MonoBehaviour
{
    // =============================================
    // Event_On_Ray
    // =============================================

    [Header("비활성화할 Ray 상호작용")]
    [SerializeField]
    private Event_On_Ray[] targetRayEvents;


    // =============================================
    // BoxCollider
    // =============================================

    [Header("비활성화할 BoxCollider")]
    [SerializeField]
    private BoxCollider[] targetColliders;


    // =============================================
    // 상호작용 비활성화
    // =============================================

    public void DisableInteractions()
    {
        // Event_On_Ray 비활성화
        if (targetRayEvents != null)
        {
            foreach (Event_On_Ray rayEvent in targetRayEvents)
            {
                if (rayEvent == null)
                    continue;

                rayEvent.enabled = false;

                Debug.Log(
                    "[RayInteractionDisabler] Ray 비활성화: " +
                    rayEvent.gameObject.name
                );
            }
        }


        // BoxCollider 비활성화
        if (targetColliders != null)
        {
            foreach (BoxCollider col in targetColliders)
            {
                if (col == null)
                    continue;

                col.enabled = false;

                Debug.Log(
                    "[RayInteractionDisabler] Collider 비활성화: " +
                    col.gameObject.name
                );
            }
        }
    }


    // =============================================
    // 상호작용 재활성화
    // =============================================

    public void EnableInteractions()
    {
        if (targetRayEvents != null)
        {
            foreach (Event_On_Ray rayEvent in targetRayEvents)
            {
                if (rayEvent == null)
                    continue;

                rayEvent.enabled = true;
            }
        }


        if (targetColliders != null)
        {
            foreach (BoxCollider col in targetColliders)
            {
                if (col == null)
                    continue;

                col.enabled = true;
            }
        }
    }
}
