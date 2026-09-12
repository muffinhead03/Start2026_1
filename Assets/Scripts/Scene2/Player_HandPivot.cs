using LLMUnity;
using UnityEngine;

public class Player_HandPivot : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float radius = 0.2f;
    [SerializeField] private float distance = 2f;

    private Vector3 origin;
    private Vector3 direction;
    private Vector3 defaultLocalPosition;
    float defaultDistance;

    void Start()
    {
        defaultLocalPosition = transform.localPosition;
        defaultDistance = defaultLocalPosition.magnitude;
    }

    void Update()
    {
        UpdateHandPosition();
    }

    private void UpdateHandPosition()
    {
        origin = Camera.main.transform.position;
        Vector3 pivotWorldPos = Camera.main.transform.TransformPoint(defaultLocalPosition);
        direction = (pivotWorldPos - origin).normalized;

        Vector3 targetLocalPosition = defaultLocalPosition;

        // SphereCast
        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, wallLayer))
        {
            float hitDistance = hit.distance;

            if (hitDistance < defaultDistance)
            {
                Vector3 worldTargetPosition = origin + direction * hitDistance;

                targetLocalPosition = transform.parent.InverseTransformPoint(worldTargetPosition);
            }
        }

        transform.localPosition = targetLocalPosition;
    }

    private void OnDrawGizmos()
    {
        if (Camera.main == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            origin,
            radius
        );

        Gizmos.DrawLine(
            origin,
            origin + direction * distance
        );
    }
}
