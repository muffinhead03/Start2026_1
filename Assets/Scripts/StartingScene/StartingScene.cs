using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingScene : MonoBehaviour
{
    [Header("Start Game")]
    [SerializeField] private string tutorialSceneName = "Tutorial";

    [Header("Default Continue Scene")]
    [SerializeField] private int defaultSceneNumber = 1;

    [Header("Setting Panel")]
    [SerializeField] private GameObject settingPanel;

    public void StartGame()
    {
        Debug.Log("Start Button Clicked");

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

        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
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