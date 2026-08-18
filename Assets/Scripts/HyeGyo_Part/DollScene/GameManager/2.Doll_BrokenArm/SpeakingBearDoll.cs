using UnityEngine;

public class SpeakingBearDoll : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    [Header("Bear State")]
    [SerializeField]
    private int hitCount = 0;

    [SerializeField]
    private bool isBroken = false;


    [Header("Bear Visual")]
    [SerializeField]
    private GameObject normalBear;

    [SerializeField]
    private GameObject brokenBear;

    [SerializeField]
    private GameObject brokenHead;


    [Header("Coin")]
    [SerializeField]
    private GameObject coinObject;


    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip hitVoice01;

    [SerializeField]
    private AudioClip hitVoice02;

    [SerializeField]
    private AudioClip hitVoice03;

    [SerializeField]
    private AudioClip hitVoice04;

    [Header("파손 연출")]
    [SerializeField]
    private float breakForce = 2.5f;

    [SerializeField]
    private float breakUpForce = 1.2f;


    public int HitCount => hitCount;
    public bool IsBroken => isBroken;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }


        // 정상 곰 표시
        if (normalBear != null)
        {
            normalBear.SetActive(true);
        }


        // 찢어진 곰 숨김
        if (brokenBear != null)
        {
            brokenBear.SetActive(false);
        }


        // 동전 숨김
        if (coinObject != null)
        {
            coinObject.SetActive(false);
        }
    }


    // =============================================
    // TeddyBear_Grab에서 호출
    // =============================================

    public void RegisterThrowHit()
    {
        if (isBroken)
            return;


        hitCount++;


        Debug.Log(
            $"[SpeakingBear] 충돌 횟수 : {hitCount}/5"
        );


        switch (hitCount)
        {
            case 1:
                PlayVoice(hitVoice01);
                Debug.Log("[SpeakingBear] 아야!");
                break;


            case 2:
                PlayVoice(hitVoice02);
                Debug.Log("[SpeakingBear] 아파..");
                break;


            case 3:
                PlayVoice(hitVoice03);
                Debug.Log("[SpeakingBear] 이제 그만해..");
                break;


            case 4:
                PlayVoice(hitVoice04);
                Debug.Log("[SpeakingBear] 제발 부탁이야");
                break;


            case 5:
                // 5번째에는 음성 없음
                BreakBear();
                break;
        }
    }


    private void PlayVoice(AudioClip clip)
    {
        if (audioSource == null)
            return;

        if (clip == null)
            return;


        audioSource.PlayOneShot(clip);
    }


    // =============================================
    // 곰 파손
    // =============================================

   private void BreakBear()
{
    if (isBroken)
        return;

    if (normalBear == null)
    {
        Debug.LogError(
            "[SpeakingBear] Normal Bear가 연결되지 않았습니다."
        );

        return;
    }

    if (brokenBear == null)
    {
        Debug.LogError(
            "[SpeakingBear] Broken Bear가 연결되지 않았습니다."
        );

        return;
    }


    isBroken = true;

    Debug.Log(
        "[SpeakingBear] 5번째 충돌! 곰 인형이 찢어졌습니다."
    );


    // =========================================
    // 정상 곰의 충돌 순간 위치 저장
    // =========================================

    Vector3 breakPosition =
        normalBear.transform.position;

    Quaternion breakRotation =
        normalBear.transform.rotation;


    // =========================================
    // 파손된 곰을 충돌 위치로 이동
    // =========================================

    brokenBear.transform.position =
        breakPosition;

    brokenBear.transform.rotation =
        breakRotation;

    brokenBear.SetActive(true);


    // =========================================
    // 머리 분리
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
            headRb.isKinematic = false;

            headRb.linearVelocity =
                Vector3.zero;

            headRb.angularVelocity =
                Vector3.zero;


            Vector3 headForce =
                transform.right * breakForce +
                Vector3.up * breakUpForce;


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
    // 동전 분리
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
            coinRb.isKinematic = false;

            coinRb.linearVelocity =
                Vector3.zero;

            coinRb.angularVelocity =
                Vector3.zero;


            Vector3 coinForce =
                -transform.right * breakForce +
                Vector3.up * breakUpForce;


            coinRb.AddForce(
                coinForce,
                ForceMode.Impulse
            );
        }
    }


    // =========================================
    // 파손 몸통
    // =========================================

    Rigidbody bodyRb =
        brokenBear.GetComponent<Rigidbody>();


    if (bodyRb != null)
    {
        bodyRb.isKinematic = false;

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
    // 마지막에 정상 곰 제거
    // =========================================

    normalBear.SetActive(false);
}


    // =============================================
    // 동전 획득
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
            "[SpeakingBear] 동전을 획득했습니다."
        );
    }
}