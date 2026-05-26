using UnityEngine;
using TMPro;

// 사용법:
// 1. 각 가구 오브젝트에 이 스크립트 추가
// 2. furnitureName 입력 (ex. "SOFA")
// 3. escapeItem 슬롯에 탈출 아이템 오브젝트 연결 (평소엔 비활성화)
// 4. puzzleSolver 슬롯에 PuzzleSolver 오브젝트 연결
// 5. Event_On_ 컴포넌트 OnClick → Inspect() 연결
public class FurnitureInspect : MonoBehaviour
{
    [Header("이 가구 이름")]
    public string furnitureName; // ex. "SOFA"

    [Header("탈출 아이템 오브젝트 (비활성화 상태로 배치)")]
    public GameObject escapeItem;

    [Header("퍼즐 솔버 연결")]
    public PuzzleSolver puzzleSolver;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    bool alreadyFound = false;

    public void Inspect()
    {
        if (alreadyFound) return;

        if (puzzleSolver == null)
        {
            Debug.LogError("[FurnitureInspect] PuzzleSolver 연결 안 됨");
            return;
        }

        if (puzzleSolver.CheckFurniture(furnitureName))
        {
            // 정답 가구
            alreadyFound = true;
            if (escapeItem != null)
                escapeItem.SetActive(true); // 탈출 아이템 노출

            if (infoText != null)
                infoText.text = "무언가 숨겨져 있었다...";

            Debug.Log($"[FurnitureInspect] 정답! {furnitureName}에서 탈출 아이템 발견");
        }
        else
        {
            // 오답 가구
            if (infoText != null)
                infoText.text = $"{furnitureName}을(를) 조사했지만 아무것도 없다.";
        }
    }
}
