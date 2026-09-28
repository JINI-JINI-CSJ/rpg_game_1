using System;
using System.Collections.Generic;

namespace WorldForge
{
    // ─────────────────────────────────────────────────────────────
    // 도시 등급 정의 (가변 — 사용자가 추가/삭제 가능)
    // Tier 0 은 항상 "수도" (국가당 1개 고정)
    // Tier 1, 2, ... 는 사용자가 원하는 만큼 추가
    // ─────────────────────────────────────────────────────────────
    [Serializable]
    public class CityTierDef
    {
        public string Label  = "등급";   // 에디터/툴팁에 표시할 이름
        public int    Count  = 10;       // 해당 등급 도시 수
        public byte   ColorR = 200;      // 렌더링 대표색
        public byte   ColorG = 150;
        public byte   ColorB = 50;
        public int    IconRadius = 2;    // 픽셀 반지름 (렌더링용)

        public CityTierDef() { }
        public CityTierDef(string label, int count,
                           byte r, byte g, byte b, int radius)
        {
            Label = label; Count = count;
            ColorR = r; ColorG = g; ColorB = b;
            IconRadius = radius;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // 스폿 종류 정의 (가변 — 사용자가 추가/삭제 가능)
    // ─────────────────────────────────────────────────────────────
    [Serializable]
    public class SpotTypeDef
    {
        public string Label  = "스폿";
        public int    Count  = 5;
        public byte   ColorR = 200;
        public byte   ColorG = 80;
        public byte   ColorB = 80;

        public SpotTypeDef() { }
        public SpotTypeDef(string label, int count, byte r, byte g, byte b)
        {
            Label = label; Count = count;
            ColorR = r; ColorG = g; ColorB = b;
        }
    }

    // ─────────────────────────────────────────────────────────────
    [Serializable]
    public class WorldGenSettings
    {
        // ── 기본 ──────────────────────────────────────────────────
        public int   Seed      = 42069;
        public int   MapWidth  = 256;
        public int   MapHeight = 160;

        // ── 지형 노이즈 ───────────────────────────────────────────
        public float NoiseScale    = 3.5f;
        public int   Octaves       = 6;
        public float Persistence   = 0.50f;
        public float ContinentBias = 0.40f;
        public float EdgeFalloff   = 0.70f;
        public float SeaLevel      = 0.42f;

        // ── 국가 / 강 ─────────────────────────────────────────────
        public int NumNations = 6;
        public int NumRivers  = 10;

        // ── 도시 등급 목록 ────────────────────────────────────────
        // [0] = 수도 (Count 는 NumNations 에 연동, 여기선 무시됨)
        // [1..] = 사용자 정의 등급
        public List<CityTierDef> CityTierDefs = new List<CityTierDef>
        {
            new CityTierDef("수도",   0,  245, 215,  50, 3),  // [0] 수도 — Count 무시
            new CityTierDef("대도시", 12, 235, 155,  40, 2),  // [1]
            new CityTierDef("중도시", 20, 200, 120,  30, 1),  // [2]
            new CityTierDef("소도시", 30, 160,  90,  20, 1),  // [3]
        };

        // ── 스폿 종류 목록 ────────────────────────────────────────
        public List<SpotTypeDef> SpotTypeDefs = new List<SpotTypeDef>
        {
            new SpotTypeDef("던전",    4, 200,  50,  50),  // [0]
            new SpotTypeDef("고대유적",4, 200, 170,  75),  // [1]
            new SpotTypeDef("마법탑",  3, 135,  85, 200),  // [2]
            new SpotTypeDef("묘지",    3, 100, 135, 170),  // [3]
            new SpotTypeDef("화산",    2, 255, 100,   0),  // [4]
        };

        // ── 편의 프로퍼티 ─────────────────────────────────────────
        /// <summary>수도 제외 도시 총 수</summary>
        public int TotalNonCapitalCities
        {
            get
            {
                int n = 0;
                for (int i = 1; i < CityTierDefs.Count; i++) n += CityTierDefs[i].Count;
                return n;
            }
        }

        /// <summary>수도 포함 전체 도시 수</summary>
        public int TotalCities => NumNations + TotalNonCapitalCities;

        /// <summary>전체 스폿 수</summary>
        public int TotalSpots
        {
            get
            {
                int n = 0;
                foreach (var d in SpotTypeDefs) n += d.Count;
                return n;
            }
        }

        // ── 프리셋 ────────────────────────────────────────────────
        public static WorldGenSettings Archipelago()
        {
            var s = new WorldGenSettings
            {
                NoiseScale = 2.0f, Octaves = 7,
                ContinentBias = 0.10f, EdgeFalloff = 0.30f, SeaLevel = 0.60f,
                NumNations = 8, NumRivers = 12,
            };
            s.CityTierDefs = new List<CityTierDef>
            {
                new CityTierDef("수도",   0,  245,215, 50, 3),
                new CityTierDef("대도시",16,  235,155, 40, 2),
                new CityTierDef("중도시",24,  200,120, 30, 1),
                new CityTierDef("소도시",40,  160, 90, 20, 1),
            };
            s.SpotTypeDefs = new List<SpotTypeDef>
            {
                new SpotTypeDef("던전",    6, 200, 50, 50),
                new SpotTypeDef("고대유적",6, 200,170, 75),
                new SpotTypeDef("마법탑",  4, 135, 85,200),
                new SpotTypeDef("묘지",    4, 100,135,170),
                new SpotTypeDef("화산",    2, 255,100,  0),
            };
            return s;
        }

        public static WorldGenSettings Pangaea()
        {
            var s = new WorldGenSettings
            {
                NoiseScale = 5.0f, Octaves = 5,
                ContinentBias = 0.70f, EdgeFalloff = 0.90f, SeaLevel = 0.35f,
                NumNations = 6, NumRivers = 10,
            };
            s.CityTierDefs = new List<CityTierDef>
            {
                new CityTierDef("수도",   0,  245,215, 50, 3),
                new CityTierDef("대도시",12,  235,155, 40, 2),
                new CityTierDef("중도시",20,  200,120, 30, 1),
                new CityTierDef("소도시",30,  160, 90, 20, 1),
            };
            s.SpotTypeDefs = new List<SpotTypeDef>
            {
                new SpotTypeDef("던전",    4, 200, 50, 50),
                new SpotTypeDef("고대유적",4, 200,170, 75),
                new SpotTypeDef("마법탑",  3, 135, 85,200),
                new SpotTypeDef("묘지",    3, 100,135,170),
                new SpotTypeDef("화산",    2, 255,100,  0),
            };
            return s;
        }

        public static WorldGenSettings Mountainous()
        {
            var s = new WorldGenSettings
            {
                NoiseScale = 2.5f, Octaves = 8, Persistence = 0.65f, SeaLevel = 0.40f,
                NumNations = 5, NumRivers = 8,
            };
            s.CityTierDefs = new List<CityTierDef>
            {
                new CityTierDef("수도",   0, 245,215, 50, 3),
                new CityTierDef("거점",   8, 235,155, 40, 2),
                new CityTierDef("마을",  14, 200,120, 30, 1),
            };
            s.SpotTypeDefs = new List<SpotTypeDef>
            {
                new SpotTypeDef("던전",    6, 200, 50, 50),
                new SpotTypeDef("마법탑",  4, 135, 85,200),
                new SpotTypeDef("화산",    4, 255,100,  0),
                new SpotTypeDef("용의둥지",2, 50,200, 80),
            };
            return s;
        }
    }
}
