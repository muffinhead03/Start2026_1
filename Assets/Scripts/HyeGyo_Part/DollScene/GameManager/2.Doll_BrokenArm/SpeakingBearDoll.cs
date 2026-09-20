using System.Collections.Generic;
using UnityEngine;

public class SpeakingBearDoll : MonoBehaviour
{
    // =============================================
    // Doll Scene Game Manager
    // =============================================

    [Header("Doll Scene Game Manager")]

    [SerializeField]
    private DollScene_GameManager gameManager;


    // =============================================
    // Bear State
    // =============================================

    [Header("Bear State")]

    [SerializeField]
    private int hitCount = 0;

    [SerializeField]
    private bool isBroken = false;


    // =============================================
    // Bear Visual
    // =============================================

    [Header("Bear Visual")]

    [SerializeField]
    private GameObject normalBear;

    [SerializeField]
    private GameObject brokenBear;

    [SerializeField]
    private GameObject brokenHead;


    // =============================================
    // Coin
    // =============================================

    [Header("Coin")]

    [SerializeField]
    private GameObject coinObject;


    // =============================================
    // Bear Random Voice
    // =============================================

    [Header("Bear Random Voice")]

    [Tooltip(
        "곰 랜덤 음성을 재생할 Play_Audio.\n" +
        "AudioCollection의 Doll_BearVoiceAudio를 연결"
    )]
    [SerializeField]
    private Play_Audio bearVoiceAudio;


    [Tooltip(
        "충돌할 때 랜덤으로 재생할 곰 음성.\n" +
        "4개, 5개 등 원하는 만큼 등록 가능.\n" +
        "한 번 재생된 음성은 다시 재생되지 않음."
    )]
    [SerializeField]
    private AudioClip[] hitVoices;


    // 현재 곰 목소리가 재생 중인지 확인하는 용도
    private AudioSource bearVoiceSource;


    // 아직 한 번도 재생되지 않은 Voice 목록
    private readonly List<AudioClip> remainingHitVoices =
        new List<AudioClip>();


    // =============================================
    // Break / Coin Sound
    // =============================================

    [Header("Break / Coin Sound")]

    [Tooltip(
        "5번째 충돌에서 곰이 찢어지고 동전이 나올 때 " +
        "효과음을 재생할 Play_Audio.\n" +
        "AudioCollection의 Doll_BearBreakCoinAudio를 연결"
    )]
    [SerializeField]
    private Play_Audio breakCoinAudio;


    [Tooltip(
        "곰 파손 + 동전 등장 시 반드시 재생할 효과음"
    )]
    [SerializeField]
    private AudioClip breakCoinClip;


    // =============================================
    // Break Effect
    // =============================================

    [Header("파손 연출")]

    [SerializeField]
    private float breakForce = 2.5f;

    [SerializeField]
    private float breakUpForce = 1.2f;


    // =============================================
    // Public State
    // =============================================

    public int HitCount => hitCount;

    public bool IsBroken => isBroken;


    // =============================================
    // Start
    // =============================================

    private void Start()
    {
        // -----------------------------------------
        // Game Manager 자동 탐색
        // -----------------------------------------

        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }


        // -----------------------------------------
        // Bear Voice AudioSource
        //
        // 실제 재생용이 아니라
        // 현재 재생 중인지 검사하는 용도
        // -----------------------------------------

        if (bearVoiceAudio != null)
        {
            bearVoiceSource =
                bearVoiceAudio.GetComponent<AudioSource>();
        }


        // -----------------------------------------
        // 아직 사용하지 않은 Voice 목록 생성
        // -----------------------------------------

        InitializeRemainingVoices();


        // -----------------------------------------
        // 정상 곰 표시
        // -----------------------------------------

        if (normalBear != null)
        {
            normalBear.SetActive(true);
        }


        // -----------------------------------------
        // 찢어진 곰 숨김
        // -----------------------------------------

        if (brokenBear != null)
        {
            brokenBear.SetActive(false);
        }


        // -----------------------------------------
        // 동전 숨김
        // -----------------------------------------

        if (coinObject != null)
        {
            coinObject.SetActive(false);
        }
    }


    // =============================================
    // Voice List 초기화
    // =============================================

    private void InitializeRemainingVoices()
    {
        remainingHitVoices.Clear();


        if (hitVoices == null)
        {
            return;
        }


        for (int i = 0; i < hitVoices.Length; i++)
        {
            AudioClip clip =
                hitVoices[i];


            if (clip == null)
            {
                continue;
            }


            // Inspector에 같은 AudioClip을
            // 실수로 두 번 넣었어도 중복 등록하지 않음
            if (remainingHitVoices.Contains(clip))
            {
                continue;
            }


            remainingHitVoices.Add(
                clip
            );
        }


        Debug.Log(
            "[SpeakingBear] " +
            $"사용 가능한 Bear Voice : " +
            $"{remainingHitVoices.Count}개"
        );
    }


    // =============================================
    // TeddyBear_Grab에서 호출
    // =============================================

    public void RegisterThrowHit()
    {
        // 이미 찢어진 곰이면 처리 안 함
        if (isBroken)
        {
            return;
        }


        hitCount++;


        Debug.Log(
            $"[SpeakingBear] 충돌 횟수 : {hitCount}/5"
        );


        // =========================================
        // 충돌할 때마다
        // 아직 사용하지 않은 Voice 중 랜덤 1개
        // =========================================

        PlayRandomBearVoice();


        // =========================================
        // 5번째 충돌 → 곰 파손
        // =========================================

        if (hitCount >= 5)
        {
            BreakBear();
        }
    }


    // =============================================
    // Random Bear Voice
    // =============================================

    private void PlayRandomBearVoice()
    {
        // -----------------------------------------
        // 기존 Bear Voice가 아직 재생 중이면
        // 새로운 Voice는 겹쳐서 재생하지 않음
        //
        // 이 경우 Voice 목록에서도 제거하지 않음
        // -----------------------------------------

        if (bearVoiceSource != null &&
            bearVoiceSource.isPlaying)
        {
            Debug.Log(
                "[SpeakingBear] " +
                "Bear Voice 재생 중 → 새로운 Voice 생략"
            );

            return;
        }


        // -----------------------------------------
        // Play_Audio 확인
        // -----------------------------------------

        if (bearVoiceAudio == null)
        {
            Debug.LogWarning(
                "[SpeakingBear] " +
                "Bear Voice Play_Audio가 연결되지 않았습니다."
            );

            return;
        }


        // -----------------------------------------
        // 남은 Voice 확인
        // -----------------------------------------

        if (remainingHitVoices.Count == 0)
        {
            Debug.Log(
                "[SpeakingBear] " +
                "모든 Bear Voice를 이미 재생했습니다."
            );

            return;
        }


        // -----------------------------------------
        // 아직 안 나온 Voice 중 랜덤 선택
        // -----------------------------------------

        int randomIndex =
            Random.Range(
                0,
                remainingHitVoices.Count
            );


        AudioClip selectedClip =
            remainingHitVoices[randomIndex];


        if (selectedClip == null)
        {
            return;
        }


        // -----------------------------------------
        // 먼저 실제 재생
        // -----------------------------------------

        bearVoiceAudio.PlayAudio(
            selectedClip
        );


        // -----------------------------------------
        // 재생한 Voice는 목록에서 제거
        //
        // 이후 절대로 다시 랜덤 선택되지 않음
        // -----------------------------------------

        remainingHitVoices.RemoveAt(
            randomIndex
        );


        Debug.Log(
            "[SpeakingBear] " +
            $"Bear Voice 재생 : {selectedClip.name} / " +
            $"남은 Voice : {remainingHitVoices.Count}"
        );
    }


    // =============================================
    // Bear Break
    // =============================================

    private void BreakBear()
    {
        if (isBroken)
        {
            return;
        }


        // -----------------------------------------
        // Reference 확인
        // -----------------------------------------

        if (normalBear == null)
        {
            Debug.LogError(
                "[SpeakingBear] " +
                "Normal Bear가 연결되지 않았습니다."
            );

            return;
        }


        if (brokenBear == null)
        {
            Debug.LogError(
                "[SpeakingBear] " +
                "Broken Bear가 연결되지 않았습니다."
            );

            return;
        }


        isBroken = true;


        Debug.Log(
            "[SpeakingBear] " +
            "5번째 충돌! 곰 인형이 찢어졌습니다."
        );


        // =========================================
        // 정상 곰의 충돌 순간 위치 / 회전 저장
        // =========================================

        Vector3 breakPosition =
            normalBear.transform.position;


        Quaternion breakRotation =
            normalBear.transform.rotation;


        // =========================================
        // 찢어진 곰 활성화
        // =========================================

        brokenBear.transform.position =
            breakPosition;


        brokenBear.transform.rotation =
            breakRotation;


        brokenBear.SetActive(true);


        // =========================================
        // 머리 파편 활성화 / 분리
        // =========================================

        if (brokenHead != null)
        {
            brokenHead.SetActive(true);


            brokenHead.transform.SetParent(
                null,
                true
            );


            Rigidbody headRb =
                brokenHead.GetComponent<Rigidbody>();


            if (headRb != null)
            {
                headRb.isKinematic =
                    false;


                headRb.useGravity =
                    true;


                headRb.linearVelocity =
                    Vector3.zero;


                headRb.angularVelocity =
                    Vector3.zero;


                Vector3 headForce =
                    transform.right *
                    breakForce +
                    Vector3.up *
                    breakUpForce;


                headRb.AddForce(
                    headForce,
                    ForceMode.Impulse
                );


                headRb.AddTorque(
                    Random.insideUnitSphere *
                    breakForce,
                    ForceMode.Impulse
                );
            }
        }


        // =========================================
        // Coin 활성화 / 분리
        // =========================================

        if (coinObject != null)
        {
            coinObject.SetActive(true);


            coinObject.transform.SetParent(
                null,
                true
            );


            Rigidbody coinRb =
                coinObject.GetComponent<Rigidbody>();


            if (coinRb != null)
            {
                coinRb.isKinematic =
                    false;


                coinRb.useGravity =
                    true;


                coinRb.linearVelocity =
                    Vector3.zero;


                coinRb.angularVelocity =
                    Vector3.zero;


                Vector3 coinForce =
                    -transform.right *
                    breakForce +
                    Vector3.up *
                    breakUpForce;


                coinRb.AddForce(
                    coinForce,
                    ForceMode.Impulse
                );
            }
        }


        // =========================================
        // 찢어진 몸통 Rigidbody 활성화
        // =========================================

        Rigidbody bodyRb =
            brokenBear.GetComponent<Rigidbody>();


        if (bodyRb != null)
        {
            bodyRb.isKinematic =
                false;


            bodyRb.useGravity =
                true;


            bodyRb.linearVelocity =
                Vector3.zero;


            bodyRb.angularVelocity =
                Vector3.zero;


            bodyRb.AddForce(
                Vector3.up * 0.5f,
                ForceMode.Impulse
            );
        }


        // =========================================
        // 파손 + Coin 등장 Sound
        //
        // Bear Voice와 별도의 Play_Audio이므로
        // 곰 Voice와 관계없이 반드시 재생 가능
        // =========================================

        PlayBreakCoinSound();


        // =========================================
        // 마지막에 정상 곰 제거
        // =========================================

        normalBear.SetActive(false);
    }


    // =============================================
    // Break / Coin Sound
    // =============================================

    private void PlayBreakCoinSound()
    {
        if (breakCoinAudio == null)
        {
            Debug.LogWarning(
                "[SpeakingBear] " +
                "Break/Coin Play_Audio가 연결되지 않았습니다."
            );

            return;
        }


        if (breakCoinClip == null)
        {
            Debug.LogWarning(
                "[SpeakingBear] " +
                "Break/Coin AudioClip이 연결되지 않았습니다."
            );

            return;
        }


        // 팀 공용 Play_Audio 사용
        breakCoinAudio.PlayAudio(
            breakCoinClip
        );


        Debug.Log(
            "[SpeakingBear] " +
            "곰 파손 + Coin 등장 Sound 재생"
        );
    }


    // =============================================
    // Coin 획득
    // =============================================

    public void CollectCoin()
    {
        if (!isBroken)
        {
            return;
        }


        if (gameManager != null)
        {
            gameManager.CompleteFindCoin();
        }


        if (coinObject != null)
        {
            coinObject.SetActive(false);
        }


        Debug.Log(
            "[SpeakingBear] 동전을 획득했습니다."
        );
    }
}