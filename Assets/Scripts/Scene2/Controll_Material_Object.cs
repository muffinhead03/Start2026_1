using UnityEngine;

public class Controll_Material_Object : MonoBehaviour
{
    [SerializeField] private Material[] materials;

    [SerializeField] private MeshRenderer meshRenderer;

    public void ShowMaterial(int id)
    {
        meshRenderer.material = materials[id];
    }
}
