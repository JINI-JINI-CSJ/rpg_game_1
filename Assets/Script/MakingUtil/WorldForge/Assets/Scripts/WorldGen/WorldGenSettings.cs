namespace WorldForge
{
    [System.Serializable]
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

        // ── 해수면 ────────────────────────────────────────────────
        public float SeaLevel = 0.42f;

        // ── 국가 / 강 ─────────────────────────────────────────────
        public int NumNations = 6;
        public int NumRivers  = 10;

        // ── 도시 등급별 수 ────────────────────────────────────────
        public int NumMajorCities  = 12;
        public int NumMinorCities  = 20;
        public int NumVillages     = 30;

        // ── 스폿 종류별 수 (5가지 독립 설정) ─────────────────────
        public int NumDungeons    = 4;
        public int NumRuins       = 4;
        public int NumMagicTowers = 3;
        public int NumGraveyards  = 3;
        public int NumVolcanoes   = 2;

        // 총 도시 수 (수도 = 국가 수)
        public int TotalCities =>
            NumNations + NumMajorCities + NumMinorCities + NumVillages;

        // 총 스폿 수
        public int TotalSpots =>
            NumDungeons + NumRuins + NumMagicTowers + NumGraveyards + NumVolcanoes;

        // ── 프리셋 ────────────────────────────────────────────────
        public static WorldGenSettings Archipelago() => new WorldGenSettings
        {
            NoiseScale = 2.0f, Octaves = 7,
            ContinentBias = 0.10f, EdgeFalloff = 0.30f, SeaLevel = 0.60f,
            NumNations = 8,
            NumMajorCities = 16, NumMinorCities = 24, NumVillages = 40,
            NumDungeons = 6, NumRuins = 6, NumMagicTowers = 4,
            NumGraveyards = 4, NumVolcanoes = 2,
        };

        public static WorldGenSettings Pangaea() => new WorldGenSettings
        {
            NoiseScale = 5.0f, Octaves = 5,
            ContinentBias = 0.70f, EdgeFalloff = 0.90f, SeaLevel = 0.35f,
            NumNations = 6,
            NumMajorCities = 12, NumMinorCities = 20, NumVillages = 30,
            NumDungeons = 4, NumRuins = 4, NumMagicTowers = 3,
            NumGraveyards = 3, NumVolcanoes = 2,
        };

        public static WorldGenSettings Mountainous() => new WorldGenSettings
        {
            NoiseScale = 2.5f, Octaves = 8, Persistence = 0.65f, SeaLevel = 0.40f,
            NumNations = 5,
            NumMajorCities = 8, NumMinorCities = 14, NumVillages = 20,
            NumDungeons = 6, NumRuins = 3, NumMagicTowers = 4,
            NumGraveyards = 2, NumVolcanoes = 4,
        };
    }
}
