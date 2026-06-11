using UnityEngine;
using UnityEngine.UI;

namespace WorldForge
{
    /// <summary>
    /// 런타임 설정 팝업 UI.
    /// Canvas > Panel 오브젝트에 붙이고 각 Slider/Toggle/Text 를 연결하세요.
    ///
    /// Hierarchy 예시:
    ///   Canvas
    ///   └─ WorldForgePanel (이 컴포넌트)
    ///      ├─ SeedInput      (InputField)
    ///      ├─ BtnRandomSeed  (Button)
    ///      ├─ SlNoiseScale   (Slider 0.5~8)
    ///      ├─ SlOctaves      (Slider 2~9)
    ///      ├─ SlPersistence  (Slider 0.2~0.8)
    ///      ├─ SlSeaLevel     (Slider 0.2~0.7)
    ///      ├─ SlContinentBias(Slider 0~0.8)
    ///      ├─ SlEdgeFalloff  (Slider 0~1)
    ///      ├─ SlNumNations   (Slider 2~14)
    ///      ├─ SlNumCities    (Slider 4~50)
    ///      ├─ SlNumRivers    (Slider 0~30)
    ///      ├─ SlNumSpots     (Slider 0~40)
    ///      ├─ BtnGenerate    (Button)
    ///      ├─ BtnClose       (Button)
    ///      ├─ TglNations … TglGrid (Toggle × 7)
    ///      └─ StatsPanel
    ///         ├─ TxtStatLand / TxtStatSea / TxtStatNations
    ///         └─ TxtStatCities / TxtStatSpots / TxtStatRivers
    /// </summary>
    public class WorldForgePanel : MonoBehaviour
    {
        [Header("Manager")]
        public WorldForgeManager Manager;

        // ── Seed ─────────────────────────────────────────────────
        [Header("Seed")]
        public InputField SeedInput;
        public Button     BtnRandomSeed;

        // ── 지형 슬라이더 ─────────────────────────────────────────
        [Header("Terrain Sliders")]
        public Slider SlNoiseScale;
        public Slider SlOctaves;
        public Slider SlPersistence;
        public Slider SlSeaLevel;
        public Slider SlContinentBias;
        public Slider SlEdgeFalloff;

        [Header("Feature Sliders")]
        public Slider SlNumNations;       // 0 ~ 200
        public Slider SlNumMajorCities;   // 0 ~ 500
        public Slider SlNumMinorCities;   // 0 ~ 1000
        public Slider SlNumVillages;      // 0 ~ 2000
        public Slider SlNumRivers;        // 0 ~ 500
        public Slider SlNumSpots;         // 0 ~ 500

        // ── 레이블 (슬라이더 옆에 현재값 표시) ───────────────────
        [Header("Slider Value Labels")]
        public Text LblNoiseScale;
        public Text LblOctaves;
        public Text LblPersistence;
        public Text LblSeaLevel;
        public Text LblContinentBias;
        public Text LblEdgeFalloff;
        public Text LblNumNations;
        public Text LblNumMajorCities;
        public Text LblNumMinorCities;
        public Text LblNumVillages;
        public Text LblNumRivers;
        public Text LblNumSpots;

        // ── 버튼 ─────────────────────────────────────────────────
        [Header("Buttons")]
        public Button BtnGenerate;
        public Button BtnClose;
        public Button BtnPresetArchipelago;
        public Button BtnPresetPangaea;
        public Button BtnPresetMountain;

        // ── 레이어 토글 ───────────────────────────────────────────
        [Header("Layer Toggles")]
        public Toggle TglNations;
        public Toggle TglBorders;
        public Toggle TglRivers;
        public Toggle TglRoads;
        public Toggle TglCities;
        public Toggle TglSpots;
        public Toggle TglGrid;

        // ── 통계 텍스트 ───────────────────────────────────────────
        [Header("Stats")]
        public Text TxtStatLand;
        public Text TxtStatSea;
        public Text TxtStatNations;
        public Text TxtStatCities;
        public Text TxtStatSpots;
        public Text TxtStatRivers;

        // ════════════════════════════════════════════════════════
        private void Start()
        {
            InitSliders();
            BindEvents();
            if (Manager) Manager.OnWorldGenerated += UpdateStats;
        }

        private void OnDestroy()
        {
            if (Manager) Manager.OnWorldGenerated -= UpdateStats;
        }

        // ── 슬라이더 초기화 ───────────────────────────────────────
        private void InitSliders()
        {
            var s = Manager ? Manager.Settings : new WorldGenSettings();

            SetSlider(SlNoiseScale,    s.NoiseScale,    0.5f, 8f,   LblNoiseScale,   "F1");
            SetSlider(SlOctaves,       s.Octaves,       2,    9,    LblOctaves,      "F0");
            SetSlider(SlPersistence,   s.Persistence,   0.2f, 0.8f, LblPersistence,  "F2");
            SetSlider(SlSeaLevel,      s.SeaLevel,      0.2f, 0.7f, LblSeaLevel,     "P0");
            SetSlider(SlContinentBias, s.ContinentBias, 0f,   0.8f, LblContinentBias,"F2");
            SetSlider(SlEdgeFalloff,   s.EdgeFalloff,   0f,   1f,   LblEdgeFalloff,  "F2");
            SetSlider(SlNumNations,      s.NumNations,      0, 200,   LblNumNations,      "F0");
            SetSlider(SlNumMajorCities,  s.NumMajorCities,  0, 500,   LblNumMajorCities,  "F0");
            SetSlider(SlNumMinorCities,  s.NumMinorCities,  0, 1000,  LblNumMinorCities,  "F0");
            SetSlider(SlNumVillages,     s.NumVillages,     0, 2000,  LblNumVillages,     "F0");
            SetSlider(SlNumRivers,       s.NumRivers,       0, 500,   LblNumRivers,       "F0");
            SetSlider(SlNumSpots,        s.NumSpots,        0, 500,   LblNumSpots,        "F0");

            if (SeedInput) SeedInput.text = s.Seed.ToString();

            // 토글 초기값
            var o = Manager ? Manager.RenderOpts : new RenderOptions();
            SetTgl(TglNations, o.ShowNations);
            SetTgl(TglBorders, o.ShowBorders);
            SetTgl(TglRivers,  o.ShowRivers);
            SetTgl(TglRoads,   o.ShowRoads);
            SetTgl(TglCities,  o.ShowCities);
            SetTgl(TglSpots,   o.ShowSpots);
            SetTgl(TglGrid,    o.ShowGrid);
        }

        // ── 이벤트 바인딩 ─────────────────────────────────────────
        private void BindEvents()
        {
            // Sliders
            BindSlider(SlNoiseScale,    LblNoiseScale,    "F1", v => Apply(s => s.NoiseScale    = v));
            BindSlider(SlOctaves,       LblOctaves,       "F0", v => Apply(s => s.Octaves       = (int)v));
            BindSlider(SlPersistence,   LblPersistence,   "F2", v => Apply(s => s.Persistence   = v));
            BindSlider(SlSeaLevel,      LblSeaLevel,      "P0", v => Apply(s => s.SeaLevel      = v));
            BindSlider(SlContinentBias, LblContinentBias, "F2", v => Apply(s => s.ContinentBias = v));
            BindSlider(SlEdgeFalloff,   LblEdgeFalloff,   "F2", v => Apply(s => s.EdgeFalloff   = v));
            BindSlider(SlNumNations,     LblNumNations,     "F0", v => Apply(s => s.NumNations     = (int)v));
            BindSlider(SlNumMajorCities, LblNumMajorCities, "F0", v => Apply(s => s.NumMajorCities = (int)v));
            BindSlider(SlNumMinorCities, LblNumMinorCities, "F0", v => Apply(s => s.NumMinorCities = (int)v));
            BindSlider(SlNumVillages,    LblNumVillages,    "F0", v => Apply(s => s.NumVillages    = (int)v));
            BindSlider(SlNumRivers,      LblNumRivers,      "F0", v => Apply(s => s.NumRivers      = (int)v));
            BindSlider(SlNumSpots,       LblNumSpots,       "F0", v => Apply(s => s.NumSpots       = (int)v));

            // Seed
            if (SeedInput) SeedInput.onEndEdit.AddListener(v => { if (int.TryParse(v, out int sv)) Apply(s => s.Seed = sv); });
            if (BtnRandomSeed) BtnRandomSeed.onClick.AddListener(() =>
            {
                int r = UnityEngine.Random.Range(1, 999999);
                Apply(s => s.Seed = r);
                if (SeedInput) SeedInput.text = r.ToString();
            });

            // Buttons
            if (BtnGenerate) BtnGenerate.onClick.AddListener(() => Manager?.Generate());
            if (BtnClose)    BtnClose.onClick.AddListener(()    => gameObject.SetActive(false));

            // Presets
            if (BtnPresetArchipelago) BtnPresetArchipelago.onClick.AddListener(() => LoadPreset(WorldGenSettings.Archipelago()));
            if (BtnPresetPangaea)     BtnPresetPangaea.onClick.AddListener(()     => LoadPreset(WorldGenSettings.Pangaea()));
            if (BtnPresetMountain)    BtnPresetMountain.onClick.AddListener(()    => LoadPreset(WorldGenSettings.Mountainous()));

            // Toggles
            BindTgl(TglNations, v => { if(Manager) { Manager.RenderOpts.ShowNations = v; Manager.Redraw(); }});
            BindTgl(TglBorders, v => { if(Manager) { Manager.RenderOpts.ShowBorders = v; Manager.Redraw(); }});
            BindTgl(TglRivers,  v => { if(Manager) { Manager.RenderOpts.ShowRivers  = v; Manager.Redraw(); }});
            BindTgl(TglRoads,   v => { if(Manager) { Manager.RenderOpts.ShowRoads   = v; Manager.Redraw(); }});
            BindTgl(TglCities,  v => { if(Manager) { Manager.RenderOpts.ShowCities  = v; Manager.Redraw(); }});
            BindTgl(TglSpots,   v => { if(Manager) { Manager.RenderOpts.ShowSpots   = v; Manager.Redraw(); }});
            BindTgl(TglGrid,    v => { if(Manager) { Manager.RenderOpts.ShowGrid    = v; Manager.Redraw(); }});
        }

        // ── 통계 업데이트 ─────────────────────────────────────────
        private void UpdateStats(WorldData w)
        {
            if (w == null) return;
            int total = w.Width * w.Height;
            int land  = 0;
            foreach (var b in w.Biomes) if (BiomeClassifier.IsLand(b)) land++;

            SetTxt(TxtStatLand,    $"{land:N0}");
            SetTxt(TxtStatSea,     $"{(total - land):N0}");
            SetTxt(TxtStatNations, $"{w.Nations.Count}");

            int caps=0, maj=0, min=0, vil=0;
            foreach (var c in w.Cities)
                switch (c.Tier)
                {
                    case CityTier.Capital: caps++; break;
                    case CityTier.Major:   maj++;  break;
                    case CityTier.Minor:   min++;  break;
                    default:               vil++;  break;
                }
            SetTxt(TxtStatCities, $"수도{caps} 대{maj} 중{min} 소{vil}");
            SetTxt(TxtStatSpots,  $"{w.Spots.Count}");
            SetTxt(TxtStatRivers, $"{w.Rivers.Count}");
        }

        // ── 프리셋 로드 ───────────────────────────────────────────
        private void LoadPreset(WorldGenSettings preset)
        {
            if (!Manager) return;
            preset.Seed = Manager.Settings.Seed; // seed 유지
            Manager.Settings = preset;
            InitSliders();
        }

        // ── 헬퍼 ─────────────────────────────────────────────────
        private void Apply(System.Action<WorldGenSettings> fn) { if (Manager) fn(Manager.Settings); }

        private static void SetSlider(Slider sl, float val, float min, float max, Text lbl, string fmt)
        {
            if (!sl) return;
            sl.minValue = min; sl.maxValue = max; sl.value = val;
            if (lbl) lbl.text = val.ToString(fmt);
        }

        private static void BindSlider(Slider sl, Text lbl, string fmt, System.Action<float> onChanged)
        {
            if (!sl) return;
            sl.onValueChanged.AddListener(v => { if (lbl) lbl.text = v.ToString(fmt); onChanged(v); });
        }

        private static void SetTgl(Toggle tgl, bool val) { if (tgl) tgl.isOn = val; }
        private static void BindTgl(Toggle tgl, System.Action<bool> fn) { if (tgl) tgl.onValueChanged.AddListener(fn.Invoke); }
        private static void SetTxt(Text t, string v) { if (t) t.text = v; }
    }
}
