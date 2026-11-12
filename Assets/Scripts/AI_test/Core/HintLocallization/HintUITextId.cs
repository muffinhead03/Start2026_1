/// <summary>
/// 힌트 패널에 나오는 고정 UI 문구의 종류. 실제 문장(언어별)은 IHintUITextProvider 구현체가 돌려줌.
/// </summary>
public enum HintUITextId
{
    /// <summary>무전기 잡음 (예: "치지직...") — 인사말·관찰 문장·폴백 문구 앞에 붙음</summary>
    RadioNoise,

    /// <summary>패널을 처음 열었을 때 인사말 (잡음 뒤에 붙음, 예: "도움이 필요해?")</summary>
    Greeting,

    /// <summary>LLM 응답을 기다리는 동안 점(...)과 함께 나오는 문구 (예: "치지직.. 교신 중")</summary>
    Connecting,

    /// <summary>힌트 횟수를 다 썼을 때</summary>
    NoHintsLeft,

    /// <summary>남은 스텝이 없을 때</summary>
    AllCluesFound,

    /// <summary>LLM도 고정 문구도 없을 때</summary>
    NoResponse,

    /// <summary>베타 테스트용 폴백 표시 태그 (예: "[폴백]")</summary>
    FallbackTag,

    // ── 아래는 패널에 붙어 있는 고정 라벨 (HintUILocalizedLabel 컴포넌트가 사용) ──

    /// <summary>힌트 버튼 글자 (예: "퍼즐 힌트")</summary>
    HintButton,

    /// <summary>패널 닫기 안내 (예: "ESC: 닫기")</summary>
    EscClose,

    /// <summary>폴백 배지 제목 (예: "Fall Back")</summary>
    FallbackBadgeTitle,

    /// <summary>폴백 배지 부제 (예: "(에러 아닙니다)")</summary>
    FallbackBadgeSub,
}