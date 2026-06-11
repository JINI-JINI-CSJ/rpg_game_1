using System;
using System.Collections.Generic;

namespace WorldForge
{
    /// <summary>
    /// 월드맵 생성 파이프라인 — 순수 C# (Unity 의존 없음)
    /// </summary>
    public static class WorldGenerator
    {
        // ── 국가 이름 풀 ──────────────────────────────────────────
        private static readonly string[] NationNames =
        {
            "아에로시아","발드로스","선메르","드락홀드","퍼른솔","애쉬가드",
            "크리스탈리아","아이언메르","쏜우드","엠버바스트","프로스트마크","섀도우렐름",
            "골든마크","에메랄디아",
        };

        private static readonly (byte r, byte g, byte b)[] NationColors =
        {
            (155,34,34),(34,82,155),(34,139,68),(139,107,18),
            (104,34,139),(34,122,122),(139,66,24),(58,104,42),
            (130,50,104),(42,66,122),(114,98,24),(50,50,139),
            (139,82,50),(40,98,82),
        };

        // ── 도시 이름 음절 ────────────────────────────────────────
        private static readonly string[] Syl1 = { "아","엘","모","발","카","토르","에르","벨","코","달","페르","갈","할","이르","자르" };
        private static readonly string[] Syl2 = { "란","엔","인","온","우르","아스","에스","이움","오스","악스","엑스","오르","이아","이스","안" };
        private static readonly string[] Suf  = { "성","요새","항구","관문","탑","마을","포구","고원","협곡","숲","평원","호수" };

        private static string RandCityName(Mulberry32 rng) =>
            Syl1[rng.NextInt(Syl1.Length)] + Syl2[rng.NextInt(Syl2.Length)] + " " + Suf[rng.NextInt(Suf.Length)];

        // ── 스폿 정의 ─────────────────────────────────────────────
        private static readonly (SpotType type, string[] names, bool prefHigh, bool allowMount)[] SpotDefs =
        {
            (SpotType.Dungeon,     new[]{"어둠의 지하","철의 미궁","고블린 소굴","트롤 동굴","흑마의 지하","저주받은 던전","심연의 미로","지옥의 입구"}, true,  true ),
            (SpotType.AncientRuin, new[]{"고대 신전","잊혀진 왕국","석상의 묘","사라진 제국","낡은 제단","황폐한 도시","고대의 탑","침묵의 신전"},   false, false),
            (SpotType.MagicTower,  new[]{"별의 탑","결정탑","점술사의 탑","금지된 탑","에테르 첨탑","마법사의 은신처","칠흑탑","섬광의 탑"},         true,  false),
            (SpotType.Graveyard,   new[]{"전사자의 묘","저주받은 묘지","영혼의 안식처","망자의 언덕","흑사의 묘","어둠의 납골당","영원한 잠","무너진 영묘"}, false, false),
            (SpotType.Volcano,     new[]{"화염산","용암의 심장","지옥의 불구덩이","불타는 봉우리","마그마의 심연","화신의 산","적화봉","분노의 화구"}, true,  true ),
            (SpotType.DragonLair,  new[]{"고대 용의 소굴","드래곤 봉우리","비늘 산","화룡의 둥지","폭풍룡의 절벽","빙룡의 동굴","독룡의 계곡","흑룡의 성채"}, true, true),
        };

        // ════════════════════════════════════════════════════════
        // MAIN ENTRY
        // ════════════════════════════════════════════════════════
        public static WorldData Generate(WorldGenSettings s)
        {
            var world = new WorldData(s.MapWidth, s.MapHeight);
            world.Settings = s;

            var rng = new Mulberry32(s.Seed);

            int W = s.MapWidth, H = s.MapHeight;
            int maxDim = Math.Max(W, H);

            // ── Step 1: Perlin 노이즈 객체 생성 (seed XOR 로 독립) ──
            var perlinH = new PerlinNoise(s.Seed);
            var perlinT = new PerlinNoise(s.Seed ^ unchecked((int)0xABCD1234));
            var perlinM = new PerlinNoise(s.Seed ^ unchecked((int)0xDEAD5678));

            float noiseScaleN = s.NoiseScale * maxDim / 10f;
            float noiseScaleT = s.NoiseScale * maxDim / 15f;
            float noiseScaleM = s.NoiseScale * maxDim / 12f;

            // ── Step 2: 높이맵 + 온도맵 ─────────────────────────────
            float hMin = float.MaxValue, hMax = float.MinValue;

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    int idx = world.Idx(x, y);

                    // fBm height (-1~1 → 0~1)
                    float h = (perlinH.FBm(x, y, s.Octaves, noiseScaleN, s.Persistence) + 1f) * 0.5f;

                    // Edge falloff (타원형)
                    float nx = (x / (float)W) * 2f - 1f;
                    float ny = (y / (float)H) * 2f - 1f;
                    float dist = MathF.Sqrt(nx * nx + ny * ny);
                    float edge = Math.Max(0f, 1f - dist * s.EdgeFalloff);

                    h = h * (1f - s.ContinentBias) + edge * s.ContinentBias;

                    world.HeightMap[idx] = h;
                    if (h < hMin) hMin = h;
                    if (h > hMax) hMax = h;

                    // 온도 (위도 + 노이즈 + 고도 보정)
                    float lat = 1f - MathF.Abs(y / (float)H - 0.5f) * 2f;
                    float tn  = (perlinT.FBm(x, y, 3, noiseScaleT, 0.5f) + 1f) * 0.5f;
                    world.TempMap[idx] = Math.Clamp(lat * 0.75f + tn * 0.25f - h * 0.30f + 0.1f, 0f, 1f);
                }
            }

            // 높이 정규화 0~1
            float hRange = hMax - hMin;
            if (hRange < 1e-5f) hRange = 1f;
            for (int i = 0; i < world.HeightMap.Length; i++)
                world.HeightMap[i] = (world.HeightMap[i] - hMin) / hRange;

            // ── Step 3: 해수면 임계값 — 퍼센타일 기반 ─────────────
            float[] sorted = (float[])world.HeightMap.Clone();
            Array.Sort(sorted);
            world.SeaThreshold   = sorted[(int)(sorted.Length * s.SeaLevel)];
            world.MountThreshold = sorted[(int)(sorted.Length * 0.85f)];

            // ── Step 4: 바이옴 분류 ──────────────────────────────────
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    int idx = world.Idx(x, y);
                    world.Biomes[idx] = BiomeClassifier.Classify(
                        world.HeightMap[idx], world.TempMap[idx],
                        world.SeaThreshold, world.MountThreshold);
                }

            // ── Step 5: 강 생성 ──────────────────────────────────────
            GenerateRivers(world, s, rng);

            // ── Step 6: 국가 영토 (Voronoi) ──────────────────────────
            GenerateNations(world, s, rng);

            // ── Step 7: 도시 배치 ────────────────────────────────────
            GenerateCities(world, s, rng);

            // ── Step 8: 교역로 ───────────────────────────────────────
            GenerateRoads(world);

            // ── Step 9: 특수 스폿 ────────────────────────────────────
            GenerateSpots(world, s, rng);

            return world;
        }

        // ════════════════════════════════════════════════════════
        // STEP 5: 강
        // ════════════════════════════════════════════════════════
        private static void GenerateRivers(WorldData w, WorldGenSettings s, Mulberry32 rng)
        {
            int W = w.Width, H = w.Height;
            float seaTh = w.SeaThreshold;
            float mountMid = seaTh + (w.MountThreshold - seaTh) * 0.5f;

            // 높은 타일 후보 수집 (산 중간 이상)
            var highTiles = new List<int>();
            for (int i = 0; i < w.HeightMap.Length; i++)
                if (w.HeightMap[i] > mountMid) highTiles.Add(i);
            highTiles.Sort((a, b) => w.HeightMap[b].CompareTo(w.HeightMap[a]));

            var usedStarts = new HashSet<int>();
            int attempts = 0;

            while (w.Rivers.Count < s.NumRivers && attempts < highTiles.Count)
            {
                int startIdx = highTiles[attempts++];
                int sx = startIdx % W, sy = startIdx / W;
                int startKey = (sy >> 2) * 200 + (sx >> 2);
                if (!usedStarts.Add(startKey)) continue;

                var path = new List<int[]>();
                int cx = sx, cy = sy;
                var visited = new HashSet<int>();

                for (int step = 0; step < 500; step++)
                {
                    int key = w.Idx(cx, cy);
                    if (!visited.Add(key)) break;
                    path.Add(new[] { cx, cy });
                    if (w.HeightMap[key] < seaTh) break;

                    // 가장 낮은 이웃 탐색
                    int bx = cx, by = cy;
                    float bh = w.HeightMap[key];
                    int[][] dirs = { new[]{-1,0}, new[]{1,0}, new[]{0,-1}, new[]{0,1},
                                     new[]{-1,-1}, new[]{1,-1}, new[]{-1,1}, new[]{1,1} };
                    foreach (var d in dirs)
                    {
                        int nx = cx + d[0], ny = cy + d[1];
                        if (!w.InBounds(nx, ny)) continue;
                        float nh = w.HeightMap[w.Idx(nx, ny)];
                        if (nh < bh) { bh = nh; bx = nx; by = ny; }
                    }
                    if (bx == cx && by == cy) break; // 고원
                    cx = bx; cy = by;
                    w.RiverMap[w.Idx(cx, cy)] = true;
                }

                if (path.Count > 12)
                    w.Rivers.Add(path.ToArray());
            }
        }

        // ════════════════════════════════════════════════════════
        // STEP 6: 국가
        // ════════════════════════════════════════════════════════
        private static void GenerateNations(WorldData w, WorldGenSettings s, Mulberry32 rng)
        {
            int W = w.Width, H = w.Height;

            // 육지 타일 목록
            var landTiles = new List<int>();
            for (int i = 0; i < w.HeightMap.Length; i++)
                if (BiomeClassifier.IsLand(w.Biomes[i])) landTiles.Add(i);
            if (landTiles.Count == 0) return;

            // Farthest-point 수도 배치
            var capPositions = new List<(int x, int y)>();
            int fi = landTiles[rng.NextInt(landTiles.Count)];
            capPositions.Add((fi % W, fi / W));

            int sampleCount = Math.Max(80, landTiles.Count / 12);

            for (int n = 1; n < s.NumNations; n++)
            {
                int bx = -1, by = -1;
                float bd = -1f;
                for (int t = 0; t < sampleCount; t++)
                {
                    int li = landTiles[rng.NextInt(landTiles.Count)];
                    int lx = li % W, ly = li / W;
                    float minD = float.MaxValue;
                    foreach (var cap in capPositions)
                        minD = Math.Min(minD, (lx - cap.x) * (lx - cap.x) + (ly - cap.y) * (ly - cap.y));
                    if (minD > bd) { bd = minD; bx = lx; by = ly; }
                }
                if (bx >= 0) capPositions.Add((bx, by));
            }

            // NationData 생성
            for (int n = 0; n < capPositions.Count; n++)
            {
                var (nx, ny) = capPositions[n];
                var col = NationColors[n % NationColors.Length];
                w.Nations.Add(new NationData
                {
                    Id = n,
                    CapitalX = nx, CapitalY = ny,
                    Name = NationNames[n % NationNames.Length],
                    R = col.r, G = col.g, B = col.b
                });
            }

            // Voronoi 영토 분할
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    if (!BiomeClassifier.IsLand(w.Biomes[w.Idx(x, y)])) continue;
                    int best = -1;
                    float bd2 = float.MaxValue;
                    for (int n = 0; n < capPositions.Count; n++)
                    {
                        float d = (x - capPositions[n].x) * (x - capPositions[n].x)
                                + (y - capPositions[n].y) * (y - capPositions[n].y);
                        if (d < bd2) { bd2 = d; best = n; }
                    }
                    w.NationMap[w.Idx(x, y)] = best;
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // STEP 7: 도시 (4등급 — 수도 / 대도시 / 중도시 / 소도시)
        // ════════════════════════════════════════════════════════
        private static void GenerateCities(WorldData w, WorldGenSettings s, Mulberry32 rng)
        {
            int W = w.Width, H = w.Height;
            float seaTh   = w.SeaThreshold;
            float mountTh = w.MountThreshold;

            // ── 타일 점수 계산 (입지 점수) ────────────────────────
            var perlinM  = new PerlinNoise(s.Seed ^ unchecked((int)0xCAFEBABE));
            float scaleM = s.NoiseScale * Math.Max(W, H) / 10f;

            var scores = new float[W * H];
            int[][] dirs4 = { new[]{-1,0},new[]{1,0},new[]{0,-1},new[]{0,1} };
            int[][] dirs5 = { new[]{-1,0},new[]{1,0},new[]{0,-1},new[]{0,1},new[]{0,0} };

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    float h = w.HeightMap[w.Idx(x, y)];
                    if (h < seaTh || h > mountTh) continue;

                    float sc = 0f;
                    foreach (var d in dirs4)
                    {
                        int nx = x+d[0], ny = y+d[1];
                        if (w.InBounds(nx,ny) && w.RiverMap[w.Idx(nx,ny)]) sc += 2.5f;
                    }
                    foreach (var d in dirs5)
                    {
                        int nx = x+d[0], ny = y+d[1];
                        if (w.InBounds(nx,ny) && w.HeightMap[w.Idx(nx,ny)] < seaTh) sc += 1.2f;
                    }
                    sc += (1f - (h - seaTh) / (mountTh - seaTh)) * 1.5f;
                    sc += (perlinM.FBm(x, y, 2, scaleM, 0.5f) + 1f) * 0.5f * 0.4f;
                    scores[w.Idx(x, y)] = sc;
                }
            }

            // 후보 목록 (점수 내림차순)
            var allCands = new List<int>();
            for (int i = 0; i < scores.Length; i++)
                if (scores[i] > 0f) allCands.Add(i);
            allCands.Sort((a, b) => scores[b].CompareTo(scores[a]));

            var nameRng = new Mulberry32(s.Seed ^ 0x9999);

            // ── 총 도시 수 기반 글로벌 최소 거리 ─────────────────
            int totalCities = s.TotalCities;
            // 육지 타일 수 추정으로 평균 간격 계산
            int landEst = (int)(W * H * (1f - s.SeaLevel));
            float baseSpacing = MathF.Sqrt((float)landEst / Math.Max(totalCities, 1));

            // 등급별 최소 거리 (수도 > 대도시 > 중도시 > 소도시)
            float capMinD    = baseSpacing * 3.0f;   // 수도끼리
            float majorMinD  = baseSpacing * 2.0f;   // 대도시끼리
            float minorMinD  = baseSpacing * 1.2f;   // 중도시끼리
            float villageMinD= baseSpacing * 0.8f;   // 소도시끼리
            // 상위 등급이 하위 등급 배치를 막는 거리
            float capBlockD  = baseSpacing * 1.8f;   // 수도→대도시/중도시/소도시
            float majorBlockD= baseSpacing * 1.0f;   // 대도시→중도시/소도시
            float minorBlockD= baseSpacing * 0.6f;   // 중도시→소도시

            // ── 1단계: 수도 배치 (국가 수도 = NumNations) ─────────
            // 국가 중심(캐피탈 포지션)에서 가장 가까운 고점수 타일 선택
            var capPositions = new List<(int cx, int cy)>();
            foreach (var nat in w.Nations)
                capPositions.Add((nat.CapitalX, nat.CapitalY));

            foreach (var (capX, capY) in capPositions)
            {
                // 수도 후보: 해당 국가 영토 안, 점수 높은 순
                int natId = w.NationMap[w.Idx(capX, capY)];
                (int bx, int by, float bs) = (capX, capY, -1f);

                foreach (int idx in allCands)
                {
                    int cx = idx % W, cy = idx / W;
                    if (w.NationMap[idx] != natId) continue;

                    // 다른 수도와 최소 거리
                    bool tooClose = false;
                    foreach (var c in w.Cities)
                        if (c.Tier == CityTier.Capital)
                        {
                            float d2 = (cx-c.X)*(cx-c.X)+(float)(cy-c.Y)*(cy-c.Y);
                            if (d2 < capMinD * capMinD) { tooClose = true; break; }
                        }
                    if (tooClose) continue;

                    if (scores[idx] > bs) { bs = scores[idx]; bx = cx; by = cy; }
                    if (bs > 0f) break; // 첫 번째 고점수 타일로 충분
                }

                w.Cities.Add(new CityData
                {
                    X = bx, Y = by,
                    Name   = RandCityName(nameRng),
                    Nation = w.NationMap[w.Idx(bx, by)],
                    Tier   = CityTier.Capital,
                    Score  = scores[w.Idx(bx, by)],
                });
            }

            // ── 공통 배치 함수 ────────────────────────────────────
            // selfMinD:  같은 등급끼리 최소 거리
            // blockD:    상위 등급(수도/대도시 등)이 이 등급을 막는 거리 배열
            void PlaceTier(CityTier tier, int count,
                           float selfMinD, float[] blockDists, CityTier[] blockTiers)
            {
                float selfMinD2 = selfMinD * selfMinD;
                int placed = 0;

                foreach (int idx in allCands)
                {
                    if (placed >= count) break;
                    int cx = idx % W, cy = idx / W;

                    bool tooClose = false;
                    foreach (var c in w.Cities)
                    {
                        float d2 = (cx-c.X)*(cx-c.X)+(float)(cy-c.Y)*(cy-c.Y);

                        // 같은 등급끼리 간격
                        if (c.Tier == tier && d2 < selfMinD2)
                            { tooClose = true; break; }

                        // 상위 등급이 막는 거리
                        for (int bi = 0; bi < blockTiers.Length; bi++)
                            if (c.Tier == blockTiers[bi] && d2 < blockDists[bi] * blockDists[bi])
                                { tooClose = true; break; }

                        if (tooClose) break;
                    }
                    if (tooClose) continue;

                    w.Cities.Add(new CityData
                    {
                        X = cx, Y = cy,
                        Name   = RandCityName(nameRng),
                        Nation = w.NationMap[idx],
                        Tier   = tier,
                        Score  = scores[idx],
                    });
                    placed++;
                }
            }

            // ── 2단계: 대도시 ─────────────────────────────────────
            PlaceTier(CityTier.Major,   s.NumMajorCities,
                selfMinD:   majorMinD,
                blockDists: new[]{ capBlockD },
                blockTiers: new[]{ CityTier.Capital });

            // ── 3단계: 중도시 ─────────────────────────────────────
            PlaceTier(CityTier.Minor,   s.NumMinorCities,
                selfMinD:   minorMinD,
                blockDists: new[]{ capBlockD, majorBlockD },
                blockTiers: new[]{ CityTier.Capital, CityTier.Major });

            // ── 4단계: 소도시 ─────────────────────────────────────
            PlaceTier(CityTier.Village, s.NumVillages,
                selfMinD:   villageMinD,
                blockDists: new[]{ capBlockD, majorBlockD, minorBlockD },
                blockTiers: new[]{ CityTier.Capital, CityTier.Major, CityTier.Minor });
        }

        // ════════════════════════════════════════════════════════
        // STEP 8: 교역로
        // ════════════════════════════════════════════════════════
        private static void GenerateRoads(WorldData w)
        {
            int count = w.Cities.Count;
            for (int i = 0; i < count; i++)
            {
                var ci = w.Cities[i];
                // 등급이 높을수록 더 많은 교역로
                int kmax = ci.Tier == CityTier.Capital ? 4
                         : ci.Tier == CityTier.Major   ? 3
                         : ci.Tier == CityTier.Minor   ? 2
                                                        : 1;
                var dists = new List<(int j, float d)>();
                for (int j = 0; j < count; j++)
                    if (i != j)
                    {
                        var cj = w.Cities[j];
                        float d = (cj.X-ci.X)*(cj.X-ci.X)+(float)(cj.Y-ci.Y)*(cj.Y-ci.Y);
                        dists.Add((j, d));
                    }
                dists.Sort((a, b) => a.d.CompareTo(b.d));

                int k = 0;
                foreach (var (j, _) in dists)
                {
                    if (k >= kmax) break;
                    if (i < j) { w.Roads.Add((i, j)); k++; }
                }
            }
        }

        // ════════════════════════════════════════════════════════
        // STEP 9: 특수 스폿
        // ════════════════════════════════════════════════════════
        private static void GenerateSpots(WorldData w, WorldGenSettings s, Mulberry32 rng)
        {
            if (s.NumSpots <= 0) return;

            int W = w.Width, H = w.Height;
            float seaTh = w.SeaThreshold;

            // ── 육지 타일 전체 수집 (고도 조건 없음) ──────────────
            var landCands = new List<(int x, int y)>();
            for (int y = 2; y < H - 2; y++)
                for (int x = 2; x < W - 2; x++)
                    if (w.HeightMap[w.Idx(x, y)] >= seaTh)
                        landCands.Add((x, y));

            if (landCands.Count == 0) return;

            // ── 도시와의 최소 거리² ────────────────────────────────
            // 총 도시 수가 많을수록 거리 기준을 줄여 배치 실패를 방지
            float cityMinD2 = MathF.Pow(
                (W + H) * 0.5f / Math.Max(s.TotalCities + 1, 4) * 1.2f, 2);

            // ── 스폿 간 최소 거리² (육지 면적 기반) ───────────────
            float spotMinD2 = MathF.Pow(
                MathF.Sqrt(landCands.Count / Math.Max(s.NumSpots, 1)) * 0.6f, 2);

            // ── 타입 순환 배분 후 셔플 ────────────────────────────
            var typeSeq = new List<int>();
            for (int i = 0; i < s.NumSpots; i++) typeSeq.Add(i % SpotDefs.Length);
            for (int i = typeSeq.Count - 1; i > 0; i--)
            {
                int j = rng.NextInt(i + 1);
                (typeSeq[i], typeSeq[j]) = (typeSeq[j], typeSeq[i]);
            }

            // ── 배치: 랜덤 샘플링 → 도시·스폿 거리만 체크 ──────────
            // 고도/바이옴 선호 점수는 제거 — 순수 거리 조건만 사용
            var nameIdx = new int[SpotDefs.Length];

            // 시도 횟수: 후보가 적으면 완화
            int maxTries = Math.Max(500, landCands.Count / 2);

            foreach (int si in typeSeq)
            {
                var (sType, names, _, _) = SpotDefs[si];

                (int x, int y) placed = (-1, -1);

                for (int t = 0; t < maxTries; t++)
                {
                    var (cx, cy) = landCands[rng.NextInt(landCands.Count)];

                    // 도시와 최소 거리
                    bool tooClose = false;
                    foreach (var c in w.Cities)
                    {
                        int dx = cx - c.X, dy = cy - c.Y;
                        if (dx * dx + dy * dy < cityMinD2) { tooClose = true; break; }
                    }
                    if (tooClose) continue;

                    // 기존 스폿과 최소 거리
                    foreach (var sp in w.Spots)
                    {
                        int dx = cx - sp.X, dy = cy - sp.Y;
                        if (dx * dx + dy * dy < spotMinD2) { tooClose = true; break; }
                    }
                    if (tooClose) continue;

                    placed = (cx, cy);
                    break;
                }

                // 거리 조건을 만족하는 위치를 못 찾으면 스폿 간 거리만 완화해서 재시도
                if (placed.x < 0)
                {
                    for (int t = 0; t < maxTries; t++)
                    {
                        var (cx, cy) = landCands[rng.NextInt(landCands.Count)];
                        bool tooClose = false;
                        foreach (var c in w.Cities)
                        {
                            int dx = cx - c.X, dy = cy - c.Y;
                            if (dx * dx + dy * dy < cityMinD2 * 0.5f) { tooClose = true; break; }
                        }
                        if (!tooClose) { placed = (cx, cy); break; }
                    }
                }

                if (placed.x < 0) continue; // 그래도 없으면 스킵

                w.Spots.Add(new SpotData
                {
                    X    = placed.x,
                    Y    = placed.y,
                    Type = sType,
                    Name = names[nameIdx[si] % names.Length],
                });
                nameIdx[si]++;
            }
        }
    }
}
