using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Play_Audio : MonoBehaviour
{
    private AudioSource source;

    [SerializeField] private AudioClip[] clips;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip ac)
    {
        if (source == null || ac == null) return;

        source.PlayOneShot(ac);
    }

    public void PlayAudio(int id)
    {
        if (source == null || id >= clips.Length || id <0 ) return;

        source.PlayOneShot(clips[id]);
    }

    public void PlayAudioLoop()
    {
        if (source == null) return;

        source.Play();
        source.loop = true;
    }

    public void PauseAudio()
    {
        if (source == null) return;

        source.Pause();
    }
}
