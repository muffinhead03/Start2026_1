using UnityEngine;

public class Object_Pwd_Locked : MonoBehaviour
{
    public bool isDebug;
    private bool isLocked = true;

    [Header("열리는 애니메이션")]
    public Animator animator;

    void Start()
    {
        if (isDebug) UnlockDoor();
    }

    public void OnInteract()
    {
        if (!isLocked) OpenCloseDoor();
    }

    public void UnlockDoor()
    {

        isLocked = false;
        Debug.Log("비밀번호 일치! 문이 열립니다.");
        OpenCloseDoor();
    }

    private void OpenCloseDoor()
    {
        int value = (animator.GetInteger("Open") == 0) ? 1 : 0;
        animator.SetInteger("Open", value);
    }
}