/// <summary>
/// 키 → 현재 언어의 문장 후보 목록을 돌려주는 번역 테이블.
/// Core(HintEngine 등)는 이 인터페이스만 알고, 문장이 어디에 어떻게 저장돼 있는지(이 게임은 HintLanguageData)는 모름.
///
/// 후보가 여러 개인 이유: 같은 상황에서 다시 물어도 매번 같은 말을 하지 않게 하려고 (랜덤으로 하나 고름).
/// 문장 안의 {0}, {1}은 string.Format 자리표시자 — 언어마다 어순이 달라서 문장 틀째로 번역함.
/// </summary>
public interface IHintPhraseTable
{
    /// <summary>
    /// key에 해당하는 현재 언어 문장 후보들. 현재 언어가 없으면 한국어, 키 자체가 없으면 null
    /// (null이면 호출하는 쪽이 자기 기본값을 쓰거나, 키 대신 원래 문자열을 그대로 씀).
    /// </summary>
    string[] GetCandidates(string key);
}