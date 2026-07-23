using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TilemapTool
{
    /// <summary>
    /// 에디터 윈도우 없이 씬에서 Play 버튼을 눌러 사용하는 런타임 타일맵 툴.
    /// - XZ 평면 기준 그리드
    /// - 레이어 0 = 베이스 바닥 레이어(기본 제공), 그 외 유저 오브젝트 레이어 자유 추가
    /// - 유저 오브젝트는 배치 시 4방향(0/90/180/270) 지정 가능
    /// - 팔레트: [string id, prefab, int userValue], 레이어 순서와 1:1로 매칭 (palettes[i] ↔ layer i)
    /// - 바이너리 저장/불러오기 지원
    ///
    /// 조작법 (기본값)
    ///   좌클릭       : 현재 레이어 + 현재 레이어의 팔레트 선택 항목으로 배치 (드래그 페인트 가능)
    ///   우클릭       : 현재 레이어에서 삭제 (드래그 삭제 가능)
    ///   R            : 배치 방향 90도 회전 (베이스 레이어는 방향 미적용)
    ///   Tab          : 활성 레이어 다음으로 전환
    ///   왼쪽 상단 GUI: 팔레트 선택 / 레이어 관리 / 커스텀 데이터 / 저장·불러오기
    /// </summary>
    public class RuntimeTilemapEditor : MonoBehaviour
    {
        [Header("맵 설정")]
        public TileMapSettings settings = new TileMapSettings();

        [Header("참조")]
        [Tooltip("레이어 순서와 1:1로 매칭됩니다. palettes[0]은 베이스(바닥) 레이어용, palettes[1]은 두 번째 레이어용... " +
                 "런타임에 레이어를 추가로 만들 경우, 그만큼의 팔레트를 미리 등록해 두어야 합니다.")]
        public List<TilePalette> palettes = new List<TilePalette>();
        public Camera targetCamera;
        public Transform mapRoot;

        [Header("옵션")]
        public bool dragPaint = true;
        public float baseLayerYOffset = 0f;
        public float layerYStep = 0.01f; // 레이어마다 살짝 띄워 z-fighting 방지

        [Header("저장 파일 (기본 위치: 에디터=Assets/TilemapSaves, 빌드=persistentDataPath)")]
        public string saveFileName = "tilemap_save.bin";

        [Header("그리드 표시 (Scene / Game 뷰 공통)")]
        public bool showGrid = true;
        public float gridY = 0.001f;
        public Color gridLineColor = new Color(1f, 1f, 1f, 0.35f);
        public Color gridBorderColor = new Color(0.2f, 0.85f, 1f, 0.9f);
        public Color cursorFillColor = new Color(1f, 1f, 0f, 0.22f);
        public Color cursorOutlineColor = new Color(1f, 0.9f, 0f, 1f);

        [Header("커스텀 데이터 보유 셀 표시")]
        public bool highlightCustomData = true;
        public Color defaultHighlightColor = new Color(1f, 0.25f, 0.9f, 0.55f);
        public float customDataBlinkSpeed = 2.5f; // 0이면 점멸 없이 고정 색상

        private static Material lineMaterial;

        // ---- 내부 상태 ----
        private readonly List<TileLayer> layers = new List<TileLayer>();
        private readonly Dictionary<(int layerIndex, string key), GameObject> spawnedObjects =
            new Dictionary<(int, string), GameObject>();

        private int activeLayerIndex = 0;
        private int activePaletteIndex = 0;
        private TileDirection currentDirection = TileDirection.North;

        private bool hasHover;
        private int hoverX, hoverZ;

        private Vector2 paletteScroll;
        private Vector2 layerScroll;
        private string newLayerNameInput = "NewLayer";
        private Rect panelRect = new Rect(10, 10, 280, 860);

        // 다음 배치에 적용할 커스텀 데이터 (툴 편집용, 값은 문자열로 입력받음)
        private readonly Dictionary<string, string> pendingCustomData = new Dictionary<string, string>();
        private string newCustomKeyInput = "";
        private string newCustomValueInput = "";
        private Vector2 customDataScroll;
        private Color pendingHighlightColor;
        private Texture2D swatchTex;
        private string saveDirectory;

        private void Awake()
        {
            if (targetCamera == null) targetCamera = Camera.main;
            if (mapRoot == null)
            {
                var rootGo = new GameObject("MapRoot");
                mapRoot = rootGo.transform;
                mapRoot.SetParent(transform);
            }

            pendingHighlightColor = defaultHighlightColor;

#if UNITY_EDITOR
            // Play 모드로 Editor 안에서 실행 중일 때는 Assets 폴더 하위에 기본 저장
            saveDirectory = Path.Combine(Application.dataPath, "TilemapSaves");
#else
            // 실제 빌드에서는 Assets 폴더가 존재하지 않으므로 persistentDataPath 사용
            saveDirectory = Application.persistentDataPath;
#endif
            EnsureBaseLayerExists();
        }

        /// <summary>레이어 인덱스에 1:1로 매칭되는 팔레트를 반환. 없으면 null.</summary>
        private TilePalette GetPaletteForLayer(int layerIndex)
        {
            return (layerIndex >= 0 && layerIndex < palettes.Count) ? palettes[layerIndex] : null;
        }

        private void EnsureBaseLayerExists()
        {
            if (layers.Count == 0)
            {
                layers.Add(new TileLayer
                {
                    layerName = "Base Floor",
                    isBaseLayer = true,
                    yOffset = baseLayerYOffset
                });
            }
        }

        private void Update()
        {
            HandleKeyboardShortcuts();

            if (IsPointerOverPanel())
            {
                hasHover = false;
                return;
            }

            if (TryGetCellUnderMouse(out int x, out int z))
            {
                hoverX = x;
                hoverZ = z;
                hasHover = settings.InBounds(x, z);

                if (hasHover)
                {
                    if (Input.GetMouseButtonDown(0) || (dragPaint && Input.GetMouseButton(0)))
                        PlaceAtCell(x, z);

                    if (Input.GetMouseButtonDown(1) || (dragPaint && Input.GetMouseButton(1)))
                        EraseAtCell(x, z);
                }
            }
            else
            {
                hasHover = false;
            }
        }

        private void HandleKeyboardShortcuts()
        {
            if (Input.GetKeyDown(KeyCode.R))
                currentDirection = (TileDirection)(((int)currentDirection + 1) % 4);

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                activeLayerIndex = (activeLayerIndex + 1) % layers.Count;
                activePaletteIndex = 0;
            }
        }

        private bool TryGetCellUnderMouse(out int x, out int z)
        {
            x = z = 0;
            if (targetCamera == null) return false;

            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (!plane.Raycast(ray, out float enter)) return false;

            Vector3 hit = ray.GetPoint(enter);
            x = Mathf.FloorToInt(hit.x / settings.tileSize);
            z = Mathf.FloorToInt(hit.z / settings.tileSize);
            return true;
        }

        private bool IsPointerOverPanel()
        {
            Vector2 mouse = Input.mousePosition;
            float guiY = Screen.height - mouse.y;
            return panelRect.Contains(new Vector2(mouse.x, guiY));
        }

        // ---------------- 배치 / 삭제 ----------------

        private void PlaceAtCell(int x, int z)
        {
            var layer = layers[activeLayerIndex];
            var layerPalette = GetPaletteForLayer(activeLayerIndex);
            var entry = layerPalette != null ? layerPalette.GetByIndex(activePaletteIndex) : null;
            if (entry == null || entry.prefab == null) return;

            var existing = layer.Get(x, z);
            bool sameEntry = existing != null && existing.paletteId == entry.id &&
                              (layer.isBaseLayer || existing.direction == currentDirection);

            // 프리팹/방향이 이미 동일하고 새로 적용할 커스텀 데이터도 없으면 굳이 재생성하지 않음
            if (sameEntry && pendingCustomData.Count == 0)
                return;

            RemoveSpawned(activeLayerIndex, x, z);

            var placement = new ObjectPlacement
            {
                x = x,
                z = z,
                paletteId = entry.id,
                direction = layer.isBaseLayer ? TileDirection.North : currentDirection,
                userValue = entry.userValue
            };

            // 같은 칸에 있던 기존 커스텀 데이터는 유지한 뒤, pending 데이터로 덮어쓴다.
            if (existing?.customData != null)
            {
                foreach (var kv in existing.customData)
                    placement.customData[kv.Key] = kv.Value;
            }
            foreach (var kv in pendingCustomData)
                placement.customData[kv.Key] = kv.Value;

            // 강조색: 이번에 새 커스텀 데이터를 적용하는 중이면 pending 색상을 사용,
            // 아니면 기존 배치의 색을 유지(없으면 기본값).
            placement.highlightColor = pendingCustomData.Count > 0
                ? pendingHighlightColor
                : (existing?.highlightColor ?? defaultHighlightColor);

            layer.Set(placement);

            SpawnVisual(activeLayerIndex, placement, entry);
        }

        private void EraseAtCell(int x, int z)
        {
            var layer = layers[activeLayerIndex];
            if (layer.Get(x, z) == null) return;

            layer.Remove(x, z);
            RemoveSpawned(activeLayerIndex, x, z);
        }

        private void SpawnVisual(int layerIndex, ObjectPlacement placement, TilePaletteEntry entry)
        {
            var layer = layers[layerIndex];
            Vector3 pos = settings.CellToWorld(placement.x, placement.z, layer.yOffset);
            Quaternion rot = Quaternion.Euler(0f, (int)placement.direction * 90f, 0f);

            var go = Instantiate(entry.prefab, pos, rot, mapRoot);
            go.name = $"{entry.id}_{placement.x}_{placement.z}";

            spawnedObjects[(layerIndex, TileLayer.Key(placement.x, placement.z))] = go;
        }

        private void RemoveSpawned(int layerIndex, int x, int z)
        {
            var key = (layerIndex, TileLayer.Key(x, z));
            if (spawnedObjects.TryGetValue(key, out var go))
            {
                if (go != null) Destroy(go);
                spawnedObjects.Remove(key);
            }
        }

        private void RebuildAllVisuals()
        {
            foreach (var kv in spawnedObjects)
                if (kv.Value != null) Destroy(kv.Value);
            spawnedObjects.Clear();

            for (int li = 0; li < layers.Count; li++)
            {
                var layerPalette = GetPaletteForLayer(li);
                if (layerPalette == null) continue;

                foreach (var placement in layers[li].placements.Values)
                {
                    var entry = layerPalette.GetById(placement.paletteId);
                    if (entry != null && entry.prefab != null)
                        SpawnVisual(li, placement, entry);
                }
            }
        }

        // ---------------- 레이어 관리 ----------------

        private void AddLayer(string layerName)
        {
            layers.Add(new TileLayer
            {
                layerName = string.IsNullOrEmpty(layerName) ? $"Layer{layers.Count}" : layerName,
                isBaseLayer = false,
                yOffset = baseLayerYOffset + layerYStep * layers.Count
            });
        }

        private void RemoveActiveLayer()
        {
            if (layers[activeLayerIndex].isBaseLayer) return; // 베이스 레이어는 삭제 불가
            if (layers.Count <= 1) return;

            ClearLayerVisuals(activeLayerIndex);
            layers.RemoveAt(activeLayerIndex);
            activeLayerIndex = Mathf.Clamp(activeLayerIndex - 1, 0, layers.Count - 1);
        }

        private void ClearActiveLayer()
        {
            layers[activeLayerIndex].Clear();
            ClearLayerVisuals(activeLayerIndex);
        }

        private void ClearLayerVisuals(int layerIndex)
        {
            var keysToRemove = new List<(int, string)>();
            foreach (var kv in spawnedObjects)
            {
                if (kv.Key.Item1 == layerIndex)
                {
                    if (kv.Value != null) Destroy(kv.Value);
                    keysToRemove.Add(kv.Key);
                }
            }
            foreach (var k in keysToRemove) spawnedObjects.Remove(k);
        }

        // ---------------- 저장 / 불러오기 ----------------

        private string GetSavePath() => Path.Combine(saveDirectory, saveFileName);

        private void SaveMap()
        {
            TilemapBinaryIO.Save(GetSavePath(), settings, layers);
        }

        private void LoadMap()
        {
            if (TilemapBinaryIO.Load(GetSavePath(), settings, out var loadedLayers))
            {
                layers.Clear();
                layers.AddRange(loadedLayers);
                if (layers.Count == 0) EnsureBaseLayerExists();
                activeLayerIndex = 0;
                RebuildAllVisuals();
            }
        }

#if UNITY_EDITOR
        // Editor에서 Play 모드로 실행 중일 때만 사용 가능한 네이티브 폴더/파일 선택창.
        // (빌드에는 포함되지 않음 - UnityEditor 어셈블리는 런타임 빌드에 존재하지 않음)

        private void BrowseSaveFolder()
        {
            string picked = EditorUtility.OpenFolderPanel("저장 폴더 선택", saveDirectory, "");
            if (!string.IsNullOrEmpty(picked))
                saveDirectory = picked;
        }

        private void SaveMapAs()
        {
            string picked = EditorUtility.SaveFilePanel("타일맵 저장", saveDirectory, saveFileName, "bin");
            if (string.IsNullOrEmpty(picked)) return;

            saveDirectory = Path.GetDirectoryName(picked);
            saveFileName = Path.GetFileName(picked);
            TilemapBinaryIO.Save(picked, settings, layers);
        }

        private void LoadMapFrom()
        {
            string picked = EditorUtility.OpenFilePanel("타일맵 불러오기", saveDirectory, "bin");
            if (string.IsNullOrEmpty(picked)) return;

            saveDirectory = Path.GetDirectoryName(picked);
            saveFileName = Path.GetFileName(picked);

            if (TilemapBinaryIO.Load(picked, settings, out var loadedLayers))
            {
                layers.Clear();
                layers.AddRange(loadedLayers);
                if (layers.Count == 0) EnsureBaseLayerExists();
                activeLayerIndex = 0;
                RebuildAllVisuals();
            }
        }
#endif

        // ---------------- 그리드 시각화 ----------------

        private void OnDrawGizmos()
        {
            if (settings == null) return;

            Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
            float w = settings.width * settings.tileSize;
            float d = settings.depth * settings.tileSize;

            for (int x = 0; x <= settings.width; x++)
            {
                Vector3 a = new Vector3(x * settings.tileSize, 0f, 0f);
                Vector3 b = new Vector3(x * settings.tileSize, 0f, d);
                Gizmos.DrawLine(a, b);
            }
            for (int z = 0; z <= settings.depth; z++)
            {
                Vector3 a = new Vector3(0f, 0f, z * settings.tileSize);
                Vector3 b = new Vector3(w, 0f, z * settings.tileSize);
                Gizmos.DrawLine(a, b);
            }

            if (Application.isPlaying && hasHover)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = settings.CellToWorld(hoverX, hoverZ, 0.02f);
                Gizmos.DrawWireCube(center, new Vector3(settings.tileSize, 0.05f, settings.tileSize));
            }
        }

        // Scene 뷰용 Gizmos와 별개로, 실제 빌드/Game 뷰 화면에도 그리드와 커서가 보이도록
        // GL 즉시모드 렌더링을 사용한다. (Unity 내장 셰이더라 별도 머티리얼 에셋 불필요)
        private static void CreateLineMaterial()
        {
            if (lineMaterial != null) return;

            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }

        private void OnRenderObject()
        {
            if (!Application.isPlaying) return;
            if (!showGrid && !hasHover && !highlightCustomData) return;

            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(mapRoot != null ? mapRoot.localToWorldMatrix : Matrix4x4.identity);

            if (showGrid) DrawGridGL();
            if (highlightCustomData) DrawCustomDataHighlightsGL();
            if (hasHover) DrawCursorHighlightGL();

            GL.PopMatrix();
        }

        private void DrawGridGL()
        {
            float w = settings.width * settings.tileSize;
            float d = settings.depth * settings.tileSize;

            GL.Begin(GL.LINES);
            GL.Color(gridLineColor);
            for (int x = 0; x <= settings.width; x++)
            {
                float px = x * settings.tileSize;
                GL.Vertex3(px, gridY, 0f);
                GL.Vertex3(px, gridY, d);
            }
            for (int z = 0; z <= settings.depth; z++)
            {
                float pz = z * settings.tileSize;
                GL.Vertex3(0f, gridY, pz);
                GL.Vertex3(w, gridY, pz);
            }
            GL.End();

            // 맵 전체 크기를 알아보기 쉽도록 외곽선을 다른 색으로 강조
            GL.Begin(GL.LINES);
            GL.Color(gridBorderColor);
            GL.Vertex3(0f, gridY, 0f); GL.Vertex3(w, gridY, 0f);
            GL.Vertex3(w, gridY, 0f); GL.Vertex3(w, gridY, d);
            GL.Vertex3(w, gridY, d); GL.Vertex3(0f, gridY, d);
            GL.Vertex3(0f, gridY, d); GL.Vertex3(0f, gridY, 0f);
            GL.End();
        }

        private void DrawCursorHighlightGL()
        {
            Vector3 c = settings.CellToWorld(hoverX, hoverZ, gridY + 0.001f);
            float half = settings.tileSize * 0.5f;

            GL.Begin(GL.QUADS);
            GL.Color(cursorFillColor);
            GL.Vertex3(c.x - half, c.y, c.z - half);
            GL.Vertex3(c.x - half, c.y, c.z + half);
            GL.Vertex3(c.x + half, c.y, c.z + half);
            GL.Vertex3(c.x + half, c.y, c.z - half);
            GL.End();

            GL.Begin(GL.LINE_STRIP);
            GL.Color(cursorOutlineColor);
            GL.Vertex3(c.x - half, c.y, c.z - half);
            GL.Vertex3(c.x - half, c.y, c.z + half);
            GL.Vertex3(c.x + half, c.y, c.z + half);
            GL.Vertex3(c.x + half, c.y, c.z - half);
            GL.Vertex3(c.x - half, c.y, c.z - half);
            GL.End();
        }

        /// <summary>
        /// customData를 가진 배치가 있는 셀만 골라 placement.highlightColor로 채운 사각형을 그린다.
        /// customData가 없는 셀은 대상에서 제외되므로 별도 표시가 없다.
        /// </summary>
        private void DrawCustomDataHighlightsGL()
        {
            float blink = customDataBlinkSpeed > 0f
                ? (Mathf.Sin(Time.time * customDataBlinkSpeed) * 0.5f + 0.5f)
                : 1f;

            GL.Begin(GL.QUADS);
            foreach (var layer in layers)
            {
                foreach (var placement in layer.placements.Values)
                {
                    if (placement.customData == null || placement.customData.Count == 0) continue;

                    Vector3 c = settings.CellToWorld(placement.x, placement.z, layer.yOffset + 0.0015f);
                    float half = settings.tileSize * 0.5f;

                    Color col = placement.highlightColor;
                    col.a *= blink;
                    GL.Color(col);
                    GL.Vertex3(c.x - half, c.y, c.z - half);
                    GL.Vertex3(c.x - half, c.y, c.z + half);
                    GL.Vertex3(c.x + half, c.y, c.z + half);
                    GL.Vertex3(c.x + half, c.y, c.z - half);
                }
            }
            GL.End();
        }

        // ---------------- GUI ----------------

        private void OnGUI()
        {
            panelRect = GUILayout.Window(GetInstanceID(), panelRect, DrawPanel, "Tilemap Tool");
        }

        private void DrawColorSwatch(Color c, float w, float h)
        {
            if (swatchTex == null)
                swatchTex = new Texture2D(1, 1);
            swatchTex.SetPixel(0, 0, c);
            swatchTex.Apply();
            GUILayout.Label(swatchTex, GUILayout.Width(w), GUILayout.Height(h));
        }

        private void DrawPanel(int id)
        {
            GUILayout.Label($"Map: {settings.width} x {settings.depth}  (tile {settings.tileSize})");
            GUILayout.Label(hasHover ? $"Hover Cell: ({hoverX}, {hoverZ})" : "Hover Cell: -");
            GUILayout.Label($"Direction: {currentDirection}  (R 키로 회전)");
            showGrid = GUILayout.Toggle(showGrid, "Show Grid");

            GUILayout.Space(6);
            var activePalette = GetPaletteForLayer(activeLayerIndex);
            GUILayout.Label($"Palette (Layer {activeLayerIndex} 전용)", GUI.skin.box);
            paletteScroll = GUILayout.BeginScrollView(paletteScroll, GUILayout.Height(120));
            if (activePalette != null)
            {
                for (int i = 0; i < activePalette.entries.Count; i++)
                {
                    var e = activePalette.entries[i];
                    bool selected = i == activePaletteIndex;
                    string label = $"{(selected ? "▶ " : "")}{e.id}  (v={e.userValue})";
                    if (GUILayout.Toggle(selected, label, "Button"))
                        activePaletteIndex = i;
                }
            }
            else
            {
                GUILayout.Label($"palettes[{activeLayerIndex}]가 비어있습니다.\nInspector에서 레이어 순서에 맞게 등록하세요.");
            }
            GUILayout.EndScrollView();

            GUILayout.Space(6);
            GUILayout.Label("Layers", GUI.skin.box);
            layerScroll = GUILayout.BeginScrollView(layerScroll, GUILayout.Height(100));
            for (int i = 0; i < layers.Count; i++)
            {
                var l = layers[i];
                bool selected = i == activeLayerIndex;
                string tag = l.isBaseLayer ? "[Base] " : "";
                if (GUILayout.Toggle(selected, $"{(selected ? "▶ " : "")}{tag}{l.layerName}", "Button") && !selected)
                {
                    activeLayerIndex = i;
                    activePaletteIndex = 0;
                }
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            newLayerNameInput = GUILayout.TextField(newLayerNameInput);
            if (GUILayout.Button("+ Add", GUILayout.Width(60)))
                AddLayer(newLayerNameInput);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Layer"))
                ClearActiveLayer();
            if (GUILayout.Button("Remove Layer"))
                RemoveActiveLayer();
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            GUILayout.Label("Custom Data (배치 시 함께 저장)", GUI.skin.box);
            GUILayout.BeginHorizontal();
            newCustomKeyInput = GUILayout.TextField(newCustomKeyInput, GUILayout.Width(80));
            newCustomValueInput = GUILayout.TextField(newCustomValueInput);
            if (GUILayout.Button("+", GUILayout.Width(24)) && !string.IsNullOrEmpty(newCustomKeyInput))
            {
                pendingCustomData[newCustomKeyInput] = newCustomValueInput;
                newCustomKeyInput = string.Empty;
                newCustomValueInput = string.Empty;
            }
            GUILayout.EndHorizontal();

            customDataScroll = GUILayout.BeginScrollView(customDataScroll, GUILayout.Height(60));
            string keyToRemove = null;
            foreach (var kv in pendingCustomData)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{kv.Key} = {kv.Value}");
                if (GUILayout.Button("x", GUILayout.Width(20))) keyToRemove = kv.Key;
                GUILayout.EndHorizontal();
            }
            if (keyToRemove != null) pendingCustomData.Remove(keyToRemove);
            GUILayout.EndScrollView();

            if (pendingCustomData.Count > 0 && GUILayout.Button("Clear Pending Data"))
                pendingCustomData.Clear();

            GUILayout.Space(4);
            GUILayout.Label("Highlight Color (customData 있는 셀만 표시)");
            GUILayout.BeginHorizontal();
            DrawColorSwatch(pendingHighlightColor, 28, 18);
            GUILayout.BeginVertical();
            pendingHighlightColor.r = GUILayout.HorizontalSlider(pendingHighlightColor.r, 0f, 1f);
            pendingHighlightColor.g = GUILayout.HorizontalSlider(pendingHighlightColor.g, 0f, 1f);
            pendingHighlightColor.b = GUILayout.HorizontalSlider(pendingHighlightColor.b, 0f, 1f);
            pendingHighlightColor.a = GUILayout.HorizontalSlider(pendingHighlightColor.a, 0.05f, 1f);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (hasHover)
            {
                var hoverLayer = layers[activeLayerIndex];
                var hoverPlacement = hoverLayer.Get(hoverX, hoverZ);
                if (hoverPlacement != null && hoverPlacement.customData.Count > 0)
                {
                    GUILayout.Label("Hover Cell Data:");
                    foreach (var kv in hoverPlacement.customData)
                        GUILayout.Label($"  {kv.Key} = {kv.Value}");
                    if (GUILayout.Button("Clear Hover Cell Data"))
                        hoverPlacement.customData.Clear();
                }
            }

            GUILayout.Space(6);
            GUILayout.Label("Save / Load", GUI.skin.box);
            GUILayout.Label("폴더: " + saveDirectory, GUI.skin.label);
#if UNITY_EDITOR
            if (GUILayout.Button("Browse Folder..."))
                BrowseSaveFolder();
#endif
            saveFileName = GUILayout.TextField(saveFileName);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
                SaveMap();
            if (GUILayout.Button("Load"))
                LoadMap();
            GUILayout.EndHorizontal();
#if UNITY_EDITOR
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save As..."))
                SaveMapAs();
            if (GUILayout.Button("Load From..."))
                LoadMapFrom();
            GUILayout.EndHorizontal();
#else
            GUILayout.Label("(빌드에서는 폴더/파일 선택창 대신 위 경로를 사용합니다)");
#endif

            GUILayout.Space(4);
            GUILayout.Label("좌클릭:배치 / 우클릭:삭제 / R:방향회전 / Tab:레이어전환\nWASD/화살표:이동 / 휠:줌 / 휠클릭 드래그:패닝", GUI.skin.box);

            GUI.DragWindow();
        }
    }
}
