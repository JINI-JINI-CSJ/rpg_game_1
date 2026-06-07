namespace WorldForge
{
    /// <summary>
    /// 월드 생성 파라미터 — 순수 C# (Unity 의존 없음)
    /// </summary>
    [System.Serializable]
    public class WorldGenSettings
    {
        // ── 기본 ──────────────────────────────────────────────────
        public int Seed          = 42069;
        public int MapWidth      = 256;
        public int MapHeight     = 160;

        // ── 지형 노이즈 ───────────────────────────────────────────
        public float NoiseScale    = 3.5f;  // 클수록 대륙이 완만
        public int   Octaves       = 6;     // 디테일 레이어 수
        public float Persistence   = 0.50f; // 각 옥타브 감쇠율
        public float ContinentBias = 0.40f; // 중앙 육지 편향 강도
        public float EdgeFalloff   = 0.70f; // 가장자리 바다 강도

        // ── 해수면 / 산 ───────────────────────────────────────────
        /// <summary>0~1: 전체 타일 중 이 비율이 바다가 됨</summary>
        public float SeaLevel    = 0.42f;

        // ── 지물 수 (상한 없음 — 맵 크기에 맞게 조절) ──────────────
        public int NumNations  = 6;
        public int NumCities   = 18;
        public int NumRivers   = 10;
        public int NumSpots    = 12;

        // ── 프리셋 ────────────────────────────────────────────────
        public static WorldGenSettings Archipelago() => new()
        {
            NoiseScale    = 2.0f,
            Octaves       = 7,
            ContinentBias = 0.10f,
            EdgeFalloff   = 0.30f,
            SeaLevel      = 0.60f,
            NumNations    = 8,
            NumCities     = 20,
        };

        public static WorldGenSettings Pangaea() => new()
        {
            NoiseScale    = 5.0f,
            Octaves       = 5,
            ContinentBias = 0.70f,
            EdgeFalloff   = 0.90f,
            SeaLevel      = 0.35f,
            NumNations    = 6,
        };

        public static WorldGenSettings Mountainous() => new()
        {
            NoiseScale  = 2.5f,
            Octaves     = 8,
            Persistence = 0.65f,
            SeaLevel    = 0.40f,
        };
    }
}
