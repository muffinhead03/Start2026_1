using UnityEngine;

// LoadingScene의 프리팹 루트에 붙이는 스크립트.
// LLM / LLMCharacter / LLMClient 를 들고 있는 오브젝트를 DontDestroyOnLoad로 유지해서
// 게임 세션 동안 모델을 딱 1번만 로드하도록 함.
public class LLMSingleton : MonoBehaviour
{
    public static LLMSingleton Instance { get; private set; }

    [Header("이 오브젝트(또는 자식)에 있는 LLMClient")]
    [SerializeField] private LLMClient llmClient;
    public LLMClient LlmClient => llmClient;

    void Awake()
    {
        Debug.Log($"[LLMSingleton] Awake 호출됨. 오브젝트: {gameObject.name}, llmClient 필드 연결됨? {(llmClient != null)}");

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[LLMSingleton] 이미 인스턴스가 존재함 → 이 오브젝트는 파괴됨 (중복 방지)");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[LLMSingleton] 싱글톤으로 등록 완료, DontDestroyOnLoad 적용됨");
    }
}