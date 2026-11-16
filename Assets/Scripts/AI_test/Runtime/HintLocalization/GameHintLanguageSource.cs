/// <summary>
/// 팀 공용 언어 설정(LanguageCurrentStatus)을 힌트 시스템 언어 프로필로 바꿔주는 어댑터.
/// 힌트 시스템이 게임 언어 설정에 닿는 곳은 이 파일 하나뿐 — Core(PromptBuilder 등)는 LanguageType을 모름.
///
/// 언어를 추가할 때: HintLanguageProfile에 프로필 추가 → 아래 switch에 한 줄 추가.
/// </summary>
public class GameHintLanguageSource : IHintLanguageSource
{
    public HintLanguageProfile Current => LanguageCurrentStatus.CurrentLanguage switch
    {
        LanguageType.English  => HintLanguageProfile.English,
        LanguageType.Japanese => HintLanguageProfile.Japanese,
        _                     => HintLanguageProfile.Korean,
    };
}