using Unity.VisualScripting;
using UnityEngine;

public class Object_LP : MonoBehaviour, IMouseInteractable
{
    MeshRenderer mesh;
    Material mat_original;

    [SerializeField] private Controll_Mesh controll_mesh;

    [SerializeField] private Object_Turntable turntable;

    [SerializeField] private Object_LP_Needle needle;

    [SerializeField] private Material mat_trans;

    [SerializeField] private Pwd_Turntable pwd_LP;

    private void Start()
    {
        if(mesh == null) mesh = GetComponent<MeshRenderer>();
    }

    public void OnMouseClick()
    {
        turntable.ResetPanel();

        HideLP();

        pwd_LP.PauseMusic();
    }

    public void OnMouseReleased() { }

    public void OnMouseEnter()
    {
        mat_original = mesh.sharedMaterial;

        mesh.sharedMaterial = mat_trans;
    }

    public void OnMouseExit()
    {
        mesh.sharedMaterial = mat_original;
    }

    public void ShowLP(int id)
    {
        if (mesh == null) mesh = GetComponent<MeshRenderer>();

        controll_mesh.ShowMesh(id);

        gameObject.SetActive(true);

        needle.SetActive(true);
    }

    public void HideLP()
    {
        if(mat_original!=null) mesh.sharedMaterial = mat_original;

        gameObject.SetActive(false);

        needle.SetActive(false);
    }
}
