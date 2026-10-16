using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int hintLevel = 1;
    public float mouseSensitivity = 0.1f;
    public float soundEffect = 1;
    public int language = 0;
    public int current_stage = 0;
}

public static class SaveSystem
{
    public static void SaveGame(PlayerData data)
    {
        string savePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);
    }

    public static PlayerData LoadGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            return CreateNewData();
        }
    }

    private static PlayerData CreateNewData()
    {
        return new PlayerData
        {
            hintLevel = 1,
            mouseSensitivity = 0.1f,
            soundEffect = 1,
            language = 0,
            current_stage = 2
        };
    }
}
