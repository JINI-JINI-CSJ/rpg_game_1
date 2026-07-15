using UnityEngine;

namespace ValueAnim
{
    /// <summary>
    /// A -> B 애니메이션의 반복 방식.
    /// </summary>
    public enum LoopMode
    {
        Once,       // 한 번만 재생하고 끝에서 정지
        Loop,       // 0 -> 1 반복 (끝나면 즉시 처음으로)
        PingPong    // 0 -> 1 -> 0 왕복 반복
    }

    /// <summary>
    /// 경과 시간과 duration, LoopMode를 받아 0~1 정규화된 진행도를 계산하는 공용 유틸리티.
    /// TransformValueAnimator, ShaderPropertyAnimator가 공통으로 사용합니다.
    /// </summary>
    public static class LoopTimeUtility
    {
        /// <param name="elapsed">누적 경과 시간(초)</param>
        /// <param name="duration">A->B 편도 재생 시간(초)</param>
        /// <param name="mode">반복 방식</param>
        /// <param name="finished">LoopMode.Once일 때 재생이 끝났는지 여부</param>
        /// <returns>0~1 정규화된 진행도 (이 값을 AnimationCurve/Lerp에 넣어 사용)</returns>
        public static float Evaluate(float elapsed, float duration, LoopMode mode, out bool finished)
        {
            finished = false;

            if (duration <= 0f)
            {
                finished = true;
                return 1f;
            }

            float t = elapsed / duration;

            switch (mode)
            {
                case LoopMode.Once:
                    if (t >= 1f)
                    {
                        finished = true;
                        return 1f;
                    }
                    return t;

                case LoopMode.Loop:
                    // 0~1 사이를 계속 반복 (톱니파)
                    return t - Mathf.Floor(t);

                case LoopMode.PingPong:
                    // 0->1->0 왕복
                    return Mathf.PingPong(t, 1f);

                default:
                    return Mathf.Clamp01(t);
            }
        }
    }
}
