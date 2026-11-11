using System.Collections;
using UnityEngine;

public class Intro_StepForCustomer : MonoBehaviour
{
    [SerializeField] GameObject panel_fade;

    [SerializeField] Transform player, customer;
    [SerializeField] Transform target_player, target_customer;
    [SerializeField] Player_Move player_look;

    [SerializeField] Play_Audio audio_player;

    [SerializeField] IntroSceneManager intro;

    public void SetActive()
    {
        panel_fade.SetActive(true);

        panel_fade.GetComponent<Animator>().SetTrigger("Out");

        StartCoroutine(DuringBlack());
    }

    IEnumerator DuringBlack()
    {
        yield return new WaitForSeconds(2.0f);

        SetObjectPos(player, target_player);
        SetObjectPos(customer, target_customer);

        player_look.LookForward();

        audio_player.PlayAudio(0);

        yield return new WaitForSeconds(1.0f);

        FadeIn();
    }

    private void FadeIn()
    {
        panel_fade.GetComponent<Animator>().SetTrigger("In");

        StartCoroutine(AfterBlack());
    }

    IEnumerator AfterBlack()
    {
        yield return new WaitForSeconds(2.0f);

        panel_fade.SetActive(false);

        intro.NextStep();
    }

    private void SetObjectPos(Transform obj, Transform target_obj)
    {
        obj.position = target_obj.position;
        obj.rotation = target_obj.rotation;
    }
}
