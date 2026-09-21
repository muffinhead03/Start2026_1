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
        "AudioCollection의 Doll_BearVoiceAudio 연결"
    )]
    [SerializeField]
    private Play_Audio bearVoiceAudio;


    [Tooltip(
        "충돌할 때 랜덤으로 재생할 곰 음성.\n" +
        "4개, 5개 등 원하는 만큼 등록 가능.\n" +
        "한 번 재생된 음성은 다시 나오지 않음."
    )]
    [SerializeField]
    private AudioClip[] hitVoices;


    // 현재 곰 목소리가 재생 중인지 확인하는 용도
    private AudioSource bearVoiceSource;


    // 아직 재생되지 않은 곰 목소리 목록
    private readonly List<AudioClip> remainingHitVoices =
        new List<AudioClip>();


    // =============================================
    // Bear Tear Sound
    // =============================================

    [Header("Bear Tear Sound")]

    [Tooltip(
        "5번째 충돌에서 곰이 찢어질 때 사용할 Play_Audio.\n" +
        "AudioCollection의 Doll_BearTearAudio 연결"
    )]
    [SerializeField]
    private Play_Audio bearTearAudio;


    [Tooltip(
        "곰 인형이 찢어질 때 재생할 효과음"
    )]
    [SerializeField]
    private AudioClip bearTearClip;


    // =============================================
    // Coin Sound
    // =============================================

    [Header("Coin Sound")]

    [Tooltip(
        "5번째 충돌에서 동전이 튀어나올 때 사용할 Play_Audio.\n" +
        "AudioCollection의 Doll_BearCoinAudio 연결"
    )]
    [SerializeField]
    private Play_Audio coinAudio;


    [Tooltip(
        "동전이 튀어나올 때 재생할 효과음"
    )]
    [SerializeField]
    private AudioClip coinClip;


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
        // isPlaying 확인 용도
        // -----------------------------------------

        if (bearVoiceAudio != null)
        {
            bearVoiceSource =
                bearVoiceAudio.GetComponent<AudioSource>();
        }


        // -----------------------------------------
        // 사용 가능한 Bear Voice 목록 생성
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
    // Random Voice 목록 초기화
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


            // 같은 Clip을 Inspector에
            // 실수로 두 번 등록했어도 중복 제거
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
        // 이미 찢어진 곰이면 더 이상 처리 안 함
        if (isBroken)
        {
            return;
        }


        hitCount++;


        Debug.Log(
            $"[SpeakingBear] 충돌 횟수 : {hitCount}/5"
        );


        // =========================================
        // 아직 안 나온 Bear Voice 중 랜덤 1개
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
        // 다른 Bear Voice는 겹치지 않게 함
        //
        // 이 경우 목록에서도 제거하지 않음
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
        // 남아있는 Voice 확인
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
        // 아직 안 나온 Voice 중 Random
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
        // 팀 공용 Play_Audio를 통해 재생
        // -----------------------------------------

        bearVoiceAudio.PlayAudio(
            selectedClip
        );


        // -----------------------------------------
        // 실제로 재생한 Voice는 목록에서 제거
        //
        // 이후에는 다시 선택되지 않음
        // -----------------------------------------

        remainingHitVoices.RemoveAt(
            randomIndex
        );


        Debug.Log(
            "[SpeakingBear] " +
            $"Bear Voice : {selectedClip.name} / " +
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
            "5번째 충돌! 곰 인형 파손"
        );


        // =========================================
        // 정상 곰 위치 / 회전 저장
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
        // ★ 곰 찢어지는 소리
        // ★ Coin 등장 소리
        //
        // 각각 다른 Play_Audio를 사용하므로
        // 동시에 재생 가능
        // =========================================

        PlayBreakSounds();


        // =========================================
        // 마지막에 정상 곰 제거
        // =========================================

        normalBear.SetActive(false);
    }


    // =============================================
    // Break Sounds
    // =============================================

    private void PlayBreakSounds()
    {
        // -----------------------------------------
        // 곰 찢어지는 효과음
        // -----------------------------------------

        if (bearTearAudio != null &&
            bearTearClip != null)
        {
            bearTearAudio.PlayAudio(
                bearTearClip
            );


            Debug.Log(
                "[SpeakingBear] " +
                "곰 찢어지는 Sound 재생"
            );
        }
        else
        {
            Debug.LogWarning(
                "[SpeakingBear] " +
                "Bear Tear Audio 또는 Clip이 없습니다."
            );
        }


        // -----------------------------------------
        // 동전 등장 효과음
        // -----------------------------------------

        if (coinAudio != null &&
            coinClip != null)
        {
            coinAudio.PlayAudio(
                coinClip
            );


            Debug.Log(
                "[SpeakingBear] " +
                "Coin 등장 Sound 재생"
            );
        }
        else
        {
            Debug.LogWarning(
                "[SpeakingBear] " +
                "Coin Audio 또는 Clip이 없습니다."
            );
        }
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