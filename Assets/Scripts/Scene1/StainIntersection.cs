using UnityEngine;
using TMPro;

// 사용법:
// 1. 얼룩 페어 중점 오브젝트 4개(Cross_Red 등)에 이 스크립트 추가
// 2. letter에 이 중점의 알파벳 입력 (예: 'S')
// 3. Event_On_Ray OnClick → Inspect() 연결
public class StainIntersection : MonoBehaviour
{
    [Header("이 중점의 알파벳")]
    public char letter;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("연결")]
    public HintManager hintManager;
    public FloorStainManager stainManager;

    bool inspected = false;

    public void Inspect()
    {
        if (infoText != null)
            infoText.text = $"알파벳: {letter}";

        if (inspected) return;
        inspected = true;

        hintManager?.AddLastAction("inspect_alphabet_marker_" + letter);
        stainManager?.OnMidpointInspected(gameObject);
    }

    public void Hide()
    {
        if (infoText != null) infoText.text = "";
    }
}