using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Player_Interaction : MonoBehaviour
{
    [Header("상호작용 최대 거리")]
    public float dist;

    [Header("화면 고정")]
    public Player_FixCamera fix;

    [Header("물건 잡기")]
    public Player_Grab grab;

    private Scene_UI_Manager SceneUI;

    [Header("Inventory")]
    [SerializeField]
    private InventoryUIManager inventoryUIManager;

    InputAction interact;

    Event_On_Ray CurrentTarget;
    Event_On_Ray LastTarget;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        interact.performed += ctx => Interact();
        CurrentTarget = null;

        SceneUI = GameManager.instance.SceneUI;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dist))
        {
            Debug.DrawLine(ray.origin, hit.point);

            var Interactable = hit.collider.GetComponent<Event_On_Ray>();

            // 상호작용 가능한 물체
            if (Interactable != null)
            {
                if (CurrentTarget != Interactable)
                {
                    CurrentTarget?.OnRayExit();

                    CurrentTarget = Interactable;
                    CurrentTarget.OnRayEnter();
                    SceneUI.SwitchCursor(true);
                }

                CurrentTarget.OnRayStay();

            }
            // 상호작용 불가능한 물체
            else
            {
                if (CurrentTarget != null)
                {
                    CurrentTarget.OnRayExit();
                    CurrentTarget = null;
                    SceneUI.SwitchCursor(false);
                }
            }
        }
        // 물체가 없음
        else
        {
            if (CurrentTarget != null)
            {
                CurrentTarget.OnRayExit();
                CurrentTarget = null;
                SceneUI.SwitchCursor(false);
            }
        }
    }

    void Interact()
    {
        if (fix.isPlayerFix())
        {
            fix.fixObject.UnFixCamera();
            return;
        }


        if(CurrentTarget != null)
        {
            // 상호작용
            CurrentTarget.OnRayClick();
        }
        else
        {
            // 들고 있는 물건 놓기
            grab.Release();
        }
    }



}
