using UnityEngine;

/// <summary>
/// 힌트 사용 횟수를 PlayerPrefs에 저장하는 구현체 — 게임을 껐다 켜도(이어하기) 같은 씬이면 쓴 횟수가 유지됨.
///
/// 기록은 "마지막으로 들어간 씬 1개"만 둠 (scope + 횟수):
///  - 같은 씬으로 다시 들어오면(이어하기 / 씬 재시작) 저장된 횟수를 이어서 씀
///  - 다른 씬에 들어오면 그 씬 기준 0회로 덮어씀 → 다음 씬은 새로 10회,
///    새 게임은 튜토리얼 씬(인형씬)을 거치면서 자연스럽게 초기화됨
/// 팀 세이브 파일(SaveSystem/PlayerData)은 건드리지 않으려고 GameData처럼 PlayerPrefs를 씀.
/// </summary>
public class PlayerPrefsHintUsageStore : IHintUsageStore
{
    const string ScopeKey = "HintUsage_Scope";
    const string UsedKey  = "HintUsage_Used";

    public int Restore(string scope)
    {
        if (PlayerPrefs.GetString(ScopeKey, "") == scope)
            return Mathf.Max(0, PlayerPrefs.GetInt(UsedKey, 0));

        // 다른 씬의 기록 → 이 씬 기준으로 새로 시작 (바로 저장해야 이 씬에서 힌트를 한 번도 안 써도 옛 기록이 남지 않음)
        Save(scope, 0);
        return 0;
    }

    public void Save(string scope, int used)
    {
        PlayerPrefs.SetString(ScopeKey, scope ?? "");
        PlayerPrefs.SetInt(UsedKey, Mathf.Max(0, used));
        PlayerPrefs.Save();
    }
}