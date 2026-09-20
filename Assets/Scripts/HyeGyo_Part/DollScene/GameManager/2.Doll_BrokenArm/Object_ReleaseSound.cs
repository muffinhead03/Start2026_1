using System.Collections;
using UnityEngine;

public class Object_ReleaseSound : MonoBehaviour
{
    // =============================================
    // Player
    // =============================================

    [Header("Player")]

    [SerializeField]
    private Player_Grab playerGrab;


    // =============================================
    // Object
    // =============================================

    [Header("Object")]

    [SerializeField]
    private Rigidbody targetRigidbody;

    [SerializeField]
    private Collider targetCollider;


    // =============================================
    // Audio
    // =============================================

    [Header("Release Audio")]

    [Tooltip("내려놓기 효과음을 재생할 Play_Audio")]
    [SerializeField]
    private Play_Audio releaseAudio;


    [Tooltip("일반적으로 손에서 놓았을 때 재생할 효과음")]
    [SerializeField]
    private AudioClip releaseClip;


    // =============================================
    // State
    // =============================================

    private bool wasHeld = false;

    private Coroutine releaseCheckCoroutine;


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


        if (targetRigidbody == null)
        {
            targetRigidbody =
                GetComponent<Rigidbody>();
        }


        if (targetCollider == null)
        {
            targetCollider =
                GetComponent<Collider>();
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


        // -----------------------------------------
        // 이전에는 들고 있었는데
        // 지금은 Hand에서 빠졌음
        // -----------------------------------------

        if (wasHeld &&
            !isCurrentlyHeld)
        {
            if (releaseCheckCoroutine != null)
            {
                StopCoroutine(
                    releaseCheckCoroutine
                );
            }


            releaseCheckCoroutine =
                StartCoroutine(
                    CheckReleaseType()
                );
        }


        wasHeld =
            isCurrentlyHeld;
    }


    // =============================================
    // 현재 Hand에 들려 있는지
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
    // 일반 Release인지 PutOn인지 판정
    // =============================================

    private IEnumerator CheckReleaseType()
    {
        /*
         * Player_Grab.Release() 또는 PutOn()의
         * Rigidbody / Collider 처리가 끝날 때까지
         * 한 프레임 기다립니다.
         */

        yield return null;


        // =========================================
        // PutOn 상태 확인
        //
        // PutOn 이동 중에는
        // Rigidbody가 Kinematic 상태이므로
        // 내려놓기 소리를 재생하지 않음
        // =========================================

        if (targetRigidbody != null &&
            targetRigidbody.isKinematic)
        {
            Debug.Log(
                "[Object_ReleaseSound] " +
                "PutOn 상태 → Release Sound 생략",
                this
            );


            releaseCheckCoroutine = null;

            yield break;
        }


        // =========================================
        // PutOn 상태에서는 Collider도 꺼져 있음
        // =========================================

        if (targetCollider != null &&
            !targetCollider.enabled)
        {
            Debug.Log(
                "[Object_ReleaseSound] " +
                "Collider OFF → Release Sound 생략",
                this
            );


            releaseCheckCoroutine = null;

            yield break;
        }


        // =========================================
        // 일반적으로 손에서 내려놓은 경우
        // =========================================

        PlayReleaseSound();


        releaseCheckCoroutine = null;
    }


    // =============================================
    // Release Sound
    // =============================================

    private void PlayReleaseSound()
    {
        if (releaseAudio == null)
        {
            Debug.LogWarning(
                "[Object_ReleaseSound] " +
                "Play_Audio가 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (releaseClip == null)
        {
            Debug.LogWarning(
                "[Object_ReleaseSound] " +
                "Release AudioClip이 연결되지 않았습니다.",
                this
            );

            return;
        }


        releaseAudio.PlayAudio(
            releaseClip
        );


        Debug.Log(
            "[Object_ReleaseSound] " +
            "일반 Release Sound 재생",
            this
        );
    }
}