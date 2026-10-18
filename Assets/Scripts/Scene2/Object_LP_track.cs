using UnityEngine;

public class Object_LP_track : MonoBehaviour
{
    [SerializeField] private Texture2D[] textures;

    [SerializeField] private MeshRenderer meshRenderer;

    public void ShowTrack(int id)
    {
        if (id >= textures.Length || id < 0)
        {
            HideTrack();
            return;
        }

        TextureChange.Instance.Clear();

        meshRenderer.GetPropertyBlock(TextureChange.Instance);

        TextureChange.Instance.SetTexture("_MainTex", textures[id]);

        meshRenderer.SetPropertyBlock(TextureChange.Instance);

        meshRenderer.enabled = true;
    }

    public void HideTrack()
    {
        meshRenderer.enabled = false;
    }
}
