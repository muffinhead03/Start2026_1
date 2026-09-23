using UnityEngine;

public class OrganSceneManager : MonoBehaviour
{
    [SerializeField] GameObject[] pipes;
    [SerializeField] GameObject[] papers;
    [SerializeField] GameObject[] clues;
    [SerializeField] GameObject[] puzzle_obj;

    private int step;

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
