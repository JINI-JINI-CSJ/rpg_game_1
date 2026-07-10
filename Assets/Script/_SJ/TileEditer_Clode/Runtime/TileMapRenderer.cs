using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileEditor.Core;

namespace TileEditor.Runtime
{
    /// <summary>
    /// TileMapData를 Unity Tilemap 컴포넌트들로 렌더링한다.
    /// 레이어마다 자식 GameObject(Grid 하위 Tilemap)를 자동 생성/동기화한다.
    ///
    /// [대안 렌더링 방식에 대한 메모]
    /// Unity Tilemap은 내장 컬링/배칭이 되어 있어 일반적인 맵 크기(수백~수천 셀)에는 충분히 빠르다.
    /// 다만 맵이 매우 커지거나(예: 수만 셀 이상) 매 프레임 대량의 SetTile 갱신이 필요한 경우,
    /// 청크 단위 Mesh + 텍스처 아틀라스 + GPU 인스턴싱으로 직접 그리는 방식이 더 유리할 수 있다.
    /// 이 경우 청크(예: 32x32)별로 하나의 Mesh/Material Property Block을 만들고,
    /// 변경된 청크만 다시 굽는(rebake) 구조로 가면 대규모 맵에서도 드로우콜을 크게 줄일 수 있다.
    /// 지금 구조는 TileMapData(순수 데이터)와 렌더러가 분리되어 있으므로,
    /// 필요해지면 이 클래스만 교체하여 청크 메쉬 렌더러로 바꿀 수 있다.
    /// </summary>
    [ExecuteAlways]
    public class TileMapRenderer : MonoBehaviour
    {
        public TilePaletteAsset Palette;
        public Grid TargetGrid;

        private TileMapData _map;
        private readonly List<Tilemap> _layerTilemaps = new List<Tilemap>();

        public void Bind(TileMapData map)
        {
            _map = map;
            SyncLayerCount();
            RenderAll();
        }

        public TileMapData GetMap() => _map;

        private void EnsureGrid()
        {
            if (TargetGrid == null)
            {
                TargetGrid = GetComponent<Grid>();
                if (TargetGrid == null)
                    TargetGrid = gameObject.AddComponent<Grid>();
            }
        }

        private void SyncLayerCount()
        {
            EnsureGrid();
            if (_map == null) return;

            while (_layerTilemaps.Count < _map.Layers.Count)
            {
                int index = _layerTilemaps.Count;
                var go = new GameObject($"Layer_{index}");
                go.transform.SetParent(TargetGrid.transform, false);
                var tilemap = go.AddComponent<Tilemap>();
                var renderer = go.AddComponent<TilemapRenderer>();
                renderer.sortingOrder = index;
                _layerTilemaps.Add(tilemap);
            }

            while (_layerTilemaps.Count > _map.Layers.Count)
            {
                int last = _layerTilemaps.Count - 1;
                var tilemap = _layerTilemaps[last];
                _layerTilemaps.RemoveAt(last);
                if (tilemap != null)
                {
                    if (Application.isPlaying) Destroy(tilemap.gameObject);
                    else DestroyImmediate(tilemap.gameObject);
                }
            }
        }

        public void RenderAll()
        {
            if (_map == null || Palette == null) return;
            SyncLayerCount();

            for (int l = 0; l < _map.Layers.Count; l++)
            {
                RenderLayer(l);
            }
        }

        public void RenderLayer(int layerIndex)
        {
            if (_map == null || Palette == null) return;
            if (layerIndex < 0 || layerIndex >= _layerTilemaps.Count) return;

            var layer = _map.Layers[layerIndex];
            var tilemap = _layerTilemaps[layerIndex];
            tilemap.gameObject.SetActive(layer.Visible);
            tilemap.ClearAllTiles();

            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    int value = layer.Get(x, y);
                    if (value == TileLayer.EmptyValue) continue;

                    var tileBase = Palette.GetTile(value);
                    if (tileBase == null) continue;

                    tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);
                }
            }
        }

        /// <summary>
        /// 셀 하나만 갱신하고 싶을 때 사용 (에디터 실시간 프리뷰, 런타임 부분 갱신 등).
        /// </summary>
        public void RenderCell(int layerIndex, int x, int y)
        {
            if (_map == null || Palette == null) return;
            if (layerIndex < 0 || layerIndex >= _layerTilemaps.Count) return;

            var layer = _map.Layers[layerIndex];
            var tilemap = _layerTilemaps[layerIndex];
            int value = layer.Get(x, y);
            var tileBase = value == TileLayer.EmptyValue ? null : Palette.GetTile(value);
            tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);
        }
    }
}
