using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WorldForge
{
    /// <summary>
    /// RawImage 위에서 마우스를 움직이면 해당 타일 정보를 툴팁으로 표시.
    /// RawImage 오브젝트에 추가하고, MapDisplay / OverlayDisplay 를 함께 연결.
    /// </summary>
    public class MapTooltipHandler : MonoBehaviour,
        IPointerMoveHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("References")]
        public WorldForgeManager Manager;
        public RectTransform     TooltipRect;   // 팝업 패널 RectTransform
        public Text              TooltipText;   // 팝업 안의 Text
        public RectTransform     MapRect;       // 맵 RawImage 의 RectTransform

        // ════════════════════════════════════════════════════════
        public void OnPointerMove(PointerEventData ev)
        {
            if (Manager?.CurrentWorld == null) { HideTooltip(); return; }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    MapRect, ev.position, ev.pressEventCamera, out Vector2 local))
            { HideTooltip(); return; }

            var w   = Manager.CurrentWorld;
            var rect = MapRect.rect;

            // 정규화 좌표 → 타일 좌표
            float nx = (local.x - rect.x) / rect.width;
            float ny = (local.y - rect.y) / rect.height;
            int tx = Mathf.FloorToInt(nx * w.Width);
            int ty = Mathf.FloorToInt((1f - ny) * w.Height); // Y 반전 (텍스처 origin = 좌하단)

            if (!w.InBounds(tx, ty)) { HideTooltip(); return; }

            BuildTooltip(w, tx, ty);
            ShowTooltip(ev.position);
        }

        public void OnPointerExit(PointerEventData _) => HideTooltip();

        public void OnPointerClick(PointerEventData ev)
        {
            // 클릭 정보는 디버그 로그로 (필요 시 OnTileClicked 이벤트 추가)
            if (Manager?.CurrentWorld == null) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    MapRect, ev.position, ev.pressEventCamera, out Vector2 local)) return;

            var w    = Manager.CurrentWorld;
            var rect = MapRect.rect;
            int tx = Mathf.FloorToInt((local.x - rect.x) / rect.width  * w.Width);
            int ty = Mathf.FloorToInt((1f - (local.y - rect.y) / rect.height) * w.Height);
            if (!w.InBounds(tx, ty)) return;
            Debug.Log($"[WorldForge] 클릭: ({tx},{ty}) | {BiomeClassifier.KoreanName(w.Biomes[w.Idx(tx,ty)])}");
        }

        // ── 툴팁 내용 구성 ────────────────────────────────────────
        private void BuildTooltip(WorldData w, int tx, int ty)
        {
            int idx  = w.Idx(tx, ty);
            var bio  = w.Biomes[idx];
            float h  = w.HeightMap[idx];
            float t  = w.TempMap[idx];
            int   n  = w.NationMap[idx];

            // 도시/스폿 확인 (2타일 반경)
            CityData? city = null;
            foreach (var c in w.Cities)
                if (Mathf.Abs(c.X - tx) <= 1 && Mathf.Abs(c.Y - ty) <= 1) { city = c; break; }

            SpotData? spot = null;
            foreach (var s in w.Spots)
                if (Mathf.Abs(s.X - tx) <= 1 && Mathf.Abs(s.Y - ty) <= 1) { spot = s; break; }

            var sb = new System.Text.StringBuilder();

            if (city.HasValue)
            {
                var c = city.Value;
                sb.AppendLine($"<b>{c.Name}</b>");
                string tierName = c.Tier switch
                {
                    CityTier.Capital => "★ 수도",
                    CityTier.Major   => "◆ 대도시",
                    CityTier.Minor   => "● 중도시",
                    _                => "· 소도시"
                };
                sb.AppendLine(tierName);
                if (n >= 0 && n < w.Nations.Count)
                    sb.AppendLine($"◈ {w.Nations[n].Name}");
            }
            else if (spot.HasValue)
            {
                var s = spot.Value;
                sb.AppendLine($"<b>{WorldMapRenderer.SpotEmoji(s.Type)} {s.Name}</b>");
                sb.AppendLine($"{SpotTypeName(s.Type)}");
                if (n >= 0 && n < w.Nations.Count)
                    sb.AppendLine($"◈ {w.Nations[n].Name}");
            }
            else
            {
                sb.AppendLine($"<b>{BiomeClassifier.KoreanName(bio)}</b>");
                if (n >= 0 && n < w.Nations.Count)
                    sb.AppendLine($"◈ {w.Nations[n].Name}");
            }

            sb.AppendLine($"고도 {h * 100f:F0}%  기온 {t * 100f:F0}%");
            sb.Append($"좌표 ({tx}, {ty})");

            if (TooltipText) TooltipText.text = sb.ToString();
        }

        private static string SpotTypeName(SpotType t) => t switch
        {
            SpotType.Dungeon     => "던전",
            SpotType.AncientRuin => "고대 유적",
            SpotType.MagicTower  => "마법탑",
            SpotType.Graveyard   => "묘지",
            SpotType.Volcano     => "화산",
            SpotType.DragonLair  => "용의 둥지",
            _                    => "?"
        };

        // ── 표시/숨김 ──────────────────────────────────────────────
        private void ShowTooltip(Vector2 screenPos)
        {
            if (!TooltipRect) return;
            TooltipRect.gameObject.SetActive(true);

            // 화면 밖으로 나가지 않도록 위치 보정
            Vector2 pos = screenPos + new Vector2(16f, -10f);
            var sw = Screen.width; var sh = Screen.height;
            var tw = TooltipRect.sizeDelta.x; var th = TooltipRect.sizeDelta.y;
            if (pos.x + tw > sw) pos.x = screenPos.x - tw - 4f;
            if (pos.y - th < 0)  pos.y = screenPos.y + th + 4f;

            TooltipRect.position = pos;
        }

        private void HideTooltip()
        {
            if (TooltipRect) TooltipRect.gameObject.SetActive(false);
        }
    }
}
