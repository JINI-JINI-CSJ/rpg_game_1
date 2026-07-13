using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileEditor.Runtime
{
    /// <summary>
    /// 타일 값(int)과 실제 Unity TileBase / 에디터 표시 색상을 매핑하는 팔레트.
    /// Tools > Tile Editor 에서 브러쉬 값 선택 및 프리뷰 색상으로 사용된다.
    /// </summary>
    [CreateAssetMenu(fileName = "TilePalette", menuName = "Tile Editor/Tile Palette")]
    public class TilePaletteAsset : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public int Id;
            public string DisplayName = "Tile";
            public TileBase Tile;
            public Color EditorColor = Color.white;
        }

        public List<Entry> Entries = new List<Entry>();

        private Dictionary<int, Entry> _lookup;

        private void OnEnable()
        {
            RebuildLookup();
        }

        public void RebuildLookup()
        {
            _lookup = new Dictionary<int, Entry>();
            foreach (var e in Entries)
            {
                if (!_lookup.ContainsKey(e.Id))
                    _lookup.Add(e.Id, e);
            }
        }

        public Entry GetEntry(int id)
        {
            if (_lookup == null) RebuildLookup();
            _lookup.TryGetValue(id, out var entry);
            return entry;
        }

        public TileBase GetTile(int id)
        {
            var e = GetEntry(id);
            return e?.Tile;
        }

        public Color GetColor(int id)
        {
            var e = GetEntry(id);
            if (e != null) return e.EditorColor;
            // 팔레트에 등록되지 않은 값은 해시 기반 색상으로 대체 표시 (에디터 프리뷰 전용)
            return HashToColor(id);
        }

        public static Color HashToColor(int id)
        {
            if (id == TileEditor.Core.TileLayer.EmptyValue)
                return new Color(0, 0, 0, 0);

            unchecked
            {
                int hash = (int)(id * 2654435761);
                float h = (hash & 0xFFFF) / (float)0xFFFF;
                return Color.HSVToRGB(h, 0.55f, 0.9f);
            }
        }
    }
}
