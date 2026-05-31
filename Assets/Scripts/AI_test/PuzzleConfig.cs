using System.Collections.Generic;

[System.Serializable]
public class PuzzleStep
{
    public int id;
    public string goal;
    public string hintDirection;
}

[System.Serializable]
public class PuzzleConfig
{
    public string puzzleId;
    public int totalSteps;
    public List<string> requiredClues;
    public List<PuzzleStep> steps;
}