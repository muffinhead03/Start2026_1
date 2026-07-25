using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("슬롯 UI")]
    [SerializeField] private Image slotBackground;
    [SerializeField] private Image itemIcon;

    [Header("선택 투명도")]
    [SerializeField, Range(0f, 1f)]
    private float selectedAlpha = 1f;

    [SerializeField, Range(0f, 1f)]
    private float unselectedAlpha = 0.4f;

    private string currentItemId;

    public string CurrentItemId => currentItemId;
    public bool HasItem => !string.IsNullOrEmpty(currentItemId);

    public void SetItem(string itemId, Sprite icon)
    {
        currentItemId = itemId;

        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = icon != null;
        }
    }

    public void ClearSlot()
    {
        currentItemId = null;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
    }

    public void SetSelected(bool selected)
    {
        if (slotBackground == null)
        {
            return;
        }

        Color color = slotBackground.color;
        color.a = selected ? selectedAlpha : unselectedAlpha;
        slotBackground.color = color;
    }
}