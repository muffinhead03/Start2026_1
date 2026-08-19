using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Bootstrapped Prefabs")]
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject inventoryPrefab;
    [SerializeField] private GameObject playerPrefab;

    public Canvas UICanvas { get; private set; }
    public Scene_UI_Manager SceneUI { get; private set; }
    public InventoryUIManager Inventory { get; private set; }

    public event Action OpenClosePanel;

    private Player_Move player;
    private InputActionMap ui_input;
    private InputAction settingsAction;
    private bool isPanelOpen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        RegisterInputAction();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnregisterInputAction();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitScene();
    }

    private void RegisterInputAction()
    {
        ui_input = InputSystem.actions.FindActionMap("PC_UI");
        ui_input.Enable();
        settingsAction = InputSystem.actions.FindAction("Settings");
        settingsAction.performed += OpenCloseSettings;
        isPanelOpen = false;
    }

    private void UnregisterInputAction()
    {
        ui_input.Disable();
        settingsAction.performed -= OpenCloseSettings;
        isPanelOpen = false;
    }

    private void InitScene()
    {
        if (canvasPrefab != null)
        {
            UICanvas = Instantiate(canvasPrefab).GetComponent<Canvas>();

            SceneUI = UICanvas.GetComponentInChildren<Scene_UI_Manager>();
        }

        if (inventoryPrefab != null) Inventory = Instantiate(inventoryPrefab).GetComponent<InventoryUIManager>();

        if (playerPrefab != null)
            player = Instantiate(playerPrefab).GetComponent<Player_Move>();
    }

    private void OpenCloseSettings(InputAction.CallbackContext context)
    {
        isPanelOpen = !isPanelOpen;
        OpenClosePanel?.Invoke();
        SceneUI?.SetActivePanel(0, isPanelOpen);

        if (isPanelOpen)
        {
            SceneUI?.UnlockPointer();
        }
        else
        {
            SceneUI?.LockPointer();
        }
    }

    public void OpenCloseInventory()
    {
        OpenClosePanel?.Invoke();
    }
}
