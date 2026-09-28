using System.Collections.Generic;

namespace WorldForge
{
    // ── 바이옴 (고정 — 엔진 내부 지형 분류) ─────────────────────
    public enum BiomeType
    {
        DeepOcean, Ocean, ShallowOcean, Coast,
        Grassland, Forest, Desert, Highland,
        Mountain, HighMountain, Snow, Tundra
    }

    // ─────────────────────────────────────────────────────────────
    // CityTier / SpotType : int 로 대체
    //   CityData.Tier      → int (0 = 수도, 1 = 1등급, 2 = 2등급 ...)
    //                         WorldGenSettings.CityTierDefs[Tier] 로 정의 참조
    //   SpotData.SpotTypeId→ int (0, 1, 2 ...)
    //                         WorldGenSettings.SpotTypeDefs[SpotTypeId] 로 정의 참조
    // ─────────────────────────────────────────────────────────────

    public struct CityData
    {
        public int    X, Y;
        public string Name;
        public int    Nation;   // -1 = 무국적
        public int    Tier;     // 0 = 수도(Capital), 1~ = 사용자 정의 등급
        public float  Score;
    }

    public struct SpotData
    {
        public int    X, Y;
        public int    SpotTypeId;   // WorldGenSettings.SpotTypeDefs 의 인덱스
        public string Name;
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

        public float[]     HeightMap  { get; }
        public float[]     TempMap    { get; }
        public BiomeType[] Biomes     { get; }
        public int[]       NationMap  { get; }  // -1 = 바다/무국적
        public bool[]      RiverMap   { get; }

        /// <summary>
        /// 타일별 도시 Tier 값. -1 = 도시 없음.
        /// NationMap 과 동일한 패턴으로 조회.
        /// 값은 CityData.Tier 와 같은 int (0=수도, 1~=사용자 등급).
        /// </summary>
        public int[] CityTierMap  { get; }

        /// <summary>
        /// 타일별 Cities 리스트 인덱스. -1 = 도시 없음.
        /// Cities[CityIndexMap[Idx(x,y)]] 로 즉시 CityData 접근.
        /// </summary>
        public int[] CityIndexMap { get; }

        public List<CityData>   Cities  { get; } = new List<CityData>();
        public List<SpotData>   Spots   { get; } = new List<SpotData>();
        public List<NationData> Nations { get; } = new List<NationData>();
        public List<int[][]>    Rivers  { get; } = new List<int[][]>();
        public List<(int, int)> Roads   { get; } = new List<(int, int)>();

        public float           SeaThreshold   { get; internal set; }
        public float           MountThreshold { get; internal set; }
        public WorldGenSettings Settings      { get; internal set; }

        // ── 좌표 → 리스트 인덱스 해시맵 ───────────────────────────
        private Dictionary<(int, int), int> _cityLookup;
        private Dictionary<(int, int), int> _spotLookup;

        public WorldData(int width, int height)
        {
            Width        = width;
            Height       = height;
            HeightMap    = new float[width * height];
            TempMap      = new float[width * height];
            Biomes       = new BiomeType[width * height];
            NationMap    = new int[width * height];
            RiverMap     = new bool[width * height];
            CityTierMap  = new int[width * height];
            CityIndexMap = new int[width * height];

            for (int i = 0; i < NationMap.Length;   i++) NationMap[i]   = -1;
            for (int i = 0; i < CityTierMap.Length; i++) CityTierMap[i] = -1;
            for (int i = 0; i < CityIndexMap.Length;i++) CityIndexMap[i]= -1;
        }

        public int  Idx(int x, int y)      => y * Width + x;
        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsLand(int x, int y)   => InBounds(x, y) && HeightMap[Idx(x, y)] >= SeaThreshold;

        // ── 도시 등급 조회 ────────────────────────────────────────

        /// <summary>해당 타일의 도시 Tier. 도시가 없으면 -1.</summary>
        public int GetCityTierAt(int x, int y) =>
            InBounds(x, y) ? CityTierMap[Idx(x, y)] : -1;

        /// <summary>해당 타일에 도시가 있는지 여부.</summary>
        public bool HasCityAt(int x, int y) =>
            InBounds(x, y) && CityTierMap[Idx(x, y)] >= 0;

        // ── 좌표 해시맵 ───────────────────────────────────────────

        /// <summary>
        /// Cities / Spots 리스트가 모두 채워진 후 한 번 호출.
        /// WorldGenerator.Generate() 끝 / WorldDataSerializer.Load 후 자동 호출됨.
        /// </summary>
        public void BuildLookupMaps()
        {
            _cityLookup = new Dictionary<(int, int), int>(Cities.Count);
            for (int i = 0; i < Cities.Count; i++)
            {
                var c = Cities[i];
                _cityLookup[(c.X, c.Y)] = i;
                if (InBounds(c.X, c.Y))
                {
                    int idx      = Idx(c.X, c.Y);
                    CityTierMap[idx]  = c.Tier;
                    CityIndexMap[idx] = i;
                }
            }

            _spotLookup = new Dictionary<(int, int), int>(Spots.Count);
            for (int i = 0; i < Spots.Count; i++)
                _spotLookup[(Spots[i].X, Spots[i].Y)] = i;
        }

        /// <summary>좌표에 도시가 있으면 CityData 반환 O(1).</summary>
        public bool TryGetCityAt(int x, int y, out CityData city)
        {
            EnsureLookupBuilt();
            if (_cityLookup.TryGetValue((x, y), out int i)) { city = Cities[i]; return true; }
            city = default; return false;
        }

        /// <summary>좌표에 스폿이 있으면 SpotData 반환 O(1).</summary>
        public bool TryGetSpotAt(int x, int y, out SpotData spot)
        {
            EnsureLookupBuilt();
            if (_spotLookup.TryGetValue((x, y), out int i)) { spot = Spots[i]; return true; }
            spot = default; return false;
        }

        public bool IsCityTile(int x, int y) { EnsureLookupBuilt(); return _cityLookup.ContainsKey((x, y)); }
        public bool IsSpotTile(int x, int y) { EnsureLookupBuilt(); return _spotLookup.ContainsKey((x, y)); }

        private void EnsureLookupBuilt()
        {
            if (_cityLookup == null || _spotLookup == null) BuildLookupMaps();
        }
    }
}
