// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEditor.Build;
// using UnityEditor.Build.Reporting;
// using UnityEngine;
// using System.IO;
// using System.Collections.Generic;
// using System.Text.RegularExpressions;
// using System.Linq;

// public class BuildReportAnalyzerWindow : EditorWindow
// {
//     string selectedFolder = "Assets";

//     [MenuItem("SJMisc/Build Report Folder Analyzer")]
//     public static void ShowWindow()
//     {
//         GetWindow<BuildReportAnalyzerWindow>("Build Folder Analyzer");
//     }

//     void OnGUI()
//     {
//         GUILayout.Label("분석할 루트 폴더 경로", EditorStyles.boldLabel);
//         GUILayout.BeginHorizontal();
//         EditorGUILayout.TextField("선택 폴더", selectedFolder);
//         if (GUILayout.Button("폴더 선택", GUILayout.MaxWidth(100)))
//         {
//             string fullPath = EditorUtility.OpenFolderPanel("폴더 선택", Application.dataPath, "");
//             if (!string.IsNullOrEmpty(fullPath))
//             {
//                 if (fullPath.StartsWith(Application.dataPath))
//                 {
//                     selectedFolder = "Assets" + fullPath.Substring(Application.dataPath.Length);
//                 }
//                 else
//                 {
//                     EditorUtility.DisplayDialog("오류", "Assets 폴더 안의 경로만 선택해주세요.", "확인");
//                 }
//             }
//         }
//         GUILayout.EndHorizontal();

//         if (GUILayout.Button("빌드 리포트 분석 실행"))
//         {
//             BuildReportExtractor.Build_Report_make(selectedFolder);
//         }
//     }
// }



// public class BuildReportExtractor : IPostprocessBuildWithReport
// {
//     public int callbackOrder => 0;

//     public void OnPostprocessBuild(BuildReport report)
//     {
//         Build_Report_make("Assets");
//     }

//     public static void Build_Report_make(string rootFolder)
// {
//     string editorLogPath = GetEditorLogPath();
//     if (!File.Exists(editorLogPath))
//     {
//         Debug.LogError("Editor.log 파일을 찾을 수 없습니다.");
//         return;
//     }

//     string tempPath = Path.Combine(Application.temporaryCachePath, "EditorLogCopy.txt");
//     File.Copy(editorLogPath, tempPath, true);
//     string[] lines = File.ReadAllLines(tempPath);

//     string startMarker = "used assets and files from the resources folder, sorted by uncompressed size:";
//     int startIndex = -1;
//     for (int i = lines.Length - 1; i >= 0; i--)
//     {
//         if (lines[i].ToLower().Contains(startMarker))
//         {
//             startIndex = i;
//             break;
//         }
//     }

//     if (startIndex == -1)
//     {
//         Debug.LogError("에셋 정보 섹션을 찾을 수 없습니다.");
//         return;
//     }

//     string savePath = "Assets/BuildReports/FilteredFolderReport.txt";
//     Directory.CreateDirectory(Path.GetDirectoryName(savePath));
//     Dictionary<string, long> folderSizes = new();
//     long totalSize_Folder = 0;

//     long totalSize_ALL = 0;

//     Regex sizePathRegex = new(@"^\s*([\d\.]+)\s+(kb|mb|b)\s+(.+)$", RegexOptions.IgnoreCase);

//     //Debug.Log($"Processing line: 갯수" + lines.Length);

//     using (StreamWriter writer = new StreamWriter(savePath))
//     {
//         writer.WriteLine($"빌드 시간: {System.DateTime.Now}");
//         writer.WriteLine($"📂 필터 기준: {rootFolder}");
//         writer.WriteLine("=========================================");

//         for (int i = startIndex + 1; i < lines.Length; i++)
//         {
//             string line = lines[i];
//             if (string.IsNullOrWhiteSpace(line)) break;

//             Match match = sizePathRegex.Match(line);

//             if (match.Success)
//             {
//                 float size = float.Parse(match.Groups[1].Value);
//                 string unit = match.Groups[2].Value.ToLower();
//                 string path = match.Groups[3].Value.Replace('\\', '/');

//                 //Debug.Log($"StartsWith" + path + " : " + rootFolder);

//                 //if (!path.StartsWith(rootFolder) ) continue;

//                 long bytes = unit switch
//                 {
//                     "mb" => (long)(size * 1024 * 1024),
//                     "kb" => (long)(size * 1024),
//                     _ => (long)size
//                 };

//                 totalSize_ALL += bytes;
//                 writer.WriteLine(line);

//                 //Debug.Log($"패스별 용량:" + totalSize);

//                 // Get immediate child folder: root/child
//                 //string relativePath = path.Substring(rootFolder.Length).TrimStart('/');

//                 if (!path.Contains(rootFolder) ) continue;

//                 totalSize_Folder += bytes;

//                 string path_1 = path.Substring( path.IndexOf("% ") + 2 );
//                 string relativePath = path_1.Substring(rootFolder.Length + 1);
//                 string[] subParts = relativePath.Split('/');
//                 string immediateSubFolder = subParts.Length > 1
//                     ? $"{rootFolder}/{subParts[0]}"
//                     : rootFolder; // file directly in rootFolder

//                 if (!folderSizes.ContainsKey(immediateSubFolder))
//                     folderSizes[immediateSubFolder] = 0;

//                 folderSizes[immediateSubFolder] += bytes;

//                 //Debug.Log($"폴더: {immediateSubFolder}, relativePath: { relativePath }");
//             }
//         }

//         writer.WriteLine();
//         writer.WriteLine();
//         writer.WriteLine($"📦 전체 총합: {FormatSize(totalSize_ALL)}");
//         writer.WriteLine($"📦 폴더 총합: {FormatSize(totalSize_Folder)} ({rootFolder})");
//         writer.WriteLine();

//         writer.WriteLine("📁 하위 폴더 용량 정렬");
//         writer.WriteLine("-----------------------------------------");
//         foreach (var entry in folderSizes
//             .Where(e => e.Key != rootFolder)
//             .OrderByDescending(e => e.Value))
//         {
//             writer.WriteLine($"{FormatSize(entry.Value)}\t{entry.Key}");
//         }

//         // 직접 속한 파일만 따로 표시 가능 (선택 기능)
//         if (folderSizes.ContainsKey(rootFolder))
//         {
//             writer.WriteLine();
//             writer.WriteLine($"📄 직접 포함된 파일: {FormatSize(folderSizes[rootFolder])}");
//         }
//     }

//     Debug.Log($"✅ 폴더 분석 리포트가 생성되었습니다: {savePath}");
//     AssetDatabase.Refresh();
// }


//     public static string GetEditorLogPath()
//     {
// #if UNITY_EDITOR_WIN
//         return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Unity/Editor/Editor.log");
// #elif UNITY_EDITOR_OSX
//         return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Library/Logs/Unity/Editor.log");
// #else
//         return "";
// #endif
//     }

//     static string FormatSize(long bytes)
//     {
//         if (bytes >= 1024 * 1024)
//             return $"{(bytes / 1024f / 1024f):F2} MB";
//         else if (bytes >= 1024)
//             return $"{(bytes / 1024f):F2} KB";
//         else
//             return $"{bytes} B";
//     }
// }

// #endif