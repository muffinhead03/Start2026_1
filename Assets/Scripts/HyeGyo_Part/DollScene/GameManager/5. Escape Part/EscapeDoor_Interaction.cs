using UnityEngine;


public class EscapeDoor_Interaction : MonoBehaviour
{
    // =========================================================
    // Chest Lid
    // =========================================================

    [Header("먼저 열어야 하는 상자 뚜껑")]

    [Tooltip(
        "실제로 열릴 때 Rotation이 변하는 상자 뚜껑 Transform을 넣습니다."
    )]
    [SerializeField]
    private Transform chestLid;


    [Header("상자가 열렸다고 판단할 최소 각도")]

    [Tooltip(
        "게임 시작 시 뚜껑 Rotation에서 " +
        "이 각도 이상 회전하면 열린 것으로 판단합니다."
    )]
    [SerializeField]
    private float chestOpenAngle = 20f;


    // =========================================================
    // Door
    // =========================================================

    [Header("Escape Door")]

    [SerializeField]
    private Object_Door door;


    [SerializeField]
    private BoxCollider doorBoxCollider;


    // =========================================================
    // Door Interaction
    // =========================================================

    [Header("상자가 열리면 활성화할 문 Interaction")]

    [Tooltip(
        "Event_On_Ray, Toggle Controller 등 " +
        "상자가 열리기 전에는 꺼둘 Behaviour를 넣습니다."
    )]
    [SerializeField]
    private Behaviour[] doorInteractionComponents;


    // =========================================================
    // State
    // =========================================================

    private Quaternion chestClosedRotation;


    private bool isChestOpened = false;


    private bool isOpened = false;


    // =========================================================
    // 외부 확인
    // =========================================================

    public bool IsChestOpened =>
        isChestOpened;


    public bool IsOpened =>
        isOpened;


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
                GetComponentInParent<Object_Door>();
        }


        // -----------------------------------------------------
        // Door Collider 자동 탐색
        // -----------------------------------------------------

        if (
            doorBoxCollider == null &&
            door != null
        )
        {
            doorBoxCollider =
                door.GetComponent<BoxCollider>();
        }


        // -----------------------------------------------------
        // 상자 닫힌 Rotation 저장
        // -----------------------------------------------------

        if (chestLid != null)
        {
            chestClosedRotation =
                chestLid.localRotation;
        }
        else
        {
            Debug.LogWarning(
                "[EscapeDoor_Interaction] " +
                "Chest Lid가 연결되지 않았습니다.",
                this
            );
        }


        // -----------------------------------------------------
        // 처음에는 문 Interaction 잠금
        // -----------------------------------------------------

        SetDoorInteractionEnabled(
            false
        );


        // -----------------------------------------------------
        // Error Check
        // -----------------------------------------------------

        if (door == null)
        {
            Debug.LogError(
                "[EscapeDoor_Interaction] " +
                "Object_Door를 찾을 수 없습니다.",
                this
            );
        }
    }


    // =========================================================
    // Update
    //
    // 상자 뚜껑 상태 감시
    // =========================================================

    private void Update()
    {
        if (isChestOpened)
        {
            return;
        }


        CheckChestOpened();
    }


    // =========================================================
    // 상자 열림 검사
    // =========================================================

    private void CheckChestOpened()
    {
        if (chestLid == null)
        {
            return;
        }


        float rotationDifference =
            Quaternion.Angle(
                chestClosedRotation,
                chestLid.localRotation
            );


        if (
            rotationDifference <
            chestOpenAngle
        )
        {
            return;
        }


        // =====================================================
        // 상자 열림 확정
        // =====================================================

        isChestOpened = true;


        Debug.Log(
            "[EscapeDoor_Interaction] " +
            $"상자 열림 감지 / Rotation 변화 : " +
            $"{rotationDifference:F1}°",
            this
        );


        // 문 상호작용 활성화
        SetDoorInteractionEnabled(
            true
        );
    }


    // =========================================================
    // 문 Interaction ON / OFF
    // =========================================================

    private void SetDoorInteractionEnabled(
        bool enabled
    )
    {
        if (doorInteractionComponents == null)
        {
            return;
        }


        for (
            int i = 0;
            i <
            doorInteractionComponents.Length;
            i++
        )
        {
            Behaviour component =
                doorInteractionComponents[i];


            if (component == null)
            {
                continue;
            }


            component.enabled =
                enabled;
        }


        Debug.Log(
            "[EscapeDoor_Interaction] " +
            $"Door Interaction : {enabled}",
            this
        );
    }


    // =========================================================
    // 문 상호작용
    // =========================================================

    public void Interact()
    {
        Debug.Log(
            "[EscapeDoor_Interaction] 문 상호작용",
            this
        );


        // =====================================================
        // 참조 확인
        // =====================================================

        if (door == null)
        {
            Debug.LogWarning(
                "[EscapeDoor_Interaction] " +
                "Door 참조가 없습니다.",
                this
            );

            return;
        }


        // =====================================================
        // 상자부터 열어야 함
        // =====================================================

        if (!isChestOpened)
        {
            Debug.Log(
                "[EscapeDoor_Interaction] " +
                "먼저 상자를 열어야 합니다.",
                this
            );

            return;
        }


        // =====================================================
        // 이미 열린 문
        // =====================================================

        if (isOpened)
        {
            return;
        }


        // =====================================================
        // Animator 확인
        // =====================================================

        if (door.animator == null)
        {
            Debug.LogWarning(
                "[EscapeDoor_Interaction] " +
                "Object_Door의 Animator가 없습니다.",
                this
            );

            return;
        }


        // =====================================================
        // Animator상 이미 열림
        // =====================================================

        if (
            door.animator.GetInteger("Open") != 0
        )
        {
            isOpened =
                true;


            DisableDoorCollider();


            return;
        }


        // =====================================================
        // 문 열기
        // =====================================================

        isOpened =
            true;


        door.UnlockDoor();


        DisableDoorCollider();


        Debug.Log(
            "[EscapeDoor_Interaction] " +
            "상자 열림 확인 → 탈출문 열림",
            this
        );
    }


    // =========================================================
    // Door Collider 제거
    // =========================================================

    private void DisableDoorCollider()
    {
        if (doorBoxCollider == null)
        {
            return;
        }


        doorBoxCollider.enabled =
            false;


        Debug.Log(
            "[EscapeDoor_Interaction] " +
            "Door BoxCollider 비활성화",
            this
        );
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (chestOpenAngle < 0f)
        {
            chestOpenAngle =
                0f;
        }
    }
}