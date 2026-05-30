using UnityEngine;
using TMPro;

public class Room3Keypad : MonoBehaviour
{
    public HintManager hintManager;
    public GameObject door;
    public GameObject pipe3Object; // 파이프 3 오브젝트
    public TextMeshProUGUI displayText;

    string input = "";
    const string ANSWER = "4025";
    bool solved = false;

    public void PressKey(string digit)
    {
        if (solved) return;
        if (input.Length >= 4) input = "";
        input += digit;
        if (displayText) displayText.text = input;

        if (input == ANSWER)
        {
            solved = true;
            door.SetActive(false);
            pipe3Object.SetActive(true);
            hintManager.currentPlayerState.completedSteps.Add(5);
            Debug.Log("[Room3Keypad] 방3 개방, 파이프 3 등장");
        }
    }

    public void Clear()
    {
        input = "";
        if (displayText) displayText.text = "";
    }
}