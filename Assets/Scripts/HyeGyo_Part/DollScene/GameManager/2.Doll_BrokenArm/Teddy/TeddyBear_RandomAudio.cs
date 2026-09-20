using UnityEngine;

public class TeddyBear_RandomAudio : MonoBehaviour
{
    // ================================================
    // Audio
    // ================================================

    [Header("Audio")]

    [Tooltip("기존 Play_Audio 컴포넌트")]
    [SerializeField]
    private Play_Audio playAudio;


    [Tooltip("랜덤으로 재생할 TeddyBear 사운드 5개")]
    [SerializeField]
    private AudioClip[] audioClips;


    // ================================================
    // Audio Source
    // ================================================

    private AudioSource audioSource;


    // ================================================
    // Awake
    // ================================================

    private void Awake()
    {
        if (playAudio == null)
        {
            playAudio =
                GetComponent<Play_Audio>();
        }


        audioSource =
            GetComponent<AudioSource>();
    }


    // ================================================
    // Random Audio
    // ================================================

    public void PlayRandomAudio()
    {
        // --------------------------------------------
        // 기존 소리가 재생 중이면
        // 새로운 소리를 실행하지 않음
        // --------------------------------------------

        if (audioSource != null &&
            audioSource.isPlaying)
        {
            return;
        }


        // --------------------------------------------
        // Play_Audio 확인
        // --------------------------------------------

        if (playAudio == null)
        {
            Debug.LogWarning(
                "[TeddyBear_RandomAudio] " +
                "Play_Audio가 없습니다.",
                this
            );

            return;
        }


        // --------------------------------------------
        // AudioClip 확인
        // --------------------------------------------

        if (audioClips == null ||
            audioClips.Length == 0)
        {
            Debug.LogWarning(
                "[TeddyBear_RandomAudio] " +
                "AudioClip이 등록되지 않았습니다.",
                this
            );

            return;
        }


        // --------------------------------------------
        // 랜덤 선택
        // --------------------------------------------

        int randomIndex =
            Random.Range(
                0,
                audioClips.Length
            );


        AudioClip selectedClip =
            audioClips[randomIndex];


        if (selectedClip == null)
        {
            return;
        }


        // --------------------------------------------
        // 기존 Play_Audio 시스템 사용
        // --------------------------------------------

        playAudio.PlayAudio(
            selectedClip
        );


        Debug.Log(
            "[TeddyBear_RandomAudio] " +
            $"재생 : {selectedClip.name}",
            this
        );
    }
}