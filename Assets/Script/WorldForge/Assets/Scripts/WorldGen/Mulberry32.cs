using System;

namespace WorldForge
{
    /// <summary>
    /// Mulberry32 — 빠르고 품질 좋은 시드 기반 PRNG
    /// Unity 2021+ (C# 8) 호환 — >>> 연산자 미사용
    /// </summary>
    public class Mulberry32
    {
        private uint _state;

        public Mulberry32(int seed)
        {
            _state = (uint)seed;
        }

        /// <summary>0.0 ~ 1.0 (exclusive) 균등 분포 난수</summary>
        public float NextFloat()
        {
            _state += 0x6D2B79F5u;
            uint t = (_state ^ (_state >> 15)) * (1u | _state);
            t ^= t + (t ^ (t >> 7)) * (61u | t);
            // C# 8 이하: >>> 대신 uint 타입이므로 >> 가 논리 시프트와 동일
            uint result = t ^ (t >> 14);
            return result / 4294967296f;
        }

        /// <summary>0 ~ max (exclusive) 정수 난수</summary>
        public int NextInt(int max)
        {
            return (int)(NextFloat() * max);
        }

        /// <summary>min ~ max (exclusive) 정수 난수</summary>
        public int NextInt(int min, int max)
        {
            return min + (int)(NextFloat() * (max - min));
        }

        /// <summary>Fisher-Yates 셔플</summary>
        public void Shuffle<T>(T[] arr)
        {
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = NextInt(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
    }
}
