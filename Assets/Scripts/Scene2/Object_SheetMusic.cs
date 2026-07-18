using UnityEngine;

public class Object_SheetMusic : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("새로운 머터리얼")]
    [SerializeField] Material mat;

    [Header("투명 머터리얼")]
    [SerializeField] Material mat_trans;

    [Header("메쉬 렌더러")]
    [SerializeField] MeshRenderer mesh;

    [Header("열쇠 이름")]
    [SerializeField] string keyName;

    int state;

    public void OnInteract()
    {
        if (player.GetComponent<Player_Grab>().hasKey(keyName + "_" + @"[0-9]"))
        {
            mesh.material = mat;
            mesh.enabled = true;
            player.GetComponent<Player_Grab>().UseKey();
        }
    }

    public void OnEnter()
    {
        if (player.GetComponent<Player_Grab>().hasKey(keyName + "_" + @"[0-9]"))
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
