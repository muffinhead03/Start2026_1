using System.Collections.Generic;
using UnityEngine;

// 사용법:
// 1. 씬에 빈 오브젝트 만들고 이 스크립트 추가
// 2. Inspector에서 wineEntries 등록
//    ex. a와인 / 딥레드 / 18 → S
//        b와인 / 로제핑크 / 13 → O
//        c와인 / 버건디 / 3  → F
//        d와인 / 오렌지레드 / 24 → A
// 3. 정답 가구 이름: SOFA (자동 계산)

[System.Serializable]
public class WineEntry
{
    public string wineAlphabet;   // "a"
    public string wineColor;      // "딥레드"
    public int    shiftNumber;    // 18
}

public class PuzzleSolver : MonoBehaviour
{
    [Header("와인 정보 목록 (Inspector에서 등록)")]
    public List<WineEntry> wineEntries = new List<WineEntry>();

    // 계산된 정답 가구 이름 (자동)
    [HideInInspector]
    public string answerFurnitureName;

    void Start()
    {
        answerFurnitureName = CalculateAnswer();
        Debug.Log($"[PuzzleSolver] 정답 가구: {answerFurnitureName}");
    }

    // 와인 첫 글자 + 숫자만큼 알파벳 이동 (z→a 순환)
    string CalculateAnswer()
    {
        string result = "";
        foreach (var entry in wineEntries)
        {
            if (string.IsNullOrEmpty(entry.wineAlphabet)) continue;

            char start = entry.wineAlphabet.ToLower()[0]; // 'a'
            int shifted = (start - 'a' + entry.shiftNumber) % 26;
            char answer = (char)('a' + shifted);
            result += char.ToUpper(answer);
        }
        return result; // ex. "SOFA"
    }

    // 플레이어가 가구 조사 시 호출 — 정답 여부 반환
    public bool CheckFurniture(string furnitureName)
    {
        bool correct = furnitureName.ToUpper() == answerFurnitureName.ToUpper();
        Debug.Log($"[PuzzleSolver] {furnitureName} 조사 → {(correct ? "정답!" : "오답")}");
        return correct;
    }
}
