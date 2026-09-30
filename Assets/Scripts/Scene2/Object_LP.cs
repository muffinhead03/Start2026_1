using UnityEngine;
using System.Collections;

public class Object_LP : MonoBehaviour
{
    Animator animator;
    GameObject LP;

    public void PutOnLP()
    {
        animator = GetComponent<Object_PutOn>().putOn.GetComponent<Animator>();
        animator.enabled = true;
        animator.SetTrigger("On");
        GetComponent<Collider>().enabled = false;

        StartCoroutine(PlayLP());
    }

    IEnumerator PlayLP()
    {
        yield return new WaitForSeconds(5f);

        StopLP();
    }

    public void StopLP()
    {
        animator = GetComponent<Object_PutOn>().putOn.GetComponent<Animator>();
        animator.SetTrigger("Off");
        animator.enabled = false;
        GetComponent<Collider>().enabled = true;
        GetComponent<Object_KeyLocked>().UseKey(GetComponent<Object_PutOn>().putOn.GetComponent<Object_Grabbable>().objectName);
    }
}
