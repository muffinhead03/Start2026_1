using UnityEngine;

public class Controll_Material : MonoBehaviour
{
    [Header("Textures (Korean = default)")]
    [SerializeField] private Texture2D[] textures;

    [Header("Localized Textures (empty slot -> Korean)")]
    [SerializeField] private Texture2D[] textures_English;
    [SerializeField] private Texture2D[] textures_Japanese;

    [SerializeField] private Material material;

    [Header("Start")]
    [Tooltip("Texture id applied on Start (-1 = do nothing)")]
    [SerializeField] private int initialTextureId = 0;

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
            SetTexture(initialTextureId);
        }
    }

    public void SetTexture(int id)
    {
        SetTexture("_BaseMap", id);
    }

    public void SetTexture(string property_name, int id)
    {
        Texture2D texture = GetTexture(id);
        if (texture == null || material == null) return;

        lastProperty = property_name;
        lastId = id;

        material.SetTexture(property_name, texture);
    }

    private void OnLanguageChanged(LanguageType language)
    {
        if (lastId >= 0)
        {
            SetTexture(lastProperty, lastId);
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
