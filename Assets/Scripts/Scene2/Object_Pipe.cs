using UnityEngine;
using System.Text.RegularExpressions;

public class Object_Pipe : MonoBehaviour
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

    [Header("Cl Locked Item")]
    [SerializeField] Object_Cl_Locked obj;
    private int state = 0;
    public void OnInteract()
    {
        if (state == 0)
        {
            mesh.enabled = false;
            state = 1;
        }
        else if (state == 1)
        {
            if(player.GetComponent<Player_Grab>().hasKey(keyName + "_" + @"[0-9]"))
            {
                mesh.material = mat;
                mesh.enabled = true;
                state = 2;

                obj.SetCount();
                player.GetComponent<Player_Grab>().UseKey();
            }
        }
    }

    public void OnEnter()
    {
        if(state == 1)
        {
            mesh.material = mat_trans;
            mesh.enabled = true;
        }
    }

    public void OnExit()
    {
        if (state == 1) mesh.enabled = false;
    }
}
