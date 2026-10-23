using UnityEngine;

public class Controll_Material : MonoBehaviour
{
    [SerializeField] private Texture2D[] textures;

    [SerializeField] private Material material;

    public void SetTexture(int id)
    {
        material.SetTexture("_BaseMap", textures[id]);
    }

    public void SetTexture(string property_name, int id)
    {
        material.SetTexture(property_name, textures[id]);
    }
}
