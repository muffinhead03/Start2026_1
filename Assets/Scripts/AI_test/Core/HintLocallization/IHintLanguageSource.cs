/// <summary>
/// "지금 힌트를 어떤 언어로 말해야 하는지"를 알려주는 대상.
/// HintManager는 이 인터페이스만 알고, 실제 언어 설정이 어디에 저장돼 있는지(게임의 LanguageCurrentStatus,
/// 다른 게임이라면 그 게임의 설정 등)는 구현체(어댑터)가 담당함.
/// 힌트를 요청할 때마다 다시 읽으므로, 게임 중에 언어를 바꿔도 다음 힌트부터 바로 반영됨.
/// </summary>
public interface IHintLanguageSource
{
    HintLanguageProfile Current { get; }
}