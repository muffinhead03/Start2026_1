using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Bootstrap 역할의 씬(빌드 설정상 가장 먼저 로드되는 씬) 루트에 붙이는 스크립트.
// 게임이 켜지면 StartingScene(메뉴)보다 먼저 이 씬이 로드되어 LLM을 워밍업하고,
// 완료되면 메뉴 화면(StartingScene)으로 넘어감.
// LLM 로드에 실패하거나 시간이 너무 오래 걸려도 게임은 진행됨 (힌트는 고정 문구로 대체).
public class LoadingSceneController : MonoBehaviour
{
    [Header("다음에 로드할 씬 이름 (메뉴 화면)")]
    [SerializeField] private string nextSceneName = "StartingScene";

    [Header("로딩 중 텍스트 (선택)")]
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("워밍업 최대 대기 시간(초) — 넘으면 LLM 없이 진행")]
    [SerializeField] private float warmupTimeoutSeconds = 90f;

    private LLMClient llmClient;
    private bool sceneLoading = false;

    void Start()
    {
        Debug.Log("[LoadingSceneController] Start 호출됨");

        if (LLMSingleton.Instance == null)
        {
            Debug.LogError("[LoadingSceneController] LLMSingleton.Instance가 null입니다. LLM 프리팹이 이 씬에 배치되어 있는지, LLMSingleton.cs가 붙어있는지 확인하세요. → LLM 없이 진행");
            GoToNextScene();
            return;
        }

        Debug.Log("[LoadingSceneController] LLMSingleton.Instance 확인됨");

        llmClient = LLMSingleton.Instance.LlmClient;

        if (llmClient == null)
        {
            Debug.LogError("[LoadingSceneController] LLMSingleton.LlmClient가 null입니다. LLMSingleton의 Llm Client 필드가 Inspector에서 연결됐는지 확인하세요. → LLM 없이 진행");
            GoToNextScene();
            return;
        }

        Debug.Log($"[LoadingSceneController] llmClient 확인됨. IsWarmedUp = {llmClient.IsWarmedUp}, IsFailed = {llmClient.IsFailed}");

        // 구독 전에 이미 끝났을 수 있으니 먼저 확인
        if (llmClient.IsWarmedUp || llmClient.IsFailed)
        {
            Debug.Log("[LoadingSceneController] 워밍업이 이미 끝난 상태(성공/실패) → 바로 다음 씬으로 이동");
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

        Debug.Log("[LoadingSceneController] OnWarmupComplete / OnWarmupFailed 이벤트 구독 시작");
        llmClient.OnWarmupComplete += HandleWarmupComplete;
        llmClient.OnWarmupFailed   += HandleWarmupFailed;

        StartCoroutine(WarmupTimeout());
    }

    void HandleWarmupComplete()
    {
        Debug.Log("[LoadingSceneController] HandleWarmupComplete 호출됨 → 다음 씬으로 이동");
        GoToNextScene();
    }

    void HandleWarmupFailed()
    {
        Debug.LogWarning("[LoadingSceneController] LLM 로드 실패 → 힌트는 고정 문구로 대체, 다음 씬으로 이동");
        GoToNextScene();
    }

    IEnumerator WarmupTimeout()
    {
        // 로딩 화면이라 timeScale 영향 안 받도록 Realtime 사용
        yield return new WaitForSecondsRealtime(warmupTimeoutSeconds);

        if (!sceneLoading)
        {
            // 워밍업은 백그라운드에서 계속됨. 나중에 성공하면 그때부터 AI 힌트 사용 (IsAvailable = true)
            Debug.LogWarning($"[LoadingSceneController] 워밍업이 {warmupTimeoutSeconds}초 안에 안 끝남 → LLM 없이 먼저 진행");
            GoToNextScene();
        }
    }

    void GoToNextScene()
    {
        if (sceneLoading) return;
        sceneLoading = true;

        Unsubscribe();
        Debug.Log($"[LoadingSceneController] SceneManager.LoadScene(\"{nextSceneName}\") 호출");
        SceneManager.LoadScene(nextSceneName);
    }

    void Unsubscribe()
    {
        if (llmClient == null) return;
        llmClient.OnWarmupComplete -= HandleWarmupComplete;
        llmClient.OnWarmupFailed   -= HandleWarmupFailed;
    }

    void OnDestroy()
    {
        Unsubscribe();
    }
}