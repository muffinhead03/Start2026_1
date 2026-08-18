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


    [Header("Broken Bear")]
    [SerializeField]
    private GameObject brokenBearVisual;


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


    public int HitCount => hitCount;
    public bool IsBroken => isBroken;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }


        // 찢어진 곰은 처음에는 숨김
        if (brokenBearVisual != null)
        {
            brokenBearVisual.SetActive(false);
        }


        // 동전도 처음에는 숨김
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


        isBroken = true;


        Debug.Log(
            "[SpeakingBear] 곰 인형의 목이 찢어졌습니다."
        );


        // 찢어진 곰 표시
        if (brokenBearVisual != null)
        {
            brokenBearVisual.SetActive(true);
        }


        // 동전 표시
        if (coinObject != null)
        {
            coinObject.SetActive(true);
        }


        /*
         * 현재 멀쩡한 TeddyBear는
         * 바로 SetActive(false) 하면 안 됨.
         *
         * 이 스크립트 자체가 TeddyBear에 붙어있기 때문.
         *
         * 따라서 MeshRenderer만 끄거나,
         * 나중에 비주얼을 자식 오브젝트로 분리하는 것이 좋음.
         */

        MeshRenderer renderer =
            GetComponent<MeshRenderer>();


        if (renderer != null)
        {
            renderer.enabled = false;
        }


        Collider col =
            GetComponent<Collider>();


        if (col != null)
        {
            col.enabled = false;
        }


        Rigidbody rb =
            GetComponent<Rigidbody>();


        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
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