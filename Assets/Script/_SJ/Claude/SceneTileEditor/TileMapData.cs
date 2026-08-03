using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilemapTool
{
    /// <summary>
    /// 4방향 회전값 (Y축 기준, 90도 단위)
    /// </summary>
    public enum TileDirection
    {
        North = 0, // 0도
        East  = 1, // 90도
        South = 2, // 180도
        West  = 3  // 270도
    }

    /// <summary>
    /// 맵 전체 설정 (XZ 평면 기준, Y는 높이/레이어 오프셋 용도로만 사용)
    /// </summary>
    [Serializable]
    public class TileMapSettings
    {
        [Min(1)] public int width = 20;   // X축 셀 개수
        [Min(1)] public int depth = 20;   // Z축 셀 개수
        [Min(0.01f)] public float tileSize = 1f;

        public Vector3 CellToWorld(int x, int z, float y = 0f)
        {
            return new Vector3(x * tileSize + tileSize * 0.5f, y, z * tileSize + tileSize * 0.5f);
        }

        public bool InBounds(int x, int z)
        {
            return x >= 0 && x < width && z >= 0 && z < depth;
        }
    }

    /// <summary>
    /// 팔레트에 등록되는 하나의 항목: [문자열 id, 프리팹 오브젝트, 유저 정의 int 값]
    /// </summary>
    [Serializable]
    public class TilePaletteEntry
    {
        public string id;
        public GameObject prefab;
        public int userValue;
    }

    /// <summary>
    /// 하나의 배치 정보 (셀 좌표 + 팔레트 참조 id + 방향)
    /// </summary>
    [Serializable]
    public class ObjectPlacement
    {
        public int x;
        public int z;
        public string paletteId;
        public TileDirection direction = TileDirection.North;

        public int userValue;

        /// <summary>
        /// 이 배치가 커스텀 데이터를 갖고 있을 때 맵 그리드 뷰어에서 표시할 강조 색.
        /// customData가 비어있으면 렌더러에서 이 색을 사용하지 않는다.
        /// </summary>
        public Color highlightColor = new Color(1f, 0.25f, 0.9f, 0.55f);

        /// <summary>
        /// 게임마다 자유롭게 붙이는 범용 커스텀 데이터. [string key, object value] 형태.
        /// 지원 값 타입: string, int, float, bool, long, double
        /// (그 외 타입은 저장 시 ToString()으로 문자열화됨 - TilemapBinaryIO 참고)
        /// </summary>
        public Dictionary<string, object> customData = new Dictionary<string, object>();

        public bool HasCustom(string key)
        {
            return customData != null && customData.ContainsKey(key);
        }

        public T GetCustom<T>(string key, T defaultValue = default)
        {
            if (customData != null && customData.TryGetValue(key, out var v) && v is T typed)
                return typed;
            return defaultValue;
        }

        public void SetCustom(string key, object value)
        {
            if (customData == null) customData = new Dictionary<string, object>();
            customData[key] = value;
        }

        public void RemoveCustom(string key)
        {
            customData?.Remove(key);
        }

        public Vector2Int Vector2Int()
        {
            return new Vector2Int( x, z );
        }
    }

    /// <summary>
    /// 하나의 레이어. index 0은 항상 베이스 바닥 레이어(isBaseLayer = true)이며,
    /// 그 외 유저가 자유롭게 추가하는 오브젝트 레이어들이 뒤따른다.
    /// 셀 하나당 배치는 1개만 허용(같은 자리 재배치 시 교체).
    /// </summary>
    [Serializable]
    public class TileLayer
    {
        public string layerName;
        public bool isBaseLayer;
        public float yOffset; // 레이어별 y 오프셋 (겹침 방지 / 시각적 스택용)

        // key = "x_z" -> placement
        [NonSerialized] public Dictionary<string, ObjectPlacement> placements = new Dictionary<string, ObjectPlacement>();

        public static string Key(int x, int z) => x + "_" + z;

        public ObjectPlacement Get(int x, int z)
        {
            placements.TryGetValue(Key(x, z), out var p);
            return p;
        }

        public void Set(ObjectPlacement placement)
        {
            placements[Key(placement.x, placement.z)] = placement;
        }

        public void Remove(int x, int z)
        {
            placements.Remove(Key(x, z));
        }

        public void Clear()
        {
            placements.Clear();
        }
    }
}
