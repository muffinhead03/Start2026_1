using UnityEngine;

public static class GameData
{
    public static int CurrentSceneNumber = 0;
    public static string CurrentSceneName = "";

    public static int BackgroundVolume = 100;
    public static int ObjectSoundVolume = 100;

    public static readonly string[] LanguageType = { "Korean", "English" };
    public static int CurrentLanguageIndex = 0;
    public static string CurrentLanguage = "Korean";

    private const string SavedSceneNumberKey = "SavedSceneNumber";
    private const string SavedSceneNameKey = "SavedSceneName";
    private const string BackgroundVolumeKey = "BackgroundVolume";
    private const string ObjectSoundVolumeKey = "ObjectSoundVolume";
    private const string CurrentLanguageIndexKey = "CurrentLanguageIndex";

    public static void SaveCurrentScene(int sceneNumber, string sceneName)
    {
        CurrentSceneNumber = sceneNumber;
        CurrentSceneName = sceneName;

        PlayerPrefs.SetInt(SavedSceneNumberKey, CurrentSceneNumber);
        PlayerPrefs.SetString(SavedSceneNameKey, CurrentSceneName);
        PlayerPrefs.Save();

        Debug.Log("Saved Scene Number: " + CurrentSceneNumber);
        Debug.Log("Saved Scene Name: " + CurrentSceneName);
    }

    public static int LoadSavedSceneNumber()
    {
        CurrentSceneNumber = PlayerPrefs.GetInt(SavedSceneNumberKey, 1);

        Debug.Log("Loaded Scene Number: " + CurrentSceneNumber);

        return CurrentSceneNumber;
    }

    public static string LoadSavedSceneName()
    {
        CurrentSceneName = PlayerPrefs.GetString(SavedSceneNameKey, "Tutorial");

        Debug.Log("Loaded Scene Name: " + CurrentSceneName);

        return CurrentSceneName;
    }

    public static bool HasSavedScene()
    {
        return PlayerPrefs.HasKey(SavedSceneNameKey);
    }

    public static void SaveVolumes()
    {
        BackgroundVolume = Mathf.Clamp(BackgroundVolume, 0, 100);
        ObjectSoundVolume = Mathf.Clamp(ObjectSoundVolume, 0, 100);

        PlayerPrefs.SetInt(BackgroundVolumeKey, BackgroundVolume);
        PlayerPrefs.SetInt(ObjectSoundVolumeKey, ObjectSoundVolume);
        PlayerPrefs.Save();
    }

    public static void LoadVolumes()
    {
        BackgroundVolume = PlayerPrefs.GetInt(BackgroundVolumeKey, 100);
        ObjectSoundVolume = PlayerPrefs.GetInt(ObjectSoundVolumeKey, 100);

        BackgroundVolume = Mathf.Clamp(BackgroundVolume, 0, 100);
        ObjectSoundVolume = Mathf.Clamp(ObjectSoundVolume, 0, 100);
    }

    public static void MoveLanguageIndex(int amount)
    {
        CurrentLanguageIndex += amount;

        if (CurrentLanguageIndex < 0)
        {
            CurrentLanguageIndex = LanguageType.Length - 1;
        }
        else if (CurrentLanguageIndex >= LanguageType.Length)
        {
            CurrentLanguageIndex = 0;
        }

        CurrentLanguage = LanguageType[CurrentLanguageIndex];

        PlayerPrefs.SetInt(CurrentLanguageIndexKey, CurrentLanguageIndex);
        PlayerPrefs.Save();

        Debug.Log("Current Language Index: " + CurrentLanguageIndex);
        Debug.Log("Current Language: " + CurrentLanguage);
    }

    public static void SaveLanguage()
    {
        FixLanguageIndex();

        PlayerPrefs.SetInt(CurrentLanguageIndexKey, CurrentLanguageIndex);
        PlayerPrefs.Save();
    }

    public static void LoadLanguage()
    {
        CurrentLanguageIndex = PlayerPrefs.GetInt(CurrentLanguageIndexKey, 0);
        FixLanguageIndex();
    }

    private static void FixLanguageIndex()
    {
        if (LanguageType == null || LanguageType.Length == 0)
        {
            CurrentLanguageIndex = 0;
            CurrentLanguage = "";
            return;
        }

        if (CurrentLanguageIndex < 0)
        {
            CurrentLanguageIndex = LanguageType.Length - 1;
        }
        else if (CurrentLanguageIndex >= LanguageType.Length)
        {
            CurrentLanguageIndex = 0;
        }

        CurrentLanguage = LanguageType[CurrentLanguageIndex];
    }

    public static void SaveAll()
    {
        PlayerPrefs.SetInt(SavedSceneNumberKey, CurrentSceneNumber);
        PlayerPrefs.SetString(SavedSceneNameKey, CurrentSceneName);

        BackgroundVolume = Mathf.Clamp(BackgroundVolume, 0, 100);
        ObjectSoundVolume = Mathf.Clamp(ObjectSoundVolume, 0, 100);

        PlayerPrefs.SetInt(BackgroundVolumeKey, BackgroundVolume);
        PlayerPrefs.SetInt(ObjectSoundVolumeKey, ObjectSoundVolume);

        FixLanguageIndex();
        PlayerPrefs.SetInt(CurrentLanguageIndexKey, CurrentLanguageIndex);

        PlayerPrefs.Save();
    }

    public static void LoadAll()
    {
        CurrentSceneNumber = PlayerPrefs.GetInt(SavedSceneNumberKey, 1);
        CurrentSceneName = PlayerPrefs.GetString(SavedSceneNameKey, "Tutorial");

        BackgroundVolume = PlayerPrefs.GetInt(BackgroundVolumeKey, 100);
        ObjectSoundVolume = PlayerPrefs.GetInt(ObjectSoundVolumeKey, 100);

        BackgroundVolume = Mathf.Clamp(BackgroundVolume, 0, 100);
        ObjectSoundVolume = Mathf.Clamp(ObjectSoundVolume, 0, 100);

        CurrentLanguageIndex = PlayerPrefs.GetInt(CurrentLanguageIndexKey, 0);
        FixLanguageIndex();
    }

    public static void ResetSaveData()
    {
        PlayerPrefs.DeleteKey(SavedSceneNumberKey);
        PlayerPrefs.DeleteKey(SavedSceneNameKey);
        PlayerPrefs.DeleteKey(BackgroundVolumeKey);
        PlayerPrefs.DeleteKey(ObjectSoundVolumeKey);
        PlayerPrefs.DeleteKey(CurrentLanguageIndexKey);

        PlayerPrefs.Save();

        CurrentSceneNumber = 0;
        CurrentSceneName = "";

        BackgroundVolume = 100;
        ObjectSoundVolume = 100;

        CurrentLanguageIndex = 0;
        CurrentLanguage = LanguageType[CurrentLanguageIndex];

        Debug.Log("Save data reset.");
    }
}