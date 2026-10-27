using System.Xml.Serialization;
using UnityEngine;

public class Object_Turntable: MonoBehaviour
{
    [SerializeField] private Scene_UI_Manager SceneUI;

    [SerializeField] private Object_LP LP;

    [SerializeField] private GameObject trumpet;

    [SerializeField] private Animator panel_animator;

    [SerializeField] private Pwd_Turntable pwd_LP;

    int panel_id = 4;
    int button_id = 4;
    int button_len = 5;
    int current_LP;

    string[] LP_name = { "G", "R", "B", "Y", "W" };

    string LP_object_name;

    public void OnFixed()
    {
        EnablePanel();

        trumpet.SetActive(false);

        SceneUI.UnlockPointer();
    }

    public void OnUnFixed()
    {
        DisablePanel();
        LP.HideLP();

        trumpet.SetActive(true);

        SceneUI.LockPointer();
    }

    public void ClickButton(int id)
    {
        current_LP = id;

        LP.ShowLP(current_LP);

        for(int i = 0; i < button_len; i++)
        {
            SceneUI.SetInteractableButton(button_id + i, true);
        }
        SceneUI.SetInteractableButton(button_id + current_LP, false);

        HidePanel();

        pwd_LP.PutOnLP(LP_name[current_LP]);
    }

    public void ResetPanel()
    {
        SceneUI.SetInteractableButton(button_id + current_LP, true);

        if(!panel_animator.GetBool("Open")) ShowPanel();
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
