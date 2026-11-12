using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public struct EventInDial
{
    public int dial;
    public int script;
    public UnityEvent OnScriptPrint;
}

public class Dial_System_Event: Dial_System
{
    [SerializeField] private EventInDial[] events;
    private int event_id = 0;

    protected override void ShowCurrentScript()
    {
        base.ShowCurrentScript();

        if(current_dial == events[event_id].dial && current_script == events[event_id].script)
        {
            events[event_id].OnScriptPrint?.Invoke();

            event_id++;
        }
    }
}
