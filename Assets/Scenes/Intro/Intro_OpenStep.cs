using UnityEngine;
using UnityEngine.Events;

public class Intro_StepForOpen : MonoBehaviour
{
    private bool isLightOn = false;
    private bool isClean = false;
    private int count = 0;

    [SerializeField] private UnityEvent AllStepsComplete;

    public void SetLight(bool active)
    {
        if(active) isLightOn = true;
        else isLightOn = false;

        CheckAllComplete();
    }

    public void RemoveTrash()
    {
        count++;

        if (count == 2) isClean = true;

        CheckAllComplete();
    }

    private void CheckAllComplete()
    {
        if (isLightOn && isClean) AllStepsComplete?.Invoke();
    }
}
