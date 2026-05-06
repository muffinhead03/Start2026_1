using UnityEngine;

public static class GameData
{
    public static int CurrentSceneNumber = 0;

    public static int BackgroundVolume = 100;
    public static int ObjectSoundVolume = 100;

    private const string SavedSceneNumberKey = "SavedSceneNumber";
    private const string BackgroundVolumeKey = "BackgroundVolume";
    private const string ObjectSoundVolumeKey = "ObjectSoundVolume";

    public static void SaveCurrentScene(int sceneNumber)
    {
        CurrentSceneNumber = sceneNumber;

        PlayerPrefs.SetInt(SavedSceneNumberKey, CurrentSceneNumber);
        PlayerPrefs.Save();

        Debug.Log("Saved Scene Number: " + CurrentSceneNumber);
    }

    public static int LoadSavedSceneNumber()
    {
        CurrentSceneNumber = PlayerPrefs.GetInt(SavedSceneNumberKey, 1);

        Debug.Log("Loaded Scene Number: " + CurrentSceneNumber);

        return CurrentSceneNumber;
    }

    public static bool HasSavedScene()
    {
        return PlayerPrefs.HasKey(SavedSceneNumberKey);
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
    }

    public static void SaveAll()
    {
        PlayerPrefs.SetInt(SavedSceneNumberKey, CurrentSceneNumber);

        BackgroundVolume = Mathf.Clamp(BackgroundVolume, 0, 100);
        ObjectSoundVolume = Mathf.Clamp(ObjectSoundVolume, 0, 100);

        PlayerPrefs.SetInt(BackgroundVolumeKey, BackgroundVolume);
        PlayerPrefs.SetInt(ObjectSoundVolumeKey, ObjectSoundVolume);

        PlayerPrefs.Save();
    }

    public static void LoadAll()
    {
        CurrentSceneNumber = PlayerPrefs.GetInt(SavedSceneNumberKey, 1);
        BackgroundVolume = PlayerPrefs.GetInt(BackgroundVolumeKey, 100);
        ObjectSoundVolume = PlayerPrefs.GetInt(ObjectSoundVolumeKey, 100);
    }

    public static void ResetSaveData()
    {
        PlayerPrefs.DeleteKey(SavedSceneNumberKey);
        PlayerPrefs.DeleteKey(BackgroundVolumeKey);
        PlayerPrefs.DeleteKey(ObjectSoundVolumeKey);
        PlayerPrefs.Save();

        CurrentSceneNumber = 0;
        BackgroundVolume = 100;
        ObjectSoundVolume = 100;

        Debug.Log("Save data reset.");
    }
}