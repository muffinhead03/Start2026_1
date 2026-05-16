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
        if (puzzleId == "clock_bookshelf_lock")
        {
            return new PuzzleConfig
            {
                puzzleId = puzzleId,
                totalSteps = 6,
                requiredClues = new List<string> { "book_numbers", "clock_time" },
                steps = new List<PuzzleStep>
                {
                    new PuzzleStep { id = 1, goal = "서재에 들어간다",             hintDirection = "이동 유도" },
                    new PuzzleStep { id = 2, goal = "책장을 조사한다",             hintDirection = "오브젝트 집중" },
                    new PuzzleStep { id = 3, goal = "책등의 숫자를 발견한다",      hintDirection = "단서 발견" },
                    new PuzzleStep { id = 4, goal = "시계를 조사한다",             hintDirection = "오브젝트 집중" },
                    new PuzzleStep { id = 5, goal = "시계와 책장 숫자 관계 이해",  hintDirection = "추론 유도" },
                    new PuzzleStep { id = 6, goal = "자물쇠에 정답 입력",          hintDirection = "행동 안내" },
                }
            };
        }
 
        // 퍼즐 추가할 때 여기에 else if 로 계속 추가
        return null;
    }
}
