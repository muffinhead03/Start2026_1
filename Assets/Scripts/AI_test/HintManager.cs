using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class HintManager : MonoBehaviour
{
    [Header("연결 필요")]
    [SerializeField] OllamaClient ollamaClient;
    [SerializeField] TextMeshProUGUI hintText;

    [Header("현재 퍼즐 ID (씬마다 변경)")]
    public string currentPuzzleId = "clock_bookshelf_lock";

    // 게임 중 이 값들을 계속 업데이트해줘야 함
    // 나중에 각 게임 시스템에서 이 값들을 직접 수정하거나
    // SetPlayerState() 같은 메서드를 만들어서 외부에서 주입하면 됨
    [HideInInspector]
    public PlayerState currentPlayerState;

    void Start()
    {
        // 테스트용 더미 데이터 (실제 게임에선 게임 시스템이 이 값을 채워줌)
        currentPlayerState = new PlayerState
        {
            staySeconds  = 200f,
            hintCount    = 1,
            failCount    = 3,
            hintType     = "indirect",
            completedSteps = new List<int> { 1, 2 },
            foundClues   = new List<string> { "book_numbers" }, // clock_time 아직 없음
            repeatedInspections = new List<RepeatedInspection>
            {
                new RepeatedInspection { objectName = "bookshelf", count = 4 }
            }
        };
    }

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            OnHintButtonClicked();
        }
    }

    // 힌트 버튼 OnClick에 연결
    public void OnHintButtonClicked()
    {
        if (currentPlayerState.hintCount >= 2)
        {
            hintText.text = "더 이상 도움을 받을 수 없어.";
            return;
        }

        var config = PuzzleConfigData.GetConfig(currentPuzzleId);
        if (config == null)
        {
            Debug.LogError("[HintManager] 퍼즐 설정을 찾을 수 없음: " + currentPuzzleId);
            return;
        }

        var result = HintEngine.Calculate(currentPlayerState, config);

        if (result.nextStep == null)
        {
            hintText.text = "이미 모든 단서를 찾았어.";
            return;
        }

        string systemPrompt = PromptBuilder.SystemPrompt;
        string userPrompt   = PromptBuilder.Build(result);

        hintText.text = "치지직.. 교신 중...";

        StartCoroutine(ollamaClient.RequestHint(systemPrompt, userPrompt, (hint) =>
        {
            hintText.text = hint;
            Debug.Log("[힌트 결과] 레벨: " + result.hintLevel + " / 상태: " + result.playerStatus);
        }));

        currentPlayerState.hintCount++;
    }
}
