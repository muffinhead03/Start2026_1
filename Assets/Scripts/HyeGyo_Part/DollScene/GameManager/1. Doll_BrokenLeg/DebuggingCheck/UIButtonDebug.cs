using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonDebug :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    private Button button;
    private Image image;
    private Canvas canvas;


    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();


        Debug.Log(
            $"[UIButtonDebug][INIT] " +
            $"Object={gameObject.name} / " +
            $"Button={(button != null ? "OK" : "NULL")} / " +
            $"Image={(image != null ? "OK" : "NULL")} / " +
            $"Canvas={(canvas != null ? canvas.name : "NULL")} / " +
            $"RenderMode={(canvas != null ? canvas.renderMode.ToString() : "NULL")}",
            gameObject
        );
    }


    // =========================================================
    // Mouse Enter
    // =========================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(
            $"<color=cyan>[UIButtonDebug][ENTER]</color> " +
            GetDebugInfo(eventData),
            gameObject
        );
    }


    // =========================================================
    // Mouse Exit
    // =========================================================

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(
            $"<color=grey>[UIButtonDebug][EXIT]</color> " +
            GetDebugInfo(eventData),
            gameObject
        );
    }


    // =========================================================
    // Mouse Down
    // =========================================================

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(
            $"<color=yellow>[UIButtonDebug][DOWN]</color> " +
            GetDebugInfo(eventData),
            gameObject
        );


        PrintRaycastObjects(eventData);
    }


    // =========================================================
    // Mouse Up
    // =========================================================

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(
            $"<color=orange>[UIButtonDebug][UP]</color> " +
            GetDebugInfo(eventData),
            gameObject
        );
    }


    // =========================================================
    // Click
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(
            $"<color=lime>[UIButtonDebug][★ CLICK SUCCESS ★]</color> " +
            GetDebugInfo(eventData),
            gameObject
        );
    }


    // =========================================================
    // 기본 정보
    // =========================================================

    private string GetDebugInfo(PointerEventData eventData)
    {
        bool active =
            gameObject.activeInHierarchy;

        bool interactable =
            button != null &&
            button.interactable;

        bool raycastTarget =
            image != null &&
            image.raycastTarget;


        string pointerEnterName =
            eventData.pointerEnter != null
                ? eventData.pointerEnter.name
                : "NULL";


        string pointerPressName =
            eventData.pointerPress != null
                ? eventData.pointerPress.name
                : "NULL";


        string rawPointerPressName =
            eventData.rawPointerPress != null
                ? eventData.rawPointerPress.name
                : "NULL";


        string currentRaycastName =
            eventData.pointerCurrentRaycast.gameObject != null
                ? eventData.pointerCurrentRaycast.gameObject.name
                : "NULL";


        return
            $"Object={gameObject.name} / " +
            $"Mouse={eventData.position} / " +
            $"Active={active} / " +
            $"Interactable={interactable} / " +
            $"RaycastTarget={raycastTarget} / " +
            $"PointerEnter={pointerEnterName} / " +
            $"PointerPress={pointerPressName} / " +
            $"RawPress={rawPointerPressName} / " +
            $"CurrentRaycast={currentRaycastName}";
    }


    // =========================================================
    // 현재 마우스 위치에 걸리는 UI 전부 출력
    // =========================================================

    private void PrintRaycastObjects(
        PointerEventData eventData
    )
    {
        if (EventSystem.current == null)
        {
            Debug.LogError(
                "[UIButtonDebug] EventSystem.current가 NULL입니다.",
                gameObject
            );

            return;
        }


        PointerEventData pointerData =
            new PointerEventData(
                EventSystem.current
            );

        pointerData.position =
            eventData.position;


        List<RaycastResult> results =
            new List<RaycastResult>();


        EventSystem.current.RaycastAll(
            pointerData,
            results
        );


        Debug.Log(
            $"[UIButtonDebug][RAYCAST] " +
            $"Mouse={eventData.position} / " +
            $"Count={results.Count}",
            gameObject
        );


        for (int i = 0; i < results.Count; i++)
        {
            RaycastResult result =
                results[i];


            Debug.Log(
                $"[UIButtonDebug][RAYCAST #{i}] " +
                $"Object={result.gameObject.name} / " +
                $"Module={result.module} / " +
                $"SortingLayer={result.sortingLayer} / " +
                $"SortingOrder={result.sortingOrder} / " +
                $"Depth={result.depth} / " +
                $"Distance={result.distance}",
                result.gameObject
            );
        }
    }
}