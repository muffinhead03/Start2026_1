using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("이벤트")]
    public UnityEvent putDown;
    public UnityEvent pickUp;

    private Play_Audio audio_player;

    int state;
    public GameObject putOn;

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
            PutOn();
        }
        else if(state == 1 && !player.GetComponent<Player_Grab>().isGrab())
        {
            TakeOff();

        }
    }

    void PutOn()
    {
        mesh.enabled = false;
        state = 1;
        putOn = player.GetComponent<Player_Grab>().PutOn(transform.position);

        StartCoroutine(AfterPutDown(0.5f));
    }

    void TakeOff()
    {
        Object_Grabbable grab = putOn.GetComponent<Object_Grabbable>();
        player.GetComponent<Player_Grab>().Grab(grab);
        state = 0;
        putOn = null;

        pickUp?.Invoke();
    }

    public void OnEnter()
    {
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

    public char GetKeyId()
    {
        string name = putOn.GetComponent<Object_Grabbable>().objectName;
        if (putOn != null) return name[name.Length - 1];
        else return '-';
    }

    IEnumerator AfterPutDown(float delay)
    {
        yield return new WaitForSeconds(delay);

        audio_player?.PlayAudio(audio_puton);

        putDown?.Invoke();
    }
}
