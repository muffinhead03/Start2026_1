using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LeftButtonCheck : MonoBehaviour
{
    [SerializeField]
    private Button leftButton;


    private void Update()
    {
        if (Mouse.current == null)
            return;


        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;


        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        Debug.Log(
            "========================================"
        );

        Debug.Log(
            $"[LeftCheck] Mouse Click = {mousePosition}",
            gameObject
        );


        // ==========================================
        // Button 확인
        // ==========================================

        if (leftButton == null)
        {
            Debug.LogError(
                "[LeftCheck] Button 연결 안 됨",
                gameObject
            );

            return;
        }


        RectTransform rect =
            leftButton.GetComponent<RectTransform>();

        Image image =
            leftButton.GetComponent<Image>();


        if (rect == null)
        {
            Debug.LogError(
                "[LeftCheck] RectTransform 없음",
                leftButton.gameObject
            );

            return;
        }


        // ==========================================
        // Canvas / Camera 확인
        // ==========================================

        Canvas canvas =
            leftButton.GetComponentInParent<Canvas>();

        Camera eventCamera = null;


        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = canvas.worldCamera;
        }


        // ==========================================
        // Rect 안에 마우스가 있는가
        // ==========================================

        bool inside =
            RectTransformUtility.RectangleContainsScreenPoint(
                rect,
                mousePosition,
                eventCamera
            );


        Debug.Log(
            $"[LeftCheck][BUTTON] " +
            $"Target={leftButton.name} / " +
            $"InsideRect={inside} / " +
            $"Active={leftButton.gameObject.activeInHierarchy} / " +
            $"Button.interactable={leftButton.interactable} / " +
            $"Button.IsInteractable()={leftButton.IsInteractable()} / " +
            $"Image.RaycastTarget={(image != null && image.raycastTarget)} / " +
            $"Canvas={(canvas != null ? canvas.name : "NULL")} / " +
            $"RenderMode={(canvas != null ? canvas.renderMode.ToString() : "NULL")} / " +
            $"WorldCamera={(canvas != null && canvas.worldCamera != null ? canvas.worldCamera.name : "NULL")}",
            leftButton.gameObject
        );


        // ==========================================
        // EventSystem 존재 확인
        // ==========================================

        if (EventSystem.current == null)
        {
            Debug.LogError(
                "[LeftCheck][CRITICAL] EventSystem.current = NULL"
            );

            return;
        }


        Debug.Log(
            $"[LeftCheck][EVENTSYSTEM] " +
            $"Name={EventSystem.current.gameObject.name} / " +
            $"InputModule=" +
            $"{(EventSystem.current.currentInputModule != null ? EventSystem.current.currentInputModule.GetType().Name : "NULL")}",
            EventSystem.current.gameObject
        );


        // ==========================================
        // 현재 마우스 위치 UI Raycast
        // ==========================================

        PointerEventData pointerData =
            new PointerEventData(
                EventSystem.current
            );


        pointerData.position =
            mousePosition;


        List<RaycastResult> results =
            new List<RaycastResult>();


        EventSystem.current.RaycastAll(
            pointerData,
            results
        );


        Debug.Log(
            $"[LeftCheck][RAYCAST] Count={results.Count}"
        );


        for (int i = 0; i < results.Count; i++)
        {
            RaycastResult result =
                results[i];


            Debug.Log(
                $"[LeftCheck][RAYCAST #{i}] " +
                $"Object={result.gameObject.name} / " +
                $"Module={result.module?.GetType().Name} / " +
                $"Depth={result.depth} / " +
                $"SortingLayer={result.sortingLayer} / " +
                $"SortingOrder={result.sortingOrder} / " +
                $"Distance={result.distance}",
                result.gameObject
            );
        }


        // ==========================================
        // Target Button이 실제 Raycast 목록에 있는지
        // ==========================================

        bool foundButton = false;


        foreach (RaycastResult result in results)
        {
            if (result.gameObject == leftButton.gameObject)
            {
                foundButton = true;
                break;
            }
        }


        Debug.Log(
            foundButton
                ? $"<color=lime>[LeftCheck] ★ {leftButton.name}가 Raycast 목록에 있음 ★</color>"
                : $"<color=red>[LeftCheck] ★ {leftButton.name}가 Raycast 목록에 없음 ★</color>",
            leftButton.gameObject
        );


        // ==========================================
        // 최상단 UI
        // ==========================================

        if (results.Count > 0)
        {
            Debug.Log(
                $"<color=yellow>[LeftCheck][TOP HIT] " +
                $"{results[0].gameObject.name}</color>",
                results[0].gameObject
            );
        }
        else
        {
            Debug.LogWarning(
                "[LeftCheck][TOP HIT] 아무 UI도 Raycast되지 않음"
            );
        }


        Debug.Log(
            "========================================"
        );
    }
}