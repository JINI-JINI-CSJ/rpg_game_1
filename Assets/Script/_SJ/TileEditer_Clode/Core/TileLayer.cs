using System;

namespace TileEditor.Core
{
    /// <summary>
    /// 순수 데이터 클래스. 하나의 레이어에 대한 타일 int 데이터를 1차원 배열(row-major)로 보관한다.
    /// UnityEngine 타입에 의존하지 않아 에디터/런타임/테스트에서 공용으로 사용할 수 있다.
    /// </summary>
    [Serializable]
    public class TileLayer
    {
        public const int EmptyValue = -1;

        public string Name;
        public bool Visible = true;
        public float Opacity = 1f;

        private int _width;
        private int _height;
        private int[] _tiles;

        public int Width => _width;
        public int Height => _height;
        public int[] RawTiles => _tiles;

        public TileLayer(string name, int width, int height)
        {
            Name = name;
            _width = Math.Max(1, width);
            _height = Math.Max(1, height);
            _tiles = new int[_width * _height];
            Clear(EmptyValue);
        }

        public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < _width && y < _height;

        public int Get(int x, int y)
        {
            if (!InBounds(x, y)) return EmptyValue;
            return _tiles[y * _width + x];
        }

        public void Set(int x, int y, int value)
        {
            if (!InBounds(x, y)) return;
            _tiles[y * _width + x] = value;
        }

        public void Clear(int value = EmptyValue)
        {
            for (int i = 0; i < _tiles.Length; i++) _tiles[i] = value;
        }

        /// <summary>
        /// 좌상단(0,0) 기준 앵커를 유지하며 새 크기로 리사이즈. 겹치는 영역의 기존 데이터는 보존된다.
        /// </summary>
        public void Resize(int newWidth, int newHeight, int fillValue = EmptyValue)
        {
            newWidth = Math.Max(1, newWidth);
            newHeight = Math.Max(1, newHeight);
            int[] newTiles = new int[newWidth * newHeight];
            for (int i = 0; i < newTiles.Length; i++) newTiles[i] = fillValue;

            int copyW = Math.Min(_width, newWidth);
            int copyH = Math.Min(_height, newHeight);
            for (int y = 0; y < copyH; y++)
            {
                for (int x = 0; x < copyW; x++)
                {
                    newTiles[y * newWidth + x] = _tiles[y * _width + x];
                }
            }

            _width = newWidth;
            _height = newHeight;
            _tiles = newTiles;
        }

        public TileLayer Clone()
        {
            var clone = new TileLayer(Name, _width, _height)
            {
                Visible = Visible,
                Opacity = Opacity
            };
            Array.Copy(_tiles, clone._tiles, _tiles.Length);
            return clone;
        }
    }
}
