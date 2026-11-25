using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] Dial_System dial;

    [SerializeField] Scene_UI_Manager SceneUI;
    [SerializeField] private Player_Move player;

    [SerializeField] Intro_Fade customer;

    [SerializeField] Intro_Fade after_box_open;

    [SerializeField] Intro_NextScene nextscene;

    [SerializeField] Animator box_animator;

    private enum player_mode { dial, play}

    private player_mode[] step = {player_mode.dial, player_mode.play, player_mode.dial, player_mode.dial, player_mode.play};
    private int step_id;
    private int step_customer = 3;

    private bool isCustomerCalled;

    private void Start()
    {
        step_id = 0;

        ChangeMode(step[step_id]);

        isCustomerCalled = false;
    }

    private void OnEnable()
    {
        dial.OnDialEnd += EndDial;
    }

    private void OnDisable()
    {
        dial.OnDialEnd -= EndDial;
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

    public void EndDial(int id)
    {
        if (id == 1) CallCustomer();
        else if (id == 2) return;
        else NextStep();
    }

    private void CallCustomer()
    {
        isCustomerCalled = true;

        customer.FadeOutToIn();
    }

    public void OpentheBox(bool isOpen)
    {
        if (isOpen)
        {
            after_box_open.FadeOutToIn();
            box_animator.SetInteger("Open", 1);
        }
        else nextscene.SetActive("StartingScene");
    }

    private void ChangeMode(player_mode mode)
    {
        Debug.Log(step_id);
        switch (mode)
        {
            case player_mode.dial:
                InputManager.EnableDialInput();
                SceneUI.UnlockPointer();
                SceneUI.SetActiveCursor(false);
                player.SetMoveLock(true);

                dial.StartDial();
                break;

            case player_mode.play:
                player.SetMoveLock(false);
                SceneUI.LockPointer();
                SceneUI.SetActiveCursor(true);
                InputManager.EnablePlayerInput();
                break;

            default:
                break;
        }
        
    }
}
