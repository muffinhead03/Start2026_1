using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_KeyLocked : MonoBehaviour
{
    private bool isLocked = true;

    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("해제 이벤트")]
    public UnityEvent UnlockEvent;

    [Header("열쇠")]
    public string keyName;

    public void OnInteract()
    {
        if (isLocked)
        {
            // 플레이어에게 열쇠가 있는지 확인
            if (player.GetComponent<Player_Grab>().hasKey(keyName))
            {
                player.GetComponent<Player_Grab>().UseKey();
                Debug.Log("열쇠로 문을 열었습니다.");
                UnlockEvent?.Invoke();
            }
            else
            {
                Debug.Log("문이 잠겨있습니다. 열쇠가 필요합니다.");
            }
        }
    }

    public void UseKey(string keyName)
    {
        if(Regex.IsMatch(this.keyName, keyName))
        {
            Debug.Log("열쇠로 문을 열었습니다.");
            UnlockEvent?.Invoke();
        }
        else
        {
            Debug.Log("문이 잠겨있습니다. 열쇠가 필요합니다.");
        }
    }
}
