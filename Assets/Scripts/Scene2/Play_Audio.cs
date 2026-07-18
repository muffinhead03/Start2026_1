using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Play_Audio : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip ac)
    {
        if (source == null || ac == null) return;

        source.PlayOneShot(ac);
    }
}
