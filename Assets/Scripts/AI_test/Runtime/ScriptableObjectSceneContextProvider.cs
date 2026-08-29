using System.Collections.Generic;

/// <summary>
/// SceneContextData 에셋 배열을 기반으로 씬 컨텍스트를 조회하는 제공자.
/// 해당 puzzleId의 에셋이 없거나 비어있으면 fallback(기본값: DefaultSceneContextProvider)으로 위임한다.
/// → 에셋 연결을 깜빡해도 기존 방식으로 자동 동작해서 게임이 멈추지 않음.
/// </summary>
public class ScriptableObjectSceneContextProvider : ISceneContextProvider
{
    readonly Dictionary<string, string[]> data = new Dictionary<string, string[]>();
    readonly ISceneContextProvider fallback;

    public ScriptableObjectSceneContextProvider(SceneContextData[] assets, ISceneContextProvider fallback = null)
    {
        this.fallback = fallback ?? new DefaultSceneContextProvider();

        if (assets == null) return;

        foreach (var asset in assets)
        {
            if (asset == null || string.IsNullOrEmpty(asset.puzzleId)) continue;
            data[asset.puzzleId] = asset.levelChunks;
        }
    }

    public string GetSceneContext(string puzzleId, int hintLevel)
    {
        if (!data.TryGetValue(puzzleId, out var chunks) || chunks == null || chunks.Length == 0)
            return fallback.GetSceneContext(puzzleId, hintLevel);   // 에셋 없으면 폴백으로 위임

        int level = hintLevel < 1 ? 1 : (hintLevel > chunks.Length ? chunks.Length : hintLevel);
        return string.Join(" ", chunks, 0, level);
    }
}