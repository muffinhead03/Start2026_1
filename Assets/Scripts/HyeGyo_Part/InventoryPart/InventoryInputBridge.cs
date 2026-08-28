using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class InventoryInputBridge : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    private InventoryUIManager inventoryUIManager;


    [Header("World Interact")]
    [Tooltip(
        "기존 물건 집기/상호작용 액션입니다. " +
        "PC/Interact를 연결하세요."
    )]
    [SerializeField]
    private InputActionReference worldInteractAction;


    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference inventoryToggleAction;

    [SerializeField]
    private InputActionReference inventoryCloseAction;

    [SerializeField]
    private InputActionReference inventoryConfirmAction;

    [SerializeField]
    private InputActionReference inventoryScrollAction;


    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;


    /*
     * 마지막으로 적용했던 인벤토리 상태입니다.
     *
     * 상태가 실제로 바뀌었을 때만
     * Interact Enable/Disable을 수행하기 위해 사용합니다.
     */
    private bool lastInventoryOpenState;

    private bool hasInventoryOpenState;


    private void Awake()
    {
        if (inventoryUIManager == null)
        {
            inventoryUIManager =
                GetComponentInChildren<InventoryUIManager>(true);
        }

        if (inventoryUIManager == null)
        {
            Debug.LogError(
                "[InventoryInputBridge] " +
                "InventoryUIManager가 연결되지 않았습니다.",
                this
            );

            enabled = false;
        }
    }


    private void OnEnable()
    {
        RegisterAction(
            inventoryToggleAction,
            HandleInventoryToggle
        );

        RegisterAction(
            inventoryCloseAction,
            HandleInventoryClose
        );

        RegisterAction(
            inventoryConfirmAction,
            HandleInventoryConfirm
        );

        RegisterAction(
            inventoryScrollAction,
            HandleInventoryScroll
        );


        /*
         * 게임 시작 시 현재 인벤토리 상태에 맞춰
         * PC/Interact 상태를 즉시 맞춰줍니다.
         */
        SyncWorldInteractAction(true);


        string scrollReferenceName =
            inventoryScrollAction != null
                ? inventoryScrollAction.name
                : "null";

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryInputBridge] OnEnable 완료: " +
                "ScrollReference=" +
                scrollReferenceName,
                this
            );
        }
    }


    private void Update()
    {
        /*
         * InventoryUIManager.OpenInventory(),
         * CloseInventory()가 다른 코드에서 직접 호출되더라도
         * 상태를 강제로 동기화합니다.
         */
        SyncWorldInteractAction(false);
    }


    private void OnDisable()
    {
        UnregisterAction(
            inventoryToggleAction,
            HandleInventoryToggle
        );

        UnregisterAction(
            inventoryCloseAction,
            HandleInventoryClose
        );

        UnregisterAction(
            inventoryConfirmAction,
            HandleInventoryConfirm
        );

        UnregisterAction(
            inventoryScrollAction,
            HandleInventoryScroll
        );

        hasInventoryOpenState = false;
    }


    private void HandleInventoryToggle(
        InputAction.CallbackContext context)
    {
        if (inventoryUIManager == null)
        {
            return;
        }

        inventoryUIManager.ToggleInventory();

        /*
         * Toggle 직후 바로 Interact 상태 변경.
         */
        SyncWorldInteractAction(true);
    }


    private void HandleInventoryClose(
        InputAction.CallbackContext context)
    {
        if (inventoryUIManager == null ||
            !inventoryUIManager.IsOpen)
        {
            return;
        }

        inventoryUIManager.CloseInventory();

        /*
         * 닫힌 즉시 PC/Interact 다시 활성화.
         */
        SyncWorldInteractAction(true);
    }


    private void HandleInventoryConfirm(
        InputAction.CallbackContext context)
    {
        if (inventoryUIManager == null ||
            !inventoryUIManager.IsOpen)
        {
            return;
        }

        inventoryUIManager.ConfirmSelectedItem();
    }


    private void HandleInventoryScroll(
        InputAction.CallbackContext context)
    {
        if (inventoryUIManager == null)
        {
            Debug.LogError(
                "[InventoryInputBridge] " +
                "InventoryUIManager가 없어 " +
                "스크롤을 처리할 수 없습니다.",
                this
            );

            return;
        }


        /*
         * UI/ScrollWheel은 Vector2입니다.
         *
         * x = 가로
         * y = 세로 휠
         */
        Vector2 scrollValue =
            context.ReadValue<Vector2>();

        float scrollY =
            scrollValue.y;


        if (showDebugLog)
        {
            string controlPath = "null";

            if (context.control != null)
            {
                controlPath =
                    context.control.path;
            }

            Debug.Log(
                "[InventoryInputBridge] 스크롤 입력 감지: " +
                "X=" + scrollValue.x +
                ", Y=" + scrollValue.y +
                ", Control=" + controlPath,
                this
            );
        }


        /*
         * 입력 종료 시 들어오는 0 값 무시.
         */
        if (Mathf.Abs(scrollY) < 0.01f)
        {
            return;
        }


        /*
         * 휠 위   = 이전 아이템
         * 휠 아래 = 다음 아이템
         */
        int direction =
            scrollY > 0f
                ? -1
                : 1;


        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryInputBridge] 순환 요청 전달: " +
                "Direction=" + direction,
                this
            );
        }


        inventoryUIManager.CycleSelectedItem(
            direction
        );
    }


    /*
     * =========================================================
     * 월드 Interact 제어
     * =========================================================
     *
     * Inventory Open:
     *
     *     PC/Interact = Disabled
     *
     * Inventory Closed:
     *
     *     PC/Interact = Enabled
     *
     * 따라서 E가
     *
     *     PC/Interact
     *     PC/InventoryConfirmAction
     *
     * 두 액션에 동시에 바인딩되어 있어도
     * 인벤토리 상태에 따라 하나만 동작합니다.
     */
    private void SyncWorldInteractAction(
        bool force)
    {
        if (inventoryUIManager == null)
        {
            return;
        }


        bool inventoryOpen =
            inventoryUIManager.IsOpen;


        if (!force &&
            hasInventoryOpenState &&
            lastInventoryOpenState == inventoryOpen)
        {
            return;
        }


        lastInventoryOpenState =
            inventoryOpen;

        hasInventoryOpenState =
            true;


        SetWorldInteractEnabled(
            !inventoryOpen
        );
    }


    private void SetWorldInteractEnabled(
        bool shouldEnable)
    {
        if (worldInteractAction == null)
        {
            Debug.LogError(
                "[InventoryInputBridge] " +
                "World Interact Action이 연결되지 않았습니다. " +
                "PC/Interact를 연결하세요.",
                this
            );

            return;
        }


        InputAction action =
            worldInteractAction;


        if (action == null)
        {
            Debug.LogError(
                "[InventoryInputBridge] " +
                "World Interact Action 안에 " +
                "InputAction이 없습니다.",
                this
            );

            return;
        }


        if (shouldEnable)
        {
            if (!action.enabled)
            {
                action.Enable();
            }
        }
        else
        {
            if (action.enabled)
            {
                action.Disable();
            }
        }


        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryInputBridge] World Interact 상태 변경: " +
                $"Action={action.name}, " +
                $"InventoryOpen={inventoryUIManager.IsOpen}, " +
                $"InteractEnabled={action.enabled}",
                this
            );
        }
    }


    private void RegisterAction(
        InputAction action,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            Debug.LogError(
                "[InventoryInputBridge] " +
                "InputActionReference가 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (action == null)
        {
            Debug.LogError(
                "[InventoryInputBridge] " +
                "InputActionReference 안에 Action이 없습니다.",
                this
            );

            return;
        }


        action.performed -= callback;
        action.performed += callback;

        action.Enable();


        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryInputBridge] 액션 등록 완료: " +
                $"Action={action.name}, " +
                $"Map=" +
                $"{(action.actionMap != null ? action.actionMap.name : "null")}, " +
                $"Type={action.type}, " +
                $"ExpectedControlType={action.expectedControlType}, " +
                $"BindingCount={action.bindings.Count}, " +
                $"Enabled={action.enabled}",
                this
            );
        }
    }


    private void UnregisterAction(
        InputAction action,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (action == null)
        {
            return;
        }


        action.performed -= callback;
        action.Disable();


        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryInputBridge] 액션 해제: " +
                $"Action={action.name}",
                this
            );
        }
    }
}