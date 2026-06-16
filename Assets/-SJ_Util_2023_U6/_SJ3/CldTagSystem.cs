using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ─────────────────────────────────────────────────────────────
//  CldTagManager  —  고성능 역방향 인덱스 태그 시스템
//
//  특징
//  ────
//  • 역방향 인덱스 (tag → objects) : 검색 O(1)
//  • 정방향 인덱스 (object → tags) : 태그 제거/객체 해제 O(1)
//  • 내부 자료구조 : Dictionary + HashSet (해시 기반)
//  • 문자열 정확 일치 (StringComparer.Ordinal — 가장 빠른 비교)
//  • 스레드 비안전 버전(기본, 게임 루프용) + 스레드 안전 Wrapper 제공
//  • 제네릭 버전으로 boxing 없는 값형 지원 가능
// ─────────────────────────────────────────────────────────────

namespace CldTagSystem
{
    // =========================================================
    //  핵심 인터페이스
    // =========================================================
    public interface ICldTaggable { }

    // =========================================================
    //  CldTagManager<T>  —  임의 참조형/값형 지원 제네릭 버전
    // =========================================================
    public sealed class CldTagManager<T> where T : class
    {
        // 역방향 인덱스: 태그 → 객체 집합
        private readonly Dictionary<string, HashSet<T>> _tagToObjects;
        // 정방향 인덱스: 객체 → 태그 집합
        private readonly Dictionary<T, HashSet<string>> _objectToTags;

        // 빈 읽기전용 셋 — null 반환 방지용 공유 인스턴스
        private static readonly IReadOnlyCollection<T>      _emptyObjects = Array.Empty<T>();
        private static readonly IReadOnlyCollection<string> _emptyTags    = Array.Empty<string>();

        public CldTagManager(int initialCapacity = 64)
        {
            _tagToObjects  = new Dictionary<string, HashSet<T>>(initialCapacity, StringComparer.Ordinal);
            _objectToTags  = new Dictionary<T, HashSet<string>>(initialCapacity);
        }

        // ── 등록 ──────────────────────────────────────────────

        /// <summary>객체를 매니저에 등록합니다 (태그 없이).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Register(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (!_objectToTags.ContainsKey(obj))
                _objectToTags[obj] = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>객체를 등록하고 초기 태그를 한 번에 추가합니다.</summary>
        public void Register(T obj, params string[] tags)
        {
            Register(obj);
            if (tags != null)
                foreach (var tag in tags) AddTag(obj, tag);
        }

        /// <summary>객체와 매니저의 연결을 완전히 해제합니다.</summary>
        public bool Unregister(T obj)
        {
            if (obj == null) return false;
            if (!_objectToTags.TryGetValue(obj, out var tags)) return false;

            // 역방향 인덱스에서 해당 객체 제거
            foreach (var tag in tags)
            {
                if (_tagToObjects.TryGetValue(tag, out var set))
                {
                    set.Remove(obj);
                    if (set.Count == 0) _tagToObjects.Remove(tag);   // 빈 버킷 정리
                }
            }
            _objectToTags.Remove(obj);
            return true;
        }

        // ── 태그 추가 / 제거 ─────────────────────────────────

        /// <summary>등록된 객체에 태그를 추가합니다.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddTag(T obj, string tag)
        {
            if (obj == null || tag == null) return false;
            if (!_objectToTags.TryGetValue(obj, out var objTags))
            {
                // 미등록 상태라면 자동 등록
                objTags = new HashSet<string>(StringComparer.Ordinal);
                _objectToTags[obj] = objTags;
            }
            if (!objTags.Add(tag)) return false;            // 이미 존재

            if (!_tagToObjects.TryGetValue(tag, out var tagSet))
            {
                tagSet = new HashSet<T>();
                _tagToObjects[tag] = tagSet;
            }
            tagSet.Add(obj);
            return true;
        }

        /// <summary>여러 태그를 한 번에 추가합니다.</summary>
        public void AddTags(T obj, params string[] tags)
        {
            if (tags == null) return;
            foreach (var t in tags) AddTag(obj, t);
        }

        /// <summary>객체에서 특정 태그를 제거합니다.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveTag(T obj, string tag)
        {
            if (obj == null || tag == null) return false;
            if (!_objectToTags.TryGetValue(obj, out var objTags)) return false;
            if (!objTags.Remove(tag)) return false;

            if (_tagToObjects.TryGetValue(tag, out var tagSet))
            {
                tagSet.Remove(obj);
                if (tagSet.Count == 0) _tagToObjects.Remove(tag);
            }
            return true;
        }

        /// <summary>객체의 모든 태그를 제거합니다 (객체는 등록 유지).</summary>
        public void ClearTags(T obj)
        {
            if (!_objectToTags.TryGetValue(obj, out var tags)) return;
            foreach (var tag in tags)
            {
                if (_tagToObjects.TryGetValue(tag, out var set))
                {
                    set.Remove(obj);
                    if (set.Count == 0) _tagToObjects.Remove(tag);
                }
            }
            tags.Clear();
        }

        // ── 검색 O(1) ────────────────────────────────────────

        /// <summary>
        /// 태그를 가진 모든 객체를 반환합니다.
        /// — 반환값은 내부 HashSet의 뷰이므로 수정하지 마세요.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<T> FindAll(string tag)
        {
            return _tagToObjects.TryGetValue(tag, out var set)
                ? set
                : _emptyObjects;
        }

        /// <summary>태그를 가진 첫 번째 객체를 반환합니다 (없으면 null).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Find(string tag)
        {
            if (!_tagToObjects.TryGetValue(tag, out var set) || set.Count == 0)
                return null;
            // HashSet은 foreach가 가장 빠름
            foreach (var obj in set) return obj;
            return null;
        }

        /// <summary>
        /// 모든 지정 태그를 동시에 가진 객체를 반환합니다 (AND 검색).
        /// 가장 작은 버킷을 기준으로 교집합을 구해 최적화합니다.
        /// </summary>
        public HashSet<T> FindWithAll(params string[] tags)
        {
            if (tags == null || tags.Length == 0) return new HashSet<T>();

            // 가장 작은 버킷 찾기
            HashSet<T> smallest = null;
            foreach (var tag in tags)
            {
                if (!_tagToObjects.TryGetValue(tag, out var set))
                    return new HashSet<T>();            // 하나라도 없으면 결과 없음
                if (smallest == null || set.Count < smallest.Count)
                    smallest = set;
            }

            // smallest를 복사 후 나머지로 교집합
            var result = new HashSet<T>(smallest);
            foreach (var tag in tags)
            {
                if (!_tagToObjects.TryGetValue(tag, out var set)) return new HashSet<T>();
                if (ReferenceEquals(set, smallest)) continue;
                result.IntersectWith(set);
                if (result.Count == 0) return result;   // 조기 종료
            }
            return result;
        }

        /// <summary>
        /// 지정 태그 중 하나 이상 가진 객체를 반환합니다 (OR 검색).
        /// </summary>
        public HashSet<T> FindWithAny(params string[] tags)
        {
            var result = new HashSet<T>();
            if (tags == null) return result;
            foreach (var tag in tags)
            {
                if (_tagToObjects.TryGetValue(tag, out var set))
                    result.UnionWith(set);
            }
            return result;
        }

        // ── 조회 헬퍼 ────────────────────────────────────────

        /// <summary>객체가 해당 태그를 가지고 있는지 확인합니다. O(1)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTag(T obj, string tag)
        {
            return _objectToTags.TryGetValue(obj, out var tags) && tags.Contains(tag);
        }

        /// <summary>객체의 모든 태그를 반환합니다.</summary>
        public IReadOnlyCollection<string> GetTags(T obj)
        {
            return _objectToTags.TryGetValue(obj, out var tags)
                ? tags
                : _emptyTags;
        }

        /// <summary>해당 태그를 가진 객체 수를 반환합니다. O(1)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountByTag(string tag)
        {
            return _tagToObjects.TryGetValue(tag, out var set) ? set.Count : 0;
        }

        /// <summary>등록된 총 객체 수</summary>
        public int ObjectCount => _objectToTags.Count;

        /// <summary>존재하는 고유 태그 수</summary>
        public int TagCount => _tagToObjects.Count;

        /// <summary>모든 객체 제거</summary>
        public void Clear()
        {
            _tagToObjects.Clear();
            _objectToTags.Clear();
        }
    }

    // =========================================================
    //  CldThreadSafeTagManager<T>  —  lock 기반 스레드 안전 래퍼
    //  (Unity 멀티스레드 로딩, 서버 환경 등)
    // =========================================================
    public sealed class CldThreadSafeTagManager<T> where T : class
    {
        private readonly CldTagManager<T> _inner;
        private readonly object _lock = new object();

        public CldThreadSafeTagManager(int initialCapacity = 64)
            => _inner = new CldTagManager<T>(initialCapacity);

        public void Register(T obj)                    { lock (_lock) _inner.Register(obj); }
        public void Register(T obj, params string[] t) { lock (_lock) _inner.Register(obj, t); }
        public bool Unregister(T obj)                  { lock (_lock) return _inner.Unregister(obj); }
        public bool AddTag(T obj, string tag)          { lock (_lock) return _inner.AddTag(obj, tag); }
        public bool RemoveTag(T obj, string tag)       { lock (_lock) return _inner.RemoveTag(obj, tag); }
        public bool HasTag(T obj, string tag)          { lock (_lock) return _inner.HasTag(obj, tag); }

        // 검색은 결과를 복사해서 반환 (잠금 해제 후 외부에서 사용 가능)
        public List<T> FindAll(string tag)
        {
            lock (_lock)
            {
                var r = _inner.FindAll(tag);
                return new List<T>(r);
            }
        }
        public HashSet<T> FindWithAll(params string[] tags)
        {
            lock (_lock) return _inner.FindWithAll(tags);
        }
    }

    // =========================================================
    //  CldTagManagerGlobal  —  싱글톤 전역 접근 (object 기반)
    //  게임 오브젝트처럼 공통 기반 클래스가 없을 때 사용
    // =========================================================
    public static class CldTagManagerGlobal
    {
        private static readonly CldTagManager<object> _instance = new CldTagManager<object>(256);
        public static CldTagManager<object> Instance => _instance;
    }
}


// ─────────────────────────────────────────────────────────────
//  사용 예시 (컴파일 및 실행 가능한 데모)
// ─────────────────────────────────────────────────────────────
namespace CldTagSystem.Demo
{
    using System.Diagnostics;

    // 게임 오브젝트 예시
    class GameObject
    {
        public string Name { get; }
        public GameObject(string name) => Name = name;
        public override string ToString() => $"[{Name}]";
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== CldTagManager 데모 ===\n");

            var mgr = new CldTagManager<GameObject>(initialCapacity: 128);

            // ── 1. 등록 및 태그 추가 ──────────────────────────
            var hero    = new GameObject("Hero");
            var goblin  = new GameObject("Goblin");
            var goblin2 = new GameObject("Goblin2");
            var chest   = new GameObject("Chest");
            var portal  = new GameObject("Portal");

            mgr.Register(hero,    "Player", "Ally",  "HasPhysics");
            mgr.Register(goblin,  "Enemy",  "HasAI", "HasPhysics");
            mgr.Register(goblin2, "Enemy",  "HasAI", "Sleeping");
            mgr.Register(chest,   "Item",   "Interactable");
            mgr.Register(portal,  "Trigger","Interactable");

            // ── 2. 단일 태그 검색 O(1) ───────────────────────
            Console.WriteLine("── FindAll(\"Enemy\") ──");
            foreach (var obj in mgr.FindAll("Enemy"))
                Console.WriteLine($"  {obj}");

            Console.WriteLine("\n── FindAll(\"Interactable\") ──");
            foreach (var obj in mgr.FindAll("Interactable"))
                Console.WriteLine($"  {obj}");

            // ── 3. AND 검색 ────────────────────────────────
            Console.WriteLine("\n── FindWithAll(\"Enemy\", \"HasAI\") ──");
            foreach (var obj in mgr.FindWithAll("Enemy", "HasAI"))
                Console.WriteLine($"  {obj}");

            // ── 4. OR 검색 ─────────────────────────────────
            Console.WriteLine("\n── FindWithAny(\"Player\", \"Enemy\") ──");
            foreach (var obj in mgr.FindWithAny("Player", "Enemy"))
                Console.WriteLine($"  {obj}");

            // ── 5. 태그 확인 ────────────────────────────────
            Console.WriteLine($"\n── HasTag(hero, \"Player\") = {mgr.HasTag(hero, "Player")}");
            Console.WriteLine($"── HasTag(hero, \"Enemy\")  = {mgr.HasTag(hero, "Enemy")}");

            // ── 6. 태그 런타임 변경 ─────────────────────────
            mgr.AddTag(goblin2, "Alerted");
            mgr.RemoveTag(goblin2, "Sleeping");

            Console.WriteLine("\n── goblin2 깨어남 → Alerted 추가, Sleeping 제거 ──");
            Console.Write("  Tags: ");
            Console.WriteLine(string.Join(", ", mgr.GetTags(goblin2)));

            // ── 7. 해제 ─────────────────────────────────────
            mgr.Unregister(chest);
            Console.WriteLine($"\n── chest 해제 후 FindAll(\"Interactable\").Count = {mgr.FindAll("Interactable").Count}");

            // ── 8. 성능 벤치마크 ────────────────────────────
            Console.WriteLine("\n=== 성능 벤치마크 ===");
            BenchmarkSearch();
        }

        static void BenchmarkSearch()
        {
            const int OBJ_COUNT  = 100_000;
            const int ITER_COUNT = 1_000_000;

            var mgr = new CldTagManager<GameObject>(initialCapacity: OBJ_COUNT * 2);
            var rng = new Random(42);
            string[] possibleTags = { "Enemy", "Ally", "HasAI", "HasPhysics",
                                      "Visible", "Sleeping", "Burning", "Wet" };

            // 10만 객체 등록 (랜덤 태그 2~4개씩)
            for (int i = 0; i < OBJ_COUNT; i++)
            {
                var obj = new GameObject($"Obj_{i}");
                int tagCnt = rng.Next(2, 5);
                var tags = new string[tagCnt];
                for (int j = 0; j < tagCnt; j++)
                    tags[j] = possibleTags[rng.Next(possibleTags.Length)];
                mgr.Register(obj, tags);
            }

            Console.WriteLine($"등록 완료: {mgr.ObjectCount:N0}개 객체 / {mgr.TagCount}개 고유 태그");
            Console.WriteLine($"\"Enemy\" 태그 객체 수: {mgr.CountByTag("Enemy"):N0}");

            // 단일 태그 검색 벤치
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < ITER_COUNT; i++)
                _ = mgr.FindAll("Enemy");
            sw.Stop();
            Console.WriteLine($"\nFindAll x {ITER_COUNT:N0}회: {sw.ElapsedMilliseconds} ms " +
                              $"({(double)sw.ElapsedTicks / ITER_COUNT * 1e6 / Stopwatch.Frequency:F0} ns/op)");

            // AND 검색 벤치
            sw.Restart();
            for (int i = 0; i < ITER_COUNT / 10; i++)
                _ = mgr.FindWithAll("Enemy", "HasAI");
            sw.Stop();
            Console.WriteLine($"FindWithAll(2 tags) x {ITER_COUNT/10:N0}회: {sw.ElapsedMilliseconds} ms");
        }
    }
}
