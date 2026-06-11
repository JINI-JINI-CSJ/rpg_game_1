using System.Collections.Generic;

namespace WorldForge
{
    // ── 바이옴 ────────────────────────────────────────────────────
    public enum BiomeType
    {
        DeepOcean, Ocean, ShallowOcean, Coast,
        Grassland, Forest, Desert, Highland,
        Mountain, HighMountain, Snow, Tundra
    }

    // ── 특수 스폿 종류 ────────────────────────────────────────────
    public enum SpotType
    {
        Dungeon, AncientRuin, MagicTower, Graveyard, Volcano, DragonLair
    }

    // ── 도시 등급 (4단계) ─────────────────────────────────────────
    public enum CityTier
    {
        Capital,    // 수도   — 국가당 1개, 가장 크다
        Major,      // 대도시
        Minor,      // 중도시
        Village     // 소도시
    }

    // ─────────────────────────────────────────────────────────────
    public struct CityData
    {
        public int      X, Y;
        public string   Name;
        public int      Nation;     // -1 = 무국적
        public CityTier Tier;
        public float    Score;
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
        public byte   R, G, B;
    }

    // ─────────────────────────────────────────────────────────────
    public class WorldData
    {
        public int Width  { get; }
        public int Height { get; }

        public float[]     HeightMap { get; }
        public float[]     TempMap   { get; }
        public BiomeType[] Biomes    { get; }
        public int[]       NationMap { get; }
        public bool[]      RiverMap  { get; }

        public List<CityData>   Cities  { get; } = new List<CityData>();
        public List<SpotData>   Spots   { get; } = new List<SpotData>();
        public List<NationData> Nations { get; } = new List<NationData>();
        public List<int[][]>    Rivers  { get; } = new List<int[][]>();
        public List<(int, int)> Roads   { get; } = new List<(int, int)>();

        public float SeaThreshold   { get; internal set; }
        public float MountThreshold { get; internal set; }
        public WorldGenSettings Settings { get; internal set; }

        public WorldData(int width, int height)
        {
            Width     = width;
            Height    = height;
            HeightMap = new float[width * height];
            TempMap   = new float[width * height];
            Biomes    = new BiomeType[width * height];
            NationMap = new int[width * height];
            RiverMap  = new bool[width * height];
            for (int i = 0; i < NationMap.Length; i++) NationMap[i] = -1;
        }

        public int  Idx(int x, int y)      => y * Width + x;
        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsLand(int x, int y)   => InBounds(x, y) && HeightMap[Idx(x, y)] >= SeaThreshold;
    }
}
