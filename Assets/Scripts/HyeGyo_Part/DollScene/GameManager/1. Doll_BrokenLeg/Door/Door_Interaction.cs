using UnityEngine;

public class Door_Interaction : MonoBehaviour
{
    [Header("Target Door")]

    [SerializeField]
    private Door_OpenClose door;


    // ================================================
    // Awake
    // ================================================

    private void Awake()
    {
        // Inspector에서 연결하지 않았으면
        // 부모에서 자동으로 Door_OpenClose 찾기
        if (door == null)
        {
            door =
                GetComponentInParent<Door_OpenClose>();
        }


        if (door == null)
        {
            Debug.LogWarning(
                $"[DoorInteraction] {gameObject.name} / " +
                $"부모에서 Door_OpenClose를 찾지 못했습니다."
            );
        }
    }


    // ================================================
    // E 상호작용에서 호출
    // ================================================

    public void Interact()
    {
        Debug.Log(
            $"[DoorInteraction] E 상호작용 : {gameObject.name}"
        );


        if (door == null)
        {
            Debug.LogWarning(
                "[DoorInteraction] Door가 없습니다."
            );

            return;
        }


        door.ToggleDoor();
    }
}