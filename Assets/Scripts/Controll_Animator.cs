using UnityEngine;

public class Controll_Animator : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void AnimSetInteger()
    {
        int value = (animator.GetInteger(0)==0)?1:0;
        animator.SetInteger(0, value);
    }
}
