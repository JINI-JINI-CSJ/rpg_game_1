using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

public class BuildReportAnalyzer : EditorWindow
{
    [System.Serializable]
    public class BuildAssetInfo
    {
        public string assetPath;
        public long compressedSize;
        public long uncompressedSize;
        public string folder;
        public string fileName;
        public string formattedCompressedSize;
        public string formattedUncompressedSize;
        public float compressionRatio;
        
        public BuildAssetInfo(string path, long compressed, long uncompressed)
        {
            assetPath = path;
            compressedSize = compressed;
            uncompressedSize = uncompressed;
            folder = GetFolderPath(path);
            fileName = Path.GetFileName(path);
            formattedCompressedSize = FormatBytes(compressed);
            formattedUncompressedSize = FormatBytes(uncompressed);
            compressionRatio = uncompressed > 0 ? (float)compressed / uncompressed * 100f : 0f;
        }
        
        private string GetFolderPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "Root";
            
            string dir = Path.GetDirectoryName(filePath);
            return string.IsNullOrEmpty(dir) ? "Root" : dir.Replace('\\', '/');
        }
        
        private string FormatBytes(long bytes)
        {
            if (bytes == 0) return "0 B";
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
    
    [System.Serializable]
    public class FolderBuildInfo
    {
        public string folderPath;
        public long totalCompressedSize;
        public long totalUncompressedSize;
        public int fileCount;
        public string formattedCompressedSize;
        public string formattedUncompressedSize;
        public float avgCompressionRatio;
        public List<BuildAssetInfo> assets;
        
        public FolderBuildInfo(string path)
        {
            folderPath = path;
            totalCompressedSize = 0;
            totalUncompressedSize = 0;
            fileCount = 0;
            assets = new List<BuildAssetInfo>();
        }
        
        public void AddAsset(BuildAssetInfo asset)
        {
            assets.Add(asset);
            totalCompressedSize += asset.compressedSize;
            totalUncompressedSize += asset.uncompressedSize;
            fileCount++;
            formattedCompressedSize = FormatBytes(totalCompressedSize);
            formattedUncompressedSize = FormatBytes(totalUncompressedSize);
            avgCompressionRatio = totalUncompressedSize > 0 ? (float)totalCompressedSize / totalUncompressedSize * 100f : 0f;
        }
        
        private string FormatBytes(long bytes)
        {
            if (bytes == 0) return "0 B";
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    private Vector2 scrollPosition;
    private string editorLogPath = "";
    private List<FolderBuildInfo> folderInfos = new List<FolderBuildInfo>();
    private bool showDetailedAssets = false;
    private bool showUncompressedSizes = false;
    private string totalBuildSize = "";
    private DateTime lastBuildTime;
    private int totalAssetsCount = 0;

    [MenuItem("Tools/Build Report Analyzer (Editor.log)")]
    public static void ShowWindow()
    {
        GetWindow<BuildReportAnalyzer>("Build Report Analyzer");
    }

    void OnEnable()
    {
        // 기본 Editor.log 경로 설정
        editorLogPath = GetDefaultEditorLogPath();
    }

    void OnGUI()
    {
        GUILayout.Label("Unity Build Report Analyzer (Editor.log)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Editor.log 경로 설정
        EditorGUILayout.BeginHorizontal();
        editorLogPath = EditorGUILayout.TextField("Editor.log Path:", editorLogPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("Select Editor.log", Path.GetDirectoryName(editorLogPath), "log");
            if (!string.IsNullOrEmpty(path))
            {
                editorLogPath = path;
            }
        }
        if (GUILayout.Button("Default", GUILayout.Width(60)))
        {
            editorLogPath = GetDefaultEditorLogPath();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("이 도구는 Unity Editor.log에서 빌드 완료 후 생성되는 실제 빌드 용량 정보를 분석합니다.", MessageType.Info);
        EditorGUILayout.Space();

        // 분석 버튼
        if (GUILayout.Button("Analyze Build Report from Editor.log"))
        {
            AnalyzeEditorLog();
        }

        EditorGUILayout.Space();

        // 빌드 정보 표시
        if (!string.IsNullOrEmpty(totalBuildSize))
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Build Summary", EditorStyles.boldLabel);
            GUILayout.Label($"Total Build Size: {totalBuildSize}");
            GUILayout.Label($"Total Assets: {totalAssetsCount}");
            GUILayout.Label($"Build Time: {lastBuildTime:yyyy-MM-dd HH:mm:ss}");
            GUILayout.Label($"Analyzed Folders: {folderInfos.Count}");
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // 보고서 생성 버튼 및 옵션
        if (folderInfos.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Detailed Report"))
            {
                GenerateReportFile();
            }
            showDetailedAssets = EditorGUILayout.Toggle("Show Assets", showDetailedAssets);
            showUncompressedSizes = EditorGUILayout.Toggle("Show Uncompressed", showUncompressedSizes);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 결과 표시
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DisplayResults();
            EditorGUILayout.EndScrollView();
        }
    }

    string GetDefaultEditorLogPath()
    {
        string logPath = "";
        
        #if UNITY_EDITOR_WIN
        logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                              "Unity", "Editor", "Editor.log");
        #elif UNITY_EDITOR_OSX
        logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
                              "Library", "Logs", "Unity", "Editor.log");
        #elif UNITY_EDITOR_LINUX
        logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
                              ".config", "unity3d", "Editor.log");
        #endif
        
        return logPath;
    }

    void AnalyzeEditorLog()
    {
        if (string.IsNullOrEmpty(editorLogPath) || !File.Exists(editorLogPath))
        {
            EditorUtility.DisplayDialog("Error", "Editor.log file not found. Please check the path.", "OK");
            return;
        }

        folderInfos.Clear();
        Dictionary<string, FolderBuildInfo> folderDict = new Dictionary<string, FolderBuildInfo>();
        totalAssetsCount = 0;

        try
        {
            List<string> lines = new List<string>();
            using (var fileStream = new FileStream(editorLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    //string content = reader.ReadToEnd();
                    //reader.re
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            lines.Add(line.Trim());
                        }
                    }
                    reader.Close();
                }
                fileStream.Close();                
            }


            //string[] lines = File.ReadAllLines(editorLogPath);
            bool inBuildReport = false;
            bool foundBuildInfo = false;

            Debug.Log("lines" + lines.Count);

            // 빌드 보고서 섹션 찾기 및 파싱
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                
                // 빌드 완료 시간 찾기
                if (line.Contains("Build completed") || line.Contains("Build succeeded"))
                {
                    if (TryParseBuildTime(line, out DateTime buildTime))
                    {
                        lastBuildTime = buildTime;
                    }
                }
                
                // 빌드 보고서 시작 감지
                if (line.Contains("Used Assets and files from the Resources folder, sorted by uncompressed size:") ||
                    line.Contains("Used Assets, sorted by uncompressed size:"))
                {
                    inBuildReport = true;
                    foundBuildInfo = true;
                    continue;
                }
                
                // 빌드 보고서 종료 감지
                if (inBuildReport && (line.Contains("---------------") || 
                                    line.Contains("System memory in use") ||
                                    string.IsNullOrEmpty(line)))
                {
                    if (line.Contains("System memory in use"))
                        inBuildReport = false;
                    continue;
                }
                
                // 빌드 보고서 라인 파싱
                if (inBuildReport && !string.IsNullOrEmpty(line))
                {
                    var assetInfo = ParseBuildReportLine(line);
                    if (assetInfo != null)
                    {
                        totalAssetsCount++;
                        
                        if (!folderDict.ContainsKey(assetInfo.folder))
                        {
                            folderDict[assetInfo.folder] = new FolderBuildInfo(assetInfo.folder);
                        }
                        
                        folderDict[assetInfo.folder].AddAsset(assetInfo);
                    }
                }
            }

            if (!foundBuildInfo)
            {
                EditorUtility.DisplayDialog("Warning", 
                    "No build report found in Editor.log. Please build your project first.", "OK");
                return;
            }

            // 크기순으로 정렬 (압축된 크기 기준)
            folderInfos = folderDict.Values.OrderByDescending(f => f.totalCompressedSize).ToList();
            
            // 전체 빌드 크기 계산
            long totalSize = folderInfos.Sum(f => f.totalCompressedSize);
            totalBuildSize = FormatBytes(totalSize);
            
            Debug.Log($"Editor.log Analysis Complete. Found {folderInfos.Count} folders with {totalAssetsCount} assets.");
            Debug.Log($"Total build size: {totalBuildSize}");
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to analyze Editor.log: {e.Message}", "OK");
        }
    }

    BuildAssetInfo ParseBuildReportLine(string line)
    {
        // Unity 빌드 보고서 라인 형식:
        // " 1.2 mb	 2.1 mb	 12.5% Assets/Textures/texture.png"
        // 또는
        // " 1.2 kb	 2.1 kb	  5.2% Assets/Scripts/script.cs"
        
        // 정규표현식으로 크기 정보와 경로 추출
        var pattern = @"\s*(\d+\.?\d*)\s*(kb|mb|gb|b)\s+(\d+\.?\d*)\s*(kb|mb|gb|b)\s+[\d.]+%\s+(.+)";
        var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
        
        if (match.Success)
        {
            try
            {
                // 압축된 크기
                float compressedValue = float.Parse(match.Groups[1].Value);
                string compressedUnit = match.Groups[2].Value.ToLower();
                long compressedBytes = ConvertToBytes(compressedValue, compressedUnit);
                
                // 압축되지 않은 크기
                float uncompressedValue = float.Parse(match.Groups[3].Value);
                string uncompressedUnit = match.Groups[4].Value.ToLower();
                long uncompressedBytes = ConvertToBytes(uncompressedValue, uncompressedUnit);
                
                // 에셋 경로
                string assetPath = match.Groups[5].Value.Trim();
                
                return new BuildAssetInfo(assetPath, compressedBytes, uncompressedBytes);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to parse line: {line} - {e.Message}");
            }
        }
        
        return null;
    }

    long ConvertToBytes(float value, string unit)
    {
        switch (unit.ToLower())
        {
            case "b": return (long)value;
            case "kb": return (long)(value * 1024);
            case "mb": return (long)(value * 1024 * 1024);
            case "gb": return (long)(value * 1024 * 1024 * 1024);
            default: return 0;
        }
    }

    bool TryParseBuildTime(string line, out DateTime buildTime)
    {
        buildTime = DateTime.Now;
        
        // 로그 라인에서 시간 정보 추출 시도
        var timePattern = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})";
        var match = Regex.Match(line, timePattern);
        
        if (match.Success)
        {
            return DateTime.TryParse(match.Groups[1].Value, out buildTime);
        }
        
        return false;
    }

    void DisplayResults()
    {
        if (folderInfos.Count == 0) return;

        GUILayout.Label("Build Size Analysis (Sorted by Compressed Size)", EditorStyles.boldLabel);
        
        foreach (var folder in folderInfos)
        {
            EditorGUILayout.BeginVertical("box");
            
            // 폴더 정보
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"📁 {folder.folderPath}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (showUncompressedSizes)
            {
                GUILayout.Label($"{folder.formattedUncompressedSize} → {folder.formattedCompressedSize} ({folder.avgCompressionRatio:F1}%) | {folder.fileCount} files", 
                               EditorStyles.miniLabel);
            }
            else
            {
                GUILayout.Label($"{folder.formattedCompressedSize} ({folder.avgCompressionRatio:F1}%) | {folder.fileCount} files", 
                               EditorStyles.miniLabel);
            }
            EditorGUILayout.EndHorizontal();
            
            // 상세 에셋 표시
            if (showDetailedAssets)
            {
                EditorGUI.indentLevel++;
                var topAssets = folder.assets.OrderByDescending(a => a.compressedSize).Take(10);
                foreach (var asset in topAssets)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"• {asset.fileName}", EditorStyles.miniLabel);
                    GUILayout.FlexibleSpace();
                    
                    if (showUncompressedSizes)
                    {
                        GUILayout.Label($"{asset.formattedUncompressedSize} → {asset.formattedCompressedSize} ({asset.compressionRatio:F1}%)", 
                                       EditorStyles.miniLabel);
                    }
                    else
                    {
                        GUILayout.Label(asset.formattedCompressedSize, EditorStyles.miniLabel);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                if (folder.assets.Count > 10)
                {
                    GUILayout.Label($"... and {folder.assets.Count - 10} more files", EditorStyles.miniLabel);
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }
    }

    void GenerateReportFile()
    {
        string reportContent = GenerateReportContent();
        
        string savePath = EditorUtility.SaveFilePanel(
            "Save Build Analysis Report",
            Application.dataPath,
            $"BuildReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
            "txt"
        );
        
        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllText(savePath, reportContent, Encoding.UTF8);
            EditorUtility.DisplayDialog("Success", $"Report saved to:\n{savePath}", "OK");
            
            // 파일 탐색기에서 열기
            EditorUtility.RevealInFinder(savePath);
        }
    }

    string GenerateReportContent()
    {
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("=================================================");
        sb.AppendLine("        Unity Build Report Analysis");
        sb.AppendLine("        (Based on Editor.log)");
        sb.AppendLine("=================================================");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Build Time: {lastBuildTime:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Total Build Size: {totalBuildSize}");
        sb.AppendLine($"Total Assets: {totalAssetsCount}");
        sb.AppendLine($"Total Folders: {folderInfos.Count}");
        sb.AppendLine();
        
        sb.AppendLine("=== FOLDER SIZE BREAKDOWN (Largest to Smallest) ===");
        sb.AppendLine();
        
        long totalCompressed = folderInfos.Sum(f => f.totalCompressedSize);
        long totalUncompressed = folderInfos.Sum(f => f.totalUncompressedSize);
        
        int rank = 1;
        foreach (var folder in folderInfos)
        {
            double compressedPercentage = (double)folder.totalCompressedSize / totalCompressed * 100;
            double uncompressedPercentage = (double)folder.totalUncompressedSize / totalUncompressed * 100;
            
            sb.AppendLine($"{rank}. {folder.folderPath}");
            sb.AppendLine($"   Compressed Size: {folder.formattedCompressedSize} ({compressedPercentage:F1}% of total)");
            sb.AppendLine($"   Uncompressed Size: {folder.formattedUncompressedSize} ({uncompressedPercentage:F1}% of total)");
            sb.AppendLine($"   Compression Ratio: {folder.avgCompressionRatio:F1}%");
            sb.AppendLine($"   Files: {folder.fileCount}");
            
            // 상위 10개 파일 표시
            if (folder.assets.Count > 0)
            {
                sb.AppendLine("   Top Files (by compressed size):");
                var topAssets = folder.assets.OrderByDescending(a => a.compressedSize).Take(10);
                foreach (var asset in topAssets)
                {
                    sb.AppendLine($"     • {asset.fileName}");
                    sb.AppendLine($"       Compressed: {asset.formattedCompressedSize} | Uncompressed: {asset.formattedUncompressedSize} ({asset.compressionRatio:F1}%)");
                }
                if (folder.assets.Count > 10)
                {
                    sb.AppendLine($"     ... and {folder.assets.Count - 10} more files");
                }
            }
            
            sb.AppendLine();
            rank++;
        }
        
        sb.AppendLine("=== BUILD OPTIMIZATION SUGGESTIONS ===");
        sb.AppendLine();
        
        // 최적화 제안
        if (folderInfos.Count > 0)
        {
            var largestFolder = folderInfos[0];
            sb.AppendLine($"1. Focus on '{largestFolder.folderPath}' folder ({largestFolder.formattedCompressedSize})");
            
            // 압축률이 낮은 폴더 찾기
            var poorCompressionFolders = folderInfos.Where(f => f.avgCompressionRatio > 80f && f.totalCompressedSize > 1024 * 1024).Take(3);
            if (poorCompressionFolders.Any())
            {
                sb.AppendLine();
                sb.AppendLine("2. Folders with poor compression (>80% ratio):");
                foreach (var folder in poorCompressionFolders)
                {
                    sb.AppendLine($"   • {folder.folderPath}: {folder.avgCompressionRatio:F1}% compression ratio");
                }
                sb.AppendLine("   → Consider texture compression, audio compression, or asset optimization");
            }
        }
        
        sb.AppendLine();
        sb.AppendLine("3. General Optimization Tips:");
        sb.AppendLine("   • Review texture import settings and compression formats");
        sb.AppendLine("   • Optimize audio files (compression, sample rates)");
        sb.AppendLine("   • Use asset bundles for optional content");
        sb.AppendLine("   • Enable code stripping in build settings");
        sb.AppendLine("   • Consider splitting large assets across multiple bundles");
        sb.AppendLine();
        
        // 압축 통계
        if (totalUncompressed > 0)
        {
            float overallCompressionRatio = (float)totalCompressed / totalUncompressed * 100f;
            sb.AppendLine("=== COMPRESSION STATISTICS ===");
            sb.AppendLine($"Total Uncompressed: {FormatBytes(totalUncompressed)}");
            sb.AppendLine($"Total Compressed: {FormatBytes(totalCompressed)}");
            sb.AppendLine($"Overall Compression Ratio: {overallCompressionRatio:F1}%");
            sb.AppendLine($"Space Saved: {FormatBytes(totalUncompressed - totalCompressed)}");
        }
        
        sb.AppendLine();
        sb.AppendLine("=================================================");
        sb.AppendLine("           End of Build Report");
        sb.AppendLine("=================================================");
        
        return sb.ToString();
    }
    
    private string FormatBytes(long bytes)
    {
        if (bytes == 0) return "0 B";
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}