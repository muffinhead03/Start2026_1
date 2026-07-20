using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Object_PutOn : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("투명 머터리얼")]
    [SerializeField] Material mat_trans;

    [Header("메쉬 렌더러")]
    [SerializeField] MeshRenderer mesh;

    [Header("열쇠 이름")]
    [SerializeField] string keyName;

    [Header("사운드")]
    public AudioClip audio_puton;

    private Play_Audio audio_player;

    int state;
    GameObject putOn;

    void Start()
    {
        state = 0;

        putOn = null;

        audio_player = GetComponent<Play_Audio>();
    }

    public void OnInteract()
    {
        if (state == 0 && player.GetComponent<Player_Grab>().hasKey(keyName + "_" + @"[0-9]"))
        {
            mesh.enabled = false;
            state = 1;
            putOn = player.GetComponent<Player_Grab>().PutOn(transform.position);

            StartCoroutine(PlayAudioAfter(0.5f));
        }
    }

    public void OnEnter()
    {
        if (putOn == null)
        {
            state = 0;
        }
        else
        {
            float dist = Vector3.Distance(putOn.transform.position, transform.position);
            if (dist < 0.1f) state = 1;
            else
            {
                state = 0;
                putOn = null;
            }
        }

        if (state == 0 && player.GetComponent<Player_Grab>().hasKey(keyName + "_" + @"[0-9]"))
        {
            mesh.material = mat_trans;
            mesh.enabled = true;
        }
    }

    public void OnExit()
    {
        mesh.enabled = false;
    }

    IEnumerator PlayAudioAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        audio_player.PlayAudio(audio_puton);
    }
}
