using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    /*
     * 1. 다이얼 모드. 회전, 이동 잠금
     * 2. 플레이모드, 이동 잠금
     * 3. 플레이모드
     * 4. 다이얼 모드, 회전, 이동 잠금
     * 5. 플레이모드, 이동 잠금
     */

    [SerializeField] Dial_System dial;

    [SerializeField] Player_Move player;

    [SerializeField] Scene_UI_Manager SceneUI;

    private enum player_mode { dial, play}

    private player_mode[] step = {player_mode.dial, player_mode.play, player_mode.dial, player_mode.play};
    private int step_id;

    private void Start()
    {
        step_id = 0;

        ChangeMode(step[step_id]);
    }

    public void NextStep()
    {
        step_id++;

        if(step_id >= step.Length)
        {
            SceneManager.LoadScene("DollSceneUpdate");
        }
        else
        {
            ChangeMode(step[step_id]);
        }
    }

    private void ChangeMode(player_mode mode)
    {
        switch (mode)
        {
            case player_mode.dial:
                InputManager.EnableDialInput();
                SceneUI.SetActiveCursor(false);
                player.SetMoveLock(true);

                if (step_id == 0)
                {
                    dial.StartDial(0);
                }
                else
                {
                    dial.StartDial(1);
                }
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
