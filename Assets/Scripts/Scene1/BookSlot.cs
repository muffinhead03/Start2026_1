using UnityEngine;

public class BookSlot : MonoBehaviour
{
    [Header("슬롯 순서 (0~3, 와인병 배치 순서와 동일)")]
    public int slotIndex;

    [Header("퍼즐 매니저 연결")]
    public BookShelfPuzzle bookShelfPuzzle;

    [Header("유령책 표시용")]
    public Renderer ghostRenderer;
    public Material ghostMaterial;    // 반투명 (지금 만드신 BookSlot_Ghost_Mat)
    public Material filledMaterial;   // 불투명 (책이 놓였을 때 — 원래 book01 머티리얼)

    [HideInInspector]
    public char placedLetter = '\0';

    public void OnClickSlot()
    {
        bookShelfPuzzle.TryPlaceBook(this);
    }

    public void SetFilled(bool filled)
    {
        if (ghostRenderer != null)
            ghostRenderer.material = filled ? filledMaterial : ghostMaterial;
    }
}