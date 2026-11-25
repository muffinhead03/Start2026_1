using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dial_System : MonoBehaviour
{
    [SerializeField] protected Dial[] dials;

    [SerializeField] protected Scene_UI_Manager SceneUI;

    [SerializeField] float type_speed;

    public Action<int> OnDialEnd;

    protected InputAction next;
    protected Dial dial;
    protected int current_dial;
    protected int current_script;
    protected bool isInitialized = false;

    [SerializeField] protected int panel_id = 3;
    [SerializeField] private int button_id = 4;
    [SerializeField] protected int text_talker = 3;
    [SerializeField] protected int text_talking = 4;
    private bool isChoiceDial = false;

    public virtual void InitDial()
    {
        current_dial = 0;
        isInitialized = true;
    }

    private void OnDisable()
    {
        SceneUI.LockPointer();
    }

    protected virtual void GetDial()
    {
        if (current_dial < dials.Length) dial = dials[current_dial];
        else dial = null;
    }

    public virtual void StartDial()
    {
        if (!isInitialized) InitDial();
        else if (current_dial + 1 < dials.Length) current_dial++;

        GetDial();
        
        current_script = 0;

        SceneUI.SetActivePanel(panel_id, true);
        SceneUI.SetActivePanel(button_id, false);

        RegisterInputAction();

        ShowCurrentScript();
    }

    public virtual void EndDial()
    {
        SceneUI.SetActivePanel(panel_id, false);
        SceneUI.SetActivePanel(button_id, false);

        UnregisterInputAction();

        OnDialEnd?.Invoke(current_dial);
    }

    protected void RegisterInputAction()
    {
        next = InputSystem.actions.FindAction("Next");
        next.performed += OnNextPerformed;
    }

    protected void UnregisterInputAction()
    {
        if (next != null) next.performed -= OnNextPerformed;
    }

    protected void OnNextPerformed(InputAction.CallbackContext ctx)
    {
        if (!isChoiceDial) NextScript();
        else NextChoice();
    }

    protected void NextScript()
    {
        if (current_script + 1 >= dial.scripts.Length) EndDial();
        else
        {
            current_script++;
            ShowCurrentScript();
        }
    }

    public void ClickChoiceButton()
    {
        EndDial();
    }

    public void NextChoice()
    {
        ShowChoiceButton();
    }

    protected virtual void ShowCurrentScript()
    {
        string talker = dial.scripts[current_script].talker;
        SceneUI.ChangeText(text_talker, talker);

        string script = dial.scripts[current_script].text;
        StartCoroutine(TypeText(script));
    }

    public void SetChoiceDial()
    {
        isChoiceDial = true;
    }

    protected void ShowChoiceButton()
    {
        SceneUI.SetActivePanel(button_id, true);
        SceneUI.UnlockPointer();

        next.Disable();
    }


    protected IEnumerator TypeText(string full_script)
    {
        string temp = "";

        if(next!=null) next.Disable();

        foreach (char c in full_script)
        {
            temp += c;
            SceneUI.ChangeText(text_talking, temp);
            yield return new WaitForSeconds(type_speed);
        }

        if(next!=null) next.Enable();
    }
}
