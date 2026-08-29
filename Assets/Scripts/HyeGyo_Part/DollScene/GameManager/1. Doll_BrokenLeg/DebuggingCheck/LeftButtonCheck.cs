using UnityEngine;
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


        if (leftButton == null)
        {
            Debug.LogError(
                "[LeftCheck] Left Button이 연결되지 않았습니다."
            );

            return;
        }


        RectTransform rect =
            leftButton.GetComponent<RectTransform>();


        if (rect == null)
        {
            Debug.LogError(
                "[LeftCheck] Left Button에 RectTransform이 없습니다."
            );

            return;
        }


        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        Image image =
            leftButton.GetComponent<Image>();


        bool inside =
            RectTransformUtility.RectangleContainsScreenPoint(
                rect,
                mousePosition,
                null
            );


        bool raycastTarget =
            image != null &&
            image.raycastTarget;


        Debug.Log(
            "[LeftCheck] " +
            $"Mouse={mousePosition} / " +
            $"Button={leftButton.name} / " +
            $"Active={leftButton.gameObject.activeInHierarchy} / " +
            $"Interactable={leftButton.interactable} / " +
            $"RaycastTarget={raycastTarget} / " +
            $"InsideRect={inside} / " +
            $"RectPos={rect.position} / " +
            $"RectSize={rect.rect.size}"
        );
    }
}