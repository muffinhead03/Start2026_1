using UnityEngine;
using UnityEngine.Events;

public class Event_On_Ray : MonoBehaviour, IRayInteractable
{
    [Header("Interact Prompt")]
    public string InteractPrompt;

    [Header("Ray Events")]
    public UnityEvent OnEnter;
    public UnityEvent OnStay;
    public UnityEvent OnExit;
    public UnityEvent OnClick;

    public string GetInteractPrompt()
    {
        return InteractPrompt;
    }

    public void OnRayEnter() => OnEnter?.Invoke();
    public void OnRayStay() => OnStay?.Invoke();
    public void OnRayExit() => OnExit?.Invoke();
    public void OnRayClick() => OnClick?.Invoke();
}
