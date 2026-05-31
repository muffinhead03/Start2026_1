using UnityEngine;

public class MusicBoxPlay : MonoBehaviour
{
    public HintManager hintManager;
    public AudioSource audioSource;
    bool played = false;

    public void Play()
    {
        if (!played)
        {
            played = true;
            hintManager.currentPlayerState.foundClues.Add("clue_music_box");
            hintManager.currentPlayerState.completedSteps.Add(4);
            Debug.Log("[MusicBox] 오르골 재생 — 솔도미라");
        }
        audioSource?.Play();
    }
}