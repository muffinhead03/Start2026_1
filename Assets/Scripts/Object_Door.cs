using UnityEngine;

public class Object_Door : MonoBehaviour
{
    [Header("열리는 애니메이션")]
    public Animator animator;

    public void OpenCloseDoor()
    {
        int value = (animator.GetInteger("Open") == 0) ? 1 : 0;
        animator.SetInteger("Open", value);
    }
}
