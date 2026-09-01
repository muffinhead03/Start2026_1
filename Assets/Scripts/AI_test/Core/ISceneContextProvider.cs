/// <summary>
/// 퍼즐 ID + 힌트 레벨로 프롬프트에 들어갈 씬 배경 텍스트를 조회하는 제공자가 구현해야 하는 인터페이스.
/// </summary>
public interface ISceneContextProvider
{
    string GetSceneContext(string puzzleId, int hintLevel);
}