using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace WorldForge
{
    /// <summary>
    /// WorldData ↔ 바이너리 직렬화 (순수 C#, Unity 의존 없음)
    ///
    /// 파일 포맷 (Little-endian, BinaryWriter 기본값):
    ///   [4]  매직 넘버  "WFRG"
    ///   [4]  포맷 버전 (int)
    ///   ── WorldGenSettings ──
    ///   ── 맵 크기 / 임계값 ──
    ///   ── HeightMap / TempMap / Biomes / NationMap / RiverMap ──
    ///   ── Nations / Cities / Spots / Rivers / Roads ──
    ///
    /// 버전이 바뀌면 OldVersion 분기 추가로 하위 호환 유지 가능.
    /// </summary>
    public static class WorldDataSerializer
    {
        private const string Magic         = "WFRG";
        private const int    FormatVersion = 1;

        // ════════════════════════════════════════════════════════
        // SAVE
        // ════════════════════════════════════════════════════════
        public static void SaveToFile(WorldData world, string path)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var bw = new BinaryWriter(fs, Encoding.UTF8);
            Write(world, bw);
        }

        public static byte[] SaveToBytes(WorldData world)
        {
            using var ms = new MemoryStream();
            using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
                Write(world, bw);
            return ms.ToArray();
        }

        private static void Write(WorldData w, BinaryWriter bw)
        {
            // ── 헤더 ──────────────────────────────────────────────
            bw.Write(Encoding.ASCII.GetBytes(Magic)); // 4바이트
            bw.Write(FormatVersion);

            // ── 생성 설정 (재현/재생성용) ──────────────────────────
            WriteSettings(bw, w.Settings);

            // ── 맵 크기 / 임계값 ────────────────────────────────────
            bw.Write(w.Width);
            bw.Write(w.Height);
            bw.Write(w.SeaThreshold);
            bw.Write(w.MountThreshold);

            // ── 타일 배열 ───────────────────────────────────────────
            WriteFloatArray(bw, w.HeightMap);
            WriteFloatArray(bw, w.TempMap);

            bw.Write(w.Biomes.Length);
            foreach (var b in w.Biomes) bw.Write((byte)b);

            bw.Write(w.NationMap.Length);
            foreach (var n in w.NationMap) bw.Write(n);

            bw.Write(w.RiverMap.Length);
            // 비트 패킹으로 용량 절약 (1bit per tile)
            WriteBoolArrayPacked(bw, w.RiverMap);

            // ── 국가 ────────────────────────────────────────────────
            bw.Write(w.Nations.Count);
            foreach (var n in w.Nations)
            {
                bw.Write(n.Id);
                bw.Write(n.CapitalX);
                bw.Write(n.CapitalY);
                WriteString(bw, n.Name);
                bw.Write(n.R); bw.Write(n.G); bw.Write(n.B);
            }

            // ── 도시 ────────────────────────────────────────────────
            bw.Write(w.Cities.Count);
            foreach (var c in w.Cities)
            {
                bw.Write(c.X);
                bw.Write(c.Y);
                WriteString(bw, c.Name);
                bw.Write(c.Nation);
                bw.Write((byte)c.Tier);
                bw.Write(c.Score);
            }

            // ── 스폿 ────────────────────────────────────────────────
            bw.Write(w.Spots.Count);
            foreach (var sp in w.Spots)
            {
                bw.Write(sp.X);
                bw.Write(sp.Y);
                bw.Write((byte)sp.Type);
                WriteString(bw, sp.Name);
            }

            // ── 강 (가변 길이 경로 목록) ────────────────────────────
            bw.Write(w.Rivers.Count);
            foreach (var path in w.Rivers)
            {
                bw.Write(path.Length);
                foreach (var pt in path)
                {
                    bw.Write(pt[0]);
                    bw.Write(pt[1]);
                }
            }

            // ── 교역로 ──────────────────────────────────────────────
            bw.Write(w.Roads.Count);
            foreach (var (a, b) in w.Roads)
            {
                bw.Write(a);
                bw.Write(b);
            }
        }

        // ════════════════════════════════════════════════════════
        // LOAD
        // ════════════════════════════════════════════════════════
        public static WorldData LoadFromFile(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs, Encoding.UTF8);
            return Read(br);
        }

        public static WorldData LoadFromBytes(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms, Encoding.UTF8);
            return Read(br);
        }

        private static WorldData Read(BinaryReader br)
        {
            // ── 헤더 검증 ───────────────────────────────────────────
            string magic = Encoding.ASCII.GetString(br.ReadBytes(4));
            if (magic != Magic)
                throw new InvalidDataException(
                    $"[WorldForge] 잘못된 파일 형식입니다 (magic='{magic}', 예상='{Magic}').");

            int version = br.ReadInt32();
            if (version > FormatVersion)
                throw new InvalidDataException(
                    $"[WorldForge] 지원하지 않는 포맷 버전입니다 (file={version}, supported<={FormatVersion}). " +
                    "최신 버전의 World Forge로 다시 시도하세요.");

            // ── 생성 설정 ───────────────────────────────────────────
            var settings = ReadSettings(br);

            // ── 맵 크기 / 임계값 ────────────────────────────────────
            int width  = br.ReadInt32();
            int height = br.ReadInt32();
            float seaTh   = br.ReadSingle();
            float mountTh = br.ReadSingle();

            var world = new WorldData(width, height)
            {
                Settings      = settings,
                SeaThreshold  = seaTh,
                MountThreshold= mountTh,
            };

            // ── 타일 배열 ───────────────────────────────────────────
            ReadFloatArrayInto(br, world.HeightMap);
            ReadFloatArrayInto(br, world.TempMap);

            int biomeLen = br.ReadInt32();
            ValidateLength(biomeLen, world.Biomes.Length, "Biomes");
            for (int i = 0; i < biomeLen; i++) world.Biomes[i] = (BiomeType)br.ReadByte();

            int nationLen = br.ReadInt32();
            ValidateLength(nationLen, world.NationMap.Length, "NationMap");
            for (int i = 0; i < nationLen; i++) world.NationMap[i] = br.ReadInt32();

            int riverLen = br.ReadInt32();
            ValidateLength(riverLen, world.RiverMap.Length, "RiverMap");
            ReadBoolArrayPackedInto(br, world.RiverMap);

            // ── 국가 ────────────────────────────────────────────────
            int nationCount = br.ReadInt32();
            for (int i = 0; i < nationCount; i++)
            {
                var n = new NationData
                {
                    Id       = br.ReadInt32(),
                    CapitalX = br.ReadInt32(),
                    CapitalY = br.ReadInt32(),
                    Name     = ReadString(br),
                    R = br.ReadByte(), G = br.ReadByte(), B = br.ReadByte(),
                };
                world.Nations.Add(n);
            }

            // ── 도시 ────────────────────────────────────────────────
            int cityCount = br.ReadInt32();
            for (int i = 0; i < cityCount; i++)
            {
                var c = new CityData
                {
                    X      = br.ReadInt32(),
                    Y      = br.ReadInt32(),
                    Name   = ReadString(br),
                    Nation = br.ReadInt32(),
                    Tier   = (CityTier)br.ReadByte(),
                    Score  = br.ReadSingle(),
                };
                world.Cities.Add(c);
            }

            // ── 스폿 ────────────────────────────────────────────────
            int spotCount = br.ReadInt32();
            for (int i = 0; i < spotCount; i++)
            {
                var sp = new SpotData
                {
                    X    = br.ReadInt32(),
                    Y    = br.ReadInt32(),
                    Type = (SpotType)br.ReadByte(),
                    Name = ReadString(br),
                };
                world.Spots.Add(sp);
            }

            // ── 강 ──────────────────────────────────────────────────
            int riverCount = br.ReadInt32();
            for (int i = 0; i < riverCount; i++)
            {
                int len = br.ReadInt32();
                var path = new int[len][];
                for (int p = 0; p < len; p++)
                    path[p] = new[] { br.ReadInt32(), br.ReadInt32() };
                world.Rivers.Add(path);
            }

            // ── 교역로 ──────────────────────────────────────────────
            int roadCount = br.ReadInt32();
            for (int i = 0; i < roadCount; i++)
            {
                int a = br.ReadInt32();
                int b = br.ReadInt32();
                world.Roads.Add((a, b));
            }

            // ── 타일 등급맵 + 좌표 해시맵 재구축 ──────────────────
            // CityTierMap / CityIndexMap / 좌표 Dictionary 는 저장하지 않고
            // Cities 리스트로부터 항상 다시 생성 (데이터 중복 방지, 일관성 보장)
            world.BuildLookupMaps();

            return world;
        }

        // ════════════════════════════════════════════════════════
        // Settings 직렬화 (필드 추가에도 안전하도록 명시적으로 기록)
        // ════════════════════════════════════════════════════════
        private static void WriteSettings(BinaryWriter bw, WorldGenSettings s)
        {
            bw.Write(s.Seed);
            bw.Write(s.MapWidth);
            bw.Write(s.MapHeight);
            bw.Write(s.NoiseScale);
            bw.Write(s.Octaves);
            bw.Write(s.Persistence);
            bw.Write(s.ContinentBias);
            bw.Write(s.EdgeFalloff);
            bw.Write(s.SeaLevel);
            bw.Write(s.NumNations);
            bw.Write(s.NumRivers);
            bw.Write(s.NumMajorCities);
            bw.Write(s.NumMinorCities);
            bw.Write(s.NumVillages);
            bw.Write(s.NumDungeons);
            bw.Write(s.NumRuins);
            bw.Write(s.NumMagicTowers);
            bw.Write(s.NumGraveyards);
            bw.Write(s.NumVolcanoes);
        }

        private static WorldGenSettings ReadSettings(BinaryReader br)
        {
            return new WorldGenSettings
            {
                Seed            = br.ReadInt32(),
                MapWidth        = br.ReadInt32(),
                MapHeight       = br.ReadInt32(),
                NoiseScale      = br.ReadSingle(),
                Octaves         = br.ReadInt32(),
                Persistence     = br.ReadSingle(),
                ContinentBias   = br.ReadSingle(),
                EdgeFalloff     = br.ReadSingle(),
                SeaLevel        = br.ReadSingle(),
                NumNations      = br.ReadInt32(),
                NumRivers       = br.ReadInt32(),
                NumMajorCities  = br.ReadInt32(),
                NumMinorCities  = br.ReadInt32(),
                NumVillages     = br.ReadInt32(),
                NumDungeons     = br.ReadInt32(),
                NumRuins        = br.ReadInt32(),
                NumMagicTowers  = br.ReadInt32(),
                NumGraveyards   = br.ReadInt32(),
                NumVolcanoes    = br.ReadInt32(),
            };
        }

        // ════════════════════════════════════════════════════════
        // 헬퍼: 배열 / 문자열 / 비트 패킹
        // ════════════════════════════════════════════════════════
        private static void WriteFloatArray(BinaryWriter bw, float[] arr)
        {
            bw.Write(arr.Length);
            var buf = new byte[arr.Length * 4];
            Buffer.BlockCopy(arr, 0, buf, 0, buf.Length);
            bw.Write(buf);
        }

        private static void ReadFloatArrayInto(BinaryReader br, float[] dst)
        {
            int len = br.ReadInt32();
            ValidateLength(len, dst.Length, "FloatArray");
            var buf = br.ReadBytes(len * 4);
            Buffer.BlockCopy(buf, 0, dst, 0, buf.Length);
        }

        /// <summary>bool[] → 1bit/타일 패킹 (RiverMap 용량 1/8로 절감)</summary>
        private static void WriteBoolArrayPacked(BinaryWriter bw, bool[] arr)
        {
            int byteLen = (arr.Length + 7) / 8;
            var packed = new byte[byteLen];
            for (int i = 0; i < arr.Length; i++)
                if (arr[i]) packed[i >> 3] |= (byte)(1 << (i & 7));
            bw.Write(packed);
        }

        private static void ReadBoolArrayPackedInto(BinaryReader br, bool[] dst)
        {
            int byteLen = (dst.Length + 7) / 8;
            var packed = br.ReadBytes(byteLen);
            for (int i = 0; i < dst.Length; i++)
                dst[i] = (packed[i >> 3] & (1 << (i & 7))) != 0;
        }

        private static void WriteString(BinaryWriter bw, string s)
        {
            // null 안전 처리
            bw.Write(s ?? string.Empty);
        }

        private static string ReadString(BinaryReader br) => br.ReadString();

        private static void ValidateLength(int fileLen, int expectedLen, string fieldName)
        {
            if (fileLen != expectedLen)
                throw new InvalidDataException(
                    $"[WorldForge] '{fieldName}' 길이가 일치하지 않습니다 " +
                    $"(file={fileLen}, expected={expectedLen}). 파일이 손상되었을 수 있습니다.");
        }
    }
}
