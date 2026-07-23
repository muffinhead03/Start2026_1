using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class EndingManager : MonoBehaviour
{
    [Header("UI")]
    public Scene_UI_Manager SceneUI;

    [Header("Player")]
    public Player_Move player;

    [Header("Talk")]
    public string talk1;
    public string talk2;

    [Header("Ending")]
    public string[] ending_credit;

    [Header("TimeLine")]
    public GameObject timeLine;

    private int credit_id;

    public void OnInteract()
    {
        SceneUI.SetActivePanel(1, true);
        SceneUI.UnlockPointer();
        player.SetMoveLock(true);
    }

    public void ClickButton(string choice)
    {
        SceneUI.SetActivePanel(2, false);
        if (choice == "thank_you") SceneUI.ChangeText(0, talk1);
        else SceneUI.ChangeText(0, talk2);
        timeLine.SetActive(true);
        credit_id = 0;
    }

    public void ChangeText()
    {
        SceneUI.ChangeText(1, ending_credit[credit_id++]);
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("StartingScene");
    }
}
