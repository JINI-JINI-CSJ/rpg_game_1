using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
///
// 문자열로 컴포넌트 추가 및 함수 호출
///
public class SJ_CompAddFunc
{

    static public   Component    Add_Comp( GameObject obj , System.Type componentType )
    {
        Component cp = obj.GetComponent( componentType );
        if( cp != null ) return cp;
        cp = obj.AddComponent(componentType);
        return cp;
    }   

    static public   Component    Add_Comp( GameObject obj , string str )
    {
        System.Type componentType = System.Type.GetType(str );
        if( componentType == null )
        {
            Debug.LogError( "에러!!! 못찾음 : " + str );
            return null;
        }
        return Add_Comp( obj , componentType );
    }

    static public   bool    Add_Comp_CallFunc( GameObject obj , string str , string func , object[] args = null )
    {
        Component cp = Add_Comp( obj , str );
        System.Type tp = cp.GetType();
        MethodInfo mi = tp.GetMethod( func, BindingFlags.Instance | BindingFlags.Public);

        if( mi == null )
        {
            Debug.LogError( "에러!!! 못찾음 : " + str  + " : " + func);
            return false;
        }
        mi.Invoke(cp , args);
        return true;
    }
}
