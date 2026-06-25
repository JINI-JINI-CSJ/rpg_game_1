using System;
using UnityEngine;

// ``ID	이름	설명	리소스	등급	직업 대 분류	HP	행동속도	물공	물방	물공명중	물공회피	마공	마방
public class CSV_CharBaseStat : SJ_CSV_BaseObj
{
    public string name;
    public string desc;
    public string res;
    public int grade;
    public JOB_BASE jOB_BASE;
    public CharPrcValue charPrcValue = new();

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);
        name = Next();
        desc = Next();
        res = Next();
        grade = Next_Int();
        Enum.TryParse<JOB_BASE>( Next() , out jOB_BASE );
        charPrcValue.ReadCSV(this);
    }
}



public class CSV_CharBaseStatPage : SJ_CSV_BasePage
{
    
}
