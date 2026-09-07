/// <summary>
/// 퍼즐 ID로 프롬프트에 들어갈 "스포일러 없는" 방 소개 텍스트를 조회하는 제공자가 구현해야 하는 인터페이스.
/// 스텝별 구체적인 힌트 내용은 여기서 다루지 않는다 — 그건 PuzzleStep.hintByLevel의 역할.
/// 이 함수는 어떤 스텝에 있든 항상 같은, 방 전체에 대한 한두 문장짜리 오리엔테이션만 준다.
/// </summary>
public interface ISceneContextProvider
{
    string GetSceneContext(string puzzleId);
}