using UnityEngine;
using UnityEngine.Events;

public class Object_Grabbable : MonoBehaviour
{
    [Header("내부 이름")]
    public string objectName;

    [Header("Localization")]
    [Tooltip("인벤토리 이름 Localization Key")]
    public string InventoryObjectName;

    [Tooltip("설명 Localization Key")]
    public string description;

    [Header("Player Character")]
    public GameObject player;

    [Header("사운드")]
    public AudioClip[] audio_grap;

    [Header("Grab Event")]
    [Tooltip("손에서 이 오브젝트를 놓았을 때 실행")]
    public UnityEvent OnReleased;

    private Play_Audio audio_player;


    private void Start()
    {
        audio_player =
            GetComponent<Play_Audio>();
    }


    /// 인벤토리 이름의 Localization Key 반환
    public string GetInventoryNameKey()
    {
        return InventoryObjectName;
    }


    /// 현재 언어 기준 인벤토리 표시 이름 반환
    public string GetInventoryDisplayName()
    {
        return LanguageData.Get(InventoryObjectName);
    }


    /// 설명 Localization Key 반환
    public string GetDescriptionKey()
    {
        return description;
    }


    /// 현재 언어 기준 설명 반환
    public string GetDisplayDescription()
    {
        return LanguageData.Get(description);
    }

    public void OnGrab()
    {
        player.GetComponent<Player_Grab>()
            .Grab(this);

        if (audio_player != null &&
            audio_grap != null &&
            audio_grap.Length > 0)
        {
            int random_id =
                Random.Range(
                    0,
                    audio_grap.Length
                );

            audio_player.PlayAudio(
                audio_grap[random_id]
            );
        }
    }

    public void OnRelease()
    {
        OnReleased?.Invoke();
    }
}