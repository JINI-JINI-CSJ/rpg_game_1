using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 범용 트리 노드 클래스 (포커스 기능 포함)
/// </summary>
/// <typeparam name="T">노드에 저장할 데이터 타입</typeparam>
public class TreeNode<T>
{
    private readonly List<TreeNode<T>> _children;
    private static TreeNode<T> _focusedNode;

    /// <summary>
    /// 노드의 데이터
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 부모 노드
    /// </summary>
    public TreeNode<T> Parent { get; private set; }

    /// <summary>
    /// 자식 노드들 (읽기 전용)
    /// </summary>
    public IReadOnlyList<TreeNode<T>> Children => _children.AsReadOnly();

    /// <summary>
    /// 루트 노드인지 확인
    /// </summary>
    public bool IsRoot => Parent == null;

    /// <summary>
    /// 리프 노드인지 확인
    /// </summary>
    public bool IsLeaf => _children.Count == 0;

    /// <summary>
    /// 현재 포커스된 노드인지 확인
    /// </summary>
    public bool IsFocused => _focusedNode == this;

    /// <summary>
    /// 현재 포커스된 노드 (전역)
    /// </summary>
    public static TreeNode<T> FocusedNode => _focusedNode;

    /// <summary>
    /// 포커스 가능한 자식 노드들 (현재 포커스된 노드의 직계 자식들)
    /// </summary>
    public IReadOnlyList<TreeNode<T>> FocusableChildren => 
        IsFocused ? Children : new List<TreeNode<T>>();

    /// <summary>
    /// 노드의 깊이 (루트부터의 거리)
    /// </summary>
    public int Depth
    {
        get
        {
            int depth = 0;
            TreeNode<T> current = Parent;
            while (current != null)
            {
                depth++;
                current = current.Parent;
            }
            return depth;
        }
    }

    /// <summary>
    /// 트리의 높이 (현재 노드부터 가장 깊은 리프까지의 거리)
    /// </summary>
    public int Height
    {
        get
        {
            if (IsLeaf) return 0;
            return _children.Max(child => child.Height) + 1;
        }
    }

    /// <summary>
    /// 포커스 변경 시 발생하는 이벤트 (루트 노드에서만 사용)
    /// </summary>
    public event Action<TreeNode<T>> OnFocusChanged;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="data">노드에 저장할 데이터</param>
    public TreeNode(T data)
    {
        Data = data;
        _children = new List<TreeNode<T>>();
    }

    /// <summary>
    /// 자식 노드 추가
    /// </summary>
    /// <param name="child">추가할 자식 노드</param>
    /// <returns>추가된 자식 노드</returns>
    public TreeNode<T> AddChild(TreeNode<T> child)
    {
        if (child == null)
            throw new ArgumentNullException(nameof(child));

        if (child.Parent != null)
            child.Parent.RemoveChild(child);

        child.Parent = this;
        _children.Add(child);
        return child;
    }

    /// <summary>
    /// 새로운 자식 노드를 데이터와 함께 추가
    /// </summary>
    /// <param name="data">자식 노드의 데이터</param>
    /// <returns>생성된 자식 노드</returns>
    public TreeNode<T> AddChild(T data)
    {
        var child = new TreeNode<T>(data);
        return AddChild(child);
    }

    /// <summary>
    /// 자식 노드 제거
    /// </summary>
    /// <param name="child">제거할 자식 노드</param>
    /// <returns>제거 성공 여부</returns>
    public bool RemoveChild(TreeNode<T> child)
    {
        if (child == null) return false;

        if (_children.Remove(child))
        {
            child.Parent = null;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 모든 자식 노드 제거
    /// </summary>
    public void ClearChildren()
    {
        foreach (var child in _children)
        {
            child.Parent = null;
        }
        _children.Clear();
    }

    /// <summary>
    /// 특정 데이터를 가진 자식 노드 찾기
    /// </summary>
    /// <param name="data">찾을 데이터</param>
    /// <returns>찾은 자식 노드 (없으면 null)</returns>
    public TreeNode<T> FindChild(T data)
    {
        return _children.FirstOrDefault(child => EqualityComparer<T>.Default.Equals(child.Data, data));
    }

    /// <summary>
    /// 조건에 맞는 자식 노드 찾기
    /// </summary>
    /// <param name="predicate">조건</param>
    /// <returns>찾은 자식 노드 (없으면 null)</returns>
    public TreeNode<T> FindChild(Func<T, bool> predicate)
    {
        return _children.FirstOrDefault(child => predicate(child.Data));
    }

    /// <summary>
    /// 루트 노드 가져오기
    /// </summary>
    /// <returns>루트 노드</returns>
    public TreeNode<T> GetRoot()
    {
        TreeNode<T> current = this;
        while (current.Parent != null)
        {
            current = current.Parent;
        }
        return current;
    }

    /// <summary>
    /// 현재 노드부터 루트까지의 경로
    /// </summary>
    /// <returns>경로 상의 노드들</returns>
    public IEnumerable<TreeNode<T>> GetPath()
    {
        var path = new List<TreeNode<T>>();
        TreeNode<T> current = this;
        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// 깊이 우선 탐색 (DFS)
    /// </summary>
    /// <returns>DFS 순서로 방문한 노드들</returns>
    public IEnumerable<TreeNode<T>> DepthFirstSearch()
    {
        yield return this;
        foreach (var child in _children)
        {
            foreach (var descendant in child.DepthFirstSearch())
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// 너비 우선 탐색 (BFS)
    /// </summary>
    /// <returns>BFS 순서로 방문한 노드들</returns>
    public IEnumerable<TreeNode<T>> BreadthFirstSearch()
    {
        var queue = new Queue<TreeNode<T>>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (var child in current._children)
            {
                queue.Enqueue(child);
            }
        }
    }

    /// <summary>
    /// 특정 데이터를 가진 노드를 전체 트리에서 찾기
    /// </summary>
    /// <param name="data">찾을 데이터</param>
    /// <returns>찾은 노드 (없으면 null)</returns>
    public TreeNode<T> Find(T data)
    {
        return DepthFirstSearch().FirstOrDefault(node => 
            EqualityComparer<T>.Default.Equals(node.Data, data));
    }

    /// <summary>
    /// 조건에 맞는 노드를 전체 트리에서 찾기
    /// </summary>
    /// <param name="predicate">조건</param>
    /// <returns>찾은 노드 (없으면 null)</returns>
    public TreeNode<T> Find(Func<T, bool> predicate)
    {
        return DepthFirstSearch().FirstOrDefault(node => predicate(node.Data));
    }

    /// <summary>
    /// 조건에 맞는 모든 노드를 전체 트리에서 찾기
    /// </summary>
    /// <param name="predicate">조건</param>
    /// <returns>찾은 노드들</returns>
    public IEnumerable<TreeNode<T>> FindAll(Func<T, bool> predicate)
    {
        return DepthFirstSearch().Where(node => predicate(node.Data));
    }

    /// <summary>
    /// 현재 노드를 포커스로 설정
    /// </summary>
    public void SetFocus()
    {
        _focusedNode = this;
        
        // 루트 노드의 이벤트 호출
        var root = GetRoot();
        root.OnFocusChanged?.Invoke(this);
    }

    /// <summary>
    /// 루트 노드를 포커스로 설정
    /// </summary>
    public void FocusRoot()
    {
        GetRoot().SetFocus();
    }

    /// <summary>
    /// 지정된 인덱스의 자식 노드로 포커스 이동 (현재 노드가 포커스된 상태에서만 가능)
    /// </summary>
    /// <param name="childIndex">자식 노드의 인덱스</param>
    /// <returns>포커스 이동 성공 여부</returns>
    public bool FocusChild(int childIndex)
    {
        if (!IsFocused || childIndex < 0 || childIndex >= _children.Count)
            return false;

        _children[childIndex].SetFocus();
        return true;
    }

    /// <summary>
    /// 특정 데이터를 가진 자식 노드로 포커스 이동
    /// </summary>
    /// <param name="childData">자식 노드의 데이터</param>
    /// <returns>포커스 이동 성공 여부</returns>
    public bool FocusChild(T childData)
    {
        if (!IsFocused) return false;

        var child = FindChild(childData);
        if (child != null)
        {
            child.SetFocus();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 조건에 맞는 자식 노드로 포커스 이동
    /// </summary>
    /// <param name="predicate">조건</param>
    /// <returns>포커스 이동 성공 여부</returns>
    public bool FocusChild(Func<T, bool> predicate)
    {
        if (!IsFocused) return false;

        var child = FindChild(predicate);
        if (child != null)
        {
            child.SetFocus();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 부모 노드로 포커스 이동 (현재 노드가 포커스된 상태에서만 가능)
    /// </summary>
    /// <returns>포커스 이동 성공 여부</returns>
    public bool FocusParent()
    {
        if (!IsFocused || Parent == null)
            return false;

        Parent.SetFocus();
        return true;
    }

    /// <summary>
    /// 현재 포커스된 노드의 탐색 가능한 자식 노드들 가져오기
    /// </summary>
    /// <returns>탐색 가능한 자식 노드들</returns>
    public static IReadOnlyList<TreeNode<T>> GetFocusableChildren()
    {
        return _focusedNode?.Children ?? new List<TreeNode<T>>();
    }

    /// <summary>
    /// 현재 포커스된 노드에서 부모로 이동 가능한지 확인
    /// </summary>
    /// <returns>부모로 이동 가능 여부</returns>
    public static bool CanFocusParent()
    {
        return _focusedNode?.Parent != null;
    }

    /// <summary>
    /// 현재 포커스 상태를 문자열로 표현 (메뉴 탐색용)
    /// </summary>
    /// <returns>현재 포커스 상태 정보</returns>
    public static string GetFocusStatus()
    {
        if (_focusedNode == null)
            return "No node is focused";

        var status = $"Focused: {_focusedNode.Data}";
        
        if (_focusedNode.Children.Count > 0)
        {
            status += "\nAvailable options:";
            for (int i = 0; i < _focusedNode.Children.Count; i++)
            {
                status += $"\n  [{i}] {_focusedNode.Children[i].Data}";
            }
        }
        
        if (_focusedNode.Parent != null)
        {
            status += $"\nParent: {_focusedNode.Parent.Data} (Press 'b' to go back)";
        }
        
        return status;
    }

    /// <summary>
    /// 트리를 문자열로 표현
    /// </summary>
    /// <param name="indent">들여쓰기</param>
    /// <returns>트리 구조를 나타내는 문자열</returns>
    public string ToString(string indent = "")
    {
        var result = indent + Data?.ToString() + Environment.NewLine;
        foreach (var child in _children)
        {
            result += child.ToString(indent + "  ");
        }
        return result;
    }

    /// <summary>
    /// 루트 노드의 포커스 변경 이벤트에 핸들러 등록
    /// </summary>
    /// <param name="handler">이벤트 핸들러</param>
    public void SubscribeToFocusChanged(Action<TreeNode<T>> handler)
    {
        var root = GetRoot();
        root.OnFocusChanged += handler;
    }

    /// <summary>
    /// 루트 노드의 포커스 변경 이벤트에서 핸들러 제거
    /// </summary>
    /// <param name="handler">이벤트 핸들러</param>
    public void UnsubscribeFromFocusChanged(Action<TreeNode<T>> handler)
    {
        var root = GetRoot();
        root.OnFocusChanged -= handler;
    }

    public override string ToString()
    {
        return Data?.ToString() ?? "null";
    }
}