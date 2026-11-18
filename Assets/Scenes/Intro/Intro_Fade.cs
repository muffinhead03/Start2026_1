using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Intro_Fade : MonoBehaviour
{
    [SerializeField] GameObject panel_fade;

    [SerializeField] UnityEvent OnBlackEnter, OnBlackExit; 

    public void FadeOutToIn()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        panel_fade.SetActive(true);

        panel_fade.GetComponent<Animator>().SetTrigger("Out");

        yield return new WaitForSeconds(2.0f);

        DuringBlack();
    }

    protected virtual void DuringBlack()
    {
        OnBlackEnter?.Invoke();

        StartCoroutine(FadeIn(1.0f));
    }

    protected IEnumerator FadeIn(float wait)
    {
        yield return new WaitForSeconds(wait);

        panel_fade.GetComponent<Animator>().SetTrigger("In");

        yield return new WaitForSeconds(2.0f);

        panel_fade.SetActive(false);

        AfterBlack();
    }

    protected virtual void AfterBlack()
    {
        OnBlackExit?.Invoke();
    }
}
