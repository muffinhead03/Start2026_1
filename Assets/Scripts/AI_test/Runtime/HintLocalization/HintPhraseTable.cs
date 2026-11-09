/// <summary>
/// Core의 IHintPhraseTable을 이 게임의 HintLanguageData로 구현한 어댑터.
/// HintEngine(Core)은 이 클래스도, HintLanguageData도 모르고 인터페이스만 앎 — HintManager.Start()에서 주입.
/// </summary>
public class HintPhraseTable : IHintPhraseTable
{
    public string[] GetCandidates(string key) => HintLanguageData.GetCandidates(key);
}