using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;

public class Object_KeyLocked : MonoBehaviour
{
    private bool isLocked = true;

    [Header("열리는 애니메이션")]
    public Animator animator;

    [Header("해제 이벤트")]
    public UnityEvent UnlockEvent;

    [Header("열쇠")]
    public string keyName;

    public void OnInteract()
    {
        if (isLocked)
        {
            // 플레이어에게 열쇠가 있는지 확인
            if (Player_Inventory.hasKey(keyName))
            {
                isLocked = false;
                Debug.Log("열쇠로 문을 열었습니다.");
                UnlockEvent?.Invoke();
                if (animator!=null) OpenCloseDoor();
            }
            else
            {
                Debug.Log("문이 잠겨있습니다. 열쇠가 필요합니다.");
                // 필요시 "딸깍" 하는 소리 재생 로직 추가
            }
        }
        else
        {
            OpenCloseDoor();
        }
    }

    private void OpenCloseDoor()
    {
        int value = (animator.GetInteger("Open") == 0) ? 1 : 0;
        animator.SetInteger("Open", value);
    }
}
