using UnityEngine;

[DisallowMultipleComponent]
public class Cld_ShakeEffect : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float magnitude = 0.15f;   // 최대 흔들림 폭
    [SerializeField] private float frequency = 25f;      // 초당 흔들림 진동 수
    [SerializeField] private AnimationCurve dampingCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private bool useUnscaledTime = false;

    private Vector3 _originalLocalPos;
    private float _elapsed;
    private bool _isShaking;
    private Vector3 _seed;

    private void Awake()
    {
        _originalLocalPos = transform.localPosition;
    }

    public void Shake()
    {
        Shake(duration, magnitude);
    }

    public void Shake(float customDuration, float customMagnitude)
    {
        duration = customDuration;
        magnitude = customMagnitude;

        // 흔들림 시작 시점의 localPosition을 기준점으로 재고정
        // (연출 도중 다른 로직이 위치를 바꿨을 수도 있으므로)
        _originalLocalPos = transform.localPosition;
        _elapsed = 0f;
        _seed = new Vector3(Random.value, Random.value, Random.value) * 100f;
        _isShaking = true;
    }

    private void Update()
    {
        if (!_isShaking) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        _elapsed += dt;

        if (_elapsed >= duration)
        {
            _isShaking = false;
            transform.localPosition = _originalLocalPos;
            return;
        }

        float t = _elapsed / duration;
        float damper = dampingCurve.Evaluate(t);

        // Perlin Noise 기반 오프셋 (부드럽고 자연스러운 흔들림)
        float time = _elapsed * frequency;
        float offsetX = (Mathf.PerlinNoise(_seed.x, time) - 0.5f) * 2f;
        float offsetY = (Mathf.PerlinNoise(_seed.y, time) - 0.5f) * 2f;
        float offsetZ = (Mathf.PerlinNoise(_seed.z, time) - 0.5f) * 2f;

        Vector3 offset = new Vector3(offsetX, offsetY, offsetZ) * magnitude * damper;
        transform.localPosition = _originalLocalPos + offset;
    }

    // 강제 중단 시 원위치 복구
    public void StopShake()
    {
        _isShaking = false;
        transform.localPosition = _originalLocalPos;
    }
}