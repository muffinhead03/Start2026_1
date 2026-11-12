using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] Dial_System dial;

    [SerializeField] Player_Move player;

    [SerializeField] Scene_UI_Manager SceneUI;

    [SerializeField] Intro_StepForCustomer customer;

    [SerializeField] Intro_NextScene nextscene;

    private enum player_mode { dial, play}

    private player_mode[] step = {player_mode.dial, player_mode.play, player_mode.dial, player_mode.dial};
    private int step_id;
    private int step_customer = 3;

    private bool isCustomerCalled;

    private void Start()
    {
        step_id = 0;

        ChangeMode(step[step_id]);

        isCustomerCalled = false;
    }

    public void NextStep()
    {
        if(step_id+1 == step_customer && !isCustomerCalled)
        {
            CallCustomer();
            return;
        }

        step_id++;

        if(step_id >= step.Length)
        {
            //SceneManager.LoadScene("DollSceneUpdate");
        }
        else
        {
            ChangeMode(step[step_id]);
        }
    }

    private void CallCustomer()
    {
        isCustomerCalled = true;

        customer.SetActive();
    }

    public void OpentheBox(bool isOpen)
    {
        if (isOpen) nextscene.SetActive("DollSceneUpdate");
        else nextscene.SetActive("StartingScene");
    }

    private void ChangeMode(player_mode mode)
    {
        switch (mode)
        {
            case player_mode.dial:
                InputManager.EnableDialInput();
                SceneUI.SetActiveCursor(false);
                player.SetMoveLock(true);

                dial.StartDial();
                break;

            case player_mode.play:
                player.SetMoveLock(false);
                SceneUI.SetActiveCursor(true);
                InputManager.EnablePlayerInput();
                break;

            default:
                break;
        }
        
    }
}
