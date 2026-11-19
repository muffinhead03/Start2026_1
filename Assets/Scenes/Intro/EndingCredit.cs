using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCredit : MonoBehaviour
{
    [SerializeField] Scene_UI_Manager SceneUI;

    [SerializeField] Player_Move player;
    [SerializeField] private int text_id;

    [SerializeField] Dial_System dial;

    int credit_id;

    [TextArea(4, 8)]
    [SerializeField] string[] ending_credit;
    [SerializeField] GameObject timeLine;

    private void OnEnable()
    {
        dial.OnDialEnd += OnDialEnd;
    }

    private void OnDisable()
    {
        dial.OnDialEnd -= OnDialEnd;
    }

    public void OnDialEnd(int id)
    {
        SetActive();
    }

    public void SetActive()
    {
        InputManager.EnableDialInput();
        SceneUI.UnlockPointer();
        SceneUI.SetActiveCursor(false);
        player.SetMoveLock(true);

        timeLine.SetActive(true);
        credit_id = 0;
    }

    public void ChangeText()
    {
        SceneUI.ChangeText(text_id, ending_credit[credit_id++]);
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("StartingScene");
    }
}
