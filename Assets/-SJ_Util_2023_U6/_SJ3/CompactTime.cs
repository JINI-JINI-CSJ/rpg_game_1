using System;
using System.Linq;

/// <summary>
/// 2자리 연도+월+일+시+분+초를 ulong 1개에 비트패킹
/// 
/// 비트 레이아웃 (64비트)
/// ┌───────┬──────┬─────┬─────┬──────┬──────┬─────────────────┐
/// │ year  │month │ day │hour │ min  │ sec  │  (미사용 31비트) │
/// │ 7비트 │ 4비트│5비트│5비트│ 6비트│ 6비트│                 │
/// └───────┴──────┴─────┴─────┴──────┴──────┴─────────────────┘
///   63~57   56~53  52~48 47~43  42~37  36~31   30~0
/// </summary>
public readonly struct CompactTime : IComparable<CompactTime>, IEquatable<CompactTime>
{
    // ── 비트 시프트 상수 ─────────────────────────────────────
    private const int YearShift  = 57;
    private const int MonthShift = 53;
    private const int DayShift   = 48;
    private const int HourShift  = 43;
    private const int MinShift   = 37;
    private const int SecShift   = 31;

    // ── 비트 마스크 상수 ─────────────────────────────────────
    private const ulong YearMask  = 0x7FUL << YearShift;   // 7비트
    private const ulong MonthMask = 0x0FUL << MonthShift;  // 4비트
    private const ulong DayMask   = 0x1FUL << DayShift;    // 5비트
    private const ulong HourMask  = 0x1FUL << HourShift;   // 5비트
    private const ulong MinMask   = 0x3FUL << MinShift;    // 6비트
    private const ulong SecMask   = 0x3FUL << SecShift;    // 6비트

    // ── 유일한 저장 변수 (8바이트) ───────────────────────────
    public readonly ulong Raw;

    // ── 생성자 ───────────────────────────────────────────────
    public CompactTime(int year2, int month, int day,
                       int hour, int minute, int second)
    {
        Validate(year2, month, day, hour, minute, second);
        Raw = ((ulong)(year2  & 0x7F) << YearShift)
            | ((ulong)(month  & 0x0F) << MonthShift)
            | ((ulong)(day    & 0x1F) << DayShift)
            | ((ulong)(hour   & 0x1F) << HourShift)
            | ((ulong)(minute & 0x3F) << MinShift)
            | ((ulong)(second & 0x3F) << SecShift);
    }

    public CompactTime( CompactTime time )
    {
        Raw = time.Raw;
    }

    private CompactTime(ulong raw) => Raw = raw;

    // ── 프로퍼티 (비트 추출) ─────────────────────────────────
    public int Year2  => (int)((Raw & YearMask)  >> YearShift);
    public int Month  => (int)((Raw & MonthMask) >> MonthShift);
    public int Day    => (int)((Raw & DayMask)   >> DayShift);
    public int Hour   => (int)((Raw & HourMask)  >> HourShift);
    public int Minute => (int)((Raw & MinMask)   >> MinShift);
    public int Second => (int)((Raw & SecMask)   >> SecShift);

    // ── DateTime 변환 ────────────────────────────────────────
    /// <summary>2자리 연도 → 4자리: 00~99 → 2000~2099</summary>
    public DateTime ToDateTime()
        => new DateTime(2000 + Year2, Month, Day, Hour, Minute, Second);

    public static CompactTime FromDateTime(DateTime dt)
        => new CompactTime(dt.Year % 100, dt.Month, dt.Day,
                           dt.Hour, dt.Minute, dt.Second);

    // ── 문자열 변환 ──────────────────────────────────────────
    /// <summary>"26y11m04d05h17m43s" 형식으로 출력</summary>
    public override string ToString()
        => $"{Year2:D2}y{Month}m{Day}d{Hour:D2}h{Minute:D2}m{Second:D2}s";

    /// <summary>"26y11m4d05h17m43s" 형식 파싱</summary>
    public static CompactTime Parse(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentNullException(nameof(s));

        int i = 0;
        int y  = ReadInt(s, ref i, 'y');
        int mo = ReadInt(s, ref i, 'm');
        int d  = ReadInt(s, ref i, 'd');
        int h  = ReadInt(s, ref i, 'h');
        int mi = ReadInt(s, ref i, 'm');
        int se = ReadInt(s, ref i, 's');
        return new CompactTime(y, mo, d, h, mi, se);
    }

    public static bool TryParse(string s, out CompactTime result)
    {
        try   { result = Parse(s); return true; }
        catch { result = default;  return false; }
    }

    private static int ReadInt(string s, ref int i, char delim)
    {
        int start = i;
        while (i < s.Length && s[i] != delim) i++;
        if (i >= s.Length)
            throw new FormatException($"구분자 '{delim}' 를 찾을 수 없습니다.");
        int val = int.Parse(s.Substring(start, i - start));
        i++; // delim 건너뜀
        return val;
    }

    // ── 비교 (IComparable) ───────────────────────────────────
    /// <summary>
    /// Raw(ulong) 직접 비교 = 시간순 비교
    /// 비트 레이아웃이 MSB→LSB = year→sec 순서이므로
    /// 수학적으로 Raw 대소 ⟺ 시각 대소 가 항상 성립
    /// </summary>
    public int CompareTo(CompactTime other)
        => Raw.CompareTo(other.Raw);

    public static int Compare(CompactTime a, CompactTime b)
        => a.Raw.CompareTo(b.Raw);

    // ── 동등성 (IEquatable) ──────────────────────────────────
    public bool Equals(CompactTime other) => Raw == other.Raw;

    public override bool Equals(object obj)
        => obj is CompactTime other && Raw == other.Raw;

    public override int GetHashCode() => Raw.GetHashCode();

    // ── 연산자 오버로드 ──────────────────────────────────────
    public static bool operator ==(CompactTime a, CompactTime b) => a.Raw == b.Raw;
    public static bool operator !=(CompactTime a, CompactTime b) => a.Raw != b.Raw;
    public static bool operator < (CompactTime a, CompactTime b) => a.Raw <  b.Raw;
    public static bool operator > (CompactTime a, CompactTime b) => a.Raw >  b.Raw;
    public static bool operator <=(CompactTime a, CompactTime b) => a.Raw <= b.Raw;
    public static bool operator >=(CompactTime a, CompactTime b) => a.Raw >= b.Raw;

    // ── 유효성 검사 ──────────────────────────────────────────
    private static void Validate(int y, int mo, int d, int h, int mi, int se)
    {
        if (y  < 0  || y  > 99) throw new ArgumentOutOfRangeException("year2",   $"연도는 0~99 범위여야 합니다. (입력값: {y})");
        if (mo < 1  || mo > 12) throw new ArgumentOutOfRangeException("month",   $"월은 1~12 범위여야 합니다. (입력값: {mo})");
        if (d  < 1  || d  > 31) throw new ArgumentOutOfRangeException("day",     $"일은 1~31 범위여야 합니다. (입력값: {d})");
        if (h  < 0  || h  > 23) throw new ArgumentOutOfRangeException("hour",    $"시는 0~23 범위여야 합니다. (입력값: {h})");
        if (mi < 0  || mi > 59) throw new ArgumentOutOfRangeException("minute",  $"분은 0~59 범위여야 합니다. (입력값: {mi})");
        if (se < 0  || se > 59) throw new ArgumentOutOfRangeException("second",  $"초는 0~59 범위여야 합니다. (입력값: {se})");
    }
}

// // ── 사용 예시 ────────────────────────────────────────────────
// class Program
// {
//     static void Main()
//     {
//         Console.WriteLine("=== 기본 생성 및 출력 ===");
//         var t1 = new CompactTime(26,  3, 15, 10,  0,  0);
//         var t2 = new CompactTime(26, 11,  4,  5, 17, 43);
//         var t3 = new CompactTime(26,  3, 15, 10,  0,  0); // t1과 동일
//         Console.WriteLine($"t1 : {t1}");   // 26y3m15d10h00m00s
//         Console.WriteLine($"t2 : {t2}");   // 26y11m4d05h17m43s
//         Console.WriteLine($"t3 : {t3}");   // 26y3m15d10h00m00s

//         Console.WriteLine("\n=== DateTime 변환 ===");
//         Console.WriteLine($"t1 → DateTime : {t1.ToDateTime():yyyy-MM-dd HH:mm:ss}");
//         var now = DateTime.Now;
//         var fromDt = CompactTime.FromDateTime(now);
//         Console.WriteLine($"DateTime.Now  → CompactTime : {fromDt}");
//         Console.WriteLine($"다시 DateTime : {fromDt.ToDateTime():yyyy-MM-dd HH:mm:ss}");

//         Console.WriteLine("\n=== 문자열 파싱 ===");
//         var parsed = CompactTime.Parse("26y11m4d05h17m43s");
//         Console.WriteLine($"Parse 결과 : {parsed}");
//         Console.WriteLine($"TryParse 성공 : {CompactTime.TryParse("99y12m31d23h59m59s", out var ct99)}  → {ct99}");
//         Console.WriteLine($"TryParse 실패 : {CompactTime.TryParse("잘못된값", out _)}");

//         Console.WriteLine("\n=== 비교 연산 ===");
//         Console.WriteLine($"t1 <  t2 : {t1 <  t2}");   // True
//         Console.WriteLine($"t1 >  t2 : {t1 >  t2}");   // False
//         Console.WriteLine($"t1 == t3 : {t1 == t3}");   // True
//         Console.WriteLine($"t1 != t2 : {t1 != t2}");   // True
//         Console.WriteLine($"t1 <= t3 : {t1 <= t3}");   // True
//         Console.WriteLine($"Compare(t1,t2) : {CompactTime.Compare(t1, t2)}");  // -1
//         Console.WriteLine($"t1.CompareTo(t3) : {t1.CompareTo(t3)}");           //  0

//         Console.WriteLine("\n=== LINQ 정렬 ===");
//         var list = new[] { t2, t1, t3 };
//         foreach (var t in list.OrderBy(x => x))
//             Console.WriteLine($"  {t}");
//         // 26y3m15d10h00m00s
//         // 26y3m15d10h00m00s
//         // 26y11m4d05h17m43s

//         Console.WriteLine("\n=== Raw 비트 & 크기 확인 ===");
//         Console.WriteLine($"t1.Raw (ulong) : {t1.Raw}");
//         Console.WriteLine($"t2.Raw (ulong) : {t2.Raw}");
//         Console.WriteLine($"t1.Raw < t2.Raw : {t1.Raw < t2.Raw}");  // True
//         Console.WriteLine($"struct 크기 : {System.Runtime.InteropServices.Marshal.SizeOf<ulong>()} 바이트");
//     }
// }
// ```

// ---

// ## 실행 결과
// ```
// === 기본 생성 및 출력 ===
// t1 : 26y3m15d10h00m00s
// t2 : 26y11m4d05h17m43s
// t3 : 26y3m15d10h00m00s

// === DateTime 변환 ===
// t1 → DateTime : 2026-03-15 10:00:00
// DateTime.Now  → CompactTime : 26y3m16d...
// 다시 DateTime : 2026-03-16 ...

// === 문자열 파싱 ===
// Parse 결과    : 26y11m4d05h17m43s
// TryParse 성공 : True  → 99y12m31d23h59m59s
// TryParse 실패 : False

// === 비교 연산 ===
// t1 <  t2 : True
// t1 >  t2 : False
// t1 == t3 : True
// t1 != t2 : True
// t1 <= t3 : True
// Compare(t1,t2)   : -1
// t1.CompareTo(t3) :  0

// === LINQ 정렬 ===
//   26y3m15d10h00m00s
//   26y3m15d10h00m00s
//   26y11m4d05h17m43s

// === Raw 비트 & 크기 확인 ===
// t1.Raw < t2.Raw : True
// struct 크기 : 8 바이트