using UnityEditor;
using UnityEngine;

/// <summary>
/// DefaultSceneContextProvider에 있던 하드코딩 텍스트를 그대로 복사해서
/// SceneContextData 에셋 2개(wine_glass_room, organ_room)를 생성하는 1회용 도구.
/// Unity 메뉴에서 "HintSystem > Generate Scene Context Assets" 실행.
/// </summary>
public static class SceneContextDataGenerator
{
    const string OutputFolder = "Assets/Scripts/AI_test/SceneContexts";

    [MenuItem("HintSystem/Generate Scene Context Assets (1회용)")]
    public static void Generate()
    {
        if (!AssetDatabase.IsValidFolder(OutputFolder))
            AssetDatabase.CreateFolder("Assets/Scripts/AI_test", "SceneContexts");

        CreateAsset("wine_glass_room", new[]
        {
            "Scene: A room themed around wine, with wine stains and alphabet letters scattered across the floor, leading into a room with an old bookshelf.",
            "Look closely at the wine stains on the floor and the wine rack in this room.",
            "Each wine stain on the floor has a slightly different color, with two stains of each color, and an alphabet letter at the midpoint between each pair. The wine rack holds 4 numbered wine bottles — match each stain's color to the wine of the same color to get a number, and shift the alphabet letter forward by that number to find the true letter.",
            "The books on the shelf are ordered A-Z by the first letter of their titles. Pull out the 4 books matching the true letters, then arrange them in the same order as the wine bottles are arranged on the rack — this reveals a hidden key to escape."
        });

        CreateAsset("organ_room", new[]
        {
            "Scene: A large old mansion themed around a broken organ. Goal: repair the organ and perform the completed sheet music to reveal a hidden key.",
            "Phase 1: search the mansion for the organ's missing pipes. Phase 2: gather scattered pieces of torn sheet music. Phase 3: play the completed score on the organ.",
            "Pipes 1 and 2 are in the living room and organ room. Pipes 3 and 4 are behind a locked door — its passcode can be found from a music box melody in the living room, and a painting hints at which LP to play to reveal each pipe. Sheet music pieces are found in the living room, a metronome room, and a large room unlocked via a bookshelf hint.",
            "Match the metronome's rhythm to open a cabinet with sheet music piece 2, follow the cabinet's hint picture on the bookshelf to unlock the large room and find piece 3 in a photo frame and piece 4 in a trash can. Combine all pieces with the one on the organ, then press the keys in the order shown on the completed score to reveal the key."
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[SceneContextDataGenerator] 완료. {OutputFolder} 폴더에 에셋 2개 생성됨.");
    }

    static void CreateAsset(string puzzleId, string[] chunks)
    {
        string path = $"{OutputFolder}/{puzzleId}.asset";

        if (AssetDatabase.LoadAssetAtPath<SceneContextData>(path) != null)
        {
            Debug.LogWarning($"[SceneContextDataGenerator] 이미 존재해서 건너뜀: {path}");
            return;
        }

        var asset = ScriptableObject.CreateInstance<SceneContextData>();
        asset.puzzleId = puzzleId;
        asset.levelChunks = chunks;
        AssetDatabase.CreateAsset(asset, path);
    }
}