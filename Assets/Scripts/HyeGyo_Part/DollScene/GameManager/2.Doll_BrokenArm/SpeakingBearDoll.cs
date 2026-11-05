
using System.Collections.Generic;
using UnityEngine;

public class SpeakingBearDoll : MonoBehaviour
{
    // =============================================
    // Game Manager
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

    public int HitCount => hitCount;
    public bool IsBroken => isBroken;


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
    // Additional Objects
    // =============================================

    [Header("파손 시 추가 활성화 오브젝트")]

    [Tooltip("파손 시 추가로 활성화할 오브젝트")]
    [SerializeField]
    private GameObject[] objectsToActivateOnBreak;


    // =============================================
    // Bear Random Voice
    // =============================================

    [Header("Bear Random Voice")]

    [SerializeField]
    private Play_Audio bearVoiceAudio;

    [SerializeField]
    private AudioClip[] hitVoices;

    private AudioSource bearVoiceSource;

    private readonly List<AudioClip> remainingHitVoices =
        new List<AudioClip>();


    // =============================================
    // Bear Tear Sound
    // =============================================

    [Header("Bear Tear Sound")]

    [SerializeField]
    private Play_Audio bearTearAudio;

    [SerializeField]
    private AudioClip bearTearClip;


    // =============================================
    // Coin Sound
    // =============================================

    [Header("Coin Sound")]

    [SerializeField]
    private Play_Audio coinAudio;

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
    // Start
    // =============================================

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }

        if (bearVoiceAudio != null)
        {
            bearVoiceSource =
                bearVoiceAudio.GetComponent<AudioSource>();
        }

        InitializeRemainingVoices();

        hitCount = 0;
        isBroken = false;


        // =========================================
        // 초기 오브젝트 상태
        // =========================================

        if (brokenHead != null)
        {
            brokenHead.SetActive(false);
        }

        if (brokenBear != null)
        {
            brokenBear.SetActive(false);
        }

        if (coinObject != null)
        {
            coinObject.SetActive(false);
        }

        if (objectsToActivateOnBreak != null)
        {
            foreach (GameObject obj in objectsToActivateOnBreak)
            {
                if (obj == null)
                    continue;

                if (obj == normalBear)
                    continue;

                obj.SetActive(false);
            }
        }

        if (normalBear != null)
        {
            normalBear.SetActive(true);
        }

        Debug.Log(
            "[SpeakingBear] 초기화 완료"
        );
    }


    // =============================================
    // Voice List Initialize
    // =============================================

    private void InitializeRemainingVoices()
    {
        remainingHitVoices.Clear();

        if (hitVoices == null)
            return;

        foreach (AudioClip clip in hitVoices)
        {
            if (clip == null)
                continue;

            if (remainingHitVoices.Contains(clip))
                continue;

            remainingHitVoices.Add(clip);
        }

        Debug.Log(
            "[SpeakingBear] Voice 준비: " +
            remainingHitVoices.Count
        );
    }


    // =============================================
    // Throw Hit
    // TeddyBear_Grab에서 호출
    // =============================================

    public void RegisterThrowHit()
    {
        if (isBroken)
            return;

        hitCount++;

        Debug.Log(
            $"[SpeakingBear] 충돌 횟수: {hitCount}/5"
        );

        // 매 충돌마다 랜덤 음성
        PlayRandomBearVoice();

        // 5번째 충돌에서 파손
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
        if (bearVoiceAudio == null)
        {
            Debug.LogWarning(
                "[SpeakingBear] Bear Voice Audio 없음"
            );

            return;
        }

        if (remainingHitVoices.Count == 0)
        {
            Debug.Log(
                "[SpeakingBear] 남은 Voice가 없습니다."
            );

            return;
        }

        // 기존 음성 중지
        if (bearVoiceSource != null &&
            bearVoiceSource.isPlaying)
        {
            bearVoiceSource.Stop();
        }

        int index =
            Random.Range(
                0,
                remainingHitVoices.Count
            );

        AudioClip selectedClip =
            remainingHitVoices[index];

        if (selectedClip == null)
            return;

        remainingHitVoices.RemoveAt(index);

        bearVoiceAudio.PlayAudio(
            selectedClip
        );

        Debug.Log(
            "[SpeakingBear] Hit Voice 재생: " +
            selectedClip.name
        );
    }


    // =============================================
    // Bear Break
    // =============================================

    private void BreakBear()
    {
        if (isBroken)
            return;

        if (normalBear == null ||
            brokenBear == null)
        {
            Debug.LogError(
                "[SpeakingBear] Bear Object 연결 오류"
            );

            return;
        }

        isBroken = true;

        Debug.Log(
            "[SpeakingBear] 5번째 충돌 → 곰 파손"
        );


        // =========================================
        // NormalBear 마지막 위치 / 회전
        // =========================================

        Vector3 breakPosition =
            normalBear.transform.position;

        Quaternion breakRotation =
            normalBear.transform.rotation;


        Debug.Log(
            "[SpeakingBear] 파손 위치: " +
            breakPosition
        );


        // =========================================
        // 찢어진 몸통
        // =========================================

        PrepareBreakObject(
            brokenBear,
            breakPosition,
            breakRotation
        );

        brokenBear.SetActive(true);


        // =========================================
        // 찢어진 머리
        // =========================================

        if (brokenHead != null)
        {
            PrepareBreakObject(
                brokenHead,
                breakPosition,
                breakRotation
            );

            brokenHead.SetActive(true);
        }


        // =========================================
        // 동전
        // =========================================

        if (coinObject != null)
        {
            PrepareBreakObject(
                coinObject,
                breakPosition,
                breakRotation
            );

            coinObject.SetActive(true);
        }


        // =========================================
        // 추가 오브젝트 활성화
        // =========================================

        if (objectsToActivateOnBreak != null)
        {
            foreach (GameObject obj in objectsToActivateOnBreak)
            {
                if (obj == null)
                    continue;

                if (obj == normalBear ||
                    obj == brokenBear ||
                    obj == brokenHead ||
                    obj == coinObject)
                {
                    continue;
                }

                PrepareBreakObject(
                    obj,
                    breakPosition,
                    breakRotation
                );

                obj.SetActive(true);
            }
        }


        // =========================================
        // 머리 파손 물리
        // =========================================

        if (brokenHead != null)
        {
            Rigidbody headRb =
                brokenHead.GetComponent<Rigidbody>();

            ApplyBreakPhysics(
                headRb,
                breakRotation * Vector3.right
            );

            if (headRb != null)
            {
                headRb.AddTorque(
                    Random.insideUnitSphere * breakForce,
                    ForceMode.Impulse
                );
            }
        }


        // =========================================
        // 동전 물리
        // =========================================

        if (coinObject != null)
        {
            Rigidbody coinRb =
                coinObject.GetComponent<Rigidbody>();

            ApplyBreakPhysics(
                coinRb,
                -(breakRotation * Vector3.right)
            );
        }


        // =========================================
        // 찢어진 몸통 물리
        // =========================================

        Rigidbody bodyRb =
            brokenBear.GetComponent<Rigidbody>();

        if (bodyRb != null)
        {
            bodyRb.isKinematic = false;
            bodyRb.useGravity = true;

            bodyRb.linearVelocity = Vector3.zero;
            bodyRb.angularVelocity = Vector3.zero;

            bodyRb.AddForce(
                Vector3.up * 0.5f,
                ForceMode.Impulse
            );
        }


        // =========================================
        // 파손 효과음
        // =========================================

        PlayBreakSounds();


        // =========================================
        // 정상 곰 비활성화
        // =========================================

        normalBear.SetActive(false);

        Debug.Log(
            "[SpeakingBear] 곰 파손 완료"
        );
    }


    // =============================================
    // 파손 오브젝트 위치 설정
    // =============================================

    private void PrepareBreakObject(
        GameObject obj,
        Vector3 position,
        Quaternion rotation)
    {
        if (obj == null)
            return;

        // 기존 부모에서 분리
        obj.transform.SetParent(
            null,
            true
        );

        // NormalBear의 마지막 위치로 이동
        obj.transform.SetPositionAndRotation(
            position,
            rotation
        );

        Debug.Log(
            "[SpeakingBear] 파손 오브젝트 위치 설정: " +
            obj.name
        );
    }


    // =============================================
    // 파손 Rigidbody 물리
    // =============================================

    private void ApplyBreakPhysics(
        Rigidbody rb,
        Vector3 direction)
    {
        if (rb == null)
            return;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 force =
            direction * breakForce +
            Vector3.up * breakUpForce;

        rb.AddForce(
            force,
            ForceMode.Impulse
        );
    }


    // =============================================
    // Break Sounds
    // =============================================

    private void PlayBreakSounds()
    {
        // 곰 찢어지는 효과음
        if (bearTearAudio != null &&
            bearTearClip != null)
        {
            bearTearAudio.PlayAudio(
                bearTearClip
            );
        }
        else
        {
            Debug.LogWarning(
                "[SpeakingBear] 곰 파손 Sound 연결 없음"
            );
        }


        // 동전 등장 효과음
        if (coinAudio != null &&
            coinClip != null)
        {
            coinAudio.PlayAudio(
                coinClip
            );
        }
        else
        {
            Debug.LogWarning(
                "[SpeakingBear] Coin Sound 연결 없음"
            );
        }
    }


    // =============================================
    // Coin Collect
    // =============================================

    public void CollectCoin()
    {
        if (!isBroken)
            return;

        if (gameManager != null)
        {
            gameManager.CompleteFindCoin();
        }

        if (coinObject != null)
        {
            coinObject.SetActive(false);
        }

        Debug.Log(
            "[SpeakingBear] 동전 획득 완료"
        );
    }
}
