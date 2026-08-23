using UnityEngine;
using TMPro;

// LoadingText 오브젝트에 붙이는 스크립트.
// "로딩 중" 뒤에 점이 . → .. → ... → (없음) 순서로 반복되며 붙음
public class LoadingDotsAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string baseText = "로딩 중";
    [SerializeField] private float dotInterval = 0.4f;

    float timer = 0f;
    int dotCount = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= dotInterval)
        {
            timer = 0f;
            dotCount = (dotCount + 1) % 4; // 0,1,2,3 반복
            text.text = baseText + new string('.', dotCount);
        }
    }
}