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

        /// <summary>
        /// 타일별 도시 등급. 값은 CityTier(0~3), 도시가 없는 타일은 -1.
        /// NationMap 과 동일한 패턴(타일 → 분류값)으로 조회.
        /// </summary>
        public sbyte[] CityTierMap { get; }

        /// <summary>
        /// 타일별 Cities 리스트 인덱스. 도시가 없는 타일은 -1.
        /// "이 타일의 도시 등급이 뭐지?" 가 아니라 "이 타일의 도시 상세정보(CityData)" 가
        /// 필요할 때 Cities[CityIndexMap[Idx(x,y)]] 로 즉시 접근.
        /// </summary>
        public int[] CityIndexMap { get; }

        public List<CityData>   Cities  { get; } = new List<CityData>();
        public List<SpotData>   Spots   { get; } = new List<SpotData>();
        public List<NationData> Nations { get; } = new List<NationData>();
        public List<int[][]>    Rivers  { get; } = new List<int[][]>();
        public List<(int, int)> Roads   { get; } = new List<(int, int)>();

        public float SeaThreshold   { get; internal set; }
        public float MountThreshold { get; internal set; }
        public WorldGenSettings Settings { get; internal set; }

        // ── 좌표 → 인덱스 해시맵 (생성/로드 후 BuildLookupMaps() 로 채움) ──
        // key: (x, y) 좌표, value: Cities / Spots 리스트의 인덱스
        private Dictionary<(int x, int y), int> _cityLookup;
        private Dictionary<(int x, int y), int> _spotLookup;

        public WorldData(int width, int height)
        {
            Width       = width;
            Height      = height;
            HeightMap   = new float[width * height];
            TempMap     = new float[width * height];
            Biomes      = new BiomeType[width * height];
            NationMap   = new int[width * height];
            RiverMap    = new bool[width * height];
            CityTierMap = new sbyte[width * height];
            CityIndexMap= new int[width * height];

            for (int i = 0; i < NationMap.Length; i++)   NationMap[i]   = -1;
            for (int i = 0; i < CityTierMap.Length; i++) CityTierMap[i] = -1;
            for (int i = 0; i < CityIndexMap.Length; i++) CityIndexMap[i] = -1;
        }

        public int  Idx(int x, int y)      => y * Width + x;
        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsLand(int x, int y)   => InBounds(x, y) && HeightMap[Idx(x, y)] >= SeaThreshold;

        // ════════════════════════════════════════════════════════
        // 도시 등급 조회 (NationMap 과 동일한 사용 패턴)
        // ════════════════════════════════════════════════════════

        /// <summary>해당 타일의 도시 등급. 도시가 없으면 null.</summary>
        public CityTier? GetCityTierAt(int x, int y)
        {
            if (!InBounds(x, y)) return null;
            sbyte v = CityTierMap[Idx(x, y)];
            return v < 0 ? (CityTier?)null : (CityTier)v;
        }

        /// <summary>해당 타일에 도시가 있는지 여부.</summary>
        public bool HasCityAt(int x, int y) =>
            InBounds(x, y) && CityTierMap[Idx(x, y)] >= 0;

        // ════════════════════════════════════════════════════════
        // 좌표 → 도시/스폿 빠른 조회 (해시맵)
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// Cities / Spots 리스트가 채워진 뒤 반드시 한 번 호출.
        /// WorldGenerator.Generate() 마지막 단계와
        /// WorldDataSerializer.Load 직후에 자동으로 호출됩니다.
        /// </summary>
        public void BuildLookupMaps()
        {
            _cityLookup = new Dictionary<(int, int), int>(Cities.Count);
            for (int i = 0; i < Cities.Count; i++)
            {
                var c = Cities[i];
                _cityLookup[(c.X, c.Y)] = i;

                // 타일 배열도 함께 채움 (NationMap과 동일한 패턴)
                if (InBounds(c.X, c.Y))
                {
                    int idx = Idx(c.X, c.Y);
                    CityTierMap[idx]  = (sbyte)c.Tier;
                    CityIndexMap[idx] = i;
                }
            }

            _spotLookup = new Dictionary<(int, int), int>(Spots.Count);
            for (int i = 0; i < Spots.Count; i++)
            {
                var s = Spots[i];
                _spotLookup[(s.X, s.Y)] = i;
            }
        }

        /// <summary>좌표에 도시가 있으면 true와 CityData를 반환 (O(1)).</summary>
        public bool TryGetCityAt(int x, int y, out CityData city)
        {
            EnsureLookupBuilt();
            if (_cityLookup.TryGetValue((x, y), out int idx))
            {
                city = Cities[idx];
                return true;
            }
            city = default;
            return false;
        }

        /// <summary>좌표에 스폿이 있으면 true와 SpotData를 반환 (O(1)).</summary>
        public bool TryGetSpotAt(int x, int y, out SpotData spot)
        {
            EnsureLookupBuilt();
            if (_spotLookup.TryGetValue((x, y), out int idx))
            {
                spot = Spots[idx];
                return true;
            }
            spot = default;
            return false;
        }

        /// <summary>좌표가 도시가 차지한 타일인지 (O(1)).</summary>
        public bool IsCityTile(int x, int y)
        {
            EnsureLookupBuilt();
            return _cityLookup.ContainsKey((x, y));
        }

        /// <summary>좌표가 스폿이 차지한 타일인지 (O(1)).</summary>
        public bool IsSpotTile(int x, int y)
        {
            EnsureLookupBuilt();
            return _spotLookup.ContainsKey((x, y));
        }

        private void EnsureLookupBuilt()
        {
            if (_cityLookup == null || _spotLookup == null)
                BuildLookupMaps();
        }
    }
}
