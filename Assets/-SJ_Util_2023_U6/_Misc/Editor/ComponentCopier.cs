using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class ComponentCopier : EditorWindow
{
    [MenuItem("Tools/Component Copier")]
    static void Init()
    {
        ComponentCopier window = (ComponentCopier)EditorWindow.GetWindow(typeof(ComponentCopier));
        window.titleContent = new GUIContent("Component Copier");
        window.Show();
    }

    private GameObject sourceObject;
    private List<GameObject> targetObjects = new List<GameObject>();
    private Vector2 scrollPosition;
    private Vector2 targetScrollPosition;
    private Dictionary<System.Type, bool> componentSelections = new Dictionary<System.Type, bool>();
    private List<Component> sourceComponents = new List<Component>();
    
    // 복사 옵션들
    private bool copyTransform = false;
    private bool overwriteExisting = true;
    private bool copyValues = true;
    private bool copyReferences = true;
    private bool selectAllComponents = false;
    
    // 제외할 컴포넌트 타입들
    private static readonly System.Type[] excludedTypes = {
        typeof(Transform),
        typeof(RectTransform)  // Transform은 별도 옵션으로 처리
    };

    void OnGUI()
    {
        GUILayout.Label("Component Copier Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 소스와 타겟 오브젝트 선택
        DrawObjectSelection();
        
        EditorGUILayout.Space();
        
        // 복사 옵션들
        DrawCopyOptions();
        
        EditorGUILayout.Space();

        // 컴포넌트 목록 표시 및 선택
        if (sourceObject != null)
        {
            DrawComponentList();
        }

        EditorGUILayout.Space();

        // 복사 버튼
        DrawCopyButtons();
    }

    void DrawObjectSelection()
    {
        EditorGUILayout.LabelField("Object Selection", EditorStyles.boldLabel);
        
        GameObject newSource = (GameObject)EditorGUILayout.ObjectField("Source Object", sourceObject, typeof(GameObject), true);
        if (newSource != sourceObject)
        {
            sourceObject = newSource;
            RefreshComponentList();
        }

        EditorGUILayout.Space();

        // 타겟 오브젝트 목록
        EditorGUILayout.LabelField($"Target Objects ({targetObjects.Count})", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Selected GameObjects"))
        {
            AddSelectedGameObjects();
        }
        if (GUILayout.Button("Clear All"))
        {
            targetObjects.Clear();
        }
        EditorGUILayout.EndHorizontal();

        // 단일 오브젝트 추가
        EditorGUILayout.BeginHorizontal();
        GameObject singleTarget = (GameObject)EditorGUILayout.ObjectField("Add Single Target", null, typeof(GameObject), true);
        if (singleTarget != null && !targetObjects.Contains(singleTarget) && singleTarget != sourceObject)
        {
            targetObjects.Add(singleTarget);
        }
        EditorGUILayout.EndHorizontal();

        // 타겟 오브젝트 목록 표시
        if (targetObjects.Count > 0)
        {
            targetScrollPosition = EditorGUILayout.BeginScrollView(targetScrollPosition, GUILayout.Height(150));
            
            for (int i = targetObjects.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                
                targetObjects[i] = (GameObject)EditorGUILayout.ObjectField(targetObjects[i], typeof(GameObject), true);
                
                // 소스 오브젝트와 같으면 제거
                if (targetObjects[i] == sourceObject)
                {
                    targetObjects.RemoveAt(i);
                    continue;
                }
                
                // null이면 제거
                if (targetObjects[i] == null)
                {
                    targetObjects.RemoveAt(i);
                    continue;
                }
                
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    targetObjects.RemoveAt(i);
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("No target objects selected. Add target objects to copy components to.", MessageType.Info);
        }

        // 소스와 타겟이 겹치는 경우 경고
        if (sourceObject != null && targetObjects.Contains(sourceObject))
        {
            EditorGUILayout.HelpBox("Source object cannot be in target list!", MessageType.Warning);
            targetObjects.Remove(sourceObject);
        }
    }

    void DrawCopyOptions()
    {
        EditorGUILayout.LabelField("Copy Options", EditorStyles.boldLabel);
        
        copyTransform = EditorGUILayout.Toggle("Copy Transform", copyTransform);
        overwriteExisting = EditorGUILayout.Toggle("Overwrite Existing Components", overwriteExisting);
        copyValues = EditorGUILayout.Toggle("Copy Values", copyValues);
        copyReferences = EditorGUILayout.Toggle("Copy References", copyReferences);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All"))
        {
            SetAllComponentsSelection(true);
        }
        if (GUILayout.Button("Deselect All"))
        {
            SetAllComponentsSelection(false);
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawComponentList()
    {
        EditorGUILayout.LabelField($"Components on {sourceObject.name}", EditorStyles.boldLabel);
        
        if (sourceComponents.Count == 0)
        {
            EditorGUILayout.HelpBox("No components found on source object.", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        
        foreach (Component comp in sourceComponents)
        {
            if (comp == null) continue;
            
            System.Type compType = comp.GetType();
            
            // Transform은 별도 옵션으로 처리되므로 제외
            if (excludedTypes.Contains(compType)) continue;
            
            bool isSelected = componentSelections.ContainsKey(compType) ? componentSelections[compType] : false;
            
            EditorGUILayout.BeginHorizontal();
            
            bool newSelection = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
            componentSelections[compType] = newSelection;
            
            // 컴포넌트 아이콘과 이름
            GUIContent content = EditorGUIUtility.ObjectContent(comp, compType);
            EditorGUILayout.LabelField(content, GUILayout.Height(18));
            
            // 대상 오브젝트들 중 이미 있는 컴포넌트 개수 표시
            int existingCount = 0;
            foreach (GameObject target in targetObjects)
            {
                if (target != null && target.GetComponent(compType) != null)
                {
                    existingCount++;
                }
            }
            
            if (existingCount > 0)
            {
                EditorGUILayout.LabelField($"({existingCount}/{targetObjects.Count})", EditorStyles.miniLabel, GUILayout.Width(50));
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
    }

    void DrawCopyButtons()
    {
        GUI.enabled = sourceObject != null && targetObjects.Count > 0 && !targetObjects.Contains(sourceObject);
        
        if (GUILayout.Button($"Copy Selected Components to {targetObjects.Count} Objects", GUILayout.Height(30)))
        {
            CopySelectedComponents();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button($"Copy All Components to {targetObjects.Count} Objects", GUILayout.Height(25)))
        {
            CopyAllComponents();
        }
        
        GUI.enabled = true;
    }

    void AddSelectedGameObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        int addedCount = 0;
        
        foreach (GameObject obj in selectedObjects)
        {
            if (obj != sourceObject && !targetObjects.Contains(obj))
            {
                targetObjects.Add(obj);
                addedCount++;
            }
        }
        
        if (addedCount > 0)
        {
            Debug.Log($"Added {addedCount} target objects");
        }
        else
        {
            EditorUtility.DisplayDialog("No Objects Added", "No new valid objects were selected or they were already in the list.", "OK");
        }
    }

    void RefreshComponentList()
    {
        sourceComponents.Clear();
        componentSelections.Clear();
        
        if (sourceObject == null) return;
        
        sourceComponents = sourceObject.GetComponents<Component>().ToList();
        
        // 기본적으로 모든 컴포넌트 선택 (Transform 제외)
        foreach (Component comp in sourceComponents)
        {
            if (comp != null && !excludedTypes.Contains(comp.GetType()))
            {
                componentSelections[comp.GetType()] = true;
            }
        }
    }

    void SetAllComponentsSelection(bool selected)
    {
        List<System.Type> keys = new List<System.Type>(componentSelections.Keys);
        foreach (System.Type key in keys)
        {
            componentSelections[key] = selected;
        }
    }

    void CopySelectedComponents()
    {
        List<System.Type> selectedTypes = componentSelections.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        CopyComponents(selectedTypes);
    }

    void CopyAllComponents()
    {
        List<System.Type> allTypes = sourceComponents.Where(c => c != null && !excludedTypes.Contains(c.GetType())).Select(c => c.GetType()).ToList();
        CopyComponents(allTypes);
    }

    void CopyComponents(List<System.Type> componentTypes)
    {
        if (sourceObject == null || targetObjects.Count == 0) return;

        // 모든 타겟 오브젝트에 대해 Undo 기록
        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                Undo.RecordObject(target, "Copy Components");
            }
        }
        
        int totalCopied = 0;
        int totalSkipped = 0;
        int totalErrors = 0;
        List<string> errors = new List<string>();
        Dictionary<string, int> copyResults = new Dictionary<string, int>();

        // 진행률 표시
        int totalOperations = targetObjects.Count * (componentTypes.Count + (copyTransform ? 1 : 0));
        int currentOperation = 0;

        foreach (GameObject targetObject in targetObjects)
        {
            if (targetObject == null) continue;

            int copiedCount = 0;
            int skippedCount = 0;
            List<string> targetErrors = new List<string>();

            // Transform 복사 (옵션이 켜져있는 경우)
            if (copyTransform)
            {
                EditorUtility.DisplayProgressBar("Copying Components", 
                    $"Copying Transform to {targetObject.name}...", 
                    (float)currentOperation / totalOperations);
                
                CopyTransformComponent(targetObject);
                currentOperation++;
            }

            // 선택된 컴포넌트들 복사
            foreach (System.Type componentType in componentTypes)
            {
                EditorUtility.DisplayProgressBar("Copying Components", 
                    $"Copying {componentType.Name} to {targetObject.name}...", 
                    (float)currentOperation / totalOperations);

                try
                {
                    Component sourceComp = sourceObject.GetComponent(componentType);
                    if (sourceComp == null) continue;

                    Component existingComp = targetObject.GetComponent(componentType);
                    
                    // 이미 존재하는 컴포넌트 처리
                    if (existingComp != null)
                    {
                        if (overwriteExisting)
                        {
                            if (CopyComponentValues(sourceComp, existingComp))
                            {
                                copiedCount++;
                                Debug.Log($"Updated existing {componentType.Name} on {targetObject.name}");
                            }
                            else
                            {
                                skippedCount++;
                            }
                        }
                        else
                        {
                            skippedCount++;
                            Debug.Log($"Skipped {componentType.Name} - already exists on {targetObject.name}");
                        }
                    }
                    else
                    {
                        // 새 컴포넌트 추가
                        if (CopyComponent(sourceComp, targetObject))
                        {
                            copiedCount++;
                            Debug.Log($"Added {componentType.Name} to {targetObject.name}");
                        }
                        else
                        {
                            skippedCount++;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    targetErrors.Add($"{componentType.Name}: {e.Message}");
                    skippedCount++;
                    totalErrors++;
                }
                
                currentOperation++;
            }

            // 개별 결과 저장
            copyResults[targetObject.name] = copiedCount;
            totalCopied += copiedCount;
            totalSkipped += skippedCount;
            errors.AddRange(targetErrors.Select(e => $"{targetObject.name} - {e}"));

            EditorUtility.SetDirty(targetObject);
        }

        EditorUtility.ClearProgressBar();

        // 결과 출력
        string resultMessage = $"Component Copy Complete!\n\n" +
                             $"Total Objects: {targetObjects.Count}\n" +
                             $"Total Copied: {totalCopied}\n" +
                             $"Total Skipped: {totalSkipped}";

        if (totalErrors > 0)
        {
            resultMessage += $"\nTotal Errors: {totalErrors}";
        }

        resultMessage += "\n\nPer Object Results:\n";
        foreach (var result in copyResults)
        {
            resultMessage += $"• {result.Key}: {result.Value} components copied\n";
        }
        
        if (errors.Count > 0)
        {
            resultMessage += $"\n\nDetailed Errors:\n{string.Join("\n", errors.Take(10))}";
            if (errors.Count > 10)
            {
                resultMessage += $"\n... and {errors.Count - 10} more errors (check console for details)";
            }
        }

        EditorUtility.DisplayDialog("Copy Complete", resultMessage, "OK");
        
        // 상세 에러는 콘솔에 출력
        if (errors.Count > 10)
        {
            foreach (string error in errors.Skip(10))
            {
                Debug.LogWarning($"Component Copy Error: {error}");
            }
        }
    }

    bool CopyComponent(Component sourceComponent, GameObject targetObject)
    {
        try
        {
            System.Type componentType = sourceComponent.GetType();
            Component newComponent = targetObject.AddComponent(componentType);
            
            if (copyValues)
            {
                return CopyComponentValues(sourceComponent, newComponent);
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to copy component {sourceComponent.GetType().Name} to {targetObject.name}: {e.Message}");
            return false;
        }
    }

    bool CopyComponentValues(Component source, Component target)
    {
        try
        {
            if (source.GetType() != target.GetType())
            {
                Debug.LogError("Component types don't match!");
                return false;
            }

            SerializedObject sourceObj = new SerializedObject(source);
            SerializedObject targetObj = new SerializedObject(target);

            SerializedProperty sourceProperty = sourceObj.GetIterator();
            
            while (sourceProperty.NextVisible(true))
            {
                // m_Script 프로퍼티는 건드리지 않음
                if (sourceProperty.name == "m_Script") continue;

                SerializedProperty targetProperty = targetObj.FindProperty(sourceProperty.propertyPath);
                if (targetProperty != null && targetProperty.editable)
                {
                    // 레퍼런스 복사 옵션에 따라 처리
                    if (!copyReferences && sourceProperty.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    targetProperty.serializedObject.CopyFromSerializedProperty(sourceProperty);
                }
            }

            targetObj.ApplyModifiedProperties();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to copy values for {source.GetType().Name}: {e.Message}");
            return false;
        }
    }

    void CopyTransformComponent(GameObject targetObject)
    {
        if (sourceObject == null || targetObject == null) return;

        try
        {
            Transform sourceTransform = sourceObject.transform;
            Transform targetTransform = targetObject.transform;

            Undo.RecordObject(targetTransform, "Copy Transform");

            targetTransform.localPosition = sourceTransform.localPosition;
            targetTransform.localRotation = sourceTransform.localRotation;
            targetTransform.localScale = sourceTransform.localScale;

            Debug.Log($"Copied Transform from {sourceObject.name} to {targetObject.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to copy Transform to {targetObject.name}: {e.Message}");
        }
    }

    // 컨텍스트 메뉴를 통한 빠른 복사
    [MenuItem("CONTEXT/Component/Copy All Components From This Object")]
    static void CopyAllComponentsFromContext(MenuCommand command)
    {
        Component component = command.context as Component;
        if (component != null)
        {
            ComponentCopier window = GetWindow<ComponentCopier>();
            window.sourceObject = component.gameObject;
            window.RefreshComponentList();
            window.Focus();
        }
    }

    [MenuItem("GameObject/Copy All Components", false, 0)]
    static void CopyAllComponentsFromGameObject()
    {
        if (Selection.activeGameObject != null)
        {
            ComponentCopier window = GetWindow<ComponentCopier>();
            window.sourceObject = Selection.activeGameObject;
            window.RefreshComponentList();
            
            // 선택된 다른 오브젝트들을 타겟으로 자동 추가
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 1)
            {
                foreach (GameObject obj in selectedObjects)
                {
                    if (obj != window.sourceObject && !window.targetObjects.Contains(obj))
                    {
                        window.targetObjects.Add(obj);
                    }
                }
            }
            
            window.Focus();
        }
    }
}