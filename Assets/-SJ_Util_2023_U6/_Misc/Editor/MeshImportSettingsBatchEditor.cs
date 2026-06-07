using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshImportSettingsBatchEditor : EditorWindow
{
    private string folderPath = "Assets/";
    private bool includeSubfolders = true;
    
    // 임포트 설정 옵션들
    private bool importBlendShapes = true;
    private bool importVisibility = true;
    private bool importCameras = false;
    private bool importLights = false;
    private bool keepQuads = false;
    private ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.Off;
    private bool isReadable = false;
    
    // UI 표시용 옵션들
    private bool showBlendShapes = true;
    private bool showVisibility = true;
    private bool showCameras = true;
    private bool showLights = true;
    private bool showKeepQuads = true;
    private bool showMeshCompression = true;
    private bool showReadable = true;

    [MenuItem("Tools/Mesh Import Settings Batch Editor")]
    public static void ShowWindow()
    {
        GetWindow<MeshImportSettingsBatchEditor>("Mesh Import Batch Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Mesh Import Settings Batch Editor", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 폴더 경로 설정
        GUILayout.Label("Target Folder:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                // 절대 경로를 상대 경로로 변환
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        
        includeSubfolders = EditorGUILayout.Toggle("Include Subfolders", includeSubfolders);
        
        GUILayout.Space(10);
        GUILayout.Label("Import Settings:", EditorStyles.boldLabel);

        // 각 설정 옵션들
        EditorGUILayout.BeginHorizontal();
        showBlendShapes = EditorGUILayout.Toggle("", showBlendShapes, GUILayout.Width(20));
        GUI.enabled = showBlendShapes;
        importBlendShapes = EditorGUILayout.Toggle("Import Blend Shapes", importBlendShapes);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        showVisibility = EditorGUILayout.Toggle("", showVisibility, GUILayout.Width(20));
        GUI.enabled = showVisibility;
        importVisibility = EditorGUILayout.Toggle("Import Visibility", importVisibility);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        showCameras = EditorGUILayout.Toggle("", showCameras, GUILayout.Width(20));
        GUI.enabled = showCameras;
        importCameras = EditorGUILayout.Toggle("Import Cameras", importCameras);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        showLights = EditorGUILayout.Toggle("", showLights, GUILayout.Width(20));
        GUI.enabled = showLights;
        importLights = EditorGUILayout.Toggle("Import Lights", importLights);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        showKeepQuads = EditorGUILayout.Toggle("", showKeepQuads, GUILayout.Width(20));
        GUI.enabled = showKeepQuads;
        keepQuads = EditorGUILayout.Toggle("Keep Quads (Preserve Hierarchy)", keepQuads);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        showMeshCompression = EditorGUILayout.Toggle("", showMeshCompression, GUILayout.Width(20));
        GUI.enabled = showMeshCompression;
        meshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", meshCompression);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        showReadable = EditorGUILayout.Toggle("", showReadable, GUILayout.Width(20));
        GUI.enabled = showReadable;
        isReadable = EditorGUILayout.Toggle("Read/Write Enabled", isReadable);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // 적용 버튼
        if (GUILayout.Button("Apply Settings to All Meshes", GUILayout.Height(30)))
        {
            ApplySettingsToMeshes();
        }

        GUILayout.Space(10);
        
        // 도움말
        EditorGUILayout.HelpBox(
            "체크박스를 해제하면 해당 설정은 변경되지 않습니다.\n" +
            "적용하고 싶은 설정만 체크하고 값을 설정한 후 'Apply Settings' 버튼을 클릭하세요.", 
            MessageType.Info);
    }

    void ApplySettingsToMeshes()
    {
        if (!Directory.Exists(folderPath))
        {
            EditorUtility.DisplayDialog("Error", "지정된 폴더가 존재하지 않습니다: " + folderPath, "OK");
            return;
        }

        // 폴더에서 모든 3D 모델 파일 찾기
        string[] searchPattern = { "*.fbx", "*.obj", "*.dae", "*.3ds", "*.dxf", "*.max", "*.ma", "*.mb", "*.blend" };
        var modelFiles = new System.Collections.Generic.List<string>();

        foreach (string pattern in searchPattern)
        {
            string[] files = Directory.GetFiles(folderPath, pattern, 
                includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            modelFiles.AddRange(files);
        }

        if (modelFiles.Count == 0)
        {
            EditorUtility.DisplayDialog("Info", "지정된 폴더에서 3D 모델 파일을 찾을 수 없습니다.", "OK");
            return;
        }

        int processedCount = 0;
        int totalCount = modelFiles.Count;

        try
        {
            AssetDatabase.StartAssetEditing();

            for (int i = 0; i < modelFiles.Count; i++)
            {
                string filePath = modelFiles[i].Replace('\\', '/');
                
                // 진행률 표시
                if (EditorUtility.DisplayCancelableProgressBar("Applying Import Settings", 
                    $"Processing {Path.GetFileName(filePath)} ({i + 1}/{totalCount})", 
                    (float)(i + 1) / totalCount))
                {
                    break; // 사용자가 취소한 경우
                }

                // 모델 임포터 가져오기
                ModelImporter importer = AssetImporter.GetAtPath(filePath) as ModelImporter;
                if (importer != null)
                {
                    bool needsReimport = false;

                    // 설정 적용
                    if (showBlendShapes && importer.importBlendShapes != importBlendShapes)
                    {
                        importer.importBlendShapes = importBlendShapes;
                        needsReimport = true;
                    }

                    if (showVisibility && importer.importVisibility != importVisibility)
                    {
                        importer.importVisibility = importVisibility;
                        needsReimport = true;
                    }

                    if (showCameras && importer.importCameras != importCameras)
                    {
                        importer.importCameras = importCameras;
                        needsReimport = true;
                    }

                    if (showLights && importer.importLights != importLights)
                    {
                        importer.importLights = importLights;
                        needsReimport = true;
                    }

                    if (showKeepQuads && importer.keepQuads != keepQuads)
                    {
                        importer.keepQuads = keepQuads;
                        needsReimport = true;
                    }

                    if (showMeshCompression && importer.meshCompression != meshCompression)
                    {
                        importer.meshCompression = meshCompression;
                        needsReimport = true;
                    }

                    if (showReadable && importer.isReadable != isReadable)
                    {
                        importer.isReadable = isReadable;
                        needsReimport = true;
                    }

                    if (needsReimport)
                    {
                        importer.SaveAndReimport();
                        processedCount++;
                    }
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
        }

        EditorUtility.DisplayDialog("Complete", 
            $"총 {totalCount}개의 파일 중 {processedCount}개의 파일이 처리되었습니다.", "OK");

        AssetDatabase.Refresh();
    }
}