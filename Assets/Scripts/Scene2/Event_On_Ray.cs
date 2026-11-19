using UnityEngine;
using UnityEngine.Events;

public class Event_On_Ray : MonoBehaviour, IRayInteractable
{
    [Header("Ray Events")]
    public UnityEvent OnEnter;
    public UnityEvent OnStay;
    public UnityEvent OnExit;
    public UnityEvent OnClick;

    [SerializeField] private bool isInteractable = true;

    public bool IsInteractable() { return isInteractable; }
    public void SetInteractable(bool isActive)
    {
        isInteractable = isActive;
    }

    public void OnRayEnter() => OnEnter?.Invoke();
    public void OnRayStay() => OnStay?.Invoke();
    public void OnRayExit() => OnExit?.Invoke();
    public void OnRayClick() => OnClick?.Invoke();
}
