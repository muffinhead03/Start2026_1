using System.Text.RegularExpressions;
using UnityEngine;

public class Pwd_Turntable : APwd
{
    [SerializeField] protected Player_MouseInteraction player;

    int current_index;

    private void Start()
    {
        isActive = false;

        current_index = 0;
    }
    public void SetActive(bool active)
    {
        isActive = active;

        if (active)
        {
            player.SetEnable();
        }
        else player.SetDisable();
    }

    public void PutOnLP(string n)
    {
        input = n;
    }

    public override void PressButton(string n)
    {
        if (!isActive) return;

        input += n;

        if (CheckAnswer())
        {
            input = "";

            current_index += 2;

            if(current_index >= pwd.Length)
            {
                UnlockEvent?.Invoke();
            }
        }
    }

    protected override bool CheckAnswer()
    {
        if (current_index + 1 >= pwd.Length) return false;

        string match = @"[A-Z][0-9]";
        string current_pwd = pwd[current_index] + "" + pwd[current_index + 1];
        if (!Regex.IsMatch(input, match) || !Regex.IsMatch(input, match)) return false;

        return input.Equals(current_pwd);
    }
}
