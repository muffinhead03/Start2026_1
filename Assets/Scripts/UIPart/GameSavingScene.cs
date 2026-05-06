using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSavingScene : MonoBehaviour
{
    [Header("Auto Setting")]
    [SerializeField] private bool useBuildIndexAutomatically = true;

    [Header("Manual Scene Number")]
    [SerializeField] private int sceneNumber = 1;

    private void Awake()
    {
        int numberToSave;

        if (useBuildIndexAutomatically)
        {
            numberToSave = SceneManager.GetActiveScene().buildIndex;
        }
        else
        {
            numberToSave = sceneNumber;
        }

        GameData.SaveCurrentScene(numberToSave);
    }
}