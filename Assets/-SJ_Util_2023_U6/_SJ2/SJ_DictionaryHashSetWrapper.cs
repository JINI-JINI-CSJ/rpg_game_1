using System;
using System.Collections.Generic;
using System.Linq;

public class SJ_DictionaryHashSetWrapper<TKey, TValue>
{
    private readonly Dictionary<TKey, HashSet<TValue>> _dictionary;

    public SJ_DictionaryHashSetWrapper()
    {
        _dictionary = new Dictionary<TKey, HashSet<TValue>>();
    }

    public SJ_DictionaryHashSetWrapper(IEqualityComparer<TKey> keyComparer)
    {
        _dictionary = new Dictionary<TKey, HashSet<TValue>>(keyComparer);
    }

    /// <summary>
    /// 지정된 키에 값을 추가합니다.
    /// </summary>
    /// <param name="key">키</param>
    /// <param name="value">추가할 값</param>
    /// <returns>값이 성공적으로 추가되었으면 true, 이미 존재하면 false</returns>
    public bool Add(TKey key, TValue value)
    {
        if (!_dictionary.ContainsKey(key))
        {
            _dictionary[key] = new HashSet<TValue>();
        }
        
        return _dictionary[key].Add(value);
    }

    /// <summary>
    /// 지정된 키에서 값을 제거합니다.
    /// </summary>
    /// <param name="key">키</param>
    /// <param name="value">제거할 값</param>
    /// <returns>값이 성공적으로 제거되었으면 true, 존재하지 않으면 false</returns>
    public bool Remove(TKey key, TValue value)
    {
        if (!_dictionary.ContainsKey(key))
            return false;

        bool removed = _dictionary[key].Remove(value);
        
        // HashSet이 비어있으면 키 자체를 제거
        if (_dictionary[key].Count == 0)
        {
            _dictionary.Remove(key);
        }
        
        return removed;
    }

    /// <summary>
    /// 지정된 키를 완전히 제거합니다.
    /// </summary>
    /// <param name="key">제거할 키</param>
    /// <returns>키가 성공적으로 제거되었으면 true, 존재하지 않으면 false</returns>
    public bool RemoveKey(TKey key)
    {
        return _dictionary.Remove(key);
    }

    /// <summary>
    /// 지정된 키와 값이 존재하는지 확인합니다.
    /// </summary>
    /// <param name="key">키</param>
    /// <param name="value">찾을 값</param>
    /// <returns>키와 값이 존재하면 true, 그렇지 않으면 false</returns>
    public bool Find(TKey key, TValue value)
    {
        return _dictionary.ContainsKey(key) && _dictionary[key].Contains(value);
    }

    /// <summary>
    /// 지정된 키가 존재하는지 확인합니다.
    /// </summary>
    /// <param name="key">찾을 키</param>
    /// <returns>키가 존재하면 true, 그렇지 않으면 false</returns>
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// 지정된 키에 해당하는 HashSet을 반환합니다.
    /// </summary>
    /// <param name="key">키</param>
    /// <returns>HashSet 또는 null (키가 존재하지 않는 경우)</returns>
    public HashSet<TValue> GetHashSet(TKey key)
    {
        return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
    }

    /// <summary>
    /// 지정된 키에 해당하는 HashSet의 개수를 반환합니다.
    /// </summary>
    /// <param name="key">키</param>
    /// <returns>HashSet의 개수, 키가 존재하지 않으면 0</returns>
    public int GetCount(TKey key)
    {
        return _dictionary.ContainsKey(key) ? _dictionary[key].Count : 0;
    }

    /// <summary>
    /// 전체 키의 개수를 반환합니다.
    /// </summary>
    public int KeyCount => _dictionary.Count;

    /// <summary>
    /// 모든 값의 총 개수를 반환합니다.
    /// </summary>
    public int TotalValueCount => _dictionary.Values.Sum(hashSet => hashSet.Count);

    /// <summary>
    /// 모든 키를 반환합니다.
    /// </summary>
    public IEnumerable<TKey> Keys => _dictionary.Keys;

    /// <summary>
    /// 지정된 키의 모든 값을 반환합니다.
    /// </summary>
    /// <param name="key">키</param>
    /// <returns>값들의 열거형, 키가 존재하지 않으면 빈 열거형</returns>
    public IEnumerable<TValue> GetValues(TKey key)
    {
        return _dictionary.ContainsKey(key) ? _dictionary[key] : Enumerable.Empty<TValue>();
    }

    /// <summary>
    /// 모든 데이터를 제거합니다.
    /// </summary>
    public void Clear()
    {
        _dictionary.Clear();
    }

    /// <summary>
    /// 지정된 키의 모든 값을 제거합니다 (키는 유지).
    /// </summary>
    /// <param name="key">키</param>
    public void ClearValues(TKey key)
    {
        if (_dictionary.ContainsKey(key))
        {
            _dictionary[key].Clear();
        }
    }
}

// // 사용 예제
// public class Example
// {
//     public static void Demo()
//     {
//         var wrapper = new SJ_DictionaryHashSetWrapper<string, int>();
        
//         // 값 추가
//         wrapper.Add("group1", 1);
//         wrapper.Add("group1", 2);
//         wrapper.Add("group1", 3);
//         wrapper.Add("group2", 4);
//         wrapper.Add("group2", 5);
        
//         // 값 찾기
//         Console.WriteLine($"group1에 2가 있는가? {wrapper.Find("group1", 2)}"); // True
//         Console.WriteLine($"group2에 1이 있는가? {wrapper.Find("group2", 1)}"); // False
        
//         // 개수 확인
//         Console.WriteLine($"group1의 값 개수: {wrapper.GetCount("group1")}"); // 3
//         Console.WriteLine($"group2의 값 개수: {wrapper.GetCount("group2")}"); // 2
        
//         // 값 제거
//         wrapper.Remove("group1", 2);
//         Console.WriteLine($"제거 후 group1의 값 개수: {wrapper.GetCount("group1")}"); // 2
        
//         // 전체 통계
//         Console.WriteLine($"총 키 개수: {wrapper.KeyCount}"); // 2
//         Console.WriteLine($"총 값 개수: {wrapper.TotalValueCount}"); // 4
//     }
// }