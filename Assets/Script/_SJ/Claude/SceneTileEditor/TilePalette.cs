using System.Collections.Generic;
using UnityEngine;

namespace TilemapTool
{
    /// <summary>
    /// 팔레트 컨테이너. 인스펙터에서 [id / prefab / userValue] 형태로 등록하거나
    /// 런타임에 AddEntry로 추가 가능.
    /// </summary>
    public class TilePalette : MonoBehaviour
    {
        public List<TilePaletteEntry> entries = new List<TilePaletteEntry>();

        public TilePaletteEntry GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return entries.Find(e => e.id == id);
        }

        public TilePaletteEntry GetByIndex(int index)
        {
            if (index < 0 || index >= entries.Count) return null;
            return entries[index];
        }

        public int IndexOf(string id)
        {
            return entries.FindIndex(e => e.id == id);
        }

        public void AddEntry(string id, GameObject prefab, int userValue)
        {
            if (GetById(id) != null)
            {
                Debug.LogWarning($"[TilePalette] 중복된 id 존재: {id}");
                return;
            }
            entries.Add(new TilePaletteEntry { id = id, prefab = prefab, userValue = userValue });
        }

        public void RemoveEntry(string id)
        {
            entries.RemoveAll(e => e.id == id);
        }
    }
}
