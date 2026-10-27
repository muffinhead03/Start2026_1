using UnityEngine;

public interface IRayInteractable
{
    void OnRayEnter();
    void OnRayStay();
    void OnRayExit();
    void OnRayClick();
}

public interface IMouseInteractable
{
    void OnMouseEnter();

    void OnMouseExit();

    void OnMouseClick();

    void OnMouseReleased();
}
