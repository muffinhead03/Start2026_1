using UnityEngine;
using TMPro;

public class WineGlass : MonoBehaviour
{
    [Header("이 얼룩의 색깔")]
    public string wineColor;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("연결")]
    public HintManager hintManager;
    public FloorStainManager stainManager;   // ← 추가

    void Start()
    {
        if (infoText != null) infoText.text = "";
    }

    public void Inspect()
    {
        hintManager?.AddLastAction("inspect_wine_stain_" + wineColor);
        stainManager?.OnStainInspected(gameObject);   // ← clue_A 직접 추가하던 부분을 매니저 호출로 교체

        if (infoText != null) infoText.text = $"색깔: {wineColor}";
    }
}