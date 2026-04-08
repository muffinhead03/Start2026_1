using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interaction : MonoBehaviour
{

    IRayInteractable CurrentTarget;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5))
        {
            Debug.DrawLine(ray.origin, hit.point);

        }
    }
}
