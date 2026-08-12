using UnityEngine;

namespace ValueAnim
{
    /// <summary>
    /// 셰이더 프로퍼티(Color / Float / Vector)를 A -> B 값으로 애니메이션합니다.
    /// MaterialPropertyBlock을 사용하므로 머티리얼 인스턴스가 생기지 않아 배칭이 유지되고,
    /// 프로퍼티 이름 문자열만 맞으면 URP Lit / Simple Lit / Unlit / 커스텀 셰이더 등
    /// 어떤 URP 셰이더에도 동일하게 사용할 수 있습니다.
    ///
    /// [자주 쓰는 URP 프로퍼티 이름 참고]
    /// - URP Lit / Simple Lit 기본 색상   : _BaseColor   (구버전 Standard의 _Color 아님)
    /// - URP Lit / Simple Lit Emission   : _EmissionColor (머티리얼에서 Emission 체크 필요)
    /// - URP Unlit 기본 색상             : _BaseColor
    /// - Metallic / Smoothness (Lit)     : _Metallic, _Smoothness (Float)
    /// - 커스텀 셰이더                    : 셰이더 코드의 Properties에 선언된 이름 그대로 입력
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public class ShaderPropertyAnimator : MonoBehaviour
    {
        public enum PropertyType { Color, Float, Vector }

        [Header("대상 렌더러")]
        public Renderer targetRenderer;

        [Tooltip("여러 서브메시/머티리얼 슬롯 중 적용할 인덱스. -1이면 모든 슬롯에 적용")]
        public int materialIndex = -1;

        [Header("프로퍼티 정보")]
        [Tooltip("셰이더 프로퍼티 이름. 예: URP Lit 색상은 _BaseColor, Emission은 _EmissionColor")]
        public string propertyName = "_BaseColor";
        public PropertyType propertyType = PropertyType.Color;

        [Header("Color A -> B (propertyType = Color 일 때)")]
        public Color colorA = Color.white;
        public Color colorB = Color.white;

        [Header("Float A -> B (propertyType = Float 일 때)")]
        public float floatA = 0f;
        public float floatB = 1f;

        [Header("Vector A -> B (propertyType = Vector 일 때)")]
        public Vector4 vectorA;
        public Vector4 vectorB;

        [Header("타이밍")]
        public float duration = 1f;
        public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public LoopMode loopMode = LoopMode.Once;

        [Header("재생 옵션")]
        public bool playOnStart = true;
        public bool useUnscaledTime = false;

        private float _elapsed;
        private bool _playing;
        private bool _finished;
        private MaterialPropertyBlock _mpb;
        private int _propId;

        public bool IsPlaying => _playing;
        public bool IsFinished => _finished;

        private void Awake()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<Renderer>();

            _mpb = new MaterialPropertyBlock();
            _propId = Shader.PropertyToID(propertyName);
        }

        private void Start()
        {
            if (playOnStart)
                Play();
        }

        public void Play()
        {
            _elapsed = 0f;
            _playing = true;
            _finished = false;
            ApplyAtElapsed();
        }

        public void Stop() => _playing = false;
        public void Pause() => _playing = false;
        public void Resume() => _playing = true;

        public void SetNormalizedTime(float normalized01)
        {
            _elapsed = Mathf.Clamp01(normalized01) * duration;
            ApplyAtElapsed();
        }

        private void Update()
        {
            if (!_playing || _finished) return;

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _elapsed += dt;

            ApplyAtElapsed();
        }

        private void ApplyAtElapsed()
        {
            if (targetRenderer == null || _mpb == null) return;

            float t = LoopTimeUtility.Evaluate(_elapsed, duration, loopMode, out bool finished);
            float eased = easeCurve.Evaluate(t);

            int readSlot = Mathf.Max(materialIndex, 0);
            targetRenderer.GetPropertyBlock(_mpb, readSlot);

            switch (propertyType)
            {
                case PropertyType.Color:
                    _mpb.SetColor(_propId, Color.LerpUnclamped(colorA, colorB, eased));
                    break;
                case PropertyType.Float:
                    _mpb.SetFloat(_propId, Mathf.LerpUnclamped(floatA, floatB, eased));
                    break;
                case PropertyType.Vector:
                    _mpb.SetVector(_propId, Vector4.LerpUnclamped(vectorA, vectorB, eased));
                    break;
            }

            if (materialIndex < 0)
            {
                int count = targetRenderer.sharedMaterials.Length;
                for (int i = 0; i < count; i++)
                    targetRenderer.SetPropertyBlock(_mpb, i);
            }
            else
            {
                targetRenderer.SetPropertyBlock(_mpb, materialIndex);
            }

            if (loopMode == LoopMode.Once && finished)
            {
                _playing = false;
                _finished = true;
            }
        }

#if UNITY_EDITOR
        // 인스펙터에서 propertyName을 바꿨을 때 즉시 반영
        private void OnValidate()
        {
            _propId = Shader.PropertyToID(propertyName);
        }
#endif
    }
}
