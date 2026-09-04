using System.Collections.Generic;

/// <summary>
/// SceneContextData 에셋 배열을 기반으로 씬 오리엔테이션을 조회하는 제공자.
/// 해당 puzzleId의 에셋이 없거나 비어있으면 fallback(기본값: DefaultSceneContextProvider)으로 위임한다.
/// levelChunks[0]을 오리엔테이션 텍스트로 사용한다(레벨 구분 없음 — 스텝별 세부 내용은 hintByLevel이 담당).
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

    public string GetSceneContext(string puzzleId)
    {
        if (!data.TryGetValue(puzzleId, out var chunks) || chunks == null || chunks.Length == 0)
            return fallback.GetSceneContext(puzzleId);   // 에셋 없으면 폴백으로 위임

        return chunks[0]; // 오리엔테이션 문구 하나만 사용
    }
}