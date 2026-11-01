using System.Text.RegularExpressions;
using UnityEngine;

public class Pwd_Turntable : APwd
{
    [SerializeField] protected Player_MouseInteraction player;

    [SerializeField] private Controll_Mesh controll_mesh;

    [SerializeField] private Play_Audio audio_player;

    private char[] lp_audio_id = { 'W', 'G', 'R', 'B', 'Y'};
 
    int current_index;
    string lp_id;

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
        lp_id = n;
    }

    public override void PressButton(string n)
    {
        if (!isActive) return;

        input = lp_id + n;

        PlayMusic();

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

    private void PlayMusic()
    {
        for(int i=0;i<lp_audio_id.Length;i++)
        {
            if (lp_audio_id[i] == input[0])
            {
                int track_id = input[1] - '0';
                int clip_id = 3 * i + track_id;

                audio_player.PlayAudioNoOverlap(clip_id);

                return;
            }
        }
    }

    public void PauseMusic()
    {
        audio_player.PauseAudio();
    }
}
