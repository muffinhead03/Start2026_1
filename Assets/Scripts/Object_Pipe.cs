using UnityEngine;

public class Object_Pipe : MonoBehaviour
{
    [Header("새로운 머터리얼")]
    [SerializeField] Material mat;

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
            gameObject.SetActive(false);
            state = 1;
        }
        else if (state == 1)
        {
            // 플레이어에게 열쇠가 있는지 확인
            if (Player_Inventory.hasKey(keyName))
            {
                mesh.material = mat;
                gameObject.SetActive(true);
                state = 2;

                obj.SetCount();
            }
            
        }
    }
}
