using System.Collections.Generic;

/// <summary>
/// HintEngine.Calculate()의 입력값. 현재 플레이어가 퍼즐을 어떤 상태로 플레이 중인지를 담는다.
/// 게임 쪽(HintManager 등)이 매 프레임/매 액션마다 이 값을 채워 넣고, HintEngine은 이걸 읽기만 한다.
/// </summary>
[System.Serializable]
public class PlayerState
{
    public float staySeconds;
    public int hintCount;
    public int failCount;
    public string hintType; // "direct" or "indirect"
    public List<int> completedSteps = new List<int>();
    public List<string> foundClues = new List<string>();
    public List<string> missedClues = new List<string>();
    public List<string> visitedRooms = new List<string>();
    public List<string> lastActions = new List<string>(); // 최근 상호작용 (
    public List<RepeatedInspection> repeatedInspections = new List<RepeatedInspection>();
}

/// <summary>
/// 특정 오브젝트를 플레이어가 몇 번 반복해서 조사했는지 기록. 반복 조사가 잦으면
/// "이해를 못 하고 있다"는 신호로 HintEngine의 CalcMisunderstandScore에 반영됨.
/// </summary>
[System.Serializable]
public class RepeatedInspection
{
    public string objectName;
    public int count;
}