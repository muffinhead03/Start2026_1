using UnityEngine;

public class EscapeDoor_Interaction : MonoBehaviour
{
    // =========================================================
    // Player
    // =========================================================

    [Header("Player")]

    [SerializeField]
    private Player_Grab playerGrab;


    // =========================================================
    // Door
    // =========================================================

    [Header("Escape Door")]

    [SerializeField]
    private Object_Door door;


    [SerializeField]
    private BoxCollider doorBoxCollider;


    // =========================================================
    // Required Key
    // =========================================================

    [Header("Required Key")]

    [SerializeField]
    private string keyItemName = "^Key$";


    // =========================================================
    // State
    // =========================================================

    private bool isOpened = false;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        // Player 자동 탐색
        if (playerGrab == null)
        {
            playerGrab =
                FindFirstObjectByType<Player_Grab>();
        }


        // Object_Door 자동 탐색
        if (door == null)
        {
            door =
                GetComponentInParent<Object_Door>();
        }


        // Door BoxCollider 자동 탐색
        if (doorBoxCollider == null &&
            door != null)
        {
            doorBoxCollider =
                door.GetComponent<BoxCollider>();
        }


        // Error Check
        if (playerGrab == null)
        {
            Debug.LogError(
                "[EscapeDoor_Interaction] Player_Grab을 찾을 수 없습니다.",
                this
            );
        }


        if (door == null)
        {
            Debug.LogError(
                "[EscapeDoor_Interaction] Object_Door를 찾을 수 없습니다.",
                this
            );
        }


        if (doorBoxCollider == null)
        {
            Debug.LogWarning(
                "[EscapeDoor_Interaction] Door의 BoxCollider를 찾을 수 없습니다.",
                this
            );
        }
    }


    // =========================================================
    // E 상호작용
    // =========================================================

    public void Interact()
    {
        Debug.Log(
            "[EscapeDoor_Interaction] E 상호작용",
            this
        );


        // 참조 확인
        if (playerGrab == null ||
            door == null)
        {
            Debug.LogWarning(
                "[EscapeDoor_Interaction] 필요한 참조가 없습니다.",
                this
            );

            return;
        }


        // 이미 열린 문이면 무시
        if (isOpened)
        {
            Debug.Log(
                "[EscapeDoor_Interaction] 이미 열린 탈출문입니다.",
                this
            );

            return;
        }


        // Key 확인
        if (!playerGrab.hasKey(keyItemName))
        {
            Debug.Log(
                "[EscapeDoor_Interaction] Key를 들고 있지 않습니다.",
                this
            );

            return;
        }


        // Animator 확인
        if (door.animator == null)
        {
            Debug.LogWarning(
                "[EscapeDoor_Interaction] Object_Door의 Animator가 없습니다.",
                this
            );

            return;
        }


        // 이미 Animator상 열린 상태
        if (door.animator.GetInteger("Open") != 0)
        {
            isOpened = true;

            if (doorBoxCollider != null)
            {
                doorBoxCollider.enabled = false;
            }

            Debug.Log(
                "[EscapeDoor_Interaction] 문이 이미 열린 상태입니다.",
                this
            );

            return;
        }


        // Key 확인 성공
        Debug.Log(
            "[EscapeDoor_Interaction] Key 확인 → 탈출문 잠금 해제",
            this
        );


        isOpened = true;


        // 기존 Object_Door 애니메이션 실행
        door.UnlockDoor();


        // Door BoxCollider 비활성화
        if (doorBoxCollider != null)
        {
            doorBoxCollider.enabled = false;

            Debug.Log(
                "[EscapeDoor_Interaction] Door BoxCollider 비활성화",
                this
            );
        }
    }
}