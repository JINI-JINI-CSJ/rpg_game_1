using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TileEditor.Core;
using TileEditor.Runtime;

namespace TileEditor.EditorTools
{
    /// <summary>
    /// Tools > Tile Editor 메뉴로 여는 타일맵 에디터 윈도우.
    /// 복수 레이어, 원형/사각형 브러쉬(사이즈 조절), 맵 전체 크기 조절, 바이너리 저장/로드를 지원한다.
    /// </summary>
    public class TileEditorWindow : EditorWindow
    {
        private const float CellSize = 24f;

        private TileMapData _map;
        private string _currentPath;

        private int _activeLayerIndex = -1;
        private BrushShape _brushShape = BrushShape.Square;
        private int _brushSize = 0; // 0 = 셀 1개
        private int _paintValue = 0;

        private TilePaletteAsset _palette;
        private bool _paletteEditMode;

        private Vector2 _panOffset;
        private float _zoom = 1f;

        private int _newWidth = 32;
        private int _newHeight = 32;

        private Vector2 _layerScroll;
        private Vector2Int? _lastPaintedCell;

        [MenuItem("Tools/Tile Editor")]
        public static void Open()
        {
            var window = GetWindow<TileEditorWindow>("Tile Editor");
            window.minSize = new Vector2(760, 480);
        }

        private void OnEnable()
        {
            if (_map == null)
            {
                _map = new TileMapData(_newWidth, _newHeight);
                _map.AddLayer("Layer 0");
                _activeLayerIndex = 0;
            }
            else
            {
                _newWidth = _map.Width;
                _newHeight = _map.Height;
            }
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawPaletteBar();

            EditorGUILayout.BeginHorizontal();
            DrawLayerPanel();
            DrawCanvas();
            EditorGUILayout.EndHorizontal();
        }

        // ---------------------------------------------------------------
        // 툴바
        // ---------------------------------------------------------------
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("New", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                if (EditorUtility.DisplayDialog("새 맵", "현재 맵을 새 맵으로 교체합니다. 저장하지 않은 변경사항은 사라집니다.", "계속", "취소"))
                {
                    _map = new TileMapData(_newWidth, _newHeight);
                    _map.AddLayer("Layer 0");
                    _activeLayerIndex = 0;
                    _currentPath = null;
                    _newWidth = _map.Width;
                    _newHeight = _map.Height;
                }
            }

            if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                string path = EditorUtility.OpenFilePanel("타일맵 불러오기", Application.dataPath, "tmap");
                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        _map = TileMapBinaryIO.Load(path);
                        _currentPath = path;
                        _activeLayerIndex = _map.Layers.Count > 0 ? 0 : -1;
                        _newWidth = _map.Width;
                        _newHeight = _map.Height;
                    }
                    catch (System.Exception e)
                    {
                        EditorUtility.DisplayDialog("로드 실패", e.Message, "확인");
                    }
                }
            }

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                SaveMap(false);
            }

            if (GUILayout.Button("Save As", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                SaveMap(true);
            }

            GUILayout.Space(12);
            GUILayout.Label("Size", GUILayout.Width(32));
            _newWidth = EditorGUILayout.IntField(_newWidth, GUILayout.Width(40));
            GUILayout.Label("x", GUILayout.Width(10));
            _newHeight = EditorGUILayout.IntField(_newHeight, GUILayout.Width(40));
            using (new EditorGUI.DisabledScope(_map != null && _newWidth == _map.Width && _newHeight == _map.Height))
            {
                if (GUILayout.Button("Resize Map", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    _map?.Resize(Mathf.Max(1, _newWidth), Mathf.Max(1, _newHeight));
                    GUI.FocusControl(null);
                }
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            // 브러쉬 옵션 줄
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Brush", GUILayout.Width(40));
            _brushShape = (BrushShape)EditorGUILayout.EnumPopup(_brushShape, GUILayout.Width(80));
            GUILayout.Label("Size", GUILayout.Width(32));
            _brushSize = EditorGUILayout.IntSlider(_brushSize, 0, 20, GUILayout.Width(160));
            GUILayout.Label("Value", GUILayout.Width(40));
            _paintValue = EditorGUILayout.IntField(_paintValue, GUILayout.Width(50));
            GUILayout.Space(12);
            GUILayout.Label("Zoom", GUILayout.Width(36));
            _zoom = EditorGUILayout.Slider(_zoom, 0.25f, 3f, GUILayout.Width(120));
            GUILayout.Space(12);
            EditorGUILayout.HelpBox("좌클릭: 페인트 / 우클릭: 지우기 / 휠클릭 드래그: 이동 / 휠: 줌", MessageType.None);
            EditorGUILayout.EndHorizontal();
        }

        private void SaveMap(bool forcePicker)
        {
            if (_map == null) return;
            string path = _currentPath;
            if (forcePicker || string.IsNullOrEmpty(path))
            {
                path = EditorUtility.SaveFilePanel("타일맵 저장", Application.dataPath, "NewTileMap", "tmap");
            }
            if (string.IsNullOrEmpty(path)) return;

            TileMapBinaryIO.Save(_map, path);
            _currentPath = path;
            AssetDatabase.Refresh();
        }

        // ---------------------------------------------------------------
        // 팔레트 (int 값 <-> 색상 매칭)
        // ---------------------------------------------------------------
        private void DrawPaletteBar()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Palette", EditorStyles.boldLabel, GUILayout.Width(50));
            _palette = (TilePaletteAsset)EditorGUILayout.ObjectField(_palette, typeof(TilePaletteAsset), false, GUILayout.Width(160));

            if (_palette == null)
            {
                if (GUILayout.Button("Create Palette", GUILayout.Width(100)))
                {
                    CreatePaletteAsset();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("팔레트를 만들면 값(int)마다 보여줄 색상을 지정하고, 클릭 한 번으로 칠할 값을 선택할 수 있습니다.", MessageType.None);
                EditorGUILayout.EndVertical();
                return;
            }

            if (GUILayout.Button("+ Color", GUILayout.Width(60)))
            {
                int nextId = 0;
                foreach (var e in _palette.Entries) nextId = Mathf.Max(nextId, e.Id + 1);
                _palette.Entries.Add(new TilePaletteAsset.Entry
                {
                    Id = nextId,
                    DisplayName = $"Tile {nextId}",
                    EditorColor = TilePaletteAsset.HashToColor(nextId)
                });
                _palette.RebuildLookup();
                EditorUtility.SetDirty(_palette);
                _paintValue = nextId;
            }

            _paletteEditMode = GUILayout.Toggle(_paletteEditMode, "Edit", EditorStyles.miniButton, GUILayout.Width(40));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (_palette.Entries.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                int removeIndex = -1;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(58);

                foreach (var entry in _palette.Entries)
                {
                    if (_paletteEditMode)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.Width(56));
                        entry.EditorColor = EditorGUILayout.ColorField(GUIContent.none, entry.EditorColor, false, true, false, GUILayout.Width(52), GUILayout.Height(18));
                        entry.Id = EditorGUILayout.IntField(entry.Id, GUILayout.Width(52));
                        if (GUILayout.Button("삭제", GUILayout.Width(52)))
                        {
                            removeIndex = _palette.Entries.IndexOf(entry);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        Color prevBg = GUI.backgroundColor;
                        GUI.backgroundColor = entry.EditorColor;
                        bool selected = _paintValue == entry.Id;
                        var style = new GUIStyle(GUI.skin.button) { fontStyle = selected ? FontStyle.Bold : FontStyle.Normal };
                        if (GUILayout.Button(entry.Id.ToString(), style, GUILayout.Width(36), GUILayout.Height(22)))
                        {
                            _paintValue = entry.Id;
                        }
                        GUI.backgroundColor = prevBg;
                    }
                }

                EditorGUILayout.EndHorizontal();

                if (removeIndex >= 0)
                {
                    _palette.Entries.RemoveAt(removeIndex);
                }

                if (EditorGUI.EndChangeCheck() || removeIndex >= 0)
                {
                    _palette.RebuildLookup();
                    EditorUtility.SetDirty(_palette);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void CreatePaletteAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject("팔레트 생성", "TilePalette", "asset", "팔레트를 저장할 위치를 선택하세요.");
            if (string.IsNullOrEmpty(path)) return;

            var asset = ScriptableObject.CreateInstance<TilePaletteAsset>();
            asset.Entries.Add(new TilePaletteAsset.Entry
            {
                Id = 0,
                DisplayName = "Tile 0",
                EditorColor = TilePaletteAsset.HashToColor(0)
            });
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            _palette = asset;
        }

        // ---------------------------------------------------------------
        // 레이어 패널
        // ---------------------------------------------------------------
        private void DrawLayerPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("Layers", EditorStyles.boldLabel);

            _layerScroll = EditorGUILayout.BeginScrollView(_layerScroll, GUILayout.ExpandHeight(true));
            if (_map != null)
            {
                int removeIndex = -1;
                for (int i = 0; i < _map.Layers.Count; i++)
                {
                    var layer = _map.Layers[i];
                    EditorGUILayout.BeginHorizontal(i == _activeLayerIndex ? EditorStyles.helpBox : GUIStyle.none);

                    layer.Visible = EditorGUILayout.Toggle(layer.Visible, GUILayout.Width(18));

                    bool wasActive = i == _activeLayerIndex;
                    bool nowActive = GUILayout.Toggle(wasActive, layer.Name, EditorStyles.miniButton);
                    if (nowActive) _activeLayerIndex = i;

                    if (GUILayout.Button("\u2191", GUILayout.Width(20)) && i > 0)
                    {
                        _map.MoveLayer(i, i - 1);
                        if (_activeLayerIndex == i) _activeLayerIndex = i - 1;
                    }
                    if (GUILayout.Button("\u2193", GUILayout.Width(20)) && i < _map.Layers.Count - 1)
                    {
                        _map.MoveLayer(i, i + 1);
                        if (_activeLayerIndex == i) _activeLayerIndex = i + 1;
                    }
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        removeIndex = i;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (removeIndex >= 0)
                {
                    _map.RemoveLayer(removeIndex);
                    if (_activeLayerIndex >= _map.Layers.Count) _activeLayerIndex = _map.Layers.Count - 1;
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("+ Add Layer") && _map != null)
            {
                _map.AddLayer($"Layer {_map.Layers.Count}");
                _activeLayerIndex = _map.Layers.Count - 1;
            }

            EditorGUILayout.EndVertical();
        }

        // ---------------------------------------------------------------
        // 캔버스 (그리드 표시 + 페인팅)
        // ---------------------------------------------------------------
        private void DrawCanvas()
        {
            EditorGUILayout.BeginVertical();
            Rect canvasRect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.Box(canvasRect, GUIContent.none);

            if (_map == null || _activeLayerIndex < 0)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            HandleCanvasInput(canvasRect);

            if (Event.current.type == EventType.Repaint)
            {
                DrawGrid(canvasRect);
            }

            EditorGUILayout.EndVertical();
            Repaint();
        }

        private float PixelPerCell => CellSize * _zoom;

        private void DrawGrid(Rect canvasRect)
        {
            GUI.BeginClip(canvasRect);

            var activeLayer = _map.Layers[_activeLayerIndex];
            float ppc = PixelPerCell;

            int minX = Mathf.Max(0, Mathf.FloorToInt(-_panOffset.x / ppc));
            int minY = Mathf.Max(0, Mathf.FloorToInt(-_panOffset.y / ppc));
            int maxX = Mathf.Min(activeLayer.Width - 1, Mathf.CeilToInt((canvasRect.width - _panOffset.x) / ppc));
            int maxY = Mathf.Min(activeLayer.Height - 1, Mathf.CeilToInt((canvasRect.height - _panOffset.y) / ppc));

            // 하위 레이어들은 옅게, 활성 레이어는 진하게 표시
            for (int li = 0; li < _map.Layers.Count; li++)
            {
                if (!_map.Layers[li].Visible) continue;
                bool active = li == _activeLayerIndex;
                float alphaMul = active ? 1f : 0.35f;
                DrawLayerCells(_map.Layers[li], minX, minY, maxX, maxY, ppc, alphaMul);
            }

            // 그리드 라인
            Handles.BeginGUI();
            Handles.color = new Color(1f, 1f, 1f, 0.08f);
            for (int x = minX; x <= maxX + 1; x++)
            {
                float sx = _panOffset.x + x * ppc;
                Handles.DrawLine(new Vector3(sx, 0), new Vector3(sx, canvasRect.height));
            }
            for (int y = minY; y <= maxY + 1; y++)
            {
                float sy = _panOffset.y + (activeLayer.Height - y) * ppc;
                Handles.DrawLine(new Vector3(0, sy), new Vector3(canvasRect.width, sy));
            }
            Handles.EndGUI();

            GUI.EndClip();
        }

        private void DrawLayerCells(TileLayer layer, int minX, int minY, int maxX, int maxY, float ppc, float alphaMul)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    int value = layer.Get(x, y);
                    if (value == TileLayer.EmptyValue) continue;

                    Color c = _palette != null ? _palette.GetColor(value) : TilePaletteAsset.HashToColor(value);
                    c.a *= alphaMul * Mathf.Clamp01(layer.Opacity);

                    float sx = _panOffset.x + x * ppc;
                    float sy = _panOffset.y + (layer.Height - 1 - y) * ppc;
                    EditorGUI.DrawRect(new Rect(sx, sy, ppc - 1, ppc - 1), c);
                }
            }
        }

        private void HandleCanvasInput(Rect canvasRect)
        {
            Event e = Event.current;
            if (!canvasRect.Contains(e.mousePosition)) return;

            Vector2 local = e.mousePosition - canvasRect.position;

            // 휠(중간 버튼) 드래그로 팬
            if (e.type == EventType.MouseDrag && e.button == 2)
            {
                _panOffset += e.delta;
                e.Use();
                return;
            }

            // 휠 스크롤로 줌
            if (e.type == EventType.ScrollWheel)
            {
                _zoom = Mathf.Clamp(_zoom - e.delta.y * 0.05f, 0.25f, 3f);
                e.Use();
                return;
            }

            bool isPaintButton = e.button == 0;
            bool isEraseButton = e.button == 1;
            if (!isPaintButton && !isEraseButton) return;

            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            {
                Vector2Int cell = LocalToCell(local);
                PaintAt(cell, isEraseButton);
                e.Use();
            }
            else if (e.type == EventType.MouseUp)
            {
                _lastPaintedCell = null;
                e.Use();
            }
        }

        private Vector2Int LocalToCell(Vector2 local)
        {
            float ppc = PixelPerCell;
            var layer = _map.Layers[_activeLayerIndex];
            int x = Mathf.FloorToInt((local.x - _panOffset.x) / ppc);
            int y = layer.Height - 1 - Mathf.FloorToInt((local.y - _panOffset.y) / ppc);
            return new Vector2Int(x, y);
        }

        private void PaintAt(Vector2Int cell, bool erase)
        {
            if (_activeLayerIndex < 0) return;

            // 드래그 중 빠르게 움직여 생기는 빈틈을 이전 셀과 보간해서 메운다.
            Vector2Int from = _lastPaintedCell ?? cell;
            int value = erase ? TileLayer.EmptyValue : _paintValue;
            foreach (var c in LineCells(from, cell))
            {
                _map.PaintBrush(_activeLayerIndex, c.x, c.y, _brushSize, _brushShape, value);
            }
            _lastPaintedCell = cell;
        }

        // 두 셀 사이를 Bresenham으로 보간하여 빠른 드래그에도 끊김 없이 페인트한다.
        private static IEnumerable<Vector2Int> LineCells(Vector2Int a, Vector2Int b)
        {
            int x0 = a.x, y0 = a.y, x1 = b.x, y1 = b.y;
            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                yield return new Vector2Int(x0, y0);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
