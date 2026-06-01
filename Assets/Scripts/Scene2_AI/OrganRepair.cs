using UnityEngine;

public class OrganRepair : MonoBehaviour
{
    public HintManager hintManager;

    public void OnRepairComplete()
    {
        hintManager.currentPlayerState.completedSteps.Add(8);
        hintManager.currentPlayerState.completedSteps.Add(9);
        Debug.Log("[OrganRepair] 오르간 수리 완료!");
        // TODO: 수리 완료 연출 (조명 꺼짐 등) 여기서 트리거
    }
}