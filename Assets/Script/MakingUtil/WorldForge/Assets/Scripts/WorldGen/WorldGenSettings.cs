namespace WorldForge
{
    [System.Serializable]
    public class WorldGenSettings
    {
        // ── 기본 ──────────────────────────────────────────────────
        public int Seed       = 42069;
        public int MapWidth   = 256;
        public int MapHeight  = 160;

        // ── 지형 노이즈 ───────────────────────────────────────────
        public float NoiseScale    = 3.5f;
        public int   Octaves       = 6;
        public float Persistence   = 0.50f;
        public float ContinentBias = 0.40f;
        public float EdgeFalloff   = 0.70f;

        // ── 해수면 ────────────────────────────────────────────────
        public float SeaLevel = 0.42f;

        // ── 국가 / 강 / 스폿 ─────────────────────────────────────
        public int NumNations = 6;
        public int NumRivers  = 10;
        public int NumSpots   = 12;

        // ── 도시 등급별 수 ────────────────────────────────────────
        // 수도는 NumNations 와 연동 (국가당 1개) — 별도 설정 불필요
        public int NumMajorCities  = 12;   // 대도시
        public int NumMinorCities  = 20;   // 중도시
        public int NumVillages     = 30;   // 소도시

        // ── 프리셋 ────────────────────────────────────────────────
        public static WorldGenSettings Archipelago() => new WorldGenSettings
        {
            NoiseScale = 2.0f, Octaves = 7,
            ContinentBias = 0.10f, EdgeFalloff = 0.30f, SeaLevel = 0.60f,
            NumNations = 8,
            NumMajorCities = 16, NumMinorCities = 24, NumVillages = 40,
        };

        public static WorldGenSettings Pangaea() => new WorldGenSettings
        {
            NoiseScale = 5.0f, Octaves = 5,
            ContinentBias = 0.70f, EdgeFalloff = 0.90f, SeaLevel = 0.35f,
            NumNations = 6,
            NumMajorCities = 12, NumMinorCities = 20, NumVillages = 30,
        };

        public static WorldGenSettings Mountainous() => new WorldGenSettings
        {
            NoiseScale = 2.5f, Octaves = 8, Persistence = 0.65f, SeaLevel = 0.40f,
            NumNations = 5,
            NumMajorCities = 8, NumMinorCities = 14, NumVillages = 20,
        };

        // 하위 호환: 구 NumCities → 총 도시 수 반환
        public int TotalCities => NumNations + NumMajorCities + NumMinorCities + NumVillages;
    }
}
