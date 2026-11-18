using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int hintLevel = 1;
    public float mouseSensitivity = 0.1f;
    public float soundEffect = 1;
    public int language = 1; // 0: English, 1: Korean, 2: Japanese
    public int current_stage = 0;
}

public static class SaveSystem
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "savefile.json");

    public static void SaveGame(PlayerData data)
    {
        // null을 저장하면 JsonUtility.ToJson이 ""를 돌려줘서 빈 세이브 파일이 생김
        // → 다음 실행 때 data가 null이 되어 마우스 시점 등이 멈춤. 저장하지 않는다.
        if (data == null)
        {
            Debug.LogWarning("[SaveSystem] 저장할 데이터가 null이라 저장을 건너뜁니다.");
            return;
        }

        string json = JsonUtility.ToJson(data, true);

        try
        {
            // 임시 파일에 먼저 쓰고 교체 → 쓰는 도중 종료돼도 기존 세이브가 깨지지 않음
            string tempPath = SavePath + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }

            File.Move(tempPath, SavePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[SaveSystem] 저장 실패: " + e.Message);
        }
    }

    public static PlayerData LoadGame()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    PlayerData data = JsonUtility.FromJson<PlayerData>(json);

                    if (data != null)
                    {
                        return data;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[SaveSystem] 세이브 파일 읽기 실패: " + e.Message);
            }

            // 비어 있거나 깨진 세이브 파일 → 기본값으로 다시 시작
            Debug.LogWarning("[SaveSystem] 세이브 파일이 비어 있거나 손상되어 기본값으로 초기화합니다.");
        }

        return CreateNewData();
    }

    private static PlayerData CreateNewData()
    {
        return new PlayerData
        {
            hintLevel = 1,
            mouseSensitivity = 0.1f,
            soundEffect = 1,
            language = 1,
            current_stage = 2
        };
    }
}
