using UnityEngine;

public class Object_LP : MonoBehaviour, IMouseInteractable
{
    MeshRenderer mesh;

    Material mat_original;

    [SerializeField] private Object_Turntable turntable;

    [SerializeField] private Material mat_trans;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    public void OnMouseClick()
    {
        turntable.ClickLP();
    }

    public void OnMouseReleased() { }

    public void OnMouseEnter()
    {
        mat_original = mesh.material;

        mesh.material = mat_trans;
    }

    public void OnMouseExit()
    {
        mesh.material = mat_original;
    }
}
