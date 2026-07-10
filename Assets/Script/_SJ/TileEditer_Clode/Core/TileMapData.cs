using System;
using System.Collections.Generic;

namespace TileEditor.Core
{
    public enum BrushShape
    {
        Square,
        Circle
    }

    /// <summary>
    /// 여러 레이어를 관리하는 타일맵 데이터. 모든 레이어는 동일한 Width/Height를 공유한다.
    /// </summary>
    [Serializable]
    public class TileMapData
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public readonly List<TileLayer> Layers = new List<TileLayer>();

        public TileMapData(int width, int height)
        {
            Width = Math.Max(1, width);
            Height = Math.Max(1, height);
        }

        public TileLayer AddLayer(string name)
        {
            var layer = new TileLayer(name, Width, Height);
            Layers.Add(layer);
            return layer;
        }

        public void RemoveLayer(int index)
        {
            if (index < 0 || index >= Layers.Count) return;
            Layers.RemoveAt(index);
        }

        public void MoveLayer(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= Layers.Count) return;
            toIndex = Math.Max(0, Math.Min(Layers.Count - 1, toIndex));
            var layer = Layers[fromIndex];
            Layers.RemoveAt(fromIndex);
            Layers.Insert(toIndex, layer);
        }

        /// <summary>
        /// 전체 맵(모든 레이어) 크기 조절. 좌상단 앵커 기준으로 겹치는 영역만 보존한다.
        /// </summary>
        public void Resize(int newWidth, int newHeight)
        {
            Width = Math.Max(1, newWidth);
            Height = Math.Max(1, newHeight);
            foreach (var layer in Layers)
            {
                layer.Resize(Width, Height);
            }
        }

        public int GetTile(int layerIndex, int x, int y)
        {
            if (layerIndex < 0 || layerIndex >= Layers.Count) return TileLayer.EmptyValue;
            return Layers[layerIndex].Get(x, y);
        }

        public void SetTile(int layerIndex, int x, int y, int value)
        {
            if (layerIndex < 0 || layerIndex >= Layers.Count) return;
            Layers[layerIndex].Set(x, y, value);
        }

        /// <summary>
        /// 원형/사각형 브러쉬 영역에 값을 일괄 적용한다. size는 중심으로부터의 반경(0 = 셀 1개).
        /// </summary>
        public void PaintBrush(int layerIndex, int centerX, int centerY, int size, BrushShape shape, int value)
        {
            if (layerIndex < 0 || layerIndex >= Layers.Count) return;
            var layer = Layers[layerIndex];
            int radiusSq = size * size;

            for (int dy = -size; dy <= size; dy++)
            {
                for (int dx = -size; dx <= size; dx++)
                {
                    if (shape == BrushShape.Circle && (dx * dx + dy * dy) > radiusSq) continue;
                    layer.Set(centerX + dx, centerY + dy, value);
                }
            }
        }
    }
}
