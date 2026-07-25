using System.Collections.Generic;

[System.Serializable]
public class PuzzleStep
{
    public int id;
    public string goal;          // 내부 트래킹/로그용 목표 설명 — LLM 프롬프트에는 전달 안 함
    public string[] hintByLevel; // 레벨 1~4 각각에 대응하는 힌트 문구 (레벨 낮을수록 정보 적게)
}

[System.Serializable]
public class PuzzleConfig
{
    public string puzzleId;
    public int totalSteps;
    public List<string> requiredClues;
    public List<PuzzleStep> steps;
}