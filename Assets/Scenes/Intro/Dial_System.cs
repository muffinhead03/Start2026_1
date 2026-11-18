using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dial_System : MonoBehaviour
{
    [SerializeField] Dial[] dials;

    [SerializeField] protected Scene_UI_Manager SceneUI;

    [SerializeField] float type_speed;

    public Action<int> OnDialEnd;

    protected InputAction next;
    protected int current_dial;
    protected int current_script;

    [SerializeField] protected int panel_id = 3;
    [SerializeField] private int button_id = 4;
    [SerializeField] private int text_talker = 3;
    [SerializeField] private int text_talking = 4;
    private bool isChoiceDial = false;

    private void Start()
    {
        current_dial = 0;
    }

    private void OnDisable()
    {
        SceneUI.LockPointer();
    }

    public virtual void StartDial()
    {

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

        current_dial++;
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
        current_script++;

        if (current_script >= dials[current_dial].scripts.Length) EndDial();
        else ShowCurrentScript();
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
        string talker = dials[current_dial].scripts[current_script].talker;
        SceneUI.ChangeText(text_talker, talker);

        string script = dials[current_dial].scripts[current_script].text;
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
