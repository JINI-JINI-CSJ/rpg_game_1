using UnityEngine;

// 캐릭터 수치
public class CharPrcValue : SJ_PrcValueMng
{
    // HP	
    public int HP               { get { return Value_Int((int)CHAR_STAT.HP); } }
    // MP
    public int MP               { get { return Value_Int((int)CHAR_STAT.MP); } }
    // 행동속도	
    public int ACTION_SPEED     { get { return Value_Int((int)CHAR_STAT.ACTION_SPEED); } }
    // 물공	
    public int ATK_P            { get { return Value_Int((int)CHAR_STAT.ATK_P); } }
    // 물방	
    public int DEF_P            { get { return Value_Int((int)CHAR_STAT.DEF_P); } }
    //물공명중	
    public int HIT_RATE_P       { get { return Value_Int((int)CHAR_STAT.HIT_RATE_P); } }
    // 물공회피	
    public int EVASION_RATE_P   { get { return Value_Int((int)CHAR_STAT.EVASION_RATE_P); } }
    // 마공	
    public int ATK_M            { get { return Value_Int((int)CHAR_STAT.ATK_M); } }
    // 마방
    public int DEF_M            { get { return Value_Int((int)CHAR_STAT.DEF_M); } }

    public void ReadCSV( SJ_CSV_BaseObj csv )
    {
        SetValue( (int)CHAR_STAT.HP             , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.MP             , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.ACTION_SPEED   , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.ATK_P          , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.DEF_P          , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.HIT_RATE_P     , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.EVASION_RATE_P , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.ATK_M          , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.DEF_M          , csv.Next_Float() );
    }

    // 보너스 점수로 원본 수치를 바꾼다.
    public void RandomStatBonus( Mng_X128SS rd , int score , float add_fix )
    {
        rd.Clear_RandomDivision();
        rd.Add_RandomDivision( CHAR_STAT.HP );
        rd.Add_RandomDivision( CHAR_STAT.MP );
        rd.Add_RandomDivision( CHAR_STAT.ACTION_SPEED );
        rd.Add_RandomDivision( CHAR_STAT.ATK_P );
        rd.Add_RandomDivision( CHAR_STAT.DEF_P );
        rd.Add_RandomDivision( CHAR_STAT.HIT_RATE_P );
        rd.Add_RandomDivision( CHAR_STAT.EVASION_RATE_P );
        rd.Add_RandomDivision( CHAR_STAT.ATK_M );
        rd.Add_RandomDivision( CHAR_STAT.DEF_M );
        rd.Random_RandomDivision( score );

        foreach( var s in rd.lt_RandomDivision )
        {
            CHAR_STAT stat = (CHAR_STAT)s.Item1;
            int add_lv = s.Item2;
            SJ_PrcValue prcValue = FindAlloc_SJ_PrcValue( (int)stat );
            prcValue.src = prcValue.src * ( (float)add_lv * add_fix );
            prcValue.LastCal();
        }
    }
}
