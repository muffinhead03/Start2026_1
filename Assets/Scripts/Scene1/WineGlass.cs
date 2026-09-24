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
    public FloorStainManager stainManager;

    void Start()
    {
        if (infoText != null) infoText.text = "";
    }

    // Event_On_Ray의 On Click()에 연결
    public void Inspect()
    {
        hintManager?.AddLastAction("inspect_wine_stain_" + wineColor);
        stainManager?.OnStainInspected(gameObject);

        if (infoText != null) infoText.text = $"색깔: {wineColor}";
    }

    // Event_On_Ray의 On Exit()에 연결 — StainIntersection.Hide()와 동일한 패턴.
    // 커서가 이 오브젝트를 벗어나면(레이가 빠지면) 텍스트가 자동으로 사라짐.
    public void Hide()
    {
        if (infoText != null) infoText.text = "";
    }
}