using UnityEngine;

public class BookSoundManager : MonoBehaviour
{
    public static BookSoundManager Instance;

    public Play_Audio audioPlayer;
    public AudioClip[] pickupClips;   // 크기 2

    void Awake()
    {
        Instance = this;
    }

    public void PlayPickup()
    {
        if (audioPlayer == null || pickupClips == null || pickupClips.Length == 0) return;
        audioPlayer.PlayAudio(pickupClips[Random.Range(0, pickupClips.Length)]);
    }
}