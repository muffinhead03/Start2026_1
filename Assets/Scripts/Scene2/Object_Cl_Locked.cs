using UnityEngine;
using UnityEngine.Events;

public class Object_Cl_Locked : MonoBehaviour
{
    [Header("Unlock Events")]
    public UnityEvent Unlock;

    public int num;
    public int count=0;

    public void SetCount()
    {
        count++;
        if (count == num) Unlock?.Invoke();
    }
}
