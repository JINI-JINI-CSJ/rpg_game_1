#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace WorldForge
{
    /// <summary>
    /// Unity 에디터 전용 팝업 창 — Unity 6000.x 호환
    /// 메뉴: Tools > World Forge > Open Generator
    /// </summary>
    public class WorldForgeWindow : EditorWindow
    {
        // ── 설정 ─────────────────────────────────────────────────
        private WorldGenSettings _settings  = new WorldGenSettings();
        private RenderOptions    _renderOpts = new RenderOptions();

        // ── 미리보기 텍스처 ───────────────────────────────────────
        private WorldData  _world;
        private Texture2D  _baseTex;
        private Texture2D  _overlayTex;
        private Texture2D  _compositeTex;
        private bool       _isGenerating;

        // ── 스크롤 ────────────────────────────────────────────────
        private Vector2 _scrollSettings;

        // ── 탭 / 줌 ──────────────────────────────────────────────
        private int   _tab;
        private float _zoom = 1f;

        // ── 스타일 캐시 (OnGUI 첫 호출 시 초기화) ────────────────
        private GUIStyle _styleMiniLabel;
        private GUIStyle _styleCenter;
        private GUIStyle _styleSection;
        private bool     _stylesInit;

        // ════════════════════════════════════════════════════════
        [MenuItem("Tools/World Forge/Open Generator")]
        public static void Open()
        {
            var w = GetWindow<WorldForgeWindow>("⚔ World Forge");
            w.minSize = new Vector2(1020, 680);
            w.Show();
        }

        // ════════════════════════════════════════════════════════
        private void InitStyles()
        {
            if (_stylesInit) return;
            _stylesInit = true;

            // Unity 6 에서 toolbarLabel / centeredGreyMiniLabel 제거됨 → 직접 생성
            _styleMiniLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };

            _styleCenter = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = new Color(0.6f, 0.6f, 0.6f) }
            };

            _styleSection = new GUIStyle(EditorStyles.boldLabel)
            {
                margin = new RectOffset(0, 0, 8, 2)
            };
        }

        // ════════════════════════════════════════════════════════
        private void OnGUI()
        {
            InitStyles();
            DrawToolbar();
            GUILayout.BeginHorizontal();
            {
                DrawLeftPanel();
                DrawMapPreview();
            }
            GUILayout.EndHorizontal();
            DrawStatusBar();
        }

        // ── 툴바 ──────────────────────────────────────────────────
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("⚑ Generate", EditorStyles.toolbarButton, GUILayout.Width(100)))
                DoGenerate();
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("🎲 Random Seed", EditorStyles.toolbarButton, GUILayout.Width(110)))
            {
                _settings.Seed = UnityEngine.Random.Range(1, 999999);
                Repaint();
            }

            GUILayout.Space(6);
            if (GUILayout.Button("Archipelago", EditorStyles.toolbarButton)) LoadPreset(WorldGenSettings.Archipelago());
            if (GUILayout.Button("Pangaea",      EditorStyles.toolbarButton)) LoadPreset(WorldGenSettings.Pangaea());
            if (GUILayout.Button("Mountainous", EditorStyles.toolbarButton)) LoadPreset(WorldGenSettings.Mountainous());

            GUILayout.FlexibleSpace();

            // ── 데이터 저장/불러오기 (바이너리 .wfd) ──────────────
            GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);
            if (GUILayout.Button("📂 Load", EditorStyles.toolbarButton, GUILayout.Width(60)))
                LoadWorldData();
            GUI.backgroundColor = Color.white;

            if (_world != null)
            {
                GUI.backgroundColor = new Color(0.6f, 0.85f, 1f);
                if (GUILayout.Button("💾 Save Data", EditorStyles.toolbarButton, GUILayout.Width(90)))
                    SaveWorldData();
                GUI.backgroundColor = Color.white;

                GUI.backgroundColor = new Color(0.5f, 0.8f, 1f);
                if (GUILayout.Button("Save PNG", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    SavePng();
                GUI.backgroundColor = Color.white;
            }

            // toolbarLabel 대신 직접 만든 스타일 사용
            GUILayout.Label($"Zoom: {_zoom:F1}x", _styleMiniLabel, GUILayout.Width(70));
            _zoom = EditorGUILayout.Slider(_zoom, 0.3f, 4f, GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();
        }

        // ── 좌측 설정 패널 ────────────────────────────────────────
        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250), GUILayout.ExpandHeight(true));

            _tab = GUILayout.Toolbar(_tab, new[] { "설정", "레이어", "통계" });

            _scrollSettings = EditorGUILayout.BeginScrollView(_scrollSettings);

            switch (_tab)
            {
                case 0: DrawSettingsTab(); break;
                case 1: DrawLayersTab();   break;
                case 2: DrawStatsTab();    break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        // ── 설정 탭 ───────────────────────────────────────────────
        private void DrawSettingsTab()
        {
            SectionLabel("Seed");
            _settings.Seed = EditorGUILayout.IntField("Seed", _settings.Seed);

            SectionLabel("맵 크기");
            _settings.MapWidth  = SizeField("Width",  _settings.MapWidth,  64, 4096, 64, 2048);
            _settings.MapHeight = SizeField("Height", _settings.MapHeight, 40, 4096, 40, 1280);

            long tileCount = (long)_settings.MapWidth * _settings.MapHeight;
            if (tileCount > 500_000)
                EditorGUILayout.HelpBox(
                    tileCount > 2_000_000
                        ? $"⚠ {tileCount:N0} 타일 — 생성에 수 초 걸릴 수 있습니다 (비동기)"
                        : $"ℹ {tileCount:N0} 타일",
                    tileCount > 2_000_000 ? MessageType.Warning : MessageType.Info);
            else
                EditorGUILayout.LabelField($"타일 수: {tileCount:N0}", _styleMiniLabel);

            SectionLabel("지형 노이즈");
            _settings.NoiseScale    = EditorGUILayout.Slider("Noise Scale",    _settings.NoiseScale,    0.5f, 8f);
            _settings.Octaves       = EditorGUILayout.IntSlider("Octaves",     _settings.Octaves,       2,    9);
            _settings.Persistence   = EditorGUILayout.Slider("Persistence",    _settings.Persistence,   0.2f, 0.8f);
            _settings.ContinentBias = EditorGUILayout.Slider("Continent Bias", _settings.ContinentBias, 0f,   0.8f);
            _settings.EdgeFalloff   = EditorGUILayout.Slider("Edge Falloff",   _settings.EdgeFalloff,   0f,   1f);

            SectionLabel("해수면");
            _settings.SeaLevel = EditorGUILayout.Slider("Sea Level %", _settings.SeaLevel, 0.2f, 0.7f);

            SectionLabel("국가 / 강");
            FeatureCountField("Nations", ref _settings.NumNations, 2, 100, 2);
            FeatureCountField("Rivers",  ref _settings.NumRivers,  0, 200, 0);

            // ── 도시 등급 (가변) ──────────────────────────────────
            SectionLabel("도시 등급");
            EditorGUILayout.LabelField(
                "Tier 0 = 수도 (수도 수는 Nations 와 동일, Count 무시)", _styleMiniLabel);
            DrawTierList();

            // ── 스폿 종류 (가변) ──────────────────────────────────
            SectionLabel("스폿 종류");
            DrawSpotList();

            // 경고
            int  landEst      = (int)(tileCount * (1f - _settings.SeaLevel));
            int  totalFeatures = _settings.TotalCities + _settings.TotalSpots;
            if (totalFeatures > landEst / 4)
                EditorGUILayout.HelpBox(
                    $"⚠ 지물 합계({totalFeatures}) / 추정 육지({landEst:N0})",
                    MessageType.Warning);

            GUILayout.Space(10);
            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("⚑  Generate World", GUILayout.Height(32)))
                DoGenerate();
            GUI.backgroundColor = Color.white;
        }

        // ── 도시 등급 리스트 편집 ─────────────────────────────────
        private void DrawTierList()
        {
            var defs = _settings.CityTierDefs;
            int removeAt = -1;

            for (int i = 0; i < defs.Count; i++)
            {
                var d = defs[i];
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal();

                // 등급 번호 + 이름
                EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(24));
                d.Label = EditorGUILayout.TextField(d.Label, GUILayout.ExpandWidth(true));

                // 색상 박스 (클릭 불가 — 직접 RGB 입력)
                var col = new UnityEngine.Color(d.ColorR / 255f, d.ColorG / 255f, d.ColorB / 255f);
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(16, 16, GUILayout.Width(16)), col);

                if (i > 0 && GUILayout.Button("✕", GUILayout.Width(22))) removeAt = i;
                EditorGUILayout.EndHorizontal();

                // Count (수도 = Tier 0 은 Nations 와 연동이므로 편집 비활성)
                if (i == 0)
                    EditorGUILayout.LabelField("Count", $"{_settings.NumNations} (Nations 에 연동)", _styleMiniLabel);
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Count", GUILayout.Width(46));
                    d.Count = Mathf.Max(0, EditorGUILayout.IntField(d.Count));
                    EditorGUILayout.EndHorizontal();
                }

                // RGB + IconRadius
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("RGB", GUILayout.Width(30));
                d.ColorR = (byte)Mathf.Clamp(EditorGUILayout.IntField(d.ColorR, GUILayout.Width(38)), 0, 255);
                d.ColorG = (byte)Mathf.Clamp(EditorGUILayout.IntField(d.ColorG, GUILayout.Width(38)), 0, 255);
                d.ColorB = (byte)Mathf.Clamp(EditorGUILayout.IntField(d.ColorB, GUILayout.Width(38)), 0, 255);
                EditorGUILayout.LabelField("R", GUILayout.Width(14));
                d.IconRadius = Mathf.Clamp(EditorGUILayout.IntField(d.IconRadius, GUILayout.Width(30)), 1, 8);
                EditorGUILayout.EndHorizontal();

                defs[i] = d;
                EditorGUILayout.EndVertical();
            }

            if (removeAt >= 0) defs.RemoveAt(removeAt);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 등급 추가", GUILayout.Height(22)))
                defs.Add(new CityTierDef($"등급{defs.Count}", 10, 160, 90, 20, 1));
            EditorGUILayout.EndHorizontal();
        }

        // ── 스폿 종류 리스트 편집 ─────────────────────────────────
        private void DrawSpotList()
        {
            var defs = _settings.SpotTypeDefs;
            int removeAt = -1;

            for (int i = 0; i < defs.Count; i++)
            {
                var d = defs[i];
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(24));
                d.Label = EditorGUILayout.TextField(d.Label, GUILayout.ExpandWidth(true));

                var col = new UnityEngine.Color(d.ColorR / 255f, d.ColorG / 255f, d.ColorB / 255f);
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(16, 16, GUILayout.Width(16)), col);

                if (GUILayout.Button("✕", GUILayout.Width(22))) removeAt = i;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Count", GUILayout.Width(46));
                d.Count = Mathf.Max(0, EditorGUILayout.IntField(d.Count));
                EditorGUILayout.LabelField("RGB", GUILayout.Width(30));
                d.ColorR = (byte)Mathf.Clamp(EditorGUILayout.IntField(d.ColorR, GUILayout.Width(38)), 0, 255);
                d.ColorG = (byte)Mathf.Clamp(EditorGUILayout.IntField(d.ColorG, GUILayout.Width(38)), 0, 255);
                d.ColorB = (byte)Mathf.Clamp(EditorGUILayout.IntField(d.ColorB, GUILayout.Width(38)), 0, 255);
                EditorGUILayout.EndHorizontal();

                defs[i] = d;
                EditorGUILayout.EndVertical();
            }

            if (removeAt >= 0) defs.RemoveAt(removeAt);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 종류 추가", GUILayout.Height(22)))
                defs.Add(new SpotTypeDef($"스폿{defs.Count}", 3, 150, 150, 150));
            EditorGUILayout.EndHorizontal();
        }

        // ── 레이어 탭 ─────────────────────────────────────────────
        private void DrawLayersTab()
        {
            SectionLabel("레이어 표시");
            bool changed = false;
            changed |= LayerToggle("국가 색상",  ref _renderOpts.ShowNations);
            changed |= LayerToggle("국경선",     ref _renderOpts.ShowBorders);
            changed |= LayerToggle("강",         ref _renderOpts.ShowRivers);
            changed |= LayerToggle("교역로",     ref _renderOpts.ShowRoads);
            changed |= LayerToggle("도시",       ref _renderOpts.ShowCities);
            changed |= LayerToggle("특수 스폿",  ref _renderOpts.ShowSpots);
            changed |= LayerToggle("격자",       ref _renderOpts.ShowGrid);

            if (changed && _world != null)
            {
                _overlayTex = WorldMapRenderer.RenderOverlay(_world, _renderOpts);
                BuildComposite();
            }
        }

        // ── 통계 탭 ───────────────────────────────────────────────
        private void DrawStatsTab()
        {
            if (_world == null)
            {
                EditorGUILayout.HelpBox("Generate 후 통계가 표시됩니다.", MessageType.Info);
                return;
            }

            SectionLabel("월드 정보");
            StatRow("맵 크기",   $"{_world.Width} x {_world.Height}");
            StatRow("전체 타일", $"{_world.Width * _world.Height:N0}");
            StatRow("Seed",     $"{_world.Settings.Seed}");

            SectionLabel("지형");
            int total = _world.Width * _world.Height;
            int land  = 0;
            foreach (var b in _world.Biomes)
                if (BiomeClassifier.IsLand(b)) land++;
            int sea = total - land;
            StatRow("육지", $"{land:N0}  ({land * 100f / total:F1}%)");
            StatRow("바다", $"{sea:N0}  ({sea * 100f / total:F1}%)");

            SectionLabel("지물");
            StatRow("국가",      $"{_world.Nations.Count}");

            // 도시 등급별 집계 (가변)
            var tierCounts = new int[_world.Settings.CityTierDefs.Count];
            foreach (var c in _world.Cities)
                if (c.Tier >= 0 && c.Tier < tierCounts.Length) tierCounts[c.Tier]++;
            for (int i = 0; i < _world.Settings.CityTierDefs.Count; i++)
                StatRow(_world.Settings.CityTierDefs[i].Label, $"{tierCounts[i]}");
            StatRow("도시 합계", $"{_world.Cities.Count}");
            StatRow("강",        $"{_world.Rivers.Count}");

            // 스폿 종류별 집계 (가변)
            var spotCounts = new int[_world.Settings.SpotTypeDefs.Count];
            foreach (var sp in _world.Spots)
                if (sp.SpotTypeId >= 0 && sp.SpotTypeId < spotCounts.Length) spotCounts[sp.SpotTypeId]++;
            for (int i = 0; i < _world.Settings.SpotTypeDefs.Count; i++)
                StatRow(_world.Settings.SpotTypeDefs[i].Label, $"{spotCounts[i]}");
            StatRow("스폿 합계", $"{_world.Spots.Count}");
            StatRow("교역로",    $"{_world.Roads.Count}");

            SectionLabel("국가 목록");
            foreach (var n in _world.Nations)
            {
                EditorGUILayout.BeginHorizontal();
                var col = new Color(n.R / 255f, n.G / 255f, n.B / 255f);
                var rect = GUILayoutUtility.GetRect(14, 14, GUILayout.Width(14));
                EditorGUI.DrawRect(rect, col);
                GUILayout.Label(n.Name);
                EditorGUILayout.EndHorizontal();
            }

            SectionLabel("스폿 목록");
            // foreach (var s in _world.Spots)
            //     GUILayout.Label($"{WorldMapRenderer.SpotEmoji(s.Type)} {s.Name}  ({s.X},{s.Y})");
        }

        // ── 맵 미리보기 ───────────────────────────────────────────
        private void DrawMapPreview()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (_isGenerating)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
                GUILayout.Label("⏳ " + _progLabel, EditorStyles.largeLabel);
                long tiles = (long)_settings.MapWidth * _settings.MapHeight;
                GUILayout.Label($"{_settings.MapWidth} × {_settings.MapHeight}  ({tiles:N0} 타일)", _styleCenter);
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                Repaint(); // 애니메이션 효과
            }
            else if (_compositeTex != null)
            {
                var area = GUILayoutUtility.GetRect(0, 0,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                float tw = _compositeTex.width  * _zoom;
                float th = _compositeTex.height * _zoom;
                var drawRect = new Rect(
                    area.x + Mathf.Max(0f, (area.width  - tw) * 0.5f),
                    area.y + Mathf.Max(0f, (area.height - th) * 0.5f),
                    tw, th);
                GUI.DrawTexture(drawRect, _compositeTex, ScaleMode.ScaleToFit, false);
            }
            else
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // centeredGreyMiniLabel 대신 직접 만든 스타일
                GUILayout.Label("Generate 버튼을 눌러 월드를 생성하세요.", _styleCenter);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndVertical();
        }

        // ── 상태바 ────────────────────────────────────────────────
        private void DrawStatusBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (_world != null)
                GUILayout.Label(
                    $"Seed: {_world.Settings.Seed}  |  " +
                    $"{_world.Width}x{_world.Height}  |  " +
                    $"{(long)_world.Width * _world.Height:N0} tiles  |  " +
                    $"Cities: {_world.Cities.Count}  |  " +
                    $"Spots: {_world.Spots.Count}  |  " +
                    $"Nations: {_world.Nations.Count}  |  " +
                    _progLabel,
                    _styleMiniLabel);
            else
                GUILayout.Label("World Forge — 아직 생성된 맵이 없습니다.", _styleMiniLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // ════════════════════════════════════════════════════════
        // GENERATE / COMPOSITE / SAVE
        // ════════════════════════════════════════════════════════
        private void DoGenerate()
        {
            if (_isGenerating) return;
            _isGenerating  = true;
            _progLabel     = "데이터 생성 중...";
            _progStart     = EditorApplication.timeSinceStartup;
            Repaint();

            // 순수 C# 생성은 백그라운드 스레드에서 실행 (에디터 멈춤 방지)
            var settingsCopy = _settings; // 캡처용 복사
            System.Threading.Tasks.Task.Run(() =>
            {
                var world = WorldGenerator.Generate(settingsCopy);
                // Texture2D 생성은 반드시 메인스레드에서 해야 하므로 delayCall 사용
                EditorApplication.delayCall += () => OnGenerateDone(world);
            });
        }

        private string _progLabel = "";
        private double _progStart;

        private void OnGenerateDone(WorldData world)
        {
            _world      = world;
            _progLabel  = "텍스처 렌더링 중...";
            Repaint();

            // Texture2D 렌더는 메인스레드 (Unity API 제약)
            _baseTex    = WorldMapRenderer.RenderToTexture(_world, _renderOpts);
            _overlayTex = WorldMapRenderer.RenderOverlay(_world, _renderOpts);
            BuildComposite();

            double elapsed = EditorApplication.timeSinceStartup - _progStart;
            _progLabel    = $"완료 ({elapsed:F2}초)  —  {_world.Width}×{_world.Height}  {(long)_world.Width * _world.Height:N0} 타일";
            _isGenerating = false;
            Repaint();
        }

        private void BuildComposite()
        {
            if (_baseTex == null) return;
            int W = _baseTex.width, H = _baseTex.height;

            if (_compositeTex == null
                || _compositeTex.width  != W
                || _compositeTex.height != H)
            {
                _compositeTex = new Texture2D(W, H, TextureFormat.RGBA32, false)
                    { filterMode = FilterMode.Point };
            }

            Color[] basePixels    = _baseTex.GetPixels();
            Color[] overlayPixels = _overlayTex != null ? _overlayTex.GetPixels() : null;

            for (int i = 0; i < basePixels.Length; i++)
            {
                Color c = basePixels[i];
                if (overlayPixels != null)
                {
                    Color oc = overlayPixels[i];
                    float a  = oc.a;
                    if (a > 0.01f)
                        c = new Color(
                            c.r * (1 - a) + oc.r * a,
                            c.g * (1 - a) + oc.g * a,
                            c.b * (1 - a) + oc.b * a,
                            1f);
                }
                basePixels[i] = c;
            }
            _compositeTex.SetPixels(basePixels);
            _compositeTex.Apply();
            Repaint();
        }

        private void SavePng()
        {
            if (_compositeTex == null) return;
            string path = EditorUtility.SaveFilePanel(
                "맵 PNG 저장", "", $"world_seed{_settings.Seed}.png", "png");
            if (string.IsNullOrEmpty(path)) return;
            System.IO.File.WriteAllBytes(path, _compositeTex.EncodeToPNG());
            Debug.Log($"[WorldForge] 저장 완료: {path}");
        }

        // ── 월드 데이터 저장/불러오기 (바이너리 .wfd) ──────────────
        private void SaveWorldData()
        {
            if (_world == null) return;

            string path = EditorUtility.SaveFilePanel(
                "월드 데이터 저장", "", $"world_seed{_settings.Seed}.wfd", "wfd");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                WorldDataSerializer.SaveToFile(_world, path);
                long sizeKb = new System.IO.FileInfo(path).Length / 1024;
                Debug.Log($"[WorldForge] 월드 데이터 저장 완료: {path} ({sizeKb:N0} KB)");
                _progLabel = $"저장됨 — {System.IO.Path.GetFileName(path)} ({sizeKb:N0} KB)";
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldForge] 저장 실패: {ex.Message}");
                EditorUtility.DisplayDialog("저장 실패", ex.Message, "확인");
            }
        }

        private void LoadWorldData()
        {
            string path = EditorUtility.OpenFilePanel("월드 데이터 불러오기", "", "wfd");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                _isGenerating = true;
                Repaint();

                var loaded = WorldDataSerializer.LoadFromFile(path);

                _world    = loaded;
                _settings = loaded.Settings;
                _baseTex    = WorldMapRenderer.RenderToTexture(_world, _renderOpts);
                _overlayTex = WorldMapRenderer.RenderOverlay(_world, _renderOpts);
                BuildComposite();

                _progLabel = $"불러옴 — {System.IO.Path.GetFileName(path)}";
                Debug.Log($"[WorldForge] 불러오기 완료: {path} " +
                          $"({_world.Width}x{_world.Height}, 도시 {_world.Cities.Count}, 스폿 {_world.Spots.Count})");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WorldForge] 불러오기 실패: {ex.Message}");
                EditorUtility.DisplayDialog("불러오기 실패", ex.Message, "확인");
            }
            finally
            {
                _isGenerating = false;
                Repaint();
            }
        }

        private void LoadPreset(WorldGenSettings p)
        {
            p.Seed    = _settings.Seed;
            _settings = p;
            Repaint();
        }

        // ── UI 헬퍼 ───────────────────────────────────────────────
        private void SectionLabel(string label)
        {
            GUILayout.Space(4);
            EditorGUILayout.LabelField(label, _styleSection);
        }

        private static void StatRow(string key, string val)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(key, GUILayout.Width(90));
            GUILayout.Label(val, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        private static bool LayerToggle(string label, ref bool val)
        {
            bool nv = EditorGUILayout.Toggle(label, val);
            if (nv == val) return false;
            val = nv;
            return true;
        }

        /// <summary>
        /// 슬라이더(0~sliderMax) + IntField(min~int.MaxValue) 조합.
        /// 슬라이더 범위를 벗어난 값도 IntField로 자유롭게 입력 가능.
        /// </summary>
        /// <summary>
        /// 슬라이더(sliderMin~sliderMax) + IntField(hardMin~hardMax) 독립 동작.
        /// 슬라이더와 IntField 중 마지막으로 변경된 쪽을 채택.
        /// </summary>
        private int SizeField(string label, int value, int hardMin, int hardMax, int sliderMin, int sliderMax)
        {
            // 라벨 행
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(50));

            // IntField — 직접 타이핑 가능, 하드 범위만 적용
            int fromField = EditorGUILayout.IntField(value, GUILayout.Width(58));
            fromField = Mathf.Clamp(fromField, hardMin, hardMax);
            EditorGUILayout.EndHorizontal();

            // 슬라이더 — 별도 행에 전체 너비로
            int fromSlider = (int)EditorGUILayout.Slider(
                Mathf.Clamp(value, sliderMin, sliderMax),
                sliderMin, sliderMax);

            // 둘 중 변경된 쪽 채택 (같으면 field 우선)
            return fromField != value ? fromField : fromSlider;
        }

        private void FeatureCountField(string label, ref int value, int min, int sliderMax, int hardMin)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(60));

            // 슬라이더: 0 ~ sliderMax (빠른 조작용)
            int sliderVal = (int)GUILayout.HorizontalSlider(
                Mathf.Clamp(value, min, sliderMax),
                min, sliderMax,
                GUILayout.ExpandWidth(true));

            // IntField: 직접 타이핑, 상한 없음
            int fieldVal = EditorGUILayout.IntField(value, GUILayout.Width(58));

            // 둘 중 변경된 쪽 채택, hardMin 이하로는 내려가지 않게
            int newVal = (sliderVal != Mathf.Clamp(value, min, sliderMax)) ? sliderVal : fieldVal;
            value = Mathf.Max(hardMin, newVal);

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
