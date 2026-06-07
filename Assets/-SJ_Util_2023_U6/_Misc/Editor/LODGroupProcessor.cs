using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LODGroupProcessor : EditorWindow
{
    [MenuItem("Tools/LOD Group Processor")]
    public static void ShowWindow()
    {
        GetWindow<LODGroupProcessor>("LOD Group Processor");
    }

    private void OnGUI()
    {
        GUILayout.Label("LOD Group 처리 도구", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("사용법:", EditorStyles.label);
        GUILayout.Label("1. Hierarchy에서 처리할 상위 객체를 선택하세요");
        GUILayout.Label("2. 아래 버튼을 클릭하여 LOD Group을 처리하세요");
        GUILayout.Space(10);
        
        if (GUILayout.Button("선택된 객체의 LOD Group 처리", GUILayout.Height(30)))
        {
            ProcessSelectedObjectLODGroups();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("모든 LOD Group 처리 (씬 전체)", GUILayout.Height(30)))
        {
            ProcessAllLODGroups();
        }
    }

    private void ProcessSelectedObjectLODGroups()
    {
        GameObject selectedObject = Selection.activeGameObject;
        
        if (selectedObject == null)
        {
            Debug.LogWarning("객체를 선택해주세요!");
            EditorUtility.DisplayDialog("경고", "처리할 객체를 먼저 선택해주세요.", "확인");
            return;
        }

        // 선택된 객체 하위의 모든 LOD Group 찾기
        LODGroup[] lodGroups = selectedObject.GetComponentsInChildren<LODGroup>();
        
        if (lodGroups.Length == 0)
        {
            Debug.LogWarning("선택된 객체 하위에 LOD Group이 없습니다!");
            EditorUtility.DisplayDialog("정보", "선택된 객체 하위에 LOD Group이 없습니다.", "확인");
            return;
        }

        ProcessLODGroups(lodGroups);
    }

    private void ProcessAllLODGroups()
    {
        // 씬의 모든 LOD Group 찾기
        LODGroup[] lodGroups = FindObjectsOfType<LODGroup>();
        
        if (lodGroups.Length == 0)
        {
            Debug.LogWarning("씬에 LOD Group이 없습니다!");
            EditorUtility.DisplayDialog("정보", "씬에 LOD Group이 없습니다.", "확인");
            return;
        }

        ProcessLODGroups(lodGroups);
    }

    private void ProcessLODGroups(LODGroup[] lodGroups)
    {
        int processedCount = 0;
        
        // Undo 그룹 시작
        Undo.SetCurrentGroupName("Process LOD Groups");
        int undoGroup = Undo.GetCurrentGroup();

        try
        {
            for (int i = 0; i < lodGroups.Length; i++)
            {
                LODGroup lodGroup = lodGroups[i];
                
                if (lodGroup == null) continue;

                // 진행 상황 표시
                if (EditorUtility.DisplayCancelableProgressBar(
                    "LOD Group 처리 중...", 
                    $"처리 중: {lodGroup.name} ({i + 1}/{lodGroups.Length})", 
                    (float)(i + 1) / lodGroups.Length))
                {
                    break; // 사용자가 취소한 경우
                }

                if (ProcessSingleLODGroup(lodGroup))
                {
                    processedCount++;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // Undo 그룹 종료
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"LOD Group 처리 완료: {processedCount}개 처리됨");
        EditorUtility.DisplayDialog("완료", $"{processedCount}개의 LOD Group이 처리되었습니다.", "확인");
    }

    private bool ProcessSingleLODGroup(LODGroup lodGroup)
    {
        if (lodGroup == null) return false;

        GameObject lodGroupObject = lodGroup.gameObject;
        Transform lodGroupTransform = lodGroupObject.transform;
        
        // LOD 정보 가져오기
        LOD[] lods = lodGroup.GetLODs();
        
        if (lods.Length == 0)
        {
            Debug.LogWarning($"LOD Group '{lodGroupObject.name}'에 LOD가 없습니다.");
            return false;
        }

        // 가장 낮은 품질의 LOD 찾기 (마지막 LOD)
        LOD lowestQualityLOD = lods[lods.Length - 1];
        
        if (lowestQualityLOD.renderers == null || lowestQualityLOD.renderers.Length == 0)
        {
            Debug.LogWarning($"LOD Group '{lodGroupObject.name}'의 가장 낮은 품질 LOD에 렌더러가 없습니다.");
            return false;
        }

        // 가장 낮은 품질 LOD의 첫 번째 렌더러 객체 찾기
        Renderer lowestRenderer = lowestQualityLOD.renderers[0];
        if (lowestRenderer == null)
        {
            Debug.LogWarning($"LOD Group '{lodGroupObject.name}'의 가장 낮은 품질 LOD 렌더러가 null입니다.");
            return false;
        }

        GameObject lowestLODObject = lowestRenderer.gameObject;
        
        // 새 인스턴스 생성 (복사)
        GameObject newInstance = Instantiate(lowestLODObject, lodGroupTransform.parent);
        
        // Undo 등록
        Undo.RegisterCreatedObjectUndo(newInstance, "Create LOD Instance");
        
        // LOD Group의 트랜스폼 값을 새 인스턴스에 적용
        newInstance.transform.position = lodGroupTransform.position;
        newInstance.transform.rotation = lodGroupTransform.rotation;
        newInstance.transform.localScale = lodGroupTransform.localScale;
        
        // 이름 설정 (원본 LOD Group 이름 사용)
        newInstance.name = lodGroupObject.name + "_Processed";
        
        // LOD Group 객체 삭제
        Undo.DestroyObjectImmediate(lodGroupObject);
        
        Debug.Log($"LOD Group 처리 완료 -> '{newInstance.name}' 생성");
        
        return true;
    }
}

// 추가적인 유틸리티 클래스
public static class LODGroupProcessorUtility
{
    [MenuItem("GameObject/LOD Group/Process Selected LOD Groups", false, 0)]
    public static void ProcessSelectedLODGroupsFromMenu()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("경고", "처리할 객체를 먼저 선택해주세요.", "확인");
            return;
        }

        List<LODGroup> allLODGroups = new List<LODGroup>();
        
        // 선택된 모든 객체에서 LOD Group 찾기
        foreach (GameObject obj in selectedObjects)
        {
            LODGroup[] lodGroups = obj.GetComponentsInChildren<LODGroup>();
            allLODGroups.AddRange(lodGroups);
        }

        if (allLODGroups.Count == 0)
        {
            EditorUtility.DisplayDialog("정보", "선택된 객체들에 LOD Group이 없습니다.", "확인");
            return;
        }

        ProcessLODGroupsList(allLODGroups);
    }

    private static void ProcessLODGroupsList(List<LODGroup> lodGroups)
    {
        int processedCount = 0;
        
        Undo.SetCurrentGroupName("Process Multiple LOD Groups");
        int undoGroup = Undo.GetCurrentGroup();

        try
        {
            for (int i = 0; i < lodGroups.Count; i++)
            {
                LODGroup lodGroup = lodGroups[i];
                
                if (lodGroup == null) continue;

                if (EditorUtility.DisplayCancelableProgressBar(
                    "LOD Group 처리 중...", 
                    $"처리 중: {lodGroup.name} ({i + 1}/{lodGroups.Count})", 
                    (float)(i + 1) / lodGroups.Count))
                {
                    break;
                }

                if (ProcessSingleLODGroupUtility(lodGroup))
                {
                    processedCount++;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Undo.CollapseUndoOperations(undoGroup);
        
        Debug.Log($"LOD Group 처리 완료: {processedCount}개 처리됨");
        EditorUtility.DisplayDialog("완료", $"{processedCount}개의 LOD Group이 처리되었습니다.", "확인");
    }

    private static bool ProcessSingleLODGroupUtility(LODGroup lodGroup)
    {
        if (lodGroup == null) return false;

        GameObject lodGroupObject = lodGroup.gameObject;
        Transform lodGroupTransform = lodGroupObject.transform;
        
        LOD[] lods = lodGroup.GetLODs();
        
        if (lods.Length == 0) return false;

        // 가장 낮은 품질의 LOD (마지막 LOD)
        LOD lowestQualityLOD = lods[lods.Length - 1];
        
        if (lowestQualityLOD.renderers == null || lowestQualityLOD.renderers.Length == 0)
            return false;

        Renderer lowestRenderer = lowestQualityLOD.renderers[0];
        if (lowestRenderer == null) return false;

        GameObject lowestLODObject = lowestRenderer.gameObject;
        
        // 새 인스턴스 생성
        GameObject newInstance = Object.Instantiate(lowestLODObject, lodGroupTransform.parent);
        
        Undo.RegisterCreatedObjectUndo(newInstance, "Create LOD Instance");
        
        // 트랜스폼 적용
        newInstance.transform.position = lodGroupTransform.position;
        newInstance.transform.rotation = lodGroupTransform.rotation;
        newInstance.transform.localScale = lodGroupTransform.localScale;
        
        newInstance.name = lodGroupObject.name + "_Processed";
        
        // LOD Group 삭제
        Undo.DestroyObjectImmediate(lodGroupObject);
        
        return true;
    }
}