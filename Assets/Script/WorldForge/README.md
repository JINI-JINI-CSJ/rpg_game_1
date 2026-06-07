# ⚔ World Forge — RPG 월드맵 자동 생성기

Unity 6 / Unity 2022.3 LTS 이상 호환 (C# 9+)

---

## 📁 폴더 구조

```
Assets/
├── Scripts/
│   ├── WorldGen/               ← 순수 C# (Unity 의존 없음)
│   │   ├── Mulberry32.cs       PRNG (시드 기반 난수)
│   │   ├── PerlinNoise.cs      진짜 Perlin Noise + fBm
│   │   ├── WorldGenSettings.cs 생성 파라미터 + 프리셋
│   │   ├── WorldData.cs        전체 데이터 컨테이너
│   │   ├── BiomeClassifier.cs  바이옴 분류 로직
│   │   └── WorldGenerator.cs   생성 파이프라인 (9단계)
│   │
│   ├── Rendering/
│   │   └── WorldMapRenderer.cs WorldData → Texture2D
│   │
│   ├── UI/
│   │   ├── WorldForgePanel.cs  런타임 설정 팝업 UI
│   │   └── MapTooltipHandler.cs 마우스 호버 툴팁
│   │
│   └── WorldForgeManager.cs   MonoBehaviour 진입점
│
└── Editor/
    └── WorldForgeWindow.cs    에디터 전용 팝업 창
```

---

## 🚀 빠른 시작 (에디터 팝업)

1. 스크립트들을 Unity 프로젝트 `Assets/` 에 복사
2. 메뉴: **Tools > World Forge > Open Generator**
3. 슬라이더로 파라미터 조정 후 **⚑ Generate** 클릭
4. **💾 Save PNG** 로 이미지 저장

---

## 🎮 런타임 Scene 셋업

### 필수 오브젝트

```
Hierarchy:
├── WorldForgeManager         (WorldForgeManager.cs)
│     MapDisplay    → RawImage (맵 베이스)
│     OverlayDisplay→ RawImage (도시/스폿 오버레이, 같은 크기)
│
├── Canvas
│   ├── MapDisplay (RawImage) ← WorldForgeManager.MapDisplay 에 연결
│   ├── OverlayDisplay (RawImage)
│   │     └── MapTooltipHandler.cs 추가
│   │
│   ├── WorldForgePanel       (WorldForgePanel.cs)
│   │   ├── SeedInput         (InputField)
│   │   ├── BtnRandomSeed     (Button)
│   │   ├── SlNoiseScale      (Slider)
│   │   ├── ... (각 슬라이더)
│   │   ├── BtnGenerate       (Button)
│   │   └── BtnClose          (Button)
│   │
│   └── TooltipPanel          (비활성 상태로 시작)
│       └── TooltipText       (Text)
```

### Inspector 연결

**WorldForgeManager**
- `Map Display` → RawImage (맵)
- `Overlay Display` → RawImage (오버레이)

**WorldForgePanel**
- `Manager` → WorldForgeManager 오브젝트
- 각 Slider / Toggle / Text 를 Hierarchy 오브젝트에 연결

**MapTooltipHandler** (OverlayDisplay에 추가)
- `Manager` → WorldForgeManager
- `Tooltip Rect` → TooltipPanel RectTransform
- `Tooltip Text` → TooltipText
- `Map Rect` → MapDisplay RectTransform

---

## ⚙️ 생성 파라미터

| 파라미터 | 범위 | 설명 |
|---|---|---|
| Seed | 1 ~ 999999 | 동일 seed = 동일 맵 |
| Noise Scale | 0.5 ~ 8 | 클수록 완만한 대륙 |
| Octaves | 2 ~ 9 | 해안선 복잡도 |
| Persistence | 0.2 ~ 0.8 | 높을수록 울퉁불퉁 |
| Sea Level | 20% ~ 70% | 바다 비율 |
| Continent Bias | 0 ~ 0.8 | 중앙 육지 편향 |
| Edge Falloff | 0 ~ 1 | 가장자리 바다 강도 |
| Nations | 2 ~ 14 | 국가 수 |
| Cities | 4 ~ 50 | 도시 수 |
| Rivers | 0 ~ 30 | 강 수 |
| Spots | 0 ~ 40 | 특수 스폿 수 |

---

## 🗺 생성 파이프라인 (9단계)

```
Seed → Perlin fBm → Heightmap 정규화
     → 해수면 퍼센타일 → 바이옴 분류
     → 강 (하강 경로)
     → 국가 (Farthest-point Voronoi)
     → 도시 (적합도 점수 + Poisson 간격)
     → 교역로 (최근접 도시 연결)
     → 특수 스폿 (도시와 충분히 떨어진 위치)
```

---

## 🏛 특수 스폿 종류

| 이모지 | 종류 | 선호 지형 |
|---|---|---|
| ⚔ | 던전 | 산악, 고지대 |
| 🏛 | 고대 유적 | 평원, 내륙 |
| 🗼 | 마법탑 | 구릉, 언덕 |
| 💀 | 묘지 | 어디서나 |
| 🌋 | 화산 | 산악 |
| 🐉 | 용의 둥지 | 최고지대 |

---

## 📋 프리셋

```csharp
WorldGenSettings.Archipelago()  // 섬 군도 — 바다 많음
WorldGenSettings.Pangaea()      // 거대 대륙
WorldGenSettings.Mountainous()  // 산악 지형
```

---

## 💡 확장 아이디어

- `WorldGenerator.Generate()` 를 `Task.Run()`으로 감싸 비동기 처리
- `WorldData` 를 JSON으로 직렬화해 맵 저장/불러오기
- 타일 클릭 시 `OnTileClicked` 이벤트로 게임 로직 연결
- 도시별 인구/교역량 시뮬레이션 추가
