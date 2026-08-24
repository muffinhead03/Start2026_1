using System.Collections.Generic;
using NUnit.Framework;

public class HintEngineTests
{
    static PuzzleConfig MakeConfig(int totalSteps = 4)
    {
        var steps = new List<PuzzleStep>();
        for (int i = 1; i <= totalSteps; i++)
            steps.Add(new PuzzleStep { id = i, goal = $"step {i}", hintByLevel = new[] { "l1", "l2", "l3", "l4" } });

        return new PuzzleConfig
        {
            puzzleId = "test_room",
            totalSteps = totalSteps,
            steps = steps
        };
    }

    static PlayerState MakeState()
    {
        return new PlayerState
        {
            hintType = "indirect",
            completedSteps = new List<int>(),
            foundClues = new List<string>(),
            missedClues = new List<string>(),
            visitedRooms = new List<string>(),
            lastActions = new List<string>(),
            repeatedInspections = new List<RepeatedInspection>()
        };
    }

    [Test]
    public void FreshState_ReturnsLevel1()
    {
        var state = MakeState();
        var config = MakeConfig();

        var result = HintEngine.Calculate(state, config);

        Assert.AreEqual(1, result.hintLevel);
    }

    [Test]
    public void HighFailCount_Alone_DoesNotReachMaxLevel()
    {
        // failCount 관련 회귀 테스트: failCount가 올라가도 다른 요소가 0이면
        // 레벨이 최대치까지 올라가지 않는다는 걸 확인 (요소 하나로는 만점 불가능한 게 정상 동작)
        var state = MakeState();
        state.failCount = 10;
        var config = MakeConfig(totalSteps: 1);

        var result = HintEngine.Calculate(state, config);

        Assert.AreEqual(3, result.hintLevel);
    }

    [Test]
    public void FailCount_Increases_HintLevel_Monotonically()
    {
        // failCount 미증가 버그(known-issues 참고)의 회귀 테스트:
        // failCount가 올라갈수록 레벨이 내려가는 일은 없어야 한다.
        var config = MakeConfig(totalSteps: 1);

        var lowFail = MakeState();
        lowFail.failCount = 0;
        var lowResult = HintEngine.Calculate(lowFail, config);

        var highFail = MakeState();
        highFail.failCount = 10;
        var highResult = HintEngine.Calculate(highFail, config);

        Assert.GreaterOrEqual(highResult.hintLevel, lowResult.hintLevel);
    }

    [Test]
    public void AllFactorsMaxed_DirectType_ReachesLevel4()
    {
        // 레벨 4가 실제로 도달 가능한 조합이 존재하는지 확인.
        // (와인방 등 실제 플레이에서 레벨 4가 안 뜬다는 의심이 있었으나,
        //  이 테스트가 통과하면 HintEngine 계산식 자체는 레벨4 도달 가능하다는 뜻)
        var state = MakeState();
        state.hintType = "direct";
        state.staySeconds = 360;
        state.hintCount = 3;
        state.failCount = 10;
        state.repeatedInspections = new List<RepeatedInspection>
        {
            new RepeatedInspection { objectName = "a", count = 3 },
            new RepeatedInspection { objectName = "b", count = 3 },
            new RepeatedInspection { objectName = "c", count = 3 },
            new RepeatedInspection { objectName = "d", count = 3 },
        };
        var config = MakeConfig(totalSteps: 4); // completedSteps 0개 → progress 만점

        var result = HintEngine.Calculate(state, config);

        Assert.AreEqual(4, result.hintLevel);
    }

    [Test]
    public void AllStepsCompleted_NextStepIsNull()
    {
        var state = MakeState();
        var config = MakeConfig(totalSteps: 2);
        state.completedSteps.Add(1);
        state.completedSteps.Add(2);

        var result = HintEngine.Calculate(state, config);

        Assert.IsNull(result.nextStep);
    }

    [Test]
    public void PuzzleId_IsPassedThroughFromConfig()
    {
        var state = MakeState();
        var config = MakeConfig();

        var result = HintEngine.Calculate(state, config);

        Assert.AreEqual("test_room", result.puzzleId);
    }
}