using UnityEngine;

namespace ValueAnim
{
    /// <summary>
    /// 오브젝트의 Position / Rotation / Scale을 A -> B 값으로 애니메이션합니다.
    /// Once / Loop / PingPong 재생을 지원합니다.
    /// </summary>
    [DisallowMultipleComponent]
    public class TransformValueAnimator : MonoBehaviour
    {
        public enum SpaceMode { Local, World }

        [Header("애니메이션할 요소")]
        public bool animatePosition = false;
        public bool animateRotation = false;
        public bool animateScale = false;

        [Header("좌표 기준 (Scale은 항상 Local)")]
        public SpaceMode space = SpaceMode.Local;

        [Header("Position A -> B")]
        public Vector3 positionA;
        public Vector3 positionB;

        [Header("Rotation A -> B (Euler Angle)")]
        public Vector3 rotationA;
        public Vector3 rotationB;

        [Header("Scale A -> B")]
        public Vector3 scaleA = Vector3.one;
        public Vector3 scaleB = Vector3.one;

        [Header("타이밍")]
        [Tooltip("A->B 편도 재생 시간(초)")]
        public float duration = 1f;
        [Tooltip("0~1 진행도에 적용할 이징 커브")]
        public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public LoopMode loopMode = LoopMode.Once;

        [Header("재생 옵션")]
        public bool playOnStart = true;
        public bool useUnscaledTime = false;

        private float _elapsed;
        private bool _playing;
        private bool _finished;

        public bool IsPlaying => _playing;
        public bool IsFinished => _finished;

        private void Start()
        {
            if (playOnStart)
                Play();
        }

        /// <summary>처음부터 재생 시작</summary>
        public void Play()
        {
            _elapsed = 0f;
            _playing = true;
            _finished = false;
            ApplyAtElapsed();
        }

        /// <summary>정지하고 상태 초기화 없이 멈춤</summary>
        public void Stop() => _playing = false;

        public void Pause() => _playing = false;
        public void Resume() => _playing = true;

        /// <summary>B -> A 방향으로 재생하고 싶을 때 사용 (현재 시간 기준 역재생 시작)</summary>
        public void PlayReverseFromCurrent()
        {
            (positionA, positionB) = (positionB, positionA);
            (rotationA, rotationB) = (rotationB, rotationA);
            (scaleA, scaleB) = (scaleB, scaleA);
            Play();
        }

        /// <summary>0~1 사이 특정 지점으로 즉시 이동 (스크럽바 등에 사용)</summary>
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
            float t = LoopTimeUtility.Evaluate(_elapsed, duration, loopMode, out bool finished);
            float eased = easeCurve.Evaluate(t);

            if (animatePosition)
            {
                Vector3 pos = Vector3.LerpUnclamped(positionA, positionB, eased);
                if (space == SpaceMode.Local) transform.localPosition = pos;
                else transform.position = pos;
            }

            if (animateRotation)
            {
                Quaternion rot = Quaternion.SlerpUnclamped(
                    Quaternion.Euler(rotationA), Quaternion.Euler(rotationB), eased);
                if (space == SpaceMode.Local) transform.localRotation = rot;
                else transform.rotation = rot;
            }

            if (animateScale)
            {
                // Scale은 World 개념이 없으므로 항상 localScale에 적용
                transform.localScale = Vector3.LerpUnclamped(scaleA, scaleB, eased);
            }

            if (loopMode == LoopMode.Once && finished)
            {
                _playing = false;
                _finished = true;
            }
        }
    }
}
