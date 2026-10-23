using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Organ : Object_Pwd
{
    [Header("사운드")]
    public AudioClip[] audio_organ;

    private Play_Audio audio_player;

    bool isPipeChanged;

    void Start()
    {
        isPipeChanged = false;

        input = "";

        isActive = false;

        audio_player = GetComponent<Play_Audio>();
    }

    public void ActivateAudio()
    {
        isPipeChanged = true;
    }

    public override void PressButton(string n)
    {
        if (!isPipeChanged || !isActive) return;

        audio_player.PlayAudio(audio_organ[Convert.ToInt32(n)]);

        input += n;
        if (input.Length >= pwd.Length)
        {
            if (CheckAnswer())
            {
                UnlockEvent?.Invoke();
                GetComponent<Object_FixCamera>().UnFixCamera();
            }
            else
            {
                GetComponent<Object_FixCamera>().UnFixCamera();
            }
        }
    }
}
