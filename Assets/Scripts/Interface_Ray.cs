using UnityEngine;

public interface IRayInteractable
{
    void OnRayEnter();
    void OnRayStay();
    void OnRayExit();
    void OnRayClick();
}
