/// <summary>
/// 힌트 패널 고정 UI 문구를 현재 언어로 돌려주는 대상.
/// HintManager는 문장을 직접 들고 있지 않고 이 인터페이스로만 받음 —
/// 이 게임은 팀 공용 LanguageData로 구현(LocalizedHintUIText), 다른 게임은 자기 번역 시스템으로 구현하면 됨.
/// </summary>
public interface IHintUITextProvider
{
    string Get(HintUITextId id);
}