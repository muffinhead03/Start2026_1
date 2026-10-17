using UnityEngine;

public class Object_Turntable: MonoBehaviour
{
    [SerializeField] private Player_MouseInteraction player;

    [SerializeField] private Scene_UI_Manager SceneUI;

    [SerializeField] private GameObject LP;

    [SerializeField] private GameObject trumpet;

    [SerializeField] private Material[] mats;

    [SerializeField] private Animator panel_animator;

    int panel_id = 4;
    int button_id = 4;
    int button_len = 5;
    int current_LP;

    string LP_object_name;

    public void OnFixed()
    {
        EnablePanel();

        trumpet.SetActive(false);

        SceneUI.UnlockPointer();
        player.SetEnable();
    }

    public void OnUnFixed()
    {
        DisablePanel();
        HideLP();

        trumpet.SetActive(true);

        SceneUI.LockPointer();
        player.SetDisable();
    }

    public void ClickButton(int id)
    {
        current_LP = id;
        ShowLP(current_LP);

        for(int i = 0; i < button_len; i++)
        {
            SceneUI.SetInteractableButton(button_id + i, true);
        }
        SceneUI.SetInteractableButton(button_id + current_LP, false);

        HidePanel();
    }

    public void ClickLP()
    {
        SceneUI.SetInteractableButton(button_id + current_LP, true);

        HideLP();

        if(!panel_animator.GetBool("Open")) ShowPanel();
    }

    void ShowLP(int id)
    {
        LP.GetComponent<MeshRenderer>().material = mats[id];
        LP.SetActive(true);
    }

    void HideLP()
    {
        LP.SetActive(false);
    }

    public void TogglePanel()
    {
        if (panel_animator.GetBool("Open"))
        {
            HidePanel();
        }
        else ShowPanel();
    }

    void ShowPanel()
    {
        panel_animator.SetBool("Open", true);
    }

    void HidePanel()
    {
        panel_animator.SetBool("Open", false);
    }

    void EnablePanel()
    {
        LP_object_name = "LP_";

        for(int i=0;i<button_len; i++)
        {
            if (Player_Inventory.hasKey(LP_object_name + i))
            {
                SceneUI.SetInteractableButton(button_id+i, true);
                SceneUI.SetActiveButton(button_id + i, true);
            }
            else
            {
                SceneUI.SetActiveButton(button_id + i, false);
            }
        }

        SceneUI.SetActivePanel(panel_id, true);
    }

    void DisablePanel()
    {
        SceneUI.SetActivePanel(panel_id, false);
    }
}
