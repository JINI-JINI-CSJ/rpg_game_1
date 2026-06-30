using System;
using System.Collections.Generic;

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

// 랜덤 메이커 매니저 및 유틸
public class Mng_X128SS
{
    public uint seed;
    public bool save_state; // 저장 할 때 상태도 저장할지.. 월드 메이킹에는 저장안하고 처음부터 생성하게 된다.
    Xoshiro128StarStar xoshiro;
    public Mng_X128SS( uint _seed )
    {
        seed = _seed;
        xoshiro = new Xoshiro128StarStar(seed);
    } 

    public void Load()
    {
    }

    public void Save()
    {
    }

    public uint NextUInt()
    {
        return xoshiro.NextUInt();
    }

    public int NextInt(int min, int max)
    {
        return xoshiro.NextInt(min , max);
    }

    public float NextFloat()
    {
        return xoshiro.NextFloat();
    }

    public float NextFloat( float min , float max )
    {
        return xoshiro.NextFloat(min , max );
    }


    public class _STEP_Val
    {
        public float val; // 이전값과 더함
        public object obj;
    }

    // 간단 성공 ( 0 ~ 1 )
    public bool RandomFloat_Per( float val_succ )
    {
        if(NextFloat() <= val_succ )
        {
            return true;
        }
        return false;
    }

    // 스텝 랜덤
    List<_STEP_Val> lt_step = new List<_STEP_Val>();
    public void Step_Clear()
    {
        lt_step.Clear();
    }
    public void Step_Start_Add( float val , object obj )
    {
        Step_Clear();
        Step_Add( val , obj );
    }
    public void Step_Add( float val , object obj )
    {
        if( val < float.Epsilon ) return;

        _STEP_Val sv_pre = null;
        if( lt_step.Count > 0 )
        {
            sv_pre = lt_step[ lt_step.Count - 1 ];
        }

        _STEP_Val sv_cur = new _STEP_Val();
        sv_cur.obj = obj;
        sv_cur.val = val;
        if( sv_pre != null )sv_cur.val += sv_pre.val; // 이전꺼랑 계속 누적
        lt_step.Add( sv_cur );
    }
    public object Step_Random()
    {
        if( lt_step.Count < 1 ) return null;
        float r = NextFloat();
        _STEP_Val  sv_last = lt_step[ lt_step.Count - 1 ];
        r *= sv_last.val; // 마지막 값을 최대치 대응
        foreach( var s in lt_step )
        {
            if( r <= s.val )
            {
                return s.obj;
            }
        }
        return lt_step[ lt_step.Count - 1 ].obj;
    }

    // 인덱스 스텝
    // 퍼센트 목록 받아서 바로 결과 인덱스
    public int Step_Random_Idx( List<float> _floats_per )
    {
        if( _floats_per == null || _floats_per.Count < 1 ) return -1;
        List<float> steps = new();

        List<float> floats_per = new();

        foreach( var s in _floats_per )
        {
            if( s > float.Epsilon )
            {
                floats_per.Add(s);
            }
        }

        if( floats_per.Count < 1 ) return -1;

        for( int i = 0 ; i < floats_per.Count ; i++ )
        {
            if( i == 0 )
            {
                steps.Add( floats_per[i] );
            }
            else
            {
                float pre = floats_per[i-1];
                steps.Add( pre + floats_per[i] );
            }
        }

        float last = steps[ steps.Count - 1 ] ;
        float random = NextFloat();
        random *= last;

        for( int i = 0 ; i < steps.Count ; i++ )
        {
            if( random <= steps[i] )return i;
        }
        return steps.Count-1;
    }

    // 심플 스텝
    public T RandomListParams<T>( params T[] args )
    {
        if( args.Length < 1 ) return default;
        int idx = NextInt( 0 , args.Length );
        return args[idx];
    }

    // 리스트 목록 
    public T RandomList<T>( List<T> lt , bool remove = false )
    {
        if( lt.Count < 1 ) return default;
        int idx = NextInt( 0 , lt.Count );
        T r_obj = lt[idx];
        if( remove )
            lt.RemoveAt( idx );
        return r_obj;
    }
}
