using UnityEngine;
using UnityEngine.Events;

public class Object_Key : MonoBehaviour
{
    [Header("열쇠")]
    public string keyName;

    public void Collect()
    {
        //gameObject.SetActive(false);
        Player_Inventory.AddItem(keyName);
    }

    public void UseKey()
    {
        gameObject.SetActive(false);
    }
}
