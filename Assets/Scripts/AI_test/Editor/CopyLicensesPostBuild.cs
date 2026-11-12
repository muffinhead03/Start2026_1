using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// 빌드가 끝나면 프로젝트 루트의 Licenses 폴더(NOTICE.txt, 약관, 라이선스 원문)를
/// 빌드 결과물 옆에 Licenses 폴더로 복사함.
///
/// - Gemma 이용약관 3.1항: 재배포 시 NOTICE 파일 + 약관 사본을 같이 줘야 함
/// - LLMUnity / LlamaLib (Apache 2.0), llama.cpp (MIT): 라이선스 사본 + 저작권 고지를 같이 줘야 함
///
/// Licenses 폴더는 Assets 밖(프로젝트 루트)에 둬서 Unity가 에셋으로 가져오지 않게 함.
/// 위치: Assets/Editor/ (Editor 폴더 안에 있어야 빌드에 포함되지 않음)
/// </summary>
public class CopyLicensesPostBuild : IPostprocessBuildWithReport
{
    const string SourceFolderName = "Licenses";

    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string source = Path.Combine(projectRoot, SourceFolderName);

        if (!Directory.Exists(source))
        {
            Debug.LogError($"[CopyLicensesPostBuild] {source} 폴더가 없음 — 라이선스 파일이 빌드에 안 들어감 (스팀 배포 전 필수)");
            return;
        }

        // Windows: outputPath = ...\Game.exe → 그 폴더
        // macOS:   outputPath = .../Game.app → .app 옆 폴더
        string outputPath = report.summary.outputPath;
        string buildDir = Directory.Exists(outputPath) && !outputPath.EndsWith(".app")
            ? outputPath
            : Path.GetDirectoryName(outputPath);

        string target = Path.Combine(buildDir, SourceFolderName);
        Directory.CreateDirectory(target);

        int count = 0;
        foreach (string file in Directory.GetFiles(source))
        {
            if (file.EndsWith(".meta")) continue;
            File.Copy(file, Path.Combine(target, Path.GetFileName(file)), overwrite: true);
            count++;
        }

        string terms = Path.Combine(source, "Gemma_Terms_of_Use.txt");
        if (!File.Exists(terms) || File.ReadAllText(terms).Contains("[TODO"))
            Debug.LogWarning("[CopyLicensesPostBuild] Gemma_Terms_of_Use.txt에 약관 원문이 아직 안 들어 있음 (TODO 남아 있음)");

        Debug.Log($"[CopyLicensesPostBuild] 라이선스 파일 {count}개 복사 → {target}");
    }
}
