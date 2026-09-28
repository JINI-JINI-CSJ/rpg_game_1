using UnityEngine;
using UnityEngine.UI;

namespace WorldForge
{
    /// <summary>
    /// 런타임 설정 팝업 UI.
    /// 도시 등급 / 스폿 종류는 WorldGenSettings.CityTierDefs / SpotTypeDefs 를
    /// 동적으로 읽으므로 가변 설정에 자동 대응합니다.
    ///
    /// Inspector 연결 필수:
    ///   Manager           → WorldForgeManager
    ///   SeedInput         → InputField
    ///   BtnRandomSeed     → Button
    ///   BtnGenerate       → Button
    ///   BtnClose          → Button
    ///   (선택) StatusText → Text (상태 메시지)
    /// </summary>
    public class WorldForgePanel : MonoBehaviour
    {
        [Header("Manager")]
        public WorldForgeManager Manager;

        [Header("Seed")]
        public InputField SeedInput;
        public Button     BtnRandomSeed;

        [Header("Terrain Sliders")]
        public Slider SlNoiseScale;
        public Slider SlOctaves;
        public Slider SlPersistence;
        public Slider SlSeaLevel;
        public Slider SlContinentBias;
        public Slider SlEdgeFalloff;

        [Header("Feature Sliders")]
        public Slider SlNumNations;
        public Slider SlNumRivers;

        [Header("Slider Value Labels")]
        public Text LblNoiseScale;
        public Text LblOctaves;
        public Text LblPersistence;
        public Text LblSeaLevel;
        public Text LblContinentBias;
        public Text LblEdgeFalloff;
        public Text LblNumNations;
        public Text LblNumRivers;

        [Header("Buttons")]
        public Button BtnGenerate;
        public Button BtnClose;
        public Button BtnPresetArchipelago;
        public Button BtnPresetPangaea;
        public Button BtnPresetMountain;

        [Header("Save / Load")]
        public InputField SaveLoadFileName;
        public Button     BtnQuickSave;
        public Button     BtnQuickLoad;

        [Header("Layer Toggles")]
        public Toggle TglNations;
        public Toggle TglBorders;
        public Toggle TglRivers;
        public Toggle TglRoads;
        public Toggle TglCities;
        public Toggle TglSpots;
        public Toggle TglGrid;

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

            SetSlider(SlNoiseScale,    s.NoiseScale,    0.5f, 8f,   LblNoiseScale,    "F1");
            SetSlider(SlOctaves,       s.Octaves,       2,    9,    LblOctaves,       "F0");
            SetSlider(SlPersistence,   s.Persistence,   0.2f, 0.8f, LblPersistence,   "F2");
            SetSlider(SlSeaLevel,      s.SeaLevel,      0.2f, 0.7f, LblSeaLevel,      "P0");
            SetSlider(SlContinentBias, s.ContinentBias, 0f,   0.8f, LblContinentBias, "F2");
            SetSlider(SlEdgeFalloff,   s.EdgeFalloff,   0f,   1f,   LblEdgeFalloff,   "F2");
            SetSlider(SlNumNations,    s.NumNations,    2,    100,  LblNumNations,    "F0");
            SetSlider(SlNumRivers,     s.NumRivers,     0,    200,  LblNumRivers,     "F0");

            if (SeedInput) SeedInput.text = s.Seed.ToString();

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
            BindSlider(SlNoiseScale,    LblNoiseScale,    "F1", v => Apply(s => s.NoiseScale    = v));
            BindSlider(SlOctaves,       LblOctaves,       "F0", v => Apply(s => s.Octaves       = (int)v));
            BindSlider(SlPersistence,   LblPersistence,   "F2", v => Apply(s => s.Persistence   = v));
            BindSlider(SlSeaLevel,      LblSeaLevel,      "P0", v => Apply(s => s.SeaLevel      = v));
            BindSlider(SlContinentBias, LblContinentBias, "F2", v => Apply(s => s.ContinentBias = v));
            BindSlider(SlEdgeFalloff,   LblEdgeFalloff,   "F2", v => Apply(s => s.EdgeFalloff   = v));
            BindSlider(SlNumNations,    LblNumNations,    "F0", v => Apply(s => s.NumNations    = (int)v));
            BindSlider(SlNumRivers,     LblNumRivers,     "F0", v => Apply(s => s.NumRivers     = (int)v));

            // 도시 등급 / 스폿 수는 CityTierDefs / SpotTypeDefs 를 직접 편집해야 하므로
            // 런타임 패널에서는 슬라이더 대신 코드로 직접 설정을 변경하세요.
            // (게임 내 UI가 필요하면 TierDef 목록을 동적으로 생성하는 별도 패널을 추가하세요)

            if (SeedInput) SeedInput.onEndEdit.AddListener(v =>
            {
                if (int.TryParse(v, out int sv)) Apply(s => s.Seed = sv);
            });
            if (BtnRandomSeed) BtnRandomSeed.onClick.AddListener(() =>
            {
                int r = UnityEngine.Random.Range(1, 999999);
                Apply(s => s.Seed = r);
                if (SeedInput) SeedInput.text = r.ToString();
            });

            if (BtnGenerate) BtnGenerate.onClick.AddListener(() => Manager?.Generate());
            if (BtnClose)    BtnClose.onClick.AddListener(()    => gameObject.SetActive(false));

            if (BtnPresetArchipelago) BtnPresetArchipelago.onClick.AddListener(() => LoadPreset(WorldGenSettings.Archipelago()));
            if (BtnPresetPangaea)     BtnPresetPangaea.onClick.AddListener(()     => LoadPreset(WorldGenSettings.Pangaea()));
            if (BtnPresetMountain)    BtnPresetMountain.onClick.AddListener(()    => LoadPreset(WorldGenSettings.Mountainous()));

            if (BtnQuickSave) BtnQuickSave.onClick.AddListener(() =>
            {
                string name = (SaveLoadFileName && !string.IsNullOrWhiteSpace(SaveLoadFileName.text))
                    ? SaveLoadFileName.text : "world";
                Manager?.QuickSave(name);
            });
            if (BtnQuickLoad) BtnQuickLoad.onClick.AddListener(() =>
            {
                string name = (SaveLoadFileName && !string.IsNullOrWhiteSpace(SaveLoadFileName.text))
                    ? SaveLoadFileName.text : "world";
                Manager?.QuickLoad(name);
                InitSliders();
            });

            BindTgl(TglNations, v => { if(Manager) { Manager.RenderOpts.ShowNations = v; Manager.Redraw(); }});
            BindTgl(TglBorders, v => { if(Manager) { Manager.RenderOpts.ShowBorders = v; Manager.Redraw(); }});
            BindTgl(TglRivers,  v => { if(Manager) { Manager.RenderOpts.ShowRivers  = v; Manager.Redraw(); }});
            BindTgl(TglRoads,   v => { if(Manager) { Manager.RenderOpts.ShowRoads   = v; Manager.Redraw(); }});
            BindTgl(TglCities,  v => { if(Manager) { Manager.RenderOpts.ShowCities  = v; Manager.Redraw(); }});
            BindTgl(TglSpots,   v => { if(Manager) { Manager.RenderOpts.ShowSpots   = v; Manager.Redraw(); }});
            BindTgl(TglGrid,    v => { if(Manager) { Manager.RenderOpts.ShowGrid    = v; Manager.Redraw(); }});
        }

        // ── 통계 업데이트 (가변 등급/종류 대응) ─────────────────────
        private void UpdateStats(WorldData w)
        {
            if (w == null) return;
            int total = w.Width * w.Height, land = 0;
            foreach (var b in w.Biomes) if (BiomeClassifier.IsLand(b)) land++;

            SetTxt(TxtStatLand,    $"{land:N0}");
            SetTxt(TxtStatSea,     $"{(total - land):N0}");
            SetTxt(TxtStatNations, $"{w.Nations.Count}");

            // 도시: 등급별 집계 → "수도2 대12 중20 소30" 형태
            var tierCounts = new int[w.Settings.CityTierDefs.Count];
            foreach (var c in w.Cities)
                if (c.Tier >= 0 && c.Tier < tierCounts.Length) tierCounts[c.Tier]++;
            var citySb = new System.Text.StringBuilder();
            for (int i = 0; i < w.Settings.CityTierDefs.Count; i++)
                citySb.Append($"{w.Settings.CityTierDefs[i].Label}{tierCounts[i]} ");
            SetTxt(TxtStatCities, citySb.ToString().TrimEnd());

            // 스폿: 종류별 집계
            var spotCounts = new int[w.Settings.SpotTypeDefs.Count];
            foreach (var sp in w.Spots)
                if (sp.SpotTypeId >= 0 && sp.SpotTypeId < spotCounts.Length) spotCounts[sp.SpotTypeId]++;
            var spotSb = new System.Text.StringBuilder();
            for (int i = 0; i < w.Settings.SpotTypeDefs.Count; i++)
                spotSb.Append($"{w.Settings.SpotTypeDefs[i].Label}{spotCounts[i]} ");
            SetTxt(TxtStatSpots, spotSb.ToString().TrimEnd());

            SetTxt(TxtStatRivers, $"{w.Rivers.Count}");
        }

        // ── 프리셋 로드 ───────────────────────────────────────────
        private void LoadPreset(WorldGenSettings preset)
        {
            if (!Manager) return;
            preset.Seed    = Manager.Settings.Seed;
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
