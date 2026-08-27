using UnityEngine;


public class SpringTrainPhysicsLock : MonoBehaviour
{
    [Header("시작할 때 자식 Rigidbody 모두 잠금")]
    [SerializeField]
    private bool lockOnAwake = true;


    private void Awake()
    {
        if (lockOnAwake)
        {
            LockAllChildRigidbodies();
        }
    }


    [ContextMenu("자식 Rigidbody 모두 잠금")]
    public void LockAllChildRigidbodies()
    {
        Rigidbody[] rigidbodies =
            GetComponentsInChildren<Rigidbody>(
                true
            );


        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody rb =
                rigidbodies[i];

            if (rb == null)
            {
                continue;
            }

            rb.useGravity = false;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints =
                RigidbodyConstraints.FreezeAll;
        }


        Debug.Log(
            "[SpringTrainPhysicsLock] 자식 Rigidbody 잠금 완료 : " +
            rigidbodies.Length +
            "개",
            this
        );
    }
}