using UnityEngine;

public class Controll_Animator : MonoBehaviour
{
    Animator animator;

    public string parameter;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void AnimSetInteger()
    {
        int value = (animator.GetInteger(parameter)==0)?1:0;
        animator.SetInteger(parameter, value);
    }
}
