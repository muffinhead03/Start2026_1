using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WineBookPutOn : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("책장 (애니메이션되는 오브젝트) - 배치된 책을 여기 자식으로 붙임")]
    [SerializeField] Transform shelfParent;

    [Header("투명 머터리얼")]
    [SerializeField] Material mat_trans;

    [Header("메쉬 렌더러")]
    [SerializeField] MeshRenderer mesh;

    [Header("정답 알파벳 (예: book_S, 코드로 자동 세팅됨)")]
    [SerializeField] string keyName;

    [Header("이벤트")]
    public UnityEvent putDown;
    public UnityEvent pickUp;

    int state;
    GameObject putOn;

    void Start()
    {
        state = 0;
        putOn = null;
    }

    public void SetKeyName(string name)
    {
        keyName = name;
    }

    public void OnInteract()
    {
        if (state == 0 && player.GetComponent<Player_Grab>().hasKey(keyName))
            PutOn();
        else if (state == 1 && !player.GetComponent<Player_Grab>().isGrab())
            TakeOff();
    }

    void PutOn()
    {
        mesh.enabled = false;
        state = 1;
        putOn = player.GetComponent<Player_Grab>().PutOn(transform.position);

        // 추가: 책장이 슬라이드될 때 같이 움직이도록 다시 부모 설정
        // worldPositionStays=true 라서 지금 위치 그대로 유지되면서 부모만 바뀜
        if (putOn != null && shelfParent != null)
            putOn.transform.SetParent(shelfParent, true);

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
        if (state == 0 && player.GetComponent<Player_Grab>().hasKey(keyName))
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
        if (putOn != null)
        {
            string name = putOn.GetComponent<Object_Grabbable>().objectName;
            return name[name.Length - 1];
        }
        return '-';
    }

    IEnumerator AfterPutDown(float delay)
    {
        yield return new WaitForSeconds(delay);
        putDown?.Invoke();
    }
}