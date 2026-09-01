using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Bootstrap 역할의 씬(빌드 설정상 가장 먼저 로드되는 씬) 루트에 붙이는 스크립트.
// 게임이 켜지면 StartingScene(메뉴)보다 먼저 이 씬이 로드되어 LLM을 워밍업하고,
// 완료되면 메뉴 화면(StartingScene)으로 넘어감.
public class LoadingSceneController : MonoBehaviour
{
    [Header("다음에 로드할 씬 이름 (메뉴 화면)")]
    [SerializeField] private string nextSceneName = "StartingScene";

    [Header("로딩 중 텍스트 (선택)")]
    [SerializeField] private TextMeshProUGUI loadingText;

    void Start()
    {
        Debug.Log("[LoadingSceneController] Start 호출됨");

        if (LLMSingleton.Instance == null)
        {
            Debug.LogError("[LoadingSceneController] LLMSingleton.Instance가 null입니다. LLM 프리팹이 이 씬에 배치되어 있는지, LLMSingleton.cs가 붙어있는지 확인하세요.");
            return;
        }

        Debug.Log("[LoadingSceneController] LLMSingleton.Instance 확인됨");

        var llmClient = LLMSingleton.Instance.LlmClient;

        if (llmClient == null)
        {
            Debug.LogError("[LoadingSceneController] LLMSingleton.LlmClient가 null입니다. LLMSingleton의 Llm Client 필드가 Inspector에서 연결됐는지 확인하세요.");
            return;
        }

        Debug.Log($"[LoadingSceneController] llmClient 확인됨. 현재 IsWarmedUp = {llmClient.IsWarmedUp}");

        if (llmClient.IsWarmedUp)
        {
            Debug.Log("[LoadingSceneController] 이미 워밍업 완료된 상태 → 바로 다음 씬으로 이동");
            GoToNextScene();
            return;
        }

        if (loadingText != null)
        {
            loadingText.text = "치지직... 준비 중...";
            Debug.Log("[LoadingSceneController] loadingText 갱신됨");
        }
        else
        {
            Debug.LogWarning("[LoadingSceneController] loadingText가 연결 안 되어 있음 (선택사항이라 진행에는 문제 없음)");
        }

        Debug.Log("[LoadingSceneController] OnWarmupComplete 이벤트 구독 시작");
        llmClient.OnWarmupComplete += HandleWarmupComplete;
    }

    void HandleWarmupComplete()
    {
        Debug.Log("[LoadingSceneController] HandleWarmupComplete 호출됨 → 다음 씬으로 이동");
        var llmClient = LLMSingleton.Instance.LlmClient;
        llmClient.OnWarmupComplete -= HandleWarmupComplete;
        GoToNextScene();
    }

    void GoToNextScene()
    {
        Debug.Log($"[LoadingSceneController] SceneManager.LoadScene(\"{nextSceneName}\") 호출");
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (LLMSingleton.Instance != null && LLMSingleton.Instance.LlmClient != null)
            LLMSingleton.Instance.LlmClient.OnWarmupComplete -= HandleWarmupComplete;
    }
}