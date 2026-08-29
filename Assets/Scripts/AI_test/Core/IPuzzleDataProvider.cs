/// <summary>
/// 퍼즐 설정(PuzzleConfig)을 퍼즐 ID로 조회하는 제공자가 구현해야 하는 인터페이스.
/// HintManager는 이 인터페이스만 알고, 실제 데이터가 어디서 오는지(하드코딩된 registry,
/// JSON, 서버 등)는 몰라도 된다.
/// </summary>
public interface IPuzzleDataProvider
{
    PuzzleConfig GetConfig(string puzzleId);
}