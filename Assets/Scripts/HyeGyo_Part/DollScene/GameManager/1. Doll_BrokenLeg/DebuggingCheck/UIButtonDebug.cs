using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonDebug :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(
            $"[UIButtonDebug] ENTER : {gameObject.name}"
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(
            $"[UIButtonDebug] EXIT : {gameObject.name}"
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(
            $"[UIButtonDebug] DOWN : {gameObject.name}"
        );
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(
            $"[UIButtonDebug] UP : {gameObject.name}"
        );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(
            $"[UIButtonDebug] CLICK : {gameObject.name}"
        );
    }
}