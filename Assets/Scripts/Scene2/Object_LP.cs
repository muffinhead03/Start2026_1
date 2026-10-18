using Unity.VisualScripting;
using UnityEngine;

public class Object_LP : MonoBehaviour, IMouseInteractable
{
    MeshRenderer mesh;
    Material mat_original;

    [SerializeField] private Texture2D[] textures;

    [SerializeField] private Object_Turntable turntable;

    [SerializeField] private Object_LP_Needle needle;

    [SerializeField] private Material mat_trans;

    private void Start()
    {
        if(mesh == null) mesh = GetComponent<MeshRenderer>();
    }

    public void OnMouseClick()
    {
        turntable.ResetPanel();

        HideLP();
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

    public void ShowLP(int id)
    {
        if (mesh == null) mesh = GetComponent<MeshRenderer>();

        TextureChange.Instance.Clear();

        mesh.GetPropertyBlock(TextureChange.Instance);

        TextureChange.Instance.SetTexture("_BaseMap", textures[id]);

        mesh.SetPropertyBlock(TextureChange.Instance);

        gameObject.SetActive(true);

        needle.SetActive(true);
    }

    public void HideLP()
    {
        gameObject.SetActive(false);

        needle.SetActive(false);
    }
}
