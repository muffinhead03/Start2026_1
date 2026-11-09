/// <summary>
/// HintEngine이 관찰 문장("치지직... 책장을 보고 있었지, 지금은 첫 번째 방에 있구나...")을 만들 때 쓰는 문장 틀의 키.
/// 키 이름은 Core가 정하고(필요한 쪽이 정함), 실제 언어별 문장은 IHintPhraseTable 구현체가 채움.
/// 테이블이 없거나 키가 빠져 있으면 HintEngine 안의 한국어 기본값을 씀 — Core만 따로 가져가도 동작함.
///
/// 자리표시자:
///  - ZoneHere / Landmark / ZoneOther: {0} = 구역·랜드마크 이름
///  - JoinActionPlace / JoinActionProximity: {0} = 행동 문장, {1} = 위치 문장
///  - JoinPlace / JoinSingle: {0} = 문장 하나
/// 행동 문장은 JoinAction* 틀 안에 들어가므로 행동 라벨 안에는 쉼표(、)를 넣지 않음.
/// </summary>
public static class ObservationPhraseKeys
{
    public const string GenericAction        = "HintObs_GenericAction";        // 매칭되는 행동 라벨이 없을 때
    public const string ProximityVeryClose   = "HintObs_ProximityVeryClose";
    public const string ProximityNearby      = "HintObs_ProximityNearby";
    public const string ProximityFar         = "HintObs_ProximityFar";
    public const string ZoneHere             = "HintObs_ZoneHere";             // "{0}에 있구나"
    public const string Landmark             = "HintObs_Landmark";             // "{0} 앞에 있네"
    public const string ZoneOther            = "HintObs_ZoneOther";            // "{0} 쪽도 한번 둘러봐."
    public const string JoinActionPlace      = "HintObs_JoinActionPlace";      // "{0}, 지금은 {1}..."
    public const string JoinPlace            = "HintObs_JoinPlace";            // "{0}..."
    public const string JoinActionProximity  = "HintObs_JoinActionProximity";  // "{0}, 지금은 {1}."
    public const string JoinSingle           = "HintObs_JoinSingle";           // "{0}."
    public const string SentenceGap          = "HintObs_SentenceGap";          // 문장 사이 간격 (한국어·영어 " ", 일본어 "")
}