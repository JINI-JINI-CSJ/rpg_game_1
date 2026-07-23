using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TilemapTool
{
    /// <summary>
    /// 타일맵 바이너리 입출력. 팔레트 자체는 저장하지 않고,
    /// 각 배치의 paletteId(string)만 저장하여 로드 시 씬에 존재하는
    /// TilePalette를 통해 프리팹을 다시 찾는다.
    /// </summary>
    public static class TilemapBinaryIO
    {
        private const int MagicNumber = 0x544D4544; // "TMED"
        private const int Version = 3; // v2: customData 추가 / v3: highlightColor 추가

        private enum CustomValueType : byte
        {
            String = 0,
            Int = 1,
            Float = 2,
            Bool = 3,
            Long = 4,
            Double = 5
        }

        public static void Save(string filePath, TileMapSettings settings, List<TileLayer> layers)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            using var bw = new BinaryWriter(fs);

            bw.Write(MagicNumber);
            bw.Write(Version);

            bw.Write(settings.width);
            bw.Write(settings.depth);
            bw.Write(settings.tileSize);

            bw.Write(layers.Count);
            foreach (var layer in layers)
            {
                bw.Write(layer.isBaseLayer);
                bw.Write(layer.layerName ?? string.Empty);
                bw.Write(layer.yOffset);

                bw.Write(layer.placements.Count);
                foreach (var placement in layer.placements.Values)
                {
                    bw.Write(placement.x);
                    bw.Write(placement.z);
                    bw.Write(placement.paletteId ?? string.Empty);
                    bw.Write((byte)placement.direction);
                    bw.Write( placement.userValue );
                    WriteCustomData(bw, placement.customData);
                    WriteColor(bw, placement.highlightColor);
                }
            }

            Debug.Log($"[TilemapBinaryIO] 저장 완료: {filePath}");
        }

        public static bool Load(string filePath, TileMapSettings settings, out List<TileLayer> layers)
        {
            layers = new List<TileLayer>();

            if (!File.Exists(filePath))
            {
                Debug.LogError($"[TilemapBinaryIO] 파일이 존재하지 않습니다: {filePath}");
                return false;
            }

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            return LoadData( br , settings , out layers );
        }

        public static bool LoadData_TextAsset(TextAsset ta, TileMapSettings settings, out List<TileLayer> layers)
        {
            using var ms = new MemoryStream( ta.bytes );
            using var br = new BinaryReader(ms);
            return LoadData( br , settings , out layers );
        }

        public static bool LoadData(BinaryReader br, TileMapSettings settings, out List<TileLayer> layers)
        {
            layers = new List<TileLayer>();

            int magic = br.ReadInt32();
            if (magic != MagicNumber)
            {
                Debug.LogError("[TilemapBinaryIO] 유효하지 않은 파일 형식입니다.");
                return false;
            }

            int version = br.ReadInt32();
            if (version != Version)
            {
                Debug.LogWarning($"[TilemapBinaryIO] 버전 불일치 (파일:{version} / 현재:{Version}) - 계속 진행합니다.");
            }

            settings.width = br.ReadInt32();
            settings.depth = br.ReadInt32();
            settings.tileSize = br.ReadSingle();

            int layerCount = br.ReadInt32();
            for (int i = 0; i < layerCount; i++)
            {
                var layer = new TileLayer
                {
                    isBaseLayer = br.ReadBoolean(),
                    layerName = br.ReadString(),
                    yOffset = br.ReadSingle()
                };

                int placementCount = br.ReadInt32();
                for (int p = 0; p < placementCount; p++)
                {
                    var placement = new ObjectPlacement
                    {
                        x = br.ReadInt32(),
                        z = br.ReadInt32(),
                        paletteId = br.ReadString(),
                        direction = (TileDirection)br.ReadByte(),
                        userValue = br.ReadInt32()
                    };

                    // v2 미만 파일에는 customData 섹션이 없으므로 버전으로 분기
                    if (version >= 2)
                        placement.customData = ReadCustomData(br);

                    // v3 미만 파일에는 highlightColor 섹션이 없으므로 기본값 유지
                    if (version >= 3)
                        placement.highlightColor = ReadColor(br);

                    layer.Set(placement);
                }

                layers.Add(layer);
            }
            return true;
        }

        // ---------------- customData 직렬화 ----------------

        private static void WriteCustomData(BinaryWriter bw, Dictionary<string, object> data)
        {
            if (data == null)
            {
                bw.Write(0);
                return;
            }

            bw.Write(data.Count);
            foreach (var kv in data)
            {
                bw.Write(kv.Key ?? string.Empty);
                WriteCustomValue(bw, kv.Value);
            }
        }

        private static void WriteCustomValue(BinaryWriter bw, object value)
        {
            switch (value)
            {
                case int iv:
                    bw.Write((byte)CustomValueType.Int);
                    bw.Write(iv);
                    break;
                case float fv:
                    bw.Write((byte)CustomValueType.Float);
                    bw.Write(fv);
                    break;
                case bool bv:
                    bw.Write((byte)CustomValueType.Bool);
                    bw.Write(bv);
                    break;
                case long lv:
                    bw.Write((byte)CustomValueType.Long);
                    bw.Write(lv);
                    break;
                case double dv:
                    bw.Write((byte)CustomValueType.Double);
                    bw.Write(dv);
                    break;
                default:
                    // string 및 그 외 타입은 문자열로 저장 (게임 쪽에서 직접 파싱해서 사용)
                    bw.Write((byte)CustomValueType.String);
                    bw.Write(value?.ToString() ?? string.Empty);
                    break;
            }
        }

        private static Dictionary<string, object> ReadCustomData(BinaryReader br)
        {
            int count = br.ReadInt32();
            var dict = new Dictionary<string, object>(count);
            for (int i = 0; i < count; i++)
            {
                string key = br.ReadString();
                dict[key] = ReadCustomValue(br);
            }
            return dict;
        }

        private static object ReadCustomValue(BinaryReader br)
        {
            var type = (CustomValueType)br.ReadByte();
            switch (type)
            {
                case CustomValueType.Int: return br.ReadInt32();
                case CustomValueType.Float: return br.ReadSingle();
                case CustomValueType.Bool: return br.ReadBoolean();
                case CustomValueType.Long: return br.ReadInt64();
                case CustomValueType.Double: return br.ReadDouble();
                default: return br.ReadString();
            }
        }

        // ---------------- Color 직렬화 ----------------

        private static void WriteColor(BinaryWriter bw, Color c)
        {
            bw.Write(c.r);
            bw.Write(c.g);
            bw.Write(c.b);
            bw.Write(c.a);
        }

        private static Color ReadColor(BinaryReader br)
        {
            float r = br.ReadSingle();
            float g = br.ReadSingle();
            float b = br.ReadSingle();
            float a = br.ReadSingle();
            return new Color(r, g, b, a);
        }
    }
}
