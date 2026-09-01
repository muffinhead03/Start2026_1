using UnityEngine;

/// <summary>
/// 퍼즐 하나(씬)의 힌트 레벨별 누적 배경 정보를 담는 데이터 에셋.
/// levelChunks[0]은 레벨1용, [1]은 레벨2용 ... 순서대로 이어붙여서 사용됨.
/// </summary>
[CreateAssetMenu(fileName = "SceneContextData", menuName = "HintSystem/Scene Context Data")]
public class SceneContextData : ScriptableObject
{
    public string puzzleId;

    [TextArea(3, 10)]
    public string[] levelChunks;
}