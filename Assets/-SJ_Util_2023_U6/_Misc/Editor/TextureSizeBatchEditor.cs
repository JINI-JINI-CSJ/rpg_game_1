using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class TextureSizeBatchEditor : EditorWindow
{
    private string targetFolderPath = "Assets/";
    private int newMaxSize = 1024;
    
    // 필터 옵션들
    private bool useMinSizeFilter = false;
    private int minSizeThreshold = 256;
    private bool useMaxSizeFilter = false;
    private int maxSizeThreshold = 2048;
    
    // 미리보기 및 결과
    private List<TextureInfo> foundTextures = new List<TextureInfo>();
    private Vector2 scrollPosition;
    private bool showPreview = false;
    
    // 진행률 표시
    private bool isProcessing = false;
    private float progress = 0f;
    private string progressText = "";
    
    [System.Serializable]
    public class TextureInfo
    {
        public Texture2D texture;
        public string path;
        public int currentMaxSize;
        public int originalWidth;
        public int originalHeight;
        public bool willBeProcessed;
        
        public TextureInfo(Texture2D tex, string texPath, int maxSize, int width, int height)
        {
            texture = tex;
            path = texPath;
            currentMaxSize = maxSize;
            originalWidth = width;
            originalHeight = height;
            willBeProcessed = false;
        }
    }
    
    [MenuItem("Tools/Texture Size Batch Editor")]
    public static void ShowWindow()
    {
        GetWindow<TextureSizeBatchEditor>("Texture Size Batch Editor");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Texture Size Batch Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // 대상 폴더 설정
        EditorGUILayout.LabelField("Target Folder", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        targetFolderPath = EditorGUILayout.TextField("Folder Path", targetFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    targetFolderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Selected folder must be within the Assets folder.", "OK");
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 새로운 최대 크기 설정
        EditorGUILayout.LabelField("New Settings", EditorStyles.boldLabel);
        newMaxSize = EditorGUILayout.IntField("New Max Size", newMaxSize);
        
        // 크기 제한 필터
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Size Filters (Optional)", EditorStyles.boldLabel);
        
        useMinSizeFilter = EditorGUILayout.Toggle("Apply Min Size Filter", useMinSizeFilter);
        if (useMinSizeFilter)
        {
            EditorGUI.indentLevel++;
            minSizeThreshold = EditorGUILayout.IntField("Min Size Threshold", minSizeThreshold);
            EditorGUILayout.HelpBox("Only process textures with current max size >= " + minSizeThreshold, MessageType.Info);
            EditorGUI.indentLevel--;
        }
        
        useMaxSizeFilter = EditorGUILayout.Toggle("Apply Max Size Filter", useMaxSizeFilter);
        if (useMaxSizeFilter)
        {
            EditorGUI.indentLevel++;
            maxSizeThreshold = EditorGUILayout.IntField("Max Size Threshold", maxSizeThreshold);
            EditorGUILayout.HelpBox("Only process textures with current max size <= " + maxSizeThreshold, MessageType.Info);
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // 버튼들
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Scan Textures") && !isProcessing)
        {
            ScanTextures();
        }
        
        GUI.enabled = foundTextures.Count > 0 && !isProcessing;
        if (GUILayout.Button("Apply Changes"))
        {
            ApplyChanges();
        }
        GUI.enabled = true;
        
        EditorGUILayout.EndHorizontal();
        
        // 진행률 표시
        if (isProcessing)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Processing...", EditorStyles.boldLabel);
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), progress, progressText);
        }
        
        // 미리보기 토글
        if (foundTextures.Count > 0)
        {
            EditorGUILayout.Space();
            showPreview = EditorGUILayout.Foldout(showPreview, $"Found Textures ({foundTextures.Count})");
            
            if (showPreview)
            {
                DisplayTexturePreview();
            }
        }
    }
    
    private void ScanTextures()
    {
        foundTextures.Clear();
        
        if (!Directory.Exists(targetFolderPath))
        {
            EditorUtility.DisplayDialog("Error", "Target folder does not exist!", "OK");
            return;
        }
        
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { targetFolderPath });
        
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null)
                {
                    int currentMaxSize = importer.maxTextureSize;
                    bool shouldProcess = ShouldProcessTexture(currentMaxSize);
                    
                    TextureInfo info = new TextureInfo(texture, path, currentMaxSize, texture.width, texture.height);
                    info.willBeProcessed = shouldProcess;
                    foundTextures.Add(info);
                }
            }
        }
        
        Debug.Log($"Found {foundTextures.Count} textures. {foundTextures.FindAll(t => t.willBeProcessed).Count} will be processed.");
    }
    
    private bool ShouldProcessTexture(int currentMaxSize)
    {
        if (useMinSizeFilter && currentMaxSize < minSizeThreshold)
            return false;
            
        if (useMaxSizeFilter && currentMaxSize > maxSizeThreshold)
            return false;
            
        return true;
    }
    
    private async void ApplyChanges()
    {
        var texturesToProcess = foundTextures.FindAll(t => t.willBeProcessed);
        
        if (texturesToProcess.Count == 0)
        {
            EditorUtility.DisplayDialog("No Changes", "No textures match the filter criteria.", "OK");
            return;
        }
        
        bool proceed = EditorUtility.DisplayDialog(
            "Confirm Changes", 
            $"This will modify {texturesToProcess.Count} textures.\nNew max size: {newMaxSize}\n\nProceed?", 
            "Yes", "Cancel"
        );
        
        if (!proceed) return;
        
        isProcessing = true;
        int processedCount = 0;
        int totalCount = texturesToProcess.Count;
        
        try
        {
            AssetDatabase.StartAssetEditing();
            
            foreach (var textureInfo in texturesToProcess)
            {
                progress = (float)processedCount / totalCount;
                progressText = $"Processing {processedCount + 1}/{totalCount}: {Path.GetFileName(textureInfo.path)}";
                
                // UI 업데이트를 위해 잠시 대기
                await System.Threading.Tasks.Task.Delay(1);
                
                TextureImporter importer = AssetImporter.GetAtPath(textureInfo.path) as TextureImporter;
                if (importer != null)
                {
                    importer.maxTextureSize = newMaxSize;
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }
                
                processedCount++;
                Repaint();
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            isProcessing = false;
            progress = 0f;
            progressText = "";
            
            // 결과 재스캔
            ScanTextures();
            
            Repaint();
        }
        
        EditorUtility.DisplayDialog("Complete", $"Successfully processed {processedCount} textures!", "OK");
    }
    
    private void DisplayTexturePreview()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        
        foreach (var textureInfo in foundTextures)
        {
            EditorGUILayout.BeginHorizontal();
            
            // 처리 여부 표시
            GUI.color = textureInfo.willBeProcessed ? Color.green : Color.red;
            GUILayout.Label(textureInfo.willBeProcessed ? "✓" : "✗", GUILayout.Width(20));
            GUI.color = Color.white;
            
            // 텍스처 정보
            EditorGUILayout.ObjectField(textureInfo.texture, typeof(Texture2D), false, GUILayout.Width(100));
            
            GUILayout.Label($"{textureInfo.originalWidth}x{textureInfo.originalHeight}", GUILayout.Width(80));
            GUILayout.Label($"Max: {textureInfo.currentMaxSize}", GUILayout.Width(80));
            
            if (textureInfo.willBeProcessed)
            {
                GUILayout.Label($"→ {newMaxSize}", GUILayout.Width(60));
            }
            else
            {
                GUILayout.Label("(Skipped)", GUILayout.Width(60));
            }
            
            GUILayout.Label(textureInfo.path, EditorStyles.miniLabel);
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        // 통계 정보
        EditorGUILayout.Space();
        var toProcess = foundTextures.FindAll(t => t.willBeProcessed);
        EditorGUILayout.LabelField($"Will process: {toProcess.Count} / {foundTextures.Count} textures");
    }
}