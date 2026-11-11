using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Dial_System : MonoBehaviour
{
    [SerializeField] Dial[] dials;

    [SerializeField] Scene_UI_Manager SceneUI;

    [SerializeField] float type_speed;

    [SerializeField] UnityEvent OnDialEnd;

    private InputAction next;
    protected int current_dial;
    protected int current_script;

    private void Start()
    {
        current_dial = 0;
    }

    public void StartDial()
    {
        current_script = 0;

        SceneUI.SetActivePanel(3, true);

        RegisterInputAction();

        ShowCurrentScript();
    }

    public void EndDial()
    {
        SceneUI.SetActivePanel(3, false);

        UnregisterInputAction();

        OnDialEnd?.Invoke();

        current_dial++;
    }

    private void RegisterInputAction()
    {
        next = InputSystem.actions.FindAction("Next");
        next.performed += OnNextPerformed;
    }

    private void UnregisterInputAction()
    {
        if (next != null) next.performed -= OnNextPerformed;
    }

    private void OnNextPerformed(InputAction.CallbackContext ctx)
    {
        NextScript();
    }

    private void NextScript()
    {
        current_script++;

        if (current_script >= dials[current_dial].scripts.Length) EndDial();
        else ShowCurrentScript();
    }

    protected virtual void ShowCurrentScript()
    {
        string talker = dials[current_dial].scripts[current_script].talker;
        SceneUI.ChangeText(3, talker);

        string script = dials[current_dial].scripts[current_script].text;
        StartCoroutine(TypeText(script));
    }

    protected IEnumerator TypeText(string full_script)
    {
        string temp = "";

        next.Disable();

        foreach (char c in full_script)
        {
            temp += c;
            SceneUI.ChangeText(4, temp);
            yield return new WaitForSeconds(type_speed);
        }

        next.Enable();
    }
}
