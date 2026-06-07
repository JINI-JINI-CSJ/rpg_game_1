using UnityEngine;
using UnityEditor;
using System.Linq;

public class HierarchySorter : Editor
{
    [MenuItem("GameObject/정렬/하위 오브젝트 이름순 정렬", false, 0)]
    private static void SortChildrenByName()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("정렬 실패", "씬에서 부모 오브젝트를 선택하세요.", "확인");
            return;
        }

        // 현재 부모의 자식 Transform들을 리스트로 가져오기
        var children = selected.transform.Cast<Transform>().ToList();

        if (children.Count == 0)
        {
            EditorUtility.DisplayDialog("정렬 실패", "선택한 오브젝트에 자식이 없습니다.", "확인");
            return;
        }

        // 이름순 정렬
        var sorted = children.OrderBy(t => t.name, System.StringComparer.Ordinal).ToList();

        // Hierarchy 내 순서를 바꾸려면 SetSiblingIndex 사용
        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].SetSiblingIndex(i);
        }

        Debug.Log($"'{selected.name}'의 자식 오브젝트들을 이름순으로 정렬했습니다.");
    }
}
