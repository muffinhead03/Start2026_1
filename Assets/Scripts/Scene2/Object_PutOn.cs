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

    int state;
    GameObject putOn;

    void Start()
    {
        state = 0;
    }

    public void OnInteract()
    {
        if (state == 0 && player.GetComponent<Player_Grab>().hasKey(keyName + "_" + @"[0-9]"))
        {
            mesh.enabled = false;
            state = 1;
            putOn = player.GetComponent<Player_Grab>().PutOn(transform.position);
        }
        else if(state == 1 && !player.GetComponent<Player_Grab>().isGrab())
        {
            player.GetComponent<Player_Grab>().Grab(putOn);
            state = 0;
        }
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
        if (state == 0) mesh.enabled = false;
    }
}
