using UnityEngine;

public class ProximityMusicVolume : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform enemy;
    public AudioSource backgroundMusic;

    [Header("Distance Settings")]
    public float maxDistance = 20f;
    public float minDistance = 3f;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float minVolume = 0.02f;

    [Range(0f, 1f)]
    public float maxVolume = 1f;

    [Header("Drama Settings")]
    public AnimationCurve volumeCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.6f, 0.15f),
        new Keyframe(0.85f, 0.6f),
        new Keyframe(1f, 1f)
    );

    public float volumeChangeSpeed = 4f;

    void Start()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
    }

    void Update()
    {
        if (player == null || enemy == null || backgroundMusic == null)
            return;

        float distance = Vector3.Distance(player.position, enemy.position);

        float closeness = Mathf.InverseLerp(maxDistance, minDistance, distance);

        float dramaticCloseness = volumeCurve.Evaluate(closeness);

        float targetVolume = Mathf.Lerp(minVolume, maxVolume, dramaticCloseness);

        backgroundMusic.volume = Mathf.Lerp(
            backgroundMusic.volume,
            targetVolume,
            Time.deltaTime * volumeChangeSpeed
        );
    }
}