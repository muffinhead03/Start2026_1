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
    /// hintLevel에 맞춰 구체성이 다른 문장을 골라야 함 — 레벨이 올라도 같은 문장이면 힌트가 제자리걸음이 됨.
    /// 같은 레벨 안에서도 후보 여러 개 중 랜덤으로 골라주면, 같은 상태에서 다시 물었을 때 표현이 달라짐.
    /// 반환 문장은 그 스텝의 hintByLevel 문구를 "대신해서" 힌트 방향으로 쓰인다
    /// (LLM 프롬프트의 [Hint direction]과 LLM이 없을 때의 고정 문구 모두) — 그래서 진행 상황 + 할 행동까지 담은 완결 문장이어야 함.
    /// </summary>
    string GetProgressNote(PuzzleStep step, int hintLevel);

    /// <summary>
    /// 진행 구간 이름 (예: "early" / "almost"). 같은 스텝에서 힌트를 다시 요청했을 때
    /// "지난 힌트 이후 진행이 있었는지" 판단용 — 구간이 바뀌었으면 잘 가고 있는 거라 레벨을 올리지 않는다.
    /// 진행도 개념이 없으면 null.
    /// </summary>
    string GetProgressBand(PuzzleStep step);

    /// <summary>
    /// 지금 진행 구간에서 힌트 레벨이 최소 몇이어야 하는지. 보통 1.
    /// 이미 답이 눈앞에 보이는 상태(예: 곰 인형이 찢어져 동전이 나옴)처럼 모호한 힌트가 쓸모없을 때만 올려서 돌려줌
    /// (레벨 1~2 지침은 "숨겨진 물건 이름을 말하지 마"라서, 레벨이 낮으면 LLM이 그 물건을 말하지 않음).
    /// </summary>
    int GetMinHintLevel(PuzzleStep step);
}