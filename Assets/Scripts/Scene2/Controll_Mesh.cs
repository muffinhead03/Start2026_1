using NUnit.Framework.Internal;
using UnityEngine;

public class Controll_Mesh: MonoBehaviour
{ 
    [SerializeField] private Texture2D[] textures;

    [SerializeField] private MeshRenderer meshRenderer;

    public void ShowMesh(string property_name, int id)
    {
        if (id >= textures.Length || id < 0) return;

        TextureChange.Instance.Clear();

        meshRenderer.GetPropertyBlock(TextureChange.Instance);

        TextureChange.Instance.SetTexture(property_name, textures[id]);

        meshRenderer.SetPropertyBlock(TextureChange.Instance);

        meshRenderer.enabled = true;
    }

    public void ShowMesh(int id)
    {
        if (id >= textures.Length || id < 0) return;

        TextureChange.Instance.Clear();

        meshRenderer.GetPropertyBlock(TextureChange.Instance);

        TextureChange.Instance.SetTexture("_BaseMap", textures[id]);

        meshRenderer.SetPropertyBlock(TextureChange.Instance);

        meshRenderer.enabled = true;
    }

    public void HideMesh()
    {
        meshRenderer.enabled = false;
    }
}
