using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

// 유니티 
//using UnityEngine;

public class SJ_CSharpUtil
{
        static public MethodInfo Get_MethodInfo_Inst(object obj, string func)
        {
                if (obj == null) return null;
                System.Type tp = obj.GetType();
                return tp.GetMethod(func, BindingFlags.Instance | BindingFlags.Public);
        }

        static public MethodInfo Get_MethodInfo_Static(System.Type tp, string func)
        {
                if (tp == null) return null;
                return tp.GetMethod(func, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        static public object Call_MethodInfo(MethodInfo mi, object obj, object[] args)
        {
                if (mi == null) return null;
                return mi.Invoke(obj, args);
        }

        static public object CallStrFunc_Args(object obj, string func, object[] args)
        {
                if (obj == null) return null;
                MethodInfo mi = Get_MethodInfo_Inst(obj, func);
                if (mi == null)
                {
                        //Debug.LogError( "에러!!! 못찾음 CallStrFunc : " + func );
                        return false;
                }
                return Call_MethodInfo(mi, obj, args);
        }

        static public object CallStrFunc(object obj, string func, object arg_1 = null)
        {
                if (obj == null) return null;
                return CallStrFunc_Args(obj, func, new object[] { arg_1 });
        }
        static public object CallStrFunc(MethodInfo mi, object obj, object arg_1 = null)
        {
                if (mi == null) return null;
                return Call_MethodInfo(mi, obj, new object[] { arg_1 });
        }



        static public object CallStrFunc_NoArg(object obj, string func)
        {
                if (obj == null) return null;
                return CallStrFunc_Args(obj, func, null);
        }

        static public object CallStrFunc_Args_Static(System.Type tp, string func, object[] args)
        {
                if (tp == null) return null;
                MethodInfo mi = Get_MethodInfo_Static(tp, func);

                if (mi == null)
                {
                        //Debug.LogError( "에러!!! 못찾음 CallStrFunc_Args_Static : " + func );
                        return false;
                }
                //return mi.Invoke(null , args);
                return Call_MethodInfo(mi, null, args);
        }

        static public object CallStrFunc_Static(System.Type tp, string func, object arg_1 = null)
        {
                if (tp == null) return null;
                return CallStrFunc_Args_Static(tp, func, new object[] { arg_1 });
        }

        static public object NewClass_Str(string str)
        {
                // 네임스페이스와 클래스명 함께
                //Type customerType = Type.GetType("MyNamespace.Customer");
                Type customerType = Type.GetType(str);
                if (customerType == null)
                {
                        return null;
                }
                // Type으로부터 클래스 객체 생성
                return Activator.CreateInstance(customerType);
        }

        static public object NewClass_Type(Type customerType)
        {
                return Activator.CreateInstance(customerType);
        }


        // 비트 연산
        // 저장소 , 왼쪽으로 쉬프트 할 숫자
        static public void Bit_Add(int save, int shift)
        {
                // shift 
                // 0 : 아무것도  안함
                // 1 : 숫자 1 더하기
                // 2 이상 : 1빼고 1을 n번 쉬프트
                int v_sh = 1;
                if (shift > 1) v_sh = (int)1 << (shift - 1);
                save = save | v_sh;
        }

        // 체크
        static public bool Bit_Check(int save, int shift)
        {
                int v_sh = 1;
                if (shift > 1) v_sh = (int)1 << (shift - 1);
                save = save & v_sh;
                if (save > 0) return true;
                return false;
        }


        // 리스트에서 원하는 인덱스 가져오기 
        // 마이너스 값 : 0 번째 것을 가져온다.
        // 범위 벗어남 : 마지막 것을 가져온다.
        static public T GetList_IndexSafe<T>(List<T> lt, int index)
        {
                if (lt == null || lt.Count < 1) return default(T);
                if (index < 0) return lt[0];
                if (index >= lt.Count) return lt[lt.Count - 1];
                return lt[index];
        }

        static public bool Add_Array<T>( T[] arr , T t )
        {
                for( int i  = 0 ; i < arr.Length ; i++ )
                {
                        if( arr[i] == null )
                        {
                                arr[i] = t;
                                return true;
                        }
                }
                return false;
        }

        static public void NULL_Index_Array<T>( T[] arr , int idx )
        {
                if( arr.Length <= idx ) return ;
                arr[idx] = default;
        }


        static public bool Remove_Array<T>( T[] arr , T t )
        {
                for( int i  = 0 ; i < arr.Length ; i++ )
                {
                        if( EqualityComparer<T>.Default.Equals(arr[i], t) )
                        {
                                arr[i] = default;
                                return true;
                        }
                }
                return false;
        }
}

