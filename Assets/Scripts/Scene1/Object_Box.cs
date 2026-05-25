using UnityEngine;

public class Object_Box : MonoBehaviour
{
    [Header("필요한 열쇠 이름 (Object_Key의 keyName과 일치해야 함)")]
    public string requiredKeyName = "key1";

    [Header("탈출구 문 Animator")]
    public Animator escapeDoorAnimator;

    [Header("상자 Animator (있으면 연결)")]
    public Animator boxAnimator;

    bool isOpened = false;

    public void Interact()
    {
        if (isOpened) return;

        // 인벤토리에 열쇠가 있는지 확인
        if (Player_Inventory.hasKey(requiredKeyName))
        {
            isOpened = true;

            // 상자 닫기 애니메이션
            if (boxAnimator != null)
                boxAnimator.SetInteger("Open", 0);

            // 탈출구 문 열기
            if (escapeDoorAnimator != null)
                escapeDoorAnimator.SetInteger("Open", 1);

            Debug.Log("[Object_Box] 아이템 삽입 완료 → 탈출구 열림");
        }
        else
        {
            Debug.Log("[Object_Box] 탈출 아이템이 없습니다.");
        }
    }
}
