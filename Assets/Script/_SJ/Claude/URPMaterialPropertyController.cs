using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// URP Lit/Unlit 셰이더의 주요 파라미터(베이스 컬러, 이미션, 메탈릭, 스무니스 등)를
/// MaterialPropertyBlock으로 제어하는 컴포넌트.
/// 머티리얼 인스턴스(.material 접근)를 생성하지 않으므로 SRP Batcher / GPU Instancing이 유지됩니다.
/// [ExecuteAlways]로 플레이 모드뿐 아니라 에디트 모드에서도 인스펙터 값 변경이 씬에 바로 반영됩니다.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public class URPMaterialPropertyController : MonoBehaviour
{
    [Header("대상 렌더러")]
    [SerializeField] private Renderer _targetRenderer;
    [SerializeField] private bool _includeChildRenderers = false;

    [Header("서브메시 인덱스 (Renderer.materials 기준)")]
    [SerializeField] private int _materialIndex = 0;
    [Tooltip("켜면 위 서브메시 인덱스는 무시하고, 렌더러가 가진 모든 매터리얼 슬롯에 동일하게 적용합니다.")]
    [SerializeField] private bool _applyToAllMaterialSlots = false;

    [Header("초기값 (Start 시 자동 적용)")]
    [SerializeField] private bool _applyOnStart = true;
    [SerializeField] private Color _baseColor = Color.white;
    [SerializeField] private bool _enableEmission = false;
    [ColorUsage(true, true)]
    [SerializeField] private Color _emissionColor = Color.black;
    [SerializeField, Range(0f, 1f)] private float _metallic = 0f;
    [SerializeField, Range(0f, 1f)] private float _smoothness = 0.5f;

    // ---- URP 표준 프로퍼티 이름 ----
    private const string PROP_BASE_COLOR   = "_BaseColor";
    private const string PROP_BASE_MAP     = "_BaseMap";
    private const string PROP_EMISSION     = "_EmissionColor";
    private const string PROP_METALLIC     = "_Metallic";
    private const string PROP_SMOOTHNESS   = "_Smoothness";
    private const string KEYWORD_EMISSION  = "_EMISSION";

    // ---- 캐싱된 Shader Property ID ----
    private static readonly int ID_BaseColor  = Shader.PropertyToID(PROP_BASE_COLOR);
    private static readonly int ID_BaseMap    = Shader.PropertyToID(PROP_BASE_MAP);
    private static readonly int ID_Emission   = Shader.PropertyToID(PROP_EMISSION);
    private static readonly int ID_Metallic   = Shader.PropertyToID(PROP_METALLIC);
    private static readonly int ID_Smoothness = Shader.PropertyToID(PROP_SMOOTHNESS);

    private MaterialPropertyBlock _block;
    private List<Renderer> _renderers = new List<Renderer>();

    // OnEnable은 플레이 모드 진입 시뿐 아니라, 에디트 모드에서 컴포넌트 추가/씬 로드/
    // 스크립트 리컴파일 후에도 호출됩니다 ([ExecuteAlways] 덕분).
    private void OnEnable()
    {
        InitRenderers();

        // 에디트 모드에서 컴포넌트를 추가하거나 씬을 열었을 때 즉시 프리뷰 반영
        if (!Application.isPlaying)
            ApplyAll();
    }

    private void InitRenderers()
    {
        if (_block == null)
            _block = new MaterialPropertyBlock();

        if (_targetRenderer == null)
            _targetRenderer = GetComponent<Renderer>();

        _renderers.Clear();
        if (_targetRenderer != null)
            _renderers.Add(_targetRenderer);

        if (_includeChildRenderers)
        {
            var children = GetComponentsInChildren<Renderer>();
            foreach (var r in children)
            {
                if (!_renderers.Contains(r))
                    _renderers.Add(r);
            }
        }
    }

    private void Start()
    {
        if (_applyOnStart && Application.isPlaying)
            ApplyAll();
    }

    // ================= Public API =================

    public void SetBaseColor(Color color)
    {
        _baseColor = color;
        ForEachRendererSlot((r, slot) =>
        {
            r.GetPropertyBlock(_block, slot);
            _block.SetColor(ID_BaseColor, color);
            r.SetPropertyBlock(_block, slot);
        });
    }

    public void SetEmissionColor(Color color, bool autoEnableKeyword = true)
    {
        _emissionColor = color;
        _enableEmission = autoEnableKeyword ? true : _enableEmission;

        ForEachRendererSlot((r, slot) =>
        {
            if (autoEnableKeyword)
                EnableEmissionKeyword(r);

            r.GetPropertyBlock(_block, slot);
            _block.SetColor(ID_Emission, color);
            r.SetPropertyBlock(_block, slot);
        });
    }

    public void SetEmissionIntensity(float intensity)
    {
        Color baseColor = _emissionColor;
        // HDR 색상에서 RGB 비율은 유지하고 강도만 스케일링
        Color scaled = new Color(baseColor.r, baseColor.g, baseColor.b) * intensity;
        SetEmissionColor(scaled);
    }

    public void SetMetallic(float value)
    {
        _metallic = Mathf.Clamp01(value);
        SetFloat(ID_Metallic, _metallic);
    }

    public void SetSmoothness(float value)
    {
        _smoothness = Mathf.Clamp01(value);
        SetFloat(ID_Smoothness, _smoothness);
    }

    public void SetBaseMap(Texture texture)
    {
        ForEachRendererSlot((r, slot) =>
        {
            r.GetPropertyBlock(_block, slot);
            _block.SetTexture(ID_BaseMap, texture);
            r.SetPropertyBlock(_block, slot);
        });
    }

    /// <summary>임의의 float 프로퍼티를 이름으로 직접 설정</summary>
    public void SetFloat(string propertyName, float value)
    {
        SetFloat(Shader.PropertyToID(propertyName), value);
    }

    /// <summary>임의의 Color 프로퍼티를 이름으로 직접 설정 (HDR 지원)</summary>
    public void SetColor(string propertyName, Color value)
    {
        int id = Shader.PropertyToID(propertyName);
        ForEachRendererSlot((r, slot) =>
        {
            r.GetPropertyBlock(_block, slot);
            _block.SetColor(id, value);
            r.SetPropertyBlock(_block, slot);
        });
    }

    /// <summary>초기 설정값(_baseColor, _emissionColor 등)을 한 번에 적용</summary>
    public void ApplyAll()
    {
        SetBaseColor(_baseColor);
        SetMetallic(_metallic);
        SetSmoothness(_smoothness);

        if (_enableEmission)
            SetEmissionColor(_emissionColor);
    }

    /// <summary>MaterialPropertyBlock을 비워 머티리얼 원본값으로 되돌림</summary>
    public void ClearOverrides()
    {
        ForEachRendererSlot((r, slot) =>
        {
            r.GetPropertyBlock(_block, slot);
            _block.Clear();
            r.SetPropertyBlock(_block, slot);
        });
    }

    // ================= Internal =================

    private void SetFloat(int propertyId, float value)
    {
        ForEachRendererSlot((r, slot) =>
        {
            r.GetPropertyBlock(_block, slot);
            _block.SetFloat(propertyId, value);
            r.SetPropertyBlock(_block, slot);
        });
    }

    private void EnableEmissionKeyword(Renderer r)
    {
        // MaterialPropertyBlock은 셰이더 키워드를 직접 켤 수 없으므로
        // 공유 머티리얼에 한 번만 활성화해줍니다. (Emission 체크박스 GI 계산과 무관하게 색상 자체를 보이게 하려면 필요)
        var mats = r.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] != null && !mats[i].IsKeywordEnabled(KEYWORD_EMISSION))
            {
                mats[i].EnableKeyword(KEYWORD_EMISSION);
                mats[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
        }
    }

    private void ForEachRenderer(System.Action<Renderer> action)
    {
        if (_block == null || (_renderers.Count == 0 && _targetRenderer == null))
            InitRenderers();

        if (_renderers.Count == 0 && _targetRenderer != null)
            _renderers.Add(_targetRenderer);

        for (int i = 0; i < _renderers.Count; i++)
        {
            if (_renderers[i] != null)
                action(_renderers[i]);
        }
    }

    /// <summary>
    /// 렌더러 하나당 적용할 매터리얼 슬롯 인덱스(들)까지 함께 순회합니다.
    /// _applyToAllMaterialSlots가 켜져 있으면 해당 렌더러의 모든 슬롯(0..sharedMaterials.Length-1)에,
    /// 꺼져 있으면 _materialIndex 슬롯 하나에만 적용합니다.
    /// </summary>
    private void ForEachRendererSlot(System.Action<Renderer, int> action)
    {
        ForEachRenderer(r =>
        {
            if (_applyToAllMaterialSlots)
            {
                int count = r.sharedMaterials.Length;
                for (int slot = 0; slot < count; slot++)
                    action(r, slot);
            }
            else
            {
                action(r, _materialIndex);
            }
        });
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 인스펙터 값 변경 시 에디트 모드/플레이 모드 모두에서 실시간 프리뷰.
        // OnValidate 안에서 바로 SetPropertyBlock을 호출하면 "SendMessage cannot be called
        // during Awake" 류의 경고가 뜰 수 있어 한 프레임 지연시켜 안전하게 적용합니다.
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return; // 삭제된 경우 방지
            ApplyAll();
        };
    }
#endif
}
