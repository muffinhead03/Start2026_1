using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Core 패키지(HintEngine + PromptBuilder)만으로 동작하는 최소 예제.
/// 와인방/오르간방 같은 게임 콘텐츠 없이, 아주 작은 가짜 퍼즐 하나로
/// "판단은 로직이 어떻게 내리는지"와 "그 판단이 프롬프트로 어떻게 표현되는지"를 보여준다.
/// 실제 LLM 호출은 하지 않고, LLM에 보낼 프롬프트 텍스트를 화면에 그대로 출력한다.
/// </summary>
public class MinimalExampleDemo : MonoBehaviour
{
    [SerializeField] Button failButton;   // "실패했다" 시뮬레이션 (누를 때마다 failCount++)
    [SerializeField] Button hintButton;   // 힌트 요청 (판단 다시 계산)
    [SerializeField] Text resultText;     // 판단 결과 + 생성된 프롬프트 출력

    PlayerState state;
    PuzzleConfig config;

    // 이 예제 전용 최소 SceneContextProvider — 실제 게임에서는 ScriptableObjectSceneContextProvider 등을 씀
    class MinimalSceneContextProvider : ISceneContextProvider
    {
        public string GetSceneContext(string puzzleId, int hintLevel) =>
            "A small room with a locked box on a table.";
    }

    void Start()
    {
        state = new PlayerState
        {
            hintType = "indirect",
            completedSteps = new List<int>(),
            repeatedInspections = new List<RepeatedInspection>()
        };

        config = new PuzzleConfig
        {
            puzzleId = "minimal_room",
            totalSteps = 1,
            steps = new List<PuzzleStep>
            {
                new PuzzleStep
                {
                    id = 1,
                    goal = "find the key",
                    hintByLevel = new[]
                    {
                        "Look around the room carefully.",
                        "Something is hidden near the table.",
                        "There is a key taped under the table.",
                        "Reach under the table and grab the key taped there."
                    }
                }
            }
        };

        PromptBuilder.SceneContextProvider = new MinimalSceneContextProvider();

        failButton.onClick.AddListener(() => { state.failCount++; UpdateResult(); });
        hintButton.onClick.AddListener(UpdateResult);

        UpdateResult();
    }

    void UpdateResult()
    {
        HintResult result = HintEngine.Calculate(state, config);
        string prompt = PromptBuilder.Build(result);

        resultText.text =
            $"failCount: {state.failCount}\n" +
            $"판단 결과 → 레벨: {result.hintLevel} / 상태: {result.playerStatus}\n\n" +
            $"[LLM에 전달될 프롬프트]\n{prompt}";
    }
}