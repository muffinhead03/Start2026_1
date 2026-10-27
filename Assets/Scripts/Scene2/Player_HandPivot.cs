using LLMUnity;
using UnityEngine;

public class Player_HandPivot : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float radius = 0.2f;
    [SerializeField] private float distance = 2f;

    [SerializeField] private Player_Grab player;

    private Vector3 origin;
    private Vector3 direction;
    private Vector3 defaultLocalPosition;
    float defaultDistance;
    private bool isHandValid;

    void Start()
    {
        defaultLocalPosition = transform.localPosition;
        defaultDistance = defaultLocalPosition.magnitude;
        isHandValid = true;
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

        //Vector3 targetLocalPosition = defaultLocalPosition;

        bool isValid = true;

        // SphereCast
        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, wallLayer))
        {
            float hitDistance = hit.distance;

            if (hitDistance < defaultDistance)
            {
                //Vector3 worldTargetPosition = origin + direction * hitDistance;

                //targetLocalPosition = transform.parent.InverseTransformPoint(worldTargetPosition);

                isValid = false;
            }
        }

        if (isHandValid != isValid)
        {
            isHandValid = isValid;
            player.SetHandValidate(isHandValid);
        }
        //transform.localPosition = targetLocalPosition;
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
