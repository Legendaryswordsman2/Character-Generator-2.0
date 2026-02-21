using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class CharacterPiecesPostBuild : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string sourceDir = Path.Combine(Directory.GetCurrentDirectory(), CharacterPieceDatabase.CharacterPiecesFolderName);
        if (!Directory.Exists(sourceDir))
        {
            Debug.LogWarning($"[CharacterPiecesPostBuild] Source folder not found, skipping copy: {sourceDir}");
            return;
        }

        string buildPath = report.summary.outputPath;
        if (string.IsNullOrWhiteSpace(buildPath))
        {
            return;
        }

        // Copy next to the built player (works for Windows/Linux and macOS parent folder lookup).
        string buildFolder = Directory.Exists(buildPath)
            ? buildPath
            : Path.GetDirectoryName(buildPath);

        if (!string.IsNullOrWhiteSpace(buildFolder))
        {
            string targetDir = Path.Combine(buildFolder, CharacterPieceDatabase.CharacterPiecesFolderName);
            CopyDirectoryRecursive(sourceDir, targetDir);
            Debug.Log($"[CharacterPiecesPostBuild] Copied Character Pieces to: {targetDir}");
        }

        // Also copy inside StreamingAssets for macOS app bundles.
        if (report.summary.platform == BuildTarget.StandaloneOSX && buildPath.EndsWith(".app"))
        {
            string macStreamingAssets = Path.Combine(buildPath, "Contents", "Resources", "Data", "StreamingAssets", CharacterPieceDatabase.CharacterPiecesFolderName);
            CopyDirectoryRecursive(sourceDir, macStreamingAssets);
            Debug.Log($"[CharacterPiecesPostBuild] Copied Character Pieces to: {macStreamingAssets}");
        }
    }

    private static void CopyDirectoryRecursive(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            string relative = Path.GetRelativePath(sourceDir, dir);
            Directory.CreateDirectory(Path.Combine(targetDir, relative));
        }

        foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            if (file.EndsWith(".meta"))
            {
                continue;
            }

            string relative = Path.GetRelativePath(sourceDir, file);
            string destFile = Path.Combine(targetDir, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destFile) ?? targetDir);
            File.Copy(file, destFile, true);
        }
    }
}
