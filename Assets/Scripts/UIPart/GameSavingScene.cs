using UnityEngine;

public class GameSavingScene : MonoBehaviour
{
    [SerializeField] private int sceneNumber;

    private void Start()
    {
        GameData.CurrentSceneNumber = sceneNumber;
        GameData.SaveAll();

        Debug.Log("Saved Scene Number: " + sceneNumber);
    }
}