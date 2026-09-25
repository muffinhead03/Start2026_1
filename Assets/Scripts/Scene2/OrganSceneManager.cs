using UnityEngine;

public class OrganSceneManager : MonoBehaviour
{
    [SerializeField] GameObject[] pipes;
    [SerializeField] GameObject[] papers;
    [SerializeField] GameObject[] clues;
    [SerializeField] GameObject[] puzzle_obj;

    private int step;

    // 힌트 시스템(OrganHintBridge) 연동용 — 순서와 상관없이 "방금 완료된 스텝 번호"를 그대로 알려줌.
    // 아래 CompleteStep의 기존 로직(가장 큰 번호만 기억)은 건드리지 않음.
    public event System.Action<int> OnStepCompleted;

    // 힌트 시스템(OrganHintBridge) 위치 계산용 읽기 전용 접근자 — 배열 내용/순서는 Inspector 설정 그대로.
    public GameObject[] Pipes => pipes;
    public GameObject[] Papers => papers;
    public GameObject[] Clues => clues;
    public GameObject[] PuzzleObjects => puzzle_obj;

    private void Start()
    {
        InitStage();
    }

    void InitStage()
    {
        step = 0;
    }

    public int GetCurrentStep()
    {
        return step;
    }

    public void CompleteStep(int step)
    {
        OnStepCompleted?.Invoke(step);

        if (step < this.step) return;

        this.step = step + 1;
    }

    public void ClearStage()
    {

    }

    public void CompleteNextStep()
    {
        CompleteStep(step);
    }
}