using UnityEngine;
using System.Collections.Generic;

namespace WorldForge
{
    /// <summary>
    /// WorldData → Unity Texture2D 렌더러
    /// </summary>
    public static class WorldMapRenderer
    {
        // ── 바이옴 기본색 ──────────────────────────────────────────
        private static Color BiomeBaseColor(BiomeType b) => b switch
        {
            BiomeType.DeepOcean    => new Color(0.04f, 0.09f, 0.16f),
            BiomeType.Ocean        => new Color(0.09f, 0.20f, 0.36f),
            BiomeType.ShallowOcean => new Color(0.16f, 0.44f, 0.63f),
            BiomeType.Coast        => new Color(0.78f, 0.72f, 0.47f),
            BiomeType.Grassland    => new Color(0.35f, 0.54f, 0.24f),
            BiomeType.Forest       => new Color(0.24f, 0.42f, 0.16f),
            BiomeType.Desert       => new Color(0.83f, 0.75f, 0.44f),
            BiomeType.Highland     => new Color(0.42f, 0.44f, 0.38f),
            BiomeType.Mountain     => new Color(0.42f, 0.44f, 0.38f),
            BiomeType.HighMountain => new Color(0.60f, 0.60f, 0.60f),
            BiomeType.Snow         => new Color(0.91f, 0.91f, 0.94f),
            BiomeType.Tundra       => new Color(0.69f, 0.82f, 0.85f),
            _                      => Color.magenta
        };

        private static Color NationColor(NationData n) =>
            new Color(n.R / 255f, n.G / 255f, n.B / 255f);

        // ── 스폿 색상 ──────────────────────────────────────────────
        public static Color SpotColor(SpotType t) => t switch
        {
            SpotType.Dungeon     => new Color(0.80f, 0.20f, 0.20f),
            SpotType.AncientRuin => new Color(0.78f, 0.66f, 0.29f),
            SpotType.MagicTower  => new Color(0.53f, 0.33f, 0.80f),
            SpotType.Graveyard   => new Color(0.40f, 0.53f, 0.67f),
            SpotType.Volcano     => new Color(1.00f, 0.40f, 0.00f),
            SpotType.DragonLair  => new Color(0.20f, 0.73f, 0.27f),
            _                    => Color.white
        };

        public static string SpotEmoji(SpotType t) => t switch
        {
            SpotType.Dungeon     => "⚔",
            SpotType.AncientRuin => "🏛",
            SpotType.MagicTower  => "🗼",
            SpotType.Graveyard   => "💀",
            SpotType.Volcano     => "🌋",
            SpotType.DragonLair  => "🐉",
            _                    => "?"
        };

        // ════════════════════════════════════════════════════════
        // 메인 렌더 — Texture2D 반환 (1타일 = 1픽셀, GPU 업스케일)
        // ════════════════════════════════════════════════════════
        public static Texture2D RenderToTexture(WorldData w, RenderOptions opt)
        {
            int W = w.Width, H = w.Height;
            var tex = new Texture2D(W, H, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp
            };
            var pixels = new Color[W * H];

            // ── 1. 지형 베이스 ──────────────────────────────────────
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    int idx = w.Idx(x, y);
                    Color c = BiomeBaseColor(w.Biomes[idx]);

                    // 국가 색 혼합 (육지만)
                    if (opt.ShowNations && w.NationMap[idx] >= 0)
                    {
                        var nat = w.Nations[w.NationMap[idx]];
                        Color nc = NationColor(nat);
                        c = Color.Lerp(c, nc, 0.38f);
                    }

                    // 높이 음영
                    float shade = 0.82f + w.HeightMap[idx] * 0.32f;
                    c = new Color(
                        Mathf.Clamp01(c.r * shade),
                        Mathf.Clamp01(c.g * shade),
                        Mathf.Clamp01(c.b * shade),
                        1f);

                    pixels[y * W + x] = c;
                }
            }

            // ── 2. 국경선 ───────────────────────────────────────────
            if (opt.ShowBorders)
            {
                Color borderCol = new Color(0, 0, 0, 0.45f);
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                    {
                        int n = w.NationMap[w.Idx(x, y)];
                        if (n < 0) continue;
                        if (x + 1 < W && w.NationMap[w.Idx(x + 1, y)] != n && w.NationMap[w.Idx(x + 1, y)] >= 0)
                            pixels[y * W + (x + 1)] = BlendAlpha(pixels[y * W + (x + 1)], borderCol);
                        if (y + 1 < H && w.NationMap[w.Idx(x, y + 1)] != n && w.NationMap[w.Idx(x, y + 1)] >= 0)
                            pixels[(y + 1) * W + x] = BlendAlpha(pixels[(y + 1) * W + x], borderCol);
                    }
            }

            // ── 3. 강 ───────────────────────────────────────────────
            if (opt.ShowRivers)
            {
                var riverCol = new Color(0.20f, 0.47f, 0.78f, 0.75f);
                foreach (var path in w.Rivers)
                    foreach (var pt in path)
                        if (w.InBounds(pt[0], pt[1]))
                            pixels[w.Idx(pt[0], pt[1])] = BlendAlpha(pixels[w.Idx(pt[0], pt[1])], riverCol);
            }

            // ── 4. 격자 ─────────────────────────────────────────────
            if (opt.ShowGrid)
            {
                var gridCol = new Color(1f, 1f, 1f, 0.07f);
                for (int y = 0; y < H; y += opt.GridInterval)
                    for (int x = 0; x < W; x++)
                        pixels[y * W + x] = BlendAlpha(pixels[y * W + x], gridCol);
                for (int x = 0; x < W; x += opt.GridInterval)
                    for (int y = 0; y < H; y++)
                        pixels[y * W + x] = BlendAlpha(pixels[y * W + x], gridCol);
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        // ── 오버레이용 Texture2D (도시/스폿/교역로 — 별도 레이어) ─
        public static Texture2D RenderOverlay(WorldData w, RenderOptions opt)
        {
            int W = w.Width, H = w.Height;
            var tex = new Texture2D(W, H, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp
            };
            var pixels = new Color[W * H];
            // 전부 투명으로 초기화
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

            // ── 교역로 ───────────────────────────────────────────────
            if (opt.ShowRoads)
            {
                var roadCol = new Color(0.82f, 0.71f, 0.31f, 0.50f);
                foreach (var (ai, bi) in w.Roads)
                {
                    var a = w.Cities[ai];
                    var b = w.Cities[bi];
                    DrawLine(pixels, W, H, a.X, a.Y, b.X, b.Y, roadCol);
                }
            }

            // ── 도시 ────────────────────────────────────────────────
            if (opt.ShowCities)
            {
                // 소도시 → 중도시 → 대도시 → 수도 순으로 그려 위에 덮이게
                CityTier[] drawOrder = { CityTier.Village, CityTier.Minor,
                                         CityTier.Major,   CityTier.Capital };
                foreach (var tier in drawOrder)
                {
                    foreach (var c in w.Cities)
                    {
                        if (c.Tier != tier) continue;

                        switch (c.Tier)
                        {
                            case CityTier.Capital:
                                // 수도: 금색 큰 원 + 이중 링
                                DrawCircle(pixels, W, H, c.X, c.Y, 3,
                                    new Color(0.97f, 0.85f, 0.20f));
                                DrawCircleOutline(pixels, W, H, c.X, c.Y, 4,
                                    new Color(0.97f, 0.85f, 0.20f, 0.90f));
                                DrawCircleOutline(pixels, W, H, c.X, c.Y, 6,
                                    new Color(0.97f, 0.85f, 0.20f, 0.45f));
                                break;

                            case CityTier.Major:
                                // 대도시: 주황 중간 원 + 외곽 링
                                DrawCircle(pixels, W, H, c.X, c.Y, 2,
                                    new Color(0.92f, 0.60f, 0.15f));
                                DrawCircleOutline(pixels, W, H, c.X, c.Y, 3,
                                    new Color(0.92f, 0.60f, 0.15f, 0.70f));
                                break;

                            case CityTier.Minor:
                                // 중도시: 밝은 갈색 작은 원
                                DrawCircle(pixels, W, H, c.X, c.Y, 1,
                                    new Color(0.80f, 0.48f, 0.12f));
                                DrawCircleOutline(pixels, W, H, c.X, c.Y, 2,
                                    new Color(0.80f, 0.48f, 0.12f, 0.55f));
                                break;

                            case CityTier.Village:
                                // 소도시: 어두운 점 하나
                                DrawDot(pixels, W, H, c.X, c.Y,
                                    new Color(0.60f, 0.35f, 0.08f));
                                break;
                        }
                    }
                }
            }

            // ── 스폿 ────────────────────────────────────────────────
            if (opt.ShowSpots)
            {
                foreach (var sp in w.Spots)
                {
                    Color col = SpotColor(sp.Type);
                    DrawCircleOutline(pixels, W, H, sp.X, sp.Y, 2, col);
                    DrawDot(pixels, W, H, sp.X, sp.Y, new Color(0f, 0f, 0f, 0.6f));
                    DrawDot(pixels, W, H, sp.X, sp.Y, col * 0.5f + Color.white * 0.5f);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        // ── 픽셀 드로잉 헬퍼 ─────────────────────────────────────
        private static void DrawLine(Color[] px, int W, int H, int x0, int y0, int x1, int y1, Color col)
        {
            int dx = Mathf.Abs(x1 - x0), dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                if (x0 >= 0 && x0 < W && y0 >= 0 && y0 < H)
                    px[y0 * W + x0] = BlendAlpha(px[y0 * W + x0], col);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 <  dx) { err += dx; y0 += sy; }
            }
        }

        private static void DrawCircle(Color[] px, int W, int H, int cx, int cy, int r, Color col)
        {
            for (int dy = -r; dy <= r; dy++)
                for (int dx = -r; dx <= r; dx++)
                    if (dx * dx + dy * dy <= r * r)
                    {
                        int nx = cx + dx, ny = cy + dy;
                        if (nx >= 0 && nx < W && ny >= 0 && ny < H)
                            px[ny * W + nx] = col;
                    }
        }

        private static void DrawCircleOutline(Color[] px, int W, int H, int cx, int cy, int r, Color col)
        {
            for (int deg = 0; deg < 360; deg += 5)
            {
                float rad = deg * Mathf.Deg2Rad;
                int nx = cx + Mathf.RoundToInt(Mathf.Cos(rad) * r);
                int ny = cy + Mathf.RoundToInt(Mathf.Sin(rad) * r);
                if (nx >= 0 && nx < W && ny >= 0 && ny < H)
                    px[ny * W + nx] = col;
            }
        }

        private static void DrawDot(Color[] px, int W, int H, int x, int y, Color col)
        {
            if (x >= 0 && x < W && y >= 0 && y < H) px[y * W + x] = col;
        }

        private static Color BlendAlpha(Color bg, Color fg)
        {
            float a = fg.a;
            return new Color(
                bg.r * (1 - a) + fg.r * a,
                bg.g * (1 - a) + fg.g * a,
                bg.b * (1 - a) + fg.b * a,
                1f);
        }
    }

    // ── 렌더 옵션 ─────────────────────────────────────────────────
    [System.Serializable]
    public class RenderOptions
    {
        public bool ShowNations  = true;
        public bool ShowBorders  = false;
        public bool ShowRivers   = true;
        public bool ShowRoads    = true;
        public bool ShowCities   = true;
        public bool ShowSpots    = true;
        public bool ShowGrid     = false;
        public int  GridInterval = 10;
    }
}
