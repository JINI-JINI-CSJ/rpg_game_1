using UnityEngine;

// 캐릭터 수치
public class CharPrcValue : SJ_PrcValueMng
{
    public int HP               { get { return Value_Int((int)CHAR_STAT.HP); } }
    public int ACTION_SPEED     { get { return Value_Int((int)CHAR_STAT.ACTION_SPEED); } }
    public int ATK_P            { get { return Value_Int((int)CHAR_STAT.ATK_P); } }
    public int DEF_P            { get { return Value_Int((int)CHAR_STAT.DEF_P); } }
    public int HIT_RATE_P       { get { return Value_Int((int)CHAR_STAT.HIT_RATE_P); } }
    public int EVASION_RATE_P   { get { return Value_Int((int)CHAR_STAT.EVASION_RATE_P); } }
    public int ATK_M            { get { return Value_Int((int)CHAR_STAT.ATK_M); } }
    public int DEF_M            { get { return Value_Int((int)CHAR_STAT.DEF_M); } }

    public void ReadCSV( SJ_CSV_BaseObj csv )
    {
        SetValue( (int)CHAR_STAT.HP             , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.ACTION_SPEED   , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.ATK_P          , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.DEF_P          , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.HIT_RATE_P     , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.EVASION_RATE_P , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.ATK_M          , csv.Next_Float() );
        SetValue( (int)CHAR_STAT.DEF_M          , csv.Next_Float() );
    }
}
