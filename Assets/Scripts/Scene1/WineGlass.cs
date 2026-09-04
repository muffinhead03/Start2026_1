using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class WineGlass : MonoBehaviour
{
    [Header("이 얼룩의 색깔")]
    public string wineColor;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("연결")]
    public HintManager hintManager;
    public FloorStainManager stainManager;   // ← 추가

    bool isShowingInfo = false;   // ← 추가

    void Start()
    {
        if (infoText != null) infoText.text = "";
    }

    void Update()   // ← 추가
    {
        if (!isShowingInfo) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HideInfo();
        }
    }

    public void Inspect()
    {
        hintManager?.AddLastAction("inspect_wine_stain_" + wineColor);
        stainManager?.OnStainInspected(gameObject);   // ← clue_A 직접 추가하던 부분을 매니저 호출로 교체

        if (infoText != null) infoText.text = $"색깔: {wineColor}";
        isShowingInfo = true;   // ← 추가
    }

    void HideInfo()   // ← 추가
    {
        if (infoText != null) infoText.text = "";
        isShowingInfo = false;
    }
}