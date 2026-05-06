using UnityEngine;

public static class GameData
{
    public static int CurrentSceneNumber; // 현재 씬 번호

    public static int BackgroundVolume;
    public static int ObjectSoundVolume;

    public static void SaveAll()
    {
        PlayerPrefs.SetInt("SavedSceneNumber", CurrentSceneNumber);
        PlayerPrefs.SetInt("BackgroundVolume", BackgroundVolume);
        PlayerPrefs.SetInt("ObjectSoundVolume", ObjectSoundVolume);
        PlayerPrefs.Save();
    }

    public static void LoadAll()
    {
        CurrentSceneNumber = PlayerPrefs.GetInt("SavedSceneNumber", 1);
        BackgroundVolume = PlayerPrefs.GetInt("BackgroundVolume", 100);
        ObjectSoundVolume = PlayerPrefs.GetInt("ObjectSoundVolume", 100);
    }

    public static bool HasSavedScene()
    {
        return PlayerPrefs.HasKey("SavedSceneNumber");
    }
}