using System;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Pwd : APwd
{
    public override void PressButton(string n)
    {
        if (!isActive) return;

        if (n == "submit")
        {
            if (CheckAnswer())
            {
                UnlockEvent?.Invoke();
                GetComponent<Object_FixCamera>().UnFixCamera();
            }
            else input = "";
        }
        else input += n;
    }

    protected override bool CheckAnswer()
    {
        if (pwd == input) return true;
        else return false;
    }
}
