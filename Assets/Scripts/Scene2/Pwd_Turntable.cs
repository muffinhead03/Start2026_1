using System.Text.RegularExpressions;
using UnityEngine;

public class Pwd_Turntable : APwd
{
    [SerializeField] protected Player_MouseInteraction player;

    [SerializeField] private Controll_Mesh controll_mesh;

    int current_index;
    string track_id;

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
        track_id = n;
    }

    public override void PressButton(string n)
    {
        if (!isActive) return;

        input = track_id + n;

        if (CheckAnswer())
        {
            input = "";

            current_index += 2;

            int material_index = current_index / 2;
            controll_mesh.ShowMesh("_MainTex", material_index);

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
