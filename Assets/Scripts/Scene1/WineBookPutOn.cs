using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WineBookPutOn : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("책장 (애니메이션되는 오브젝트) - 배치된 책을 여기 자식으로 붙임")]
    [SerializeField] Transform shelfParent;

    [Header("투명 머터리얼 (책을 들고 바라볼 때)")]
    [SerializeField] Material mat_trans;

    [Header("평소 자리 표시 머터리얼 (비우면 지금 슬롯에 들어있는 머터리얼 사용)")]
    [SerializeField] Material mat_idle;

    [Header("메쉬 렌더러")]
    [SerializeField] MeshRenderer mesh;

    [Header("정답 알파벳 (예: book_S, 코드로 자동 세팅됨)")]
    [SerializeField] string keyName;

    [Header("이벤트")]
    public UnityEvent putDown;
    public UnityEvent pickUp;

    [Header("힌트 매니저 연결")]
    [SerializeField] HintManager hintManager;

    int state;
    GameObject putOn;

    void Start()
    {
        state = 0;
        putOn = null;

        // mat_idle을 비워두면 지금 슬롯 머터리얼(하얀 책 모양)을 평소 표시로 사용
        if (mat_idle == null)
            mat_idle = mesh.sharedMaterial;

        ShowIdle();
    }

    public void SetKeyName(string name)
    {
        keyName = name;
    }

    public void OnInteract()
    {
        var grab = player.GetComponent<Player_Grab>();

        if (state == 0 && grab.isGrab())
        {
            bool isCorrect = grab.hasKey(keyName);
            PutOn(isCorrect);
        }
        else if (state == 1 && !grab.isGrab())
        {
            TakeOff();
        }
    }

    void PutOn(bool isCorrect)
    {
        mesh.enabled = false;
        state = 1;
        putOn = player.GetComponent<Player_Grab>().PutOn(transform.position);

        // 책장이 슬라이드될 때 같이 움직이도록 다시 부모 설정
        if (putOn != null && shelfParent != null)
            putOn.transform.SetParent(shelfParent, true);

        if (!isCorrect)
            hintManager?.RegisterFail();

        StartCoroutine(AfterPutDown(0.5f));
    }

    void TakeOff()
    {
        Object_Grabbable grab = putOn.GetComponent<Object_Grabbable>();
        player.GetComponent<Player_Grab>().Grab(grab);
        state = 0;
        putOn = null;
        ShowIdle();

        pickUp?.Invoke();
    }

    public void OnEnter()
    {
        var grab = player.GetComponent<Player_Grab>();
        if (state == 0 && grab.isGrab())
        {
            mesh.material = mat_trans;
            mesh.enabled = true;
        }
    }

    public void OnExit()
    {
        if (state == 0) ShowIdle();
        else mesh.enabled = false;
    }

    // 비어 있는 슬롯 표시 (하얀 책 모양)
    void ShowIdle()
    {
        if (mat_idle != null)
        {
            mesh.material = mat_idle;
            mesh.enabled = true;
        }
        else
        {
            mesh.enabled = false;
        }
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