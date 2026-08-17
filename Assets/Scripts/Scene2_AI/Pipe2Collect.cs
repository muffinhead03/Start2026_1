using UnityEngine;

public class Pipe2Collect : MonoBehaviour
{
    public HintManager hintManager;
    bool collected = false;

    public void Collect()
    {
        if (collected) return;
        if(hintManager == null) { Debug.LogWarning("[Pipe2Collect] HintManager가 연결되지 않아 수집을 건너뜁니다.",this); return;}
        if(hintManager.currentPlayerState==null) {Debug.LogWarning("[Pipe2Collect] currentPlayerState가 null이라 수집을 건너뜁니다.",this); return;}
        if (hintManager.currentPlayerState.foundClues == null){Debug.LogWarning("[Pipe2Collect] foundClues가 null이라 수집을 건너뜁니다.",this); return;}
        hintManager.currentPlayerState.foundClues.Add("clue_pipe_2");
        //gameObject.SetActive(false);
        Debug.Log("[Pipe2] 파이프 2 수집");
        // 파이프 1, 2 둘 다 수집했으면 step 2 완료
        if (hintManager.currentPlayerState.foundClues.Contains("clue_pipe_1"))
            hintManager.currentPlayerState.completedSteps.Add(2);
        collected = true;
    }
}