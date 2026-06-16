using System;

public struct DeterministicRngState
{
    public uint s0, s1, s2, s3;
}

public class Xoshiro128StarStar
{
    private uint s0, s1, s2, s3;

    public Xoshiro128StarStar(uint seed)
    {
        // SplitMix32로 초기화
        s0 = SplitMix(ref seed);
        s1 = SplitMix(ref seed);
        s2 = SplitMix(ref seed);
        s3 = SplitMix(ref seed);
    }

    public Xoshiro128StarStar(DeterministicRngState state)
    {
        s0 = state.s0;
        s1 = state.s1;
        s2 = state.s2;
        s3 = state.s3;
    }

    public DeterministicRngState GetState()
    {
        return new DeterministicRngState
        {
            s0 = s0,
            s1 = s1,
            s2 = s2,
            s3 = s3
        };
    }

    public void SetState(DeterministicRngState state)
    {
        s0 = state.s0;
        s1 = state.s1;
        s2 = state.s2;
        s3 = state.s3;
    }

    public uint NextUInt()
    {
        uint result = RotateLeft(s1 * 5, 7) * 9;

        uint t = s1 << 9;

        s2 ^= s0;
        s3 ^= s1;
        s1 ^= s2;
        s0 ^= s3;

        s2 ^= t;
        s3 = RotateLeft(s3, 11);

        return result;
    }

    public int NextInt(int min, int max)
    {
        return (int)(NextUInt() % (uint)(max - min)) + min;
    }

    public float NextFloat()
    {
        return (NextUInt() >> 8) * (1f / 16777216f);
    }

    public float NextFloat( float min , float max )
    {
        return NextFloat() * (max - min) + min;
    }

    private static uint RotateLeft(uint x, int k)
    {
        return (x << k) | (x >> (32 - k));
    }

    private static uint SplitMix(ref uint seed)
    {
        uint z = (seed += 0x9E3779B9);
        z = (z ^ (z >> 16)) * 0x85EBCA6B;
        z = (z ^ (z >> 13)) * 0xC2B2AE35;
        return z ^ (z >> 16);
    }
}