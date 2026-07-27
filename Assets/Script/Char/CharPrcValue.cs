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
}
