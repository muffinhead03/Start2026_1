using UnityEngine;
using UnityEngine.Events;

public class Dial_System_Event: Dial_System
{
    [SerializeField] private int[] event_dial;
    [SerializeField] private int[] event_script;

    [SerializeField] UnityEvent EventInDial;
    private int event_id = 0;

    protected override void ShowCurrentScript()
    {
        base.ShowCurrentScript();

        if(current_dial == event_dial[event_id] && current_script == event_script[event_id])
        {
            EventInDial?.Invoke();
        }
    }
}
