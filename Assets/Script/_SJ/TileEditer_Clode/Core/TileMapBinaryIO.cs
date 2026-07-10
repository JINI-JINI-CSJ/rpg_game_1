using System;
using System.IO;
using System.Text;

namespace TileEditor.Core
{
    /// <summary>
    /// TileMapData를 바이너리(.tmap)로 저장/로드한다.
    /// 포맷: [매직넘버][버전][Width][Height][LayerCount] + 레이어별 [이름][Visible][Opacity][타일 int 배열]
    /// </summary>
    public static class TileMapBinaryIO
    {
        private const uint Magic = 0x50414D54; // "TMAP"
        private const int Version = 1;

        public static void Save(TileMapData map, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(stream, Encoding.UTF8);

            writer.Write(Magic);
            writer.Write(Version);
            writer.Write(map.Width);
            writer.Write(map.Height);
            writer.Write(map.Layers.Count);

            foreach (var layer in map.Layers)
            {
                WriteString(writer, layer.Name);
                writer.Write(layer.Visible);
                writer.Write(layer.Opacity);

                int[] tiles = layer.RawTiles;
                for (int i = 0; i < tiles.Length; i++)
                {
                    writer.Write(tiles[i]);
                }
            }
        }

        public static TileMapData Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"타일맵 파일을 찾을 수 없습니다: {path}");

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            uint magic = reader.ReadUInt32();
            if (magic != Magic)
                throw new InvalidDataException("유효하지 않은 타일맵 파일입니다 (매직넘버 불일치).");

            int version = reader.ReadInt32();
            if (version != Version)
                throw new InvalidDataException($"지원하지 않는 버전입니다: {version}");

            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int layerCount = reader.ReadInt32();

            var map = new TileMapData(width, height);

            for (int l = 0; l < layerCount; l++)
            {
                string name = ReadString(reader);
                bool visible = reader.ReadBoolean();
                float opacity = reader.ReadSingle();

                var layer = map.AddLayer(name);
                layer.Visible = visible;
                layer.Opacity = opacity;

                int[] tiles = layer.RawTiles;
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i] = reader.ReadInt32();
                }
            }

            return map;
        }

        private static void WriteString(BinaryWriter writer, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        private static string ReadString(BinaryReader reader)
        {
            int len = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
