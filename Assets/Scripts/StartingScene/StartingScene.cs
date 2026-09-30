using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingScene : MonoBehaviour
{
    [Header("Start Game")]
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private int defaultSceneNumber = 1;

    [Header("Intro Screen")]
    [SerializeField] private Image introScreen;
    [SerializeField] private float introDuration = 2f;
    [SerializeField] private float introTargetScale = 1.3f;

    [Header("Setting Panel")]
    [SerializeField] private GameSetting gameSetting;

    private void Start()
    {
        if (introScreen != null)
        {
            Color color = introScreen.color;
            color.a = 0f;
            introScreen.color = color;

            introScreen.rectTransform.localScale = Vector3.one;
            introScreen.gameObject.SetActive(false);
        }
    }
    
    public void StartGame()
    {
        Debug.Log("Start Button Clicked");

        StartCoroutine(ShowIntroScreenAndLoadScene());
    }

    private IEnumerator ShowIntroScreenAndLoadScene()
    {
        if (introScreen == null)
        {
            Debug.LogError("Intro Screen이 연결되지 않았습니다.");
            SceneManager.LoadScene(tutorialSceneName);
            yield break;
        }

        introScreen.gameObject.SetActive(true);

        Color color = introScreen.color;
        color.a = 0f;
        introScreen.color = color;

        float elapsedTime = 0f;

        while (elapsedTime < introDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / introDuration;

            color.a = Mathf.Lerp(0f, 1f, t);
            introScreen.color = color;

            float scale = Mathf.Lerp(1f, introTargetScale, t);

            yield return null;
        }

        color.a = 1f;
        introScreen.color = color;

        SceneManager.LoadScene(tutorialSceneName);
    }

    public void ContinueGame()
    {
        Debug.Log("Continue Button Clicked");

        if (!GameData.HasSavedScene())
        {
            Debug.Log("No saved scene. Loading default scene number: " + defaultSceneNumber);
            SceneManager.LoadScene(defaultSceneNumber);
            return;
        }

        int savedSceneNumber = GameData.LoadSavedSceneNumber();

        Debug.Log("Continue to Scene Number: " + savedSceneNumber);

        SceneManager.LoadScene(savedSceneNumber);
    }

    public void OpenSetting()
    {
        Debug.Log("Setting Button Clicked");

        if (gameSetting != null)
        {
            gameSetting.OpenSetting();
        }
        else
        {
            Debug.LogError("Setting Panel이 연결되지 않았습니다.");
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit Button Clicked");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}