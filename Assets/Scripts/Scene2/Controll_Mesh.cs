using UnityEngine;

public class Controll_Mesh: MonoBehaviour
{
    [Header("Textures (Korean = default)")]
    [SerializeField] private Texture2D[] textures;

    [ColorUsage (true,true)]
    [SerializeField] private Color[] emission_colors;

    [SerializeField] private float[] emission_intensity;

    [Header("Localized Textures (empty slot -> Korean)")]
    [SerializeField] private Texture2D[] textures_English;
    [SerializeField] private Texture2D[] textures_Japanese;

    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Start")]
    [Tooltip("Texture id shown on Start (-1 = do nothing, keeps old behaviour)")]
    [SerializeField] private int initialTextureId = -1;
    [SerializeField] private string initialProperty = "_BaseMap";

    private string lastProperty = "_BaseMap";
    private int lastId = -1;

    private void OnEnable()
    {
        LanguageCurrentStatus.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        LanguageCurrentStatus.OnLanguageChanged -= OnLanguageChanged;
    }

    private void Start()
    {
        if (initialTextureId >= 0 && lastId < 0)
        {
            ShowMesh(initialProperty, initialTextureId);
        }
    }

    public void ShowMesh(string property_name, int id)
    {
        Texture2D texture = GetTexture(id);
        if (texture == null) return;

        lastProperty = property_name;
        lastId = id;

        TextureChange.Instance.Clear();

        meshRenderer.GetPropertyBlock(TextureChange.Instance);

        TextureChange.Instance.SetTexture(property_name, texture);

        meshRenderer.SetPropertyBlock(TextureChange.Instance);

        meshRenderer.enabled = true;
    }

    public void SetEmission(int id)
    {
        if (id >= textures.Length || id < 0) return;

        TextureChange.Instance.Clear();

        meshRenderer.GetPropertyBlock(TextureChange.Instance);

        TextureChange.Instance.SetColor("_EmissionColor", emission_colors[id] * emission_intensity[id] );

        meshRenderer.SetPropertyBlock(TextureChange.Instance);
    }

    public void ShowMesh(int id)
    {
        ShowMesh("_BaseMap", id);
    }

    public void HideMesh()
    {
        meshRenderer.enabled = false;
    }

    private void OnLanguageChanged(LanguageType language)
    {
        // Only refresh what is currently visible
        if (lastId >= 0 && meshRenderer != null && meshRenderer.enabled)
        {
            ShowMesh(lastProperty, lastId);
        }
    }

    // Current language texture; falls back to Korean when missing
    private Texture2D GetTexture(int id)
    {
        Texture2D[] localized = LanguageCurrentStatus.CurrentLanguage switch
        {
            LanguageType.English => textures_English,
            LanguageType.Japanese => textures_Japanese,
            _ => null
        };

        if (localized != null && id >= 0 && id < localized.Length && localized[id] != null)
        {
            return localized[id];
        }

        if (textures != null && id >= 0 && id < textures.Length)
        {
            return textures[id];
        }

        return null;
    }
}
