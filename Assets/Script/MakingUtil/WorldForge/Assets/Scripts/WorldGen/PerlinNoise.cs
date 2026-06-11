using System;

namespace WorldForge
{
    /// <summary>
    /// 시드 기반 Ken Perlin Noise + fBm (fractional Brownian motion)
    /// seed가 바뀌면 permutation table이 완전히 달라져 전혀 다른 지형 생성
    /// </summary>
    public class PerlinNoise
    {
        private readonly byte[] _perm = new byte[512];

        public PerlinNoise(int seed)
        {
            var rng = new Mulberry32(seed);
            byte[] p = new byte[256];
            for (int i = 0; i < 256; i++) p[i] = (byte)i;

            // Fisher-Yates 셔플
            for (int i = 255; i > 0; i--)
            {
                int j = rng.NextInt(i + 1);
                (p[i], p[j]) = (p[j], p[i]);
            }
            // 두 배로 복사 (wrap 처리용)
            for (int i = 0; i < 512; i++) _perm[i] = p[i & 255];
        }

        // ── 내부 ──────────────────────────────────────────────────
        private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
        private static float Lerp(float a, float b, float t) => a + t * (b - a);

        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 7;
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
        }

        /// <summary>
        /// 2D Perlin Noise — 반환값 범위 약 -1 ~ 1
        /// </summary>
        public float Noise(float x, float y)
        {
            int X = (int)MathF.Floor(x) & 255;
            int Y = (int)MathF.Floor(y) & 255;
            float xf = x - MathF.Floor(x);
            float yf = y - MathF.Floor(y);
            float u = Fade(xf), v = Fade(yf);

            int A  = _perm[X]     + Y;
            int AA = _perm[A];   int AB = _perm[A + 1];
            int B  = _perm[X + 1] + Y;
            int BA = _perm[B];   int BB = _perm[B + 1];

            return Lerp(
                Lerp(Grad(_perm[AA], xf,     yf    ), Grad(_perm[BA], xf - 1, yf    ), u),
                Lerp(Grad(_perm[AB], xf,     yf - 1), Grad(_perm[BB], xf - 1, yf - 1), u),
                v
            );
        }

        /// <summary>
        /// fBm — scale / octaves / persistence 를 모두 반영한 분수 브라운 운동
        /// 반환값 범위 약 -1 ~ 1
        /// </summary>
        public float FBm(float x, float y, int octaves, float scale, float persistence)
        {
            float val   = 0f;
            float amp   = 1f;
            float freq  = 1f;
            float maxV  = 0f;

            for (int i = 0; i < octaves; i++)
            {
                val  += Noise(x * freq / scale, y * freq / scale) * amp;
                maxV += amp;
                amp  *= persistence;
                freq *= 2f;
            }
            return val / maxV;
        }
    }
}
