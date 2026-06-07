namespace WorldForge
{
    /// <summary>
    /// 높이 / 온도 → BiomeType 분류 (순수 C#)
    /// </summary>
    public static class BiomeClassifier
    {
        public static BiomeType Classify(float height, float temp,
                                          float seaTh, float mountTh)
        {
            // ── 바다 ──────────────────────────────────────────────
            if (height < seaTh * 0.40f) return BiomeType.DeepOcean;
            if (height < seaTh * 0.70f) return BiomeType.Ocean;
            if (height < seaTh * 0.88f) return BiomeType.Ocean;
            if (height < seaTh)         return BiomeType.ShallowOcean;

            // ── 해안 ──────────────────────────────────────────────
            if (height < seaTh + 0.04f) return BiomeType.Coast;

            // ── 산 ────────────────────────────────────────────────
            if (height > mountTh + 0.10f) return BiomeType.Snow;
            if (height > mountTh + 0.04f) return BiomeType.HighMountain;
            if (height > mountTh)         return BiomeType.Mountain;

            // ── 육지 바이옴 (온도 기반) ───────────────────────────
            if (temp < 0.22f) return BiomeType.Tundra;
            if (temp > 0.78f && height < seaTh + 0.30f) return BiomeType.Desert;
            if (height < seaTh + 0.18f) return BiomeType.Grassland;
            if (height < seaTh + 0.28f) return BiomeType.Forest;
            return BiomeType.Highland;
        }

        public static bool IsOcean(BiomeType b) =>
            b == BiomeType.DeepOcean ||
            b == BiomeType.Ocean     ||
            b == BiomeType.ShallowOcean;

        public static bool IsLand(BiomeType b) => !IsOcean(b);

        public static bool IsMountain(BiomeType b) =>
            b == BiomeType.Mountain     ||
            b == BiomeType.HighMountain ||
            b == BiomeType.Snow;

        /// <summary>도시 / 스폿 배치 가능한 타일</summary>
        public static bool IsHabitable(BiomeType b) =>
            IsLand(b) && b != BiomeType.Coast;

        public static string KoreanName(BiomeType b) => b switch
        {
            BiomeType.DeepOcean    => "심해",
            BiomeType.Ocean        => "바다",
            BiomeType.ShallowOcean => "얕은 바다",
            BiomeType.Coast        => "해안",
            BiomeType.Grassland    => "초원",
            BiomeType.Forest       => "숲",
            BiomeType.Desert       => "사막",
            BiomeType.Highland     => "고원",
            BiomeType.Mountain     => "산악",
            BiomeType.HighMountain => "고산",
            BiomeType.Snow         => "설산",
            BiomeType.Tundra       => "툰드라",
            _                      => "알 수 없음"
        };
    }
}
