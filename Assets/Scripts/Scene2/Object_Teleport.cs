using UnityEngine;

public class Object_Teleport : MonoBehaviour
{
    [Header("Position")]
    public Vector3 targetPos;
    public Vector3 targetRot;

    public void TeleportTarget()
    {
        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(targetRot);
    }
}
