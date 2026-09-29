/// <summary>
/// HintManager.AddLastAction()으로 들어온 action id를 "관찰 문장"용 자연어 라벨 후보로 바꿔주는 제공자가 구현해야 하는 인터페이스.
/// HintEngine(Core)은 이 인터페이스만 알고, 실제 문구(와인 라벨, 오르골 등 게임 내용)는 모른다.
/// ISceneContextProvider와 같은 패턴 — 구현체는 Runtime 쪽에 두고 HintManager.Start()에서 주입한다.
/// </summary>
public interface IActionLabelProvider
{
    /// <summary>
    /// actionId에 맞는 라벨 후보 배열(예: {"와인 라벨을 확인하고 있었지", "병 라벨을 살펴보고 있었네"}).
    /// HintEngine이 이 중 하나를 랜덤으로 고른다. 매칭되는 게 없으면 null — HintEngine이 일반 문구로 폴백함.
    /// </summary>
    string[] GetLabels(string actionId);
}