using UnityEngine;

public class Coin_ReleaseSound : MonoBehaviour
{
    // =============================================
    // Player
    // =============================================

    [Header("Player")]

    [SerializeField]
    private Player_Grab playerGrab;


    // =============================================
    // Audio
    // =============================================

    [Header("Drop Sound")]

    [Tooltip("일반적으로 E로 동전을 놓았을 때 사용할 Play_Audio")]
    [SerializeField]
    private Play_Audio dropAudio;


    [Tooltip("일반적으로 동전을 손에서 놓을 때 나는 소리")]
    [SerializeField]
    private AudioClip dropClip;


    // =============================================
    // State
    // =============================================

    private bool wasHeld = false;

    // Coin_Insert로 빠지는 경우
    // 다음 Release Sound를 1회 무시
    private bool suppressNextReleaseSound = false;


    // =============================================
    // Start
    // =============================================

    private void Start()
    {
        if (playerGrab == null)
        {
            playerGrab =
                FindFirstObjectByType<Player_Grab>();
        }


        wasHeld =
            IsHeldByPlayer();
    }


    // =============================================
    // Update
    // =============================================

    private void Update()
    {
        bool isCurrentlyHeld =
            IsHeldByPlayer();


        // 이전에는 손에 있었는데
        // 지금 Hand에서 빠졌음
        if (wasHeld &&
            !isCurrentlyHeld)
        {
            // -------------------------------------
            // Coin_Insert에 의해 빠진 경우
            // Drop Sound 재생 안 함
            // -------------------------------------

            if (suppressNextReleaseSound)
            {
                suppressNextReleaseSound = false;

                Debug.Log(
                    "[Coin_ReleaseSound] " +
                    "Coin Insert 감지 → Drop Sound 생략",
                    this
                );
            }

            // -------------------------------------
            // 일반 E Release
            // -------------------------------------

            else
            {
                PlayDropSound();
            }
        }


        wasHeld =
            isCurrentlyHeld;
    }


    // =============================================
    // Coin_Insert에서 호출
    // =============================================

    public void SuppressNextReleaseSound()
    {
        suppressNextReleaseSound = true;


        Debug.Log(
            "[Coin_ReleaseSound] " +
            "다음 Release Sound 억제 예약",
            this
        );
    }


    // =============================================
    // 현재 Player Hand에 있는지
    // =============================================

    private bool IsHeldByPlayer()
    {
        if (playerGrab == null)
        {
            return false;
        }


        if (playerGrab.Hand == null)
        {
            return false;
        }


        return
            transform.parent ==
            playerGrab.Hand;
    }


    // =============================================
    // Drop Sound
    // =============================================

    private void PlayDropSound()
    {
        if (dropAudio == null)
        {
            Debug.LogWarning(
                "[Coin_ReleaseSound] " +
                "Drop Play_Audio가 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (dropClip == null)
        {
            Debug.LogWarning(
                "[Coin_ReleaseSound] " +
                "Drop AudioClip이 연결되지 않았습니다.",
                this
            );

            return;
        }


        dropAudio.PlayAudio(
            dropClip
        );


        Debug.Log(
            "[Coin_ReleaseSound] " +
            "일반 Coin Drop Sound 재생",
            this
        );
    }
}