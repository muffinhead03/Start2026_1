using UnityEngine;

/// <summary>
/// 힌트 사용 횟수를 메모리(static)에만 들고 있는 구현체.
///  - 게임을 켜 있는 동안: 같은 씬에 다시 들어와도(씬 재시작 / 타이틀 갔다가 이어하기) 쓴 횟수 유지
///  - 다른 씬에 들어가면: 그 씬 기준 0회로 새로 시작
///  - 게임을 껐다 켜면: 무조건 초기화 (디스크에 아무것도 저장하지 않음)
/// 예전 PlayerPrefsHintUsageStore가 남긴 키는 앱 시작 시 한 번 지워서 흔적을 없앰.
/// </summary>
public class SessionHintUsageStore : IHintUsageStore
{
    static string savedScope = "";
    static int savedUsed;

    // 앱 실행 시 1회 — 에디터에서 도메인 리로드를 꺼도 Play 때마다 0부터 시작하게 static을 직접 리셋
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnLaunch()
    {
        savedScope = "";
        savedUsed = 0;

        // 이전 버전(PlayerPrefs 저장)으로 플레이한 PC에 남은 기록 정리
        if (PlayerPrefs.HasKey("HintUsage_Scope") || PlayerPrefs.HasKey("HintUsage_Used"))
        {
            PlayerPrefs.DeleteKey("HintUsage_Scope");
            PlayerPrefs.DeleteKey("HintUsage_Used");
            PlayerPrefs.Save();
        }
    }

    public int Restore(string scope)
    {
        if (savedScope == scope)
            return Mathf.Max(0, savedUsed);

        Save(scope, 0);
        return 0;
    }

    public void Save(string scope, int used)
    {
        savedScope = scope ?? "";
        savedUsed = Mathf.Max(0, used);
    }
}