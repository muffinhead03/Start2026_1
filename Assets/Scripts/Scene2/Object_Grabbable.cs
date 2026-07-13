using TMPro;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

public class Object_Grabbable : MonoBehaviour
{
    [Header("Player Character")]
    public GameObject player;

    // OnClick 에 연결할 함수
    public void OnGrab()
    {
        player.GetComponent<Player_Grab>().Grab(this.gameObject);
    }
}

