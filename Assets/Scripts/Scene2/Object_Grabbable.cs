using TMPro;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;

public class Object_Grabbable : MonoBehaviour
{
    [Header("이름")]
    public string objectName;

    [Header("설명")]
    [TextArea(3, 8)]
    public string description;

    [Header("Player Character")]
    public GameObject player;

    [Header("사운드")]
    public AudioClip[] audio_grap;

    private Play_Audio audio_player;

    void Start()
    {
        audio_player = GetComponent<Play_Audio>();
    }

    // OnClick 에 연결할 함수
    public void OnGrab()
    {
        player.GetComponent<Player_Grab>().Grab(this);

        if (audio_player != null)
        {
            int random_id = Random.Range(0, audio_grap.Length);
            audio_player.PlayAudio(audio_grap[random_id]);
        }
    }
}