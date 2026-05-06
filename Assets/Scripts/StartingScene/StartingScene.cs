using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingScene : MonoBehaviour
{
    [Header("Start Game")]
    [SerializeField] private string tutorialSceneName = "Tutorial";

    [Header("Default Continue Scene")]
    [SerializeField] private int defaultSceneNumber = 1;

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

        // TODO:
        // SettingPanel을 만든 뒤 여기에 연결
        // 예:
        // settingPanel.SetActive(true);
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