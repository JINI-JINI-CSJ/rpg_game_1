using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;

namespace WorldForge
{
    /// <summary>
    /// Scene에 배치하는 메인 매니저.
    /// WorldForgeWindow(에디터) 또는 WorldForgePanel(런타임 UI)에서 호출.
    /// </summary>
    public class WorldForgeManager : MonoBehaviour
    {
        [Header("References")]
        public RawImage   MapDisplay;
        public RawImage   OverlayDisplay;
        public GameObject LoadingPanel;
        public Text       StatusText;

        [Header("Settings")]
        public WorldGenSettings Settings = new WorldGenSettings();

        [Header("Render Options")]
        public RenderOptions RenderOpts = new RenderOptions();

        public WorldData  CurrentWorld { get; private set; }
        public bool       IsGenerating { get; private set; }

        public System.Action<WorldData> OnWorldGenerated;

        // ════════════════════════════════════════════════════════
        public void Generate()
        {
            if (IsGenerating) return;
            StartCoroutine(GenerateCoroutine());
        }

        private IEnumerator GenerateCoroutine()
        {
            IsGenerating = true;
            SetStatus("데이터 생성 중...");
            if (LoadingPanel) LoadingPanel.SetActive(true);
            yield return null;

            // ── 순수 C# 생성: 타일 수가 많으면 백그라운드 Task ──
            long tileCount = (long)Settings.MapWidth * Settings.MapHeight;
            WorldData world = null;

            if (tileCount > 200_000)
            {
                // 백그라운드 스레드 (에디터/런타임 모두 동작)
                var settingsCopy = Settings;
                var task = Task.Run(() => WorldGenerator.Generate(settingsCopy));
                while (!task.IsCompleted) yield return null;
                if (task.IsFaulted)
                {
                    Debug.LogError($"[WorldForge] 생성 오류: {task.Exception}");
                    IsGenerating = false;
                    if (LoadingPanel) LoadingPanel.SetActive(false);
                    yield break;
                }
                world = task.Result;
            }
            else
            {
                world = WorldGenerator.Generate(Settings);
            }

            CurrentWorld = world;
            SetStatus("텍스처 렌더링 중...");
            yield return null;

            var baseTex    = WorldMapRenderer.RenderToTexture(CurrentWorld, RenderOpts);
            var overlayTex = WorldMapRenderer.RenderOverlay(CurrentWorld, RenderOpts);

            if (MapDisplay)     MapDisplay.texture     = baseTex;
            if (OverlayDisplay) OverlayDisplay.texture = overlayTex;

            if (LoadingPanel) LoadingPanel.SetActive(false);
            SetStatus($"완료 — {CurrentWorld.Width}×{CurrentWorld.Height} " +
                      $"({(long)CurrentWorld.Width * CurrentWorld.Height:N0} 타일) | " +
                      $"도시 {CurrentWorld.Cities.Count} / 스폿 {CurrentWorld.Spots.Count}");

            IsGenerating = false;
            OnWorldGenerated?.Invoke(CurrentWorld);
        }

        public void ToggleNations(bool v)  { RenderOpts.ShowNations = v;  Redraw(); }
        public void ToggleBorders(bool v)  { RenderOpts.ShowBorders = v;  Redraw(); }
        public void ToggleRivers(bool v)   { RenderOpts.ShowRivers  = v;  Redraw(); }
        public void ToggleRoads(bool v)    { RenderOpts.ShowRoads   = v;  Redraw(); }
        public void ToggleCities(bool v)   { RenderOpts.ShowCities  = v;  Redraw(); }
        public void ToggleSpots(bool v)    { RenderOpts.ShowSpots   = v;  Redraw(); }
        public void ToggleGrid(bool v)     { RenderOpts.ShowGrid    = v;  Redraw(); }

        public void Redraw()
        {
            if (CurrentWorld == null) return;
            if (MapDisplay)     MapDisplay.texture     = WorldMapRenderer.RenderToTexture(CurrentWorld, RenderOpts);
            if (OverlayDisplay) OverlayDisplay.texture = WorldMapRenderer.RenderOverlay(CurrentWorld, RenderOpts);
        }

        public void RandomSeed() => Settings.Seed = Random.Range(1, 999999);

        private void SetStatus(string msg)
        {
            if (StatusText) StatusText.text = msg;
            Debug.Log($"[WorldForge] {msg}");
        }
    }
}
