using System;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Pwd : APwd
{
    [SerializeField] protected Player_MouseInteraction player;

    [SerializeField] private TextMeshPro text_mesh;

    private string input_text;

    [SerializeField] private int max_len = 10;

    public UnityEvent OnSuccess;
    public UnityEvent OnFailure;

    private void Start()
    {
        isActive = false;

        isLocked = true;

        input = "";
        input_text = "-";
        ChangeTextMesh();
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (active)
        {
            player.SetEnable();

            input = "";
            input_text = "-";
            ChangeTextMesh();
        }
        else player.SetDisable();
    }

    public override void PressButton(string n)
    {
        if (!isActive || !isLocked) return;

        if (n == "submit")
        {
            if (CheckAnswer())
            {
                isLocked = false;

                UnlockEvent?.Invoke();
                GetComponent<Object_FixCamera>().UnFixCamera();

                OnSuccess?.Invoke();
            }

            else OnFailure?.Invoke();

                input = "";
            input_text = "-";
            ChangeTextMesh();
        }
        else
        {
            if (input.Length >= max_len) return;

            input += n;
            if (input_text == "-") input_text = "*";
            else input_text += "*";
            ChangeTextMesh();
        }
    }

    private void ChangeTextMesh()
    {
        if (input_text == text_mesh.text) return;

        text_mesh.text = input_text;
    }

    protected override bool CheckAnswer()
    {
        if (pwd == input) return true;
        else return false;
    }
}
