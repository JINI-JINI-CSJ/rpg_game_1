using System.Collections.Generic;

namespace WorldForge
{
    // ── 바이옴 ────────────────────────────────────────────────────
    public enum BiomeType
    {
        DeepOcean,
        Ocean,
        ShallowOcean,
        Coast,
        Grassland,
        Forest,
        Desert,
        Highland,
        Mountain,
        HighMountain,
        Snow,
        Tundra
    }

    // ── 특수 스폿 종류 ────────────────────────────────────────────
    public enum SpotType
    {
        Dungeon,
        AncientRuin,
        MagicTower,
        Graveyard,
        Volcano,
        DragonLair
    }

    // ── 도시 규모 ─────────────────────────────────────────────────
    public enum CitySize { Small, Medium, Large }

    // ─────────────────────────────────────────────────────────────
    // 데이터 구조체들
    // ─────────────────────────────────────────────────────────────
    public struct CityData
    {
        public int    X, Y;
        public string Name;
        public int    Nation;       // -1 = 무국적
        public bool   IsCapital;
        public CitySize Size;
        public float  Score;
    }

    public struct SpotData
    {
        public int      X, Y;
        public SpotType Type;
        public string   Name;
    }

    public struct NationData
    {
        public int    Id;
        public int    CapitalX, CapitalY;
        public string Name;
        public byte   R, G, B;     // 국가 대표색
    }

    // ─────────────────────────────────────────────────────────────
    // 월드 전체 데이터 컨테이너 (순수 C#)
    // ─────────────────────────────────────────────────────────────
    public class WorldData
    {
        public int Width  { get; }
        public int Height { get; }

        // ── 타일별 배열 ──
        public float[]    HeightMap   { get; }   // 0~1 정규화
        public float[]    TempMap     { get; }   // 0=극지 1=적도
        public BiomeType[] Biomes     { get; }
        public int[]      NationMap   { get; }   // -1=바다/미분류
        public bool[]     RiverMap    { get; }

        // ── 지물 리스트 ──
        public List<CityData>   Cities  { get; } = new();
        public List<SpotData>   Spots   { get; } = new();
        public List<NationData> Nations { get; } = new();
        public List<int[][]>    Rivers  { get; } = new(); // Rivers[i] = { {x,y}, ... }
        public List<(int,int)>  Roads   { get; } = new(); // (cityIdxA, cityIdxB)

        // ── 임계값 ──
        public float SeaThreshold   { get; internal set; }
        public float MountThreshold { get; internal set; }

        // ── 생성 파라미터 (기록용) ──
        public WorldGenSettings Settings { get; internal set; }

        public WorldData(int width, int height)
        {
            Width      = width;
            Height     = height;
            HeightMap  = new float[width * height];
            TempMap    = new float[width * height];
            Biomes     = new BiomeType[width * height];
            NationMap  = new int[width * height];
            RiverMap   = new bool[width * height];

            // NationMap 기본값 -1
            for (int i = 0; i < NationMap.Length; i++) NationMap[i] = -1;
        }

        public int Idx(int x, int y) => y * Width + x;
        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsLand(int x, int y) => InBounds(x, y) && HeightMap[Idx(x, y)] >= SeaThreshold;
    }
}
