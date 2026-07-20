using TMPro;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

public class Object_Grabbable : MonoBehaviour
{
    [Header("Player Character")]
    public GameObject player;

    [Header("사운드")]
    public AudioClip audio_grap;

    private Play_Audio audio_player;

    void Start()
    {
        audio_player = GetComponent<Play_Audio>();
    }

    // OnClick 에 연결할 함수
    public void OnGrab()
    {
        player.GetComponent<Player_Grab>().Grab(this.gameObject);

        if(audio_player != null) audio_player.PlayAudio(audio_grap);
    }
}

