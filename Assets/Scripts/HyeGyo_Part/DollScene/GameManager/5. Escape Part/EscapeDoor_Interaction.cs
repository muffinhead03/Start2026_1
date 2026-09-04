using UnityEngine;

public class EscapeDoor_Interaction : MonoBehaviour
{
    // =========================================================
    // Target Door
    // =========================================================

    [Header("Target Door")]

    [SerializeField]
    private Door_OpenClose door;


    // =========================================================
    // Player
    // =========================================================

    [Header("Player")]

    [SerializeField]
    private Player_Grab playerGrab;


    // =========================================================
    // Required Item
    //
    // Player_Grab.hasKey()가 Regex.IsMatch()를 사용하므로
    // ^ $를 사용해서 정확히 "Key"만 인식
    // =========================================================

    [Header("Required Item")]

    [SerializeField]
    private string keyItemName = "^Key$";


    // =========================================================
    // Door State
    // =========================================================

    private bool isOpened = false;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        // -----------------------------------------------------
        // Door 자동 탐색
        // -----------------------------------------------------

        if (door == null)
        {
            door =
                GetComponentInParent<Door_OpenClose>();
        }


        // -----------------------------------------------------
        // Player_Grab 자동 탐색
        // -----------------------------------------------------

        if (playerGrab == null)
        {
            playerGrab =
                FindFirstObjectByType<Player_Grab>();
        }


        // -----------------------------------------------------
        // Error Check
        // -----------------------------------------------------

        if (door == null)
        {
            Debug.LogWarning(
                $"[EscapeDoor_Interaction] {gameObject.name} / " +
                $"부모에서 Door_OpenClose를 찾지 못했습니다.",
                this
            );
        }


        if (playerGrab == null)
        {
            Debug.LogError(
                "[EscapeDoor_Interaction] Player_Grab을 찾을 수 없습니다.",
                this
            );
        }
    }


    // =========================================================
    // E 상호작용에서 호출
    // =========================================================

    public void Interact()
    {
        Debug.Log(
            $"[EscapeDoor_Interaction] E 상호작용 : {gameObject.name}"
        );


        // -----------------------------------------------------
        // 필요한 참조 확인
        // -----------------------------------------------------

        if (door == null ||
            playerGrab == null)
        {
            Debug.LogWarning(
                "[EscapeDoor_Interaction] 필요한 참조가 없습니다.",
                this
            );

            return;
        }


        // -----------------------------------------------------
        // 이미 한 번 열린 문
        //
        // 이후에는 어떤 상호작용을 해도 ToggleDoor를 호출하지 않음
        // -----------------------------------------------------

        if (isOpened)
        {
            Debug.Log(
                "[EscapeDoor_Interaction] 이미 열린 문입니다."
            );

            return;
        }


        // -----------------------------------------------------
        // Key를 들고 있는지 확인
        // -----------------------------------------------------

        if (!playerGrab.hasKey(keyItemName))
        {
            Debug.Log(
                "[EscapeDoor_Interaction] Key를 들고 있지 않습니다."
            );

            return;
        }


        // -----------------------------------------------------
        // Key 보유 → 문 열기
        // -----------------------------------------------------

        Debug.Log(
            "[EscapeDoor_Interaction] Key 확인 → 탈출문을 엽니다."
        );


        // 먼저 상태를 잠금
        // 연속 입력으로 ToggleDoor가 두 번 실행되는 것을 방지
        isOpened = true;


        // 기존 Door_OpenClose는 수정하지 않고 그대로 사용
        door.ToggleDoor();
    }
}