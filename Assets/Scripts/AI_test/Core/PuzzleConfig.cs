using System.Collections.Generic;

/// <summary>
/// 퍼즐 내 개별 스텝(예: "파이프 찾기", "악보 조각 모으기") 하나에 대한 정의.
/// </summary>
[System.Serializable]
public class PuzzleStep
{
    public int id;
    public string goal;          // 내부 트래킹/로그용 목표 설명 — LLM 프롬프트에는 전달 안 함
    public string[] hintByLevel; // 레벨 1~4 각각에 대응하는 힌트 문구 (레벨 낮을수록 정보 적게)
}

/// <summary>
/// 퍼즐 하나(보통 씬 하나에 대응)의 전체 설정. IPuzzleDataProvider가 puzzleId로 조회해서 반환하는 데이터.
/// </summary>
[System.Serializable]
public class PuzzleConfig
{
    public string puzzleId;
    public int totalSteps;
    public List<string> requiredClues;
    public List<PuzzleStep> steps;
}