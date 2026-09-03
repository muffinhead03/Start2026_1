using UnityEngine;
using TMPro;

// 사용법:
// 1. 얼룩 페어 중점 오브젝트 4개(Cross_Red 등)에 이 스크립트 추가
// 2. letter에 이 중점의 알파벳 입력 (예: 'S')
// 3. Event_On_Ray OnClick → Inspect() 연결
// 4. notePickup에 이 교차점 근처에 미리 배치해둔 쪽지 오브젝트(비활성 상태) 연결
//    쪽지 오브젝트에는 Object_Grabbable + Event_On_Ray(OnClick → OnGrab) 필요
public class StainIntersection : MonoBehaviour
{
    [Header("이 중점의 알파벳")]
    public char letter;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("연결")]
    public HintManager hintManager;
    public FloorStainManager stainManager;

    [Header("쪽지 수집")]
    [Tooltip("조사 시 활성화될 쪽지 오브젝트. 처음엔 비활성 상태로 배치해두세요.")]
    public GameObject notePickup;

    bool inspected = false;

    public void Inspect()
    {
        if (infoText != null)
            infoText.text = $"알파벳: {letter}";

        if (notePickup != null && !notePickup.activeSelf)
            notePickup.SetActive(true);

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