using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/*
 * JapaneseFontFallbackSetup
 *
 * 메뉴: Tools/Localization/Setup Japanese Font Fallback
 *
 * 1) NotoSansJP ttf로 Dynamic TMP 폰트 에셋(SDF)을 만든다. (이미 있으면 재사용)
 * 2) 한국어 TMP 폰트 에셋의 Fallback 목록에 추가한다.
 *
 * → 텍스트 컴포넌트의 폰트는 그대로 두고,
 *   한국어 폰트에 없는 일본어 글자(ひらがな, 漢字 등)만 NotoSansJP로 그려진다.
 *
 * 다른 UI에도 적용하려면 Pairs에 (한국어 폰트 에셋, 같은 굵기의 NotoSansJP ttf)를 추가하면 된다.
 */
public static class JapaneseFontFallbackSetup
{
    private const string JapaneseFontFolder = "Assets/FontCollector";

    // (일본어를 보완할 기존 폰트 에셋, 같은 굵기의 NotoSansJP ttf)
    private static readonly (string targetFontAsset, string japaneseTtf)[] Pairs =
    {
        // 인벤토리 UI (ItemCanvas.prefab - 아이템 이름, ESC 안내, Inventory 제목)
        (
            "Assets/TextMesh Pro/Fonts/goorm-sans-regular SDF.asset",
            "Assets/FontCollector/NotoSansJP-Regular.ttf"
        ),

        // 설정 패널 (Setting.prefab - Language 표시 "日本語" 등)
        (
            "Assets/TextMesh Pro/Fonts/HSHwalkongSerif-Regular SDF.asset",
            "Assets/FontCollector/NotoSansJP-Regular.ttf"
        ),
    };

    [MenuItem("Tools/Localization/Setup Japanese Font Fallback")]
    public static void Setup()
    {
        var log = new List<string>();

        foreach (var (targetPath, ttfPath) in Pairs)
        {
            TMP_FontAsset target = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(targetPath);
            if (target == null)
            {
                Debug.LogError($"[JP Font] 대상 폰트 에셋을 찾을 수 없음: {targetPath}");
                continue;
            }

            TMP_FontAsset japanese = GetOrCreateDynamicFontAsset(ttfPath);
            if (japanese == null)
            {
                continue;
            }

            target.fallbackFontAssetTable ??= new List<TMP_FontAsset>();

            if (target.fallbackFontAssetTable.Contains(japanese))
            {
                log.Add($"{target.name}: 이미 {japanese.name} Fallback 있음");
                continue;
            }

            Undo.RecordObject(target, "Add Japanese Font Fallback");
            target.fallbackFontAssetTable.Add(japanese);
            EditorUtility.SetDirty(target);

            log.Add($"{target.name} ← Fallback 추가: {japanese.name}");
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[JP Font] 완료\n" + string.Join("\n", log));
    }

    private static TMP_FontAsset GetOrCreateDynamicFontAsset(string ttfPath)
    {
        string fontName = System.IO.Path.GetFileNameWithoutExtension(ttfPath) + " SDF";
        string assetPath = $"{JapaneseFontFolder}/{fontName}.asset";

        TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
        if (existing != null)
        {
            return existing;
        }

        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
        if (sourceFont == null)
        {
            Debug.LogError($"[JP Font] ttf를 찾을 수 없음: {ttfPath}");
            return null;
        }

        // Dynamic: 필요한 글자만 실행 중에 아틀라스에 추가 (일본어 전체를 굽지 않아 용량이 작음)
        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceFont,
            90,                    // sampling point size
            9,                     // padding
            GlyphRenderMode.SDFAA,
            1024,
            1024,
            AtlasPopulationMode.Dynamic,
            true                   // multi atlas
        );

        if (fontAsset == null)
        {
            Debug.LogError($"[JP Font] 폰트 에셋 생성 실패: {ttfPath}");
            return null;
        }

        fontAsset.name = fontName;
        AssetDatabase.CreateAsset(fontAsset, assetPath);

        Texture2D atlas = fontAsset.atlasTextures[0];
        atlas.name = fontName + " Atlas";
        AssetDatabase.AddObjectToAsset(atlas, fontAsset);

        Material material = fontAsset.material;
        material.name = fontName + " Material";
        AssetDatabase.AddObjectToAsset(material, fontAsset);

        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(assetPath);

        Debug.Log($"[JP Font] 생성: {assetPath}");
        return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
    }
}
