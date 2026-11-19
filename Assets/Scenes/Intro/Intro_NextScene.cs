using UnityEngine;
using UnityEngine.LowLevel;
using System.Collections;
using UnityEngine.SceneManagement;

public class Intro_NextScene : MonoBehaviour
{
    [SerializeField] GameObject panel_fade;

    private string nextscene;


    public void SetActive(string SceneName)
    {
        panel_fade.SetActive(true);

        panel_fade.GetComponent<Animator>().SetTrigger("Out");

        nextscene = SceneName;

        StartCoroutine(GoToNextScene());
    }

    IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(nextscene);
    }
}
