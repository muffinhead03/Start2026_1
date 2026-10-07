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

    [Header("UI")]
    public Scene_UI_Manager SceneUI;

    [Header("Inventory")]
    [SerializeField]
    private InventoryUIManager inventoryUIManager;

    InputAction interact;
    InputAction interact_inspecting;

    Event_On_Ray CurrentTarget;
    Event_On_Ray LastTarget;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        interact.performed += ctx => Interact();
        interact_inspecting = InputSystem.actions.FindAction("Interact While Inspecting");
        interact_inspecting.performed += ctx => InteractWhileInspecting();
        CurrentTarget = null;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dist))
        {
            //Debug.DrawLine(ray.origin, hit.point);

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
        if(CurrentTarget != null)
        {
            // 상호작용
            CurrentTarget.OnRayClick();
        }
        else
        {
            // 책이면 제자리로 (책장 근처일 때만), 아니면 기존처럼 내려놓기
            if (BookInspectGrab.TryReturnHeldBook(grab)) return;
            
            // 들고 있는 물건 놓기
            grab.Release();
        }
    }

    void InteractWhileInspecting()
    {
        // 확대 조사 중이면 E는 무조건 조사 중인 오브젝트가 처리 (취소 / 손에 넣기)
        if (InspectInput.Current != null)
        {
            InspectInput.Current.OnInteractWhileInspecting();
            return;
        }
    }

}