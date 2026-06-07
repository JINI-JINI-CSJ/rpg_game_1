using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

// 유니티 
using UnityEngine;

public class SJ_CALL_RETURN_NORMAL
{
        public  SJ_CallFuncClass_Cash._CASH_NORMAL cn = null;
        public  object return_func = null;
}

public class SJ_CALL_RETURN_UNITY
{
        public  SJ_CallFuncClass_Cash._CASH_UNITY cn = null;
        public  object return_func = null;
}


// 위의 문자열 클래스 , 함수 객체 단위로 캐쉬
public class SJ_CallFuncClass_Cash
{
        // 일반 c# 캐쉬 
        public class _CASH_NORMAL
        {
                public MethodInfo       mi;
                public object           inst_class; // 새로 만든 클래스일 경우 , 유니티처럼 컴포넌트 기능
        }

        // 일반 c# 클래스
        Dictionary<string,_CASH_NORMAL> dic_normal = new Dictionary<string,_CASH_NORMAL>();

        public _CASH_NORMAL Find_Normal( object call_obj , string str_class , string str_func )
        {
                string key = str_class + "_" + str_func;
                _CASH_NORMAL inf = null;
                if( dic_normal.TryGetValue( key, out inf )  )
                {
                        return inf;
                }

                object save_class = null;
                // 클래스 이름이 있을경우 컴포넌트 처럼 생성
                if( string.IsNullOrEmpty( str_class ) == false )
                {
                        save_class = SJ_CSharpUtil.NewClass_Str( str_class );
                }else{
                        // 없으면 본인 객체
                        save_class = call_obj;
                }

                inf = new _CASH_NORMAL();
                inf.mi = SJ_CSharpUtil.Get_MethodInfo_Inst( save_class , str_func );
                inf.inst_class = save_class;
                dic_normal[key] = inf;
                return inf;
        }

        public SJ_CALL_RETURN_NORMAL Call_Normal( object call_obj , string str_class , string str_func , object[] args )
        {
                SJ_CALL_RETURN_NORMAL r = new SJ_CALL_RETURN_NORMAL();
                _CASH_NORMAL cn = Find_Normal( call_obj , str_class , str_func );
                if( cn.mi != null )
                {
                        r.cn = cn;
                        r.return_func = SJ_CSharpUtil.CallStrFunc_Args( cn.inst_class , str_func , args );
                        return r;
                }
                return r;
        }

        public object Call_Normal( object call_obj , string str_class , string str_func )
        {
                SJ_CALL_RETURN_NORMAL r = new SJ_CALL_RETURN_NORMAL();
                _CASH_NORMAL cn = Find_Normal( call_obj , str_class , str_func );
                if( cn.mi != null )
                {
                        r.cn = cn;
                        r.return_func =  SJ_CSharpUtil.CallStrFunc_NoArg( cn.inst_class , str_func );
                }
                return r;
        }

        //=====================================================================
        // 유니티 전용
        public class _CASH_UNITY
        {
                public MethodInfo       mi;
                public Component        unity_class; 
        }

        // 일반 c# 클래스
        Dictionary<string,_CASH_UNITY> dic_unity = new Dictionary<string,_CASH_UNITY>();

        public _CASH_UNITY Find_Unity( GameObject game_obj , string str_class , string str_func )
        {
                string key = str_class + "_" + str_func;
                _CASH_UNITY inf = null;
                if( dic_unity.TryGetValue( key, out inf )  )
                {
                        return inf;
                }

                Component save_class = null;
                if( string.IsNullOrEmpty( str_class ) == false )
                {
                        System.Type componentType = System.Type.GetType( str_class );
                        if( componentType == null ) return null;
                        Component cp = game_obj.GetComponent( componentType );
                        if( cp == null )
                        {
                                cp = game_obj.AddComponent(componentType);
                        }
                        save_class = cp;
                }else{
                        // 없을수는 없다.
                        return null;
                }

                inf = new _CASH_UNITY();
                inf.mi = SJ_CSharpUtil.Get_MethodInfo_Inst( save_class , str_func );
                inf.unity_class = save_class;
                dic_unity[key] = inf;
                return inf;
        }

        public object Call_Unity( GameObject call_obj , string str_class , string str_func , object[] args )
        {
                SJ_CALL_RETURN_UNITY r = new SJ_CALL_RETURN_UNITY();
                _CASH_UNITY cn = Find_Unity( call_obj , str_class , str_func );
                if( cn.mi != null )
                {
                        r.cn = cn;
                        r.return_func = SJ_CSharpUtil.CallStrFunc_Args( cn.unity_class , str_func , args );
                }
                return r;
        }

        public object Call_Unity( GameObject call_obj , string str_class , string str_func )
        {
                SJ_CALL_RETURN_UNITY r = new SJ_CALL_RETURN_UNITY();
                _CASH_UNITY cn = Find_Unity( call_obj , str_class , str_func );
                if( cn.mi != null )
                {
                        r.cn = cn;
                        r.return_func =  SJ_CSharpUtil.CallStrFunc_NoArg( cn.unity_class , str_func );
                }
                return r;
        }

        //=====================================================================
}
