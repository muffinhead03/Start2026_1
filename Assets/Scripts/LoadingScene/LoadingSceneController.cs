using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

// Bootstrap 역할의 씬(빌드 설정상 가장 먼저 로드되는 씬) 루트에 붙이는 스크립트.
// 게임이 켜지면 StartingScene(메뉴)보다 먼저 이 씬이 로드되어 LLM을 워밍업하고,
// 완료되면 메뉴 화면(StartingScene)으로 넘어감.
// LLM 로드에 실패하거나 시간이 너무 오래 걸려도 게임은 진행됨 (힌트는 고정 문구로 대체).
//
// 추가: 로딩 바로 진행 상황을 보여줌.
//   - LLMUnity는 모델 "로드" 진행률(%)을 제공하지 않아서, 실제 단계(모델 준비 → 서버 시작 → 워밍업 완료)와
//     경과 시간(최대 warmupTimeoutSeconds)을 합쳐서 바를 채움. 단계에 도달하면 바가 그 지점까지 올라가고,
//     그 사이에는 시간에 따라 천천히 차오름. 워밍업이 끝나면 100%까지 빠르게 채운 뒤 다음 씬으로 이동.
// 추가: 기다리는 동안 남은 시간을 보여주고, skipAvailableAfterSeconds초가 지나면
// 아무 키/클릭(또는 건너뛰기 버튼)으로 바로 넘어갈 수 있음.
// 건너뛰어도 워밍업은 LLMSingleton(DontDestroyOnLoad)에서 계속 진행되고,
// 끝나면 그때부터 AI 힌트가 나옴 (그 전까지는 고정 문구 힌트).
public class LoadingSceneController : MonoBehaviour
{
    [Header("다음에 로드할 씬 이름 (메뉴 화면)")]
    [SerializeField] private string nextSceneName = "StartingScene";

    [Header("로딩 중 텍스트 (선택)")]
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("워밍업 최대 대기 시간(초) — 넘으면 LLM 없이 진행")]
    [SerializeField] private float warmupTimeoutSeconds = 90f;

    [Header("로딩 바 (둘 중 하나만 연결해도 됨)")]
    [Tooltip("UI Slider로 바를 만든 경우 연결 (Min 0, Max 1, Interactable 해제 권장)")]
    [SerializeField] private Slider progressSlider;
    [Tooltip("Image로 바를 만든 경우 연결 (Image Type = Filled, Fill Method = Horizontal)")]
    [SerializeField] private Image progressFillImage;

    [Tooltip("모델 파일 준비가 끝나면 바가 최소 이 지점까지 올라감 (0~1)")]
    [SerializeField, Range(0f, 1f)] private float modelSetupProgress = 0.15f;
    [Tooltip("LLM 서버가 시작되면(모델 로드 완료) 바가 최소 이 지점까지 올라감 (0~1). 남은 건 워밍업")]
    [SerializeField, Range(0f, 1f)] private float serverStartedProgress = 0.8f;
    [Tooltip("바가 목표 지점을 따라가는 속도 (초당 비율). 클수록 뚝뚝 끊겨 보임")]
    [SerializeField] private float barFollowSpeed = 0.5f;
    [Tooltip("워밍업이 끝났을 때 100%까지 채우는 데 쓰는 시간(초). 0이면 바로 넘어감")]
    [SerializeField] private float completeFillSeconds = 0.4f;

    [Header("남은 시간 / 건너뛰기 표시 (선택)")]
    [Tooltip("남은 시간과 건너뛰기 안내를 보여줄 텍스트. loadingText와 다른 오브젝트를 연결할 것 " +
             "(loadingText는 LoadingDotsAnimator가 계속 덮어써서 같이 쓰면 안 보임). 비워두면 표시 안 함")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Tooltip("{0} 자리에 남은 초가 들어감")]
    [SerializeField] private string remainingFormat = "AI 힌트 준비 중... (최대 {0}초)";

    [Tooltip("건너뛰기가 가능해진 뒤 남은 시간 아래에 붙는 안내 문구")]
    [SerializeField] private string skipGuide = "아무 키나 누르면 바로 시작합니다\nAI 힌트는 게임 중에 이어서 준비됩니다";

    [Tooltip("로딩 시작 후 이 초가 지나야 건너뛰기 가능 (바로 끝나는 PC에서 실수로 건너뛰는 것 방지). 0이면 처음부터 가능")]
    [SerializeField] private float skipAvailableAfterSeconds = 10f;

    [Tooltip("체크하면 아무 키 또는 마우스 클릭으로 건너뛰기")]
    [SerializeField] private bool allowAnyKeySkip = true;

    [Tooltip("건너뛰기 버튼 (선택). 건너뛰기가 가능해지면 나타남")]
    [SerializeField] private Button skipButton;

    private LLMClient llmClient;
    private bool sceneLoading = false;

    // 실제로 워밍업을 기다리는 중인지 (바로 넘어가는 경우엔 남은 시간/건너뛰기 표시 안 함)
    private bool waiting = false;
    private float waitStartTime;
    private bool skipShown = false;
    private float barValue = 0f;

    void Start()
    {
        Debug.Log("[LoadingSceneController] Start 호출됨");

        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
            skipButton.onClick.AddListener(Skip);
        }
        if (statusText != null)
            statusText.text = "";
        SetBar(0f);

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

        waiting = true;
        waitStartTime = Time.realtimeSinceStartup;
        StartCoroutine(WarmupTimeout());
    }

    void Update()
    {
        if (!waiting || sceneLoading) return;

        // 로딩 화면이라 timeScale 영향 안 받도록 realtime 기준
        float elapsed   = Time.realtimeSinceStartup - waitStartTime;
        int   remaining = Mathf.Max(0, Mathf.CeilToInt(warmupTimeoutSeconds - elapsed));
        bool  canSkip   = elapsed >= skipAvailableAfterSeconds;

        // 로딩 바: max(시간 진행, 도달한 단계) 쪽으로 부드럽게 따라감. 워밍업 완료 전에는 99%에서 멈춤
        float target = Mathf.Min(Mathf.Max(elapsed / warmupTimeoutSeconds, StageProgress()), 0.99f);
        barValue = Mathf.MoveTowards(barValue, target, barFollowSpeed * Time.unscaledDeltaTime);
        SetBar(barValue);

        if (canSkip && !skipShown)
        {
            skipShown = true;
            if (skipButton != null) skipButton.gameObject.SetActive(true);
            Debug.Log($"[LoadingSceneController] {skipAvailableAfterSeconds}초 경과 → 건너뛰기 가능");
        }

        if (statusText != null)
        {
            string line = string.Format(remainingFormat, remaining);
            statusText.text = canSkip && (allowAnyKeySkip || skipButton != null)
                ? line + "\n" + skipGuide
                : line;
        }

        if (canSkip && allowAnyKeySkip && AnyInputPressedThisFrame())
            Skip();
    }

    // 실제로 도달한 로딩 단계를 0~1로 환산 (LLMUnity가 제공하는 상태 플래그 기준)
    float StageProgress()
    {
        float p = 0f;
        if (LLMUnity.LLM.modelSetupComplete) p = modelSetupProgress;

        var character = llmClient != null ? llmClient.LlmCharacter : null;
        if (character != null && character.llm != null && character.llm.started)
            p = Mathf.Max(p, serverStartedProgress);

        return p;
    }

    void SetBar(float value)
    {
        if (progressSlider != null) progressSlider.value = value;
        if (progressFillImage != null) progressFillImage.fillAmount = value;
    }

    bool AnyInputPressedThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) return true;
        return false;
    }

    // 건너뛰기 버튼 OnClick에도 연결됨 (Start에서 AddListener)
    public void Skip()
    {
        if (sceneLoading) return;
        float elapsed = Time.realtimeSinceStartup - waitStartTime;
        // 워밍업은 백그라운드에서 계속됨. 나중에 성공하면 그때부터 AI 힌트 사용 (IsAvailable = true)
        Debug.LogWarning($"[LoadingSceneController] 플레이어가 {elapsed:F1}초에 건너뛰기 → LLM 없이 먼저 진행 (워밍업은 계속)");
        GoToNextScene();
    }

    void HandleWarmupComplete()
    {
        Debug.Log("[LoadingSceneController] HandleWarmupComplete 호출됨 → 바 채운 뒤 다음 씬으로 이동");
        if (sceneLoading || fillingToEnd) return;
        StartCoroutine(FillBarThenGo());
    }

    // 워밍업 완료 시 바를 100%까지 빠르게 채운 뒤 이동 (바가 중간에서 갑자기 사라지지 않게)
    bool fillingToEnd = false;
    IEnumerator FillBarThenGo()
    {
        fillingToEnd = true;
        waiting = false; // Update의 바/건너뛰기 처리 중지

        float start = barValue;
        float t = 0f;
        while (t < completeFillSeconds)
        {
            t += Time.unscaledDeltaTime;
            barValue = Mathf.Lerp(start, 1f, t / completeFillSeconds);
            SetBar(barValue);
            yield return null;
        }
        SetBar(1f);
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

        if (!sceneLoading && !fillingToEnd)
        {
            SetBar(1f);
            // 워밍업은 백그라운드에서 계속됨. 나중에 성공하면 그때부터 AI 힌트 사용 (IsAvailable = true)
            Debug.LogWarning($"[LoadingSceneController] 워밍업이 {warmupTimeoutSeconds}초 안에 안 끝남 → LLM 없이 먼저 진행");
            GoToNextScene();
        }
    }

    void GoToNextScene()
    {
        if (sceneLoading) return;
        sceneLoading = true;
        waiting = false;

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