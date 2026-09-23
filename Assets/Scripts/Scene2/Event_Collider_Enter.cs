using UnityEngine;
using UnityEngine.Events;

public class Event_Collider_Enter : MonoBehaviour
{
    [SerializeField] UnityEvent OnEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Move>() == null) return;

        OnEnter?.Invoke();
    }
}
