using System.Collections;
using Unity.VisualScripting;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.Events;

public class Object_PutOn_ver2 : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("투명 머터리얼")]
    [SerializeField] Material mat_trans;

    [Header("메쉬 렌더러")]
    [SerializeField] MeshRenderer[] meshes;

    [Header("열쇠 이름")]
    [SerializeField] string[] keyNames;

    [Header("사운드")]
    public AudioClip audio_puton;

    [Header("이벤트")]
    public UnityEvent OnComplete;

    private Play_Audio audio_player;

    int count;
    int total;
    public GameObject putOn;

    void Start()
    {
        count = 0;
        total = meshes.Length;

        putOn = null;

        audio_player = GetComponent<Play_Audio>();
    }

    public void OnInteract()
    {
        if (count<total)
        {
            for (int i = 0; i < keyNames.Length; i++)
            {
                if (player.GetComponent<Player_Grab>().hasKey(keyNames[i])){
                    PutOn();
                    break;
                }
            }
        }
    }

    void PutOn()
    {
        for(int i=0;i<meshes.Length;i++) meshes[i].enabled = false;
        count++;
        putOn = player.GetComponent<Player_Grab>().PutOn(transform.position);

        StartCoroutine(AfterPutDown(0.5f));
    }

    public void OnEnter()
    {
        if (count<total)
        {
            for(int i=0;i<keyNames.Length;i++)
            {
                if (player.GetComponent<Player_Grab>().hasKey(keyNames[i])){
                    meshes[i].material = mat_trans;
                    meshes[i].enabled = true;
                    break;
                }
            }
        }
    }

    public void OnExit()
    {
        for(int i=0;i<meshes.Length;i++) meshes[i].enabled = false;
    }

    IEnumerator AfterPutDown(float delay)
    {
        yield return new WaitForSeconds(delay);

        audio_player?.PlayAudio(audio_puton);

        if (count == total) OnComplete?.Invoke();
    }
}
