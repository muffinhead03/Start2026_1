using UnityEngine;

public class Book_Inspectable: Object_Inspectable
{
    [SerializeField] private int max_page;

    [SerializeField] private Controll_Material material;

    [SerializeField] private Play_Audio audio_player;
    private int current_page;

    private void Awake()
    {
        current_page = 0;
    }

    public override void OnInspectPosition()
    {
        base.OnInspectPosition();
    }

    public void NextPage()
    {
        if (current_page + 1 >= max_page) return;

        current_page++;

        material.SetTexture(current_page);

        audio_player.PlayAudio(0);
    }

    public void PreviousPage()
    {
        if (current_page <= 0) return;

        current_page--;

        material.SetTexture(current_page);

        audio_player?.PlayAudio(0);
    }
}
