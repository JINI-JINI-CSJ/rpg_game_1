using System;
using System.Collections;
using System.Collections.Generic;

// 
// 간단하게 트리거 호출 기능만 한다.
//

// 트리거 조건식 , 없으면 통과
public class SJ_TrgCall_Check
{
    virtual public bool Check(){return true;}
}

// 셀렉터
public class SJ_TrgCall_SelectUser
{
    virtual public ISJ_TrgCall_User[] FindUser(){return null;}
}

// 유저 인터페이스
public interface ISJ_TrgCall_User
{
    public void TrgCall( string func , object[] args , Dictionary<string,object> return_arg );
}

// 함수 및 인자
public class SJ_TrgCall_Func
{
    public string   func;
    public object[] args;
}


// 위에것들 묶음 , 트리거 이벤트 포함
public class SJ_TrgCall_Unit : SJ_TagBaseObj
{
    public SJ_TrgCall_Check         check;
    public SJ_TrgCall_SelectUser    select;
    public SJ_TrgCall_Func          func_arg;

    public override object OnFunc(int evt_int = 0, string evt_str = "", Dictionary<string, object> args = null)
    {
        if( check != null && check.Check() == false ) return false;
        if( func_arg == null )return false;
        ISJ_TrgCall_User[] users = select.FindUser();
        foreach( var user in users )
        {
            user.TrgCall( func_arg.func , func_arg.args , args );
        }
        return true;
    }



}
