/// <summary>
/// 힌트 사용 횟수를 게임을 껐다 켜도 유지되게 저장하는 대상.
/// scope는 "어느 구간의 횟수인지" 구분하는 문자열 (이 게임에선 씬 이름).
///
/// 저장 방식(PlayerPrefs / 세이브 파일 / 서버 등)은 구현체가 정함 — HintManager는 이 인터페이스만 앎.
/// </summary>
public interface IHintUsageStore
{
    /// <summary>
    /// 이 scope에서 이미 쓴 횟수를 돌려줌.
    /// 저장된 기록이 다른 scope 것이면(= 새 씬에 들어옴 / 새 게임) 이 scope 기준 0회로 새로 시작하고 0을 돌려줌.
    /// </summary>
    int Restore(string scope);

    /// <summary>이 scope에서 지금까지 쓴 횟수를 저장</summary>
    void Save(string scope, int used);
}