using System.Collections.Generic;

public static class WineGlassRoomData
{
    public static PuzzleConfig GetConfig()
    {
        return new PuzzleConfig
        {
            puzzleId = "wine_glass_room",
            totalSteps = 4,
            requiredClues = new List<string> { "clue_A", "clue_B", "clue_C" },
            steps = new List<PuzzleStep>
            {
                new PuzzleStep { id = 1, goal = "와인잔을 조사한다",      hintDirection = "와인잔 색깔 확인" },
                new PuzzleStep { id = 2, goal = "와인랙 라벨을 조사한다", hintDirection = "색깔-알파벳 매칭" },
                new PuzzleStep { id = 3, goal = "교차점 숫자를 확인한다", hintDirection = "숫자 확인" },
                new PuzzleStep { id = 4, goal = "정답 가구를 조사한다",   hintDirection = "SOFA 찾기" },
            }
        };
    }
}