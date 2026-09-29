/// <summary>
/// 현재 안내할 스텝 안의 "세부 진행 상황"을 판단해서 알려주는 제공자가 구현해야 하는 인터페이스.
/// 예: 곰 인형을 반복해서 던져야 하는 스텝에서 "거의 다 왔으니 조금만 더" 같은 판단.
///
/// "판단은 로직, 표현은 AI" 원칙에 따라 원본 수치(던진 횟수 등)가 아니라 이미 판단된 문장을 돌려준다 —
/// 수치를 그대로 넘기면 소형 LLM이 "2번만 더!"처럼 정답을 그대로 말할 위험이 있어서.
/// IPuzzleDataProvider / ILocationAwareProvider와 같은 패턴 — 구현은 각 씬의 XxxHintBridge가 담당.
/// </summary>
public interface IStepProgressProvider
{
    /// <summary>
    /// step에 대한 진행 상황 문장. 이 스텝에 진행도 개념이 없거나 아직 아무것도 안 했으면 null.
    /// 반환 문장은 그 스텝의 hintByLevel 문구를 "대신해서" 힌트 방향으로 쓰인다
    /// (LLM 프롬프트의 [Hint direction]과 LLM이 없을 때의 고정 문구 모두) — 그래서 진행 상황 + 할 행동까지 담은 완결 문장이어야 함.
    /// </summary>
    string GetProgressNote(PuzzleStep step);
}