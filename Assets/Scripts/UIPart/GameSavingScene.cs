using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSavingScene : MonoBehaviour
{
    [Header("Scene Save Data")]
    [SerializeField] private int sceneNumber = 1;
    [SerializeField] private string sceneName = "";

    [Header("Auto Setting")]
    [SerializeField] private bool useBuildIndexAutomatically = true;
    [SerializeField] private bool useCurrentSceneNameAutomatically = true;

    [Header("Save Option")]
    [SerializeField] private bool saveOnAwake = true;

    private void Awake()
    {
        int numberToSave = sceneNumber;
        string nameToSave = sceneName;

        if (useBuildIndexAutomatically)
        {
            numberToSave = SceneManager.GetActiveScene().buildIndex;
        }

        if (useCurrentSceneNameAutomatically || string.IsNullOrEmpty(nameToSave))
        {
            nameToSave = SceneManager.GetActiveScene().name;
        }

        GameData.CurrentSceneNumber = numberToSave;
        GameData.CurrentSceneName = nameToSave;

        if (saveOnAwake)
        {
            GameData.SaveCurrentScene(numberToSave, nameToSave);
        }

        Debug.Log("GameSavingScene Saved Number: " + numberToSave);
        Debug.Log("GameSavingScene Saved Name: " + nameToSave);
    }
}