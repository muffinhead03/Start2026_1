using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIRaycastDebug : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (EventSystem.current == null)
        {
            Debug.LogError("[UIRaycastDebug] EventSystem 없음");
            return;
        }

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        PointerEventData pointerData =
            new PointerEventData(EventSystem.current);

        pointerData.position =
            mousePosition;


        List<RaycastResult> results =
            new List<RaycastResult>();


EventSystem.current.RaycastAll(
    pointerData,
    results
);

Debug.Log(
    $"========== UI CLICK / {mousePosition} =========="
);

Debug.Log(
    $"[UIRaycastDebug] Raycast Count = {results.Count}"
);

for (int i = 0; i < results.Count; i++)
{
    Debug.Log(
        $"[UIRaycastDebug] #{i} " +
        $"Object = {results[i].gameObject.name}"
    );
}
    }
}