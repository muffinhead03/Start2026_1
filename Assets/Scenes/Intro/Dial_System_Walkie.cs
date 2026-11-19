using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class Dial_System_Walkie: Dial_System
{
    [SerializeField] string[] texts;

    [SerializeField] WalkieTalkieExamine walkietalkie;

    [SerializeField] Player_Move player;

    private InputAction open, close;

    public void ShowScriptDuringPlay(int id)
    {
        SceneUI.SetActivePanel(panel_id, true);

        StartCoroutine(TypeText(texts[id]));
    }

    public override void StartDial()
    {
        InputManager.EnableDialInput();
        SceneUI.UnlockPointer();
        SceneUI.SetActiveCursor(false);
        player.SetMoveLock(true);

        base.StartDial();

        walkietalkie.StartExamine();
    }

    public override void EndDial()
    {
        player.SetMoveLock(false);
        SceneUI.LockPointer();
        SceneUI.SetActiveCursor(true);
        InputManager.EnablePlayerInput();

        base.EndDial();

        walkietalkie.EndExamine();

        SetActive(true);
    }

    private void OnDisable()
    {
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        open = InputSystem.actions.FindAction("HintOpen");
        close = InputSystem.actions.FindAction("HintClose");

        if (isActive)
        {
            open.performed += OnShowPanel;
            close.performed += OnHidePanel;
        }
        else
        {
            open.performed -= OnShowPanel;
            close.performed -= OnHidePanel;
        }
    }



    private void OnShowPanel(InputAction.CallbackContext ctx)
    {
        ShowPanel();
    }
    private void OnHidePanel(InputAction.CallbackContext ctx)
    {
        HidePanel();
    }

    private void ShowPanel()
    {
        InputManager.EnableUIInput();
        SceneUI.UnlockPointer();
        SceneUI.SetActiveCursor(false);
        player.SetMoveLock(true);

        SceneUI.SetActivePanel(panel_id, true);

        walkietalkie.StartExamine();
    }

    private void HidePanel()
    {
        player.SetMoveLock(false);
        SceneUI.LockPointer();
        SceneUI.SetActiveCursor(true);
        InputManager.EnablePlayerInput();

        SceneUI.SetActivePanel(panel_id, false);

        walkietalkie.EndExamine();
    }
}
