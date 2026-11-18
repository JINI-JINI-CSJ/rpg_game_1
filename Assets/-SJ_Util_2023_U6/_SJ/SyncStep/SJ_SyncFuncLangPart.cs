using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SJ_Language 에서 메타 데이터로 자동으로 순차 실행
/// 시작 방법 PART 고정 및 숫자 아이디 1 부터 시작
/// -SJ_LANG_ID 
///  -> part : 고정
///  -> id : 1 부터 순차
/// </summary>
/// 
/// 주의!!!!!!!!!!!!!!
/// 대화 유닛과 같은 행에 있는 함수는 절대 "Next" 호출 금지!!! 또는 대화가 없을때만 Next 호출
/// 일단 대화 캐릭에 동시에 나오는 이미지 일러스트 함수는 제외하자
/// 
public class SJ_SyncFuncLangPart 
{
    static public SJ_SyncFuncLangPart G;

    // 기본 메세지 함수    
    public delegate void OnFunc_LangStr( string msg );
    public OnFunc_LangStr onFunc_LangStr;

    // 종료 함수
    public SJ_COMMON.Func_VOID func_end;

    // 메타 함수
    // meta_1 은 함수명
    public delegate void OnFunc_LangMETA( string meta_2 , string meta_3 );

    // 각 메타 함수 실행할 것들 , 첫번째 인자로 등록을 한다.
    public Dictionary<string , OnFunc_LangMETA > dic_OnFunc_LangMETA = new();

    // 위의 등록 없이 호출할 객체
    public object obj_call_default;



    public SJ_SyncFuncLangPart()
    {
        G = this;
    }

    static public void RegFunc_MsgBase( OnFunc_LangStr func )
    {
        if( G != null )
            G.onFunc_LangStr = func;
    }

    public void _RegFunc( string func_name , OnFunc_LangMETA func )
    {
        dic_OnFunc_LangMETA[func_name] = func;
    }

    static public void RegFunc( string func_name , OnFunc_LangMETA func )
    {
        G?._RegFunc( func_name , func );
    }

    // 처음 실행
    // 일단 태그만 받는다.
    // 받으면 1~ 최대치 까지 강제로 찾아서 순차 등록한다.
    // 등록할때 반드시 시작 태그 및 1부터 순차 증가
    // 그냥 id 인덱스로 시작하고 , 없으면 종료

    string  tag_cur;
    int     idx_cur;
    public void _StartTagMeta( string tag , int idx = 1)
    {
        tag_cur = tag;
        idx_cur = idx;
        _NextExec();
    }

    static public void StartTagMeta( string tag , int idx = 1 )
    {
        Debug.Log( "StartTagMeta : " + tag );
        G?._StartTagMeta( tag , idx );
    }


    public void _NextExec()
    {
        string lang_local = SJ_Language.Str( tag_cur , idx_cur );
        SJ_Language.META_DATA meta = SJ_Language.MetaData( tag_cur , idx_cur );
        idx_cur++;

        Debug.Log( "_NextExec : " + tag_cur + " : " + idx_cur + " : " + lang_local );

        if( string.IsNullOrEmpty( lang_local ) == false )
        {
            onFunc_LangStr?.Invoke( lang_local );
        }

        if( meta != null )
        {
            meta.Debug_Meta();
            // 메타 1 : 함수 
            // 2,3 : 인자
            OnFunc_LangMETA funcMETA = null;
            if( dic_OnFunc_LangMETA.TryGetValue( meta.meta_1 , out funcMETA ) )
            {
                funcMETA.Invoke( meta.meta_2 , meta.meta_3 );
            }
            else
            {
                // 에러 !!! 함수 없음
                //Debug.LogError( "함수없음!! : " + meta.meta_1 );

                if( obj_call_default != null )
                {
                    SJ_CSharpUtil.CallStrFunc_Args( obj_call_default , meta.meta_1 , meta.Arg23() );
                }
                else
                {
                    Debug.LogError( "함수없음!! : " + meta.meta_1 );
                }

            }
        }

        if(  string.IsNullOrEmpty( lang_local ) && meta == null )
        {
            func_end?.Invoke();
            return;
        }

    }

    static public void NextExec()
    {
        G._NextExec();
    }

}
