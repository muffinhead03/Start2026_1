using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Object_LP : MonoBehaviour
{
    [SerializeField] private Scene_UI_Manager SceneUI;

    [SerializeField] private MeshRenderer mesh;

    [SerializeField] private Material[] mats;

    int panel_id = 4;
    int button_id = 4;
    int button_len = 5;

    string LP_object_name;
    Button[] buttons;
    Animator animator;
    GameObject LP;

    public void OnFixed()
    {
        ShowButtons();

        SceneUI.UnlockPointer();
    }

    public void OnUnFixed()
    {
        HideButtons();
        HideLP();

        SceneUI.LockPointer();
    }

    public void ClickButton(int id)
    {
        mesh.material = mats[id];
        mesh.enabled = true;

        for(int i = 0; i < button_len; i++)
        {
            SceneUI.SetInteractableButton(button_id + i, true);
        }
        SceneUI.SetInteractableButton(button_id + id, false);
    }

    void ShowButtons()
    {
        LP_object_name = "LP_";

        for(int i=0;i<button_len; i++)
        {
            if (Player_Inventory.hasKey(LP_object_name + i))
            {
                SceneUI.SetInteractableButton(button_id, true);
                SceneUI.SetActiveButton(button_id + i, true);
            }
            else
            {
                SceneUI.SetActiveButton(button_id + i, false);
            }
        }

        SceneUI.SetActivePanel(panel_id, true);
    }

    void HideButtons()
    {
        SceneUI.SetActivePanel(panel_id, false);
    }

    void HideLP()
    {
        mesh.enabled = false;
    }
    public void PutOnLP()
    {
        animator = GetComponent<Object_PutOn>().putOn.GetComponent<Animator>();
        animator.enabled = true;
        animator.SetTrigger("On");
        GetComponent<Collider>().enabled = false;

        StartCoroutine(PlayLP());
    }

    IEnumerator PlayLP()
    {
        yield return new WaitForSeconds(5f);

        StopLP();
    }

    public void StopLP()
    {
        animator = GetComponent<Object_PutOn>().putOn.GetComponent<Animator>();
        animator.SetTrigger("Off");
        animator.enabled = false;
        GetComponent<Collider>().enabled = true;
        GetComponent<Object_KeyLocked>().UseKey(GetComponent<Object_PutOn>().putOn.GetComponent<Object_Grabbable>().objectName);
    }
}
