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
 
public static class PuzzleConfigData
{
    public static PuzzleConfig GetConfig(string puzzleId)
    {
        if (puzzleId == "wine_glass_room")
        {
            return new PuzzleConfig
            {
                puzzleId = puzzleId,
                totalSteps = 4,
                requiredClues = new List<string> { "clue_A", "clue_B", "clue_C" },
                steps = new List<PuzzleStep>
                {
                    new PuzzleStep { id = 1, goal = "와인잔을 조사한다", hintDirection = "와인잔 색깔 확인" },
                    new PuzzleStep { id = 2, goal = "와인랙 라벨을 조사한다", hintDirection = "색깔-알파벳 매칭" },
                    new PuzzleStep { id = 3, goal = "교차점 숫자를 확인한다", hintDirection = "숫자 확인" },
                    new PuzzleStep { id = 4, goal = "정답 가구를 조사한다", hintDirection = "SOFA 찾기" },
                }
            };
        }
 
        // 퍼즐 추가할 때 여기에 else if 로 계속 추가
        return null;
    }
}
