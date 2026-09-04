using System.Collections.Generic;

/// <summary>
/// PromptBuilder에 원래 하드코딩돼 있던 씬 컨텍스트를 그대로 담고 있는 기본 구현체.
/// ScriptableObject 에셋이 아직 준비/연결되지 않았을 때의 폴백(안전장치)으로 쓰인다.
///
/// 예전엔 레벨이 오를수록 방 정보가 누적되는 chunk 배열이었는데, 이게 nextStep 기준
/// hintByLevel과 별개 소스로 존재하다 보니 완료된 단계의 안내가 레벨 게이팅만으로
/// 뒷단계에서도 계속 노출되는 문제가 있었음(예: 4단계까지 끝났는데 레벨2에서 여전히
/// "바닥 얼룩 봐"가 나옴). 지금은 여기서 스텝별 세부 내용을 아예 다루지 않고,
/// 방 전체를 관통하는 스포일러 없는 오리엔테이션 한 줄만 항상 제공한다.
/// 스텝에 맞는 구체적 안내는 100% PuzzleStep.hintByLevel(=stepHint)이 담당 — 소스를 하나로
/// 모아서 두 소스가 어긋나는 일 자체를 없앰.
/// </summary>
public class DefaultSceneContextProvider : ISceneContextProvider
{
    static readonly Dictionary<string, string> SceneOrientation = new Dictionary<string, string>
    {
        { "wine_glass_room",
            "Scene: A dim Victorian dining room with a wine rack and stained floor, leading into a room with an old bookshelf. " +
            "The player is trying to escape by solving a puzzle hidden somewhere in this space." },

        { "organ_room",
            "Scene: A large old mansion themed around a broken organ. " +
            "The player is trying to repair the organ and perform a piece of music to escape." },
    };

    public string GetSceneContext(string puzzleId)
    {
        return SceneOrientation.TryGetValue(puzzleId, out var text)
            ? text
            : "Scene context not available.";
    }
}