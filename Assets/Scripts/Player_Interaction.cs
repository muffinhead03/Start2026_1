using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Player_Interaction : MonoBehaviour
{
    InputAction interact;

    Event_On_Ray CurrentTarget;
    Event_On_Ray LastTarget;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        interact.performed += ctx => Interact();
        CurrentTarget = null;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        RaycastHit hit;

        // 5미터 안에 물체가 있음
        if (Physics.Raycast(ray, out hit, 5))
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
                    Debug.Log("Enter");
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
            }
        }
    }

    void Interact()
    {
        Debug.Log("Click");
        if(CurrentTarget != null)
        {
            CurrentTarget.OnRayClick();

        }
    }

}
