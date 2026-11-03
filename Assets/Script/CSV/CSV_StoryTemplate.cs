using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 임무의 스토리 템플릿 
/// 단편 장편 등등 
/// - 장편 : 임무의 진행도에 의해 제목과 설명이 바뀐다.
/// -- 예 : 
///     ==> 제목 :  ->전해내려오는 이야기 ->어떤 유물에 대한 오래된 이야기 -> 어떤 검의 이야기 -> 듀랜달의 이야기 -> 듀랜달
///     ==> 개요 :  -> 어떤 마을에서 촌장의 이야기를 들었다. 아주 오래된 이야기라 한다.
///                 -> 어느 유물에 대한 이야기라는 소문을 얻었다.
///                 -> 유물이 검 종류라는 걸 알았다.
///                 -> 듀랜달 이라는 이름의 검 유물이라는걸 알았다.
///                 -> 듀랜달은 어느 지역에 있는거 같다.
///                 
/// 진행비율 또는 진행 단계별로 갱신한다.
/// 위의 예시는 단계 
/// 비율 갱신은 소문 정보의 수집량에 따라서 하자.
/// 비율로 전부 하자.  
/// 
/// [스토리 템플릿 개수] / [연퀘 단계 총 개수]  = 진행 비율
/// 위의 계산법으로 계산하면 된다.
/// 
/// 예) 약 10개 필수 정보라면 ==>  -> 1. 알수 없는 소문   -> 2. 어떤 검에 대한 소문   -> 3. 듀랜달에 대한 소문  -> 4. 듀랜달 이야기  -> 5. 듀랜달
/// 
/// 연퀘라면 루트 미션객체가 템플릿 아이디 및 진행도를 체크하여 
/// 
/// == 단어 채우기==
/// 탬플릿에선 순차 번호를 하고 , 각 미션에서 번호에 해당하는 것들을 반환하자.
/// 가능하면 인덱스 번호마다 해당 종류를 정하자.
/// 예)
/// [#1] -> 첫번째 대표 명사 ( 듀랜달 )
/// [#2] -> 두번째 종류 명사 (검)
/// [#3] -> 기타 등등
/// 
/// </summary>

// 단일 제목 개요
// 이것들을 묶는 그룹이 따로 있다.
public class CSV_StoryTemplate : SJ_CSV_BaseObj
{
    // 이름 , 개요
    //..

    // 템플릿 아이디 또는 태그 
    // 같은 스토리는 똑같은 태그 아이디
    public string   tagIDStroy;

    // 순차번호 0 시작 
    // 반드시 순차대로 해야 한다.!!!!
    // csv 순서도 위에서 아래로 순서대로 하자.
    public int      indexRatio;
    // 미션 타입 , 또는 태그 
    public string   tagMissionType;    
    // 제목 : 현지화 아이디 
    public string   titleLANG_ID;
    // 개요
    public string   descLANG_ID;

    public override void OnRead(SJ_CSV_BasePage _par, string[] _strs)
    {
        base.OnRead(_par, _strs);

        Next();
        Next();
        tagIDStroy = Next();
        indexRatio = Next_Int();
        tagMissionType = Next();        
        titleLANG_ID = Next();
        descLANG_ID = Next();
    }
}


// 같은 템플릿 아이디끼리 묶은거
public class CSV_StoryTemplate_Group
{
    public string   tagIDStroy;
    public List<CSV_StoryTemplate> cSV_s = new();
    public bool Add( CSV_StoryTemplate csv )
    {
        if( csv.tagIDStroy == tagIDStroy )
        {
            cSV_s.Add(csv);
            return true;
        }
        return false;
    }

    // 진행 비율로 반환
    // 인자로 받은 비율값보다 바로 이전 같거나 작은값의 인덱스
    public CSV_StoryTemplate GetByRatio( float ratio )
    {
        // 살짝 크게 해서 이전 인덱스가 나오도록
        ratio += 0.001f; 
        for( int i = 0 ; i < cSV_s.Count ; i++ )
        {
            float cur_r = (float)i / (float)cSV_s.Count;
            if( cur_r > ratio )
            {
                int idx = i - 1;
                if( idx < 0 ) idx = 0;
                return cSV_s[idx];
            }
        }
        return cSV_s[cSV_s.Count-1];
    }
}


public class CSV_StoryTemplatePage : SJ_CSV_BasePage
{
    public Dictionary<string,CSV_StoryTemplate_Group> dic_csvGroup = new();

    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_StoryTemplate();
    }

    public override void LoadAfter()
    {
        foreach( var s in dic_int.Values.Cast<CSV_StoryTemplate>() )
        {
            CSV_StoryTemplate_Group group = null;
            if( dic_csvGroup.TryGetValue( s.tagIDStroy , out group ) == false )
            {
                group = new();
                group.tagIDStroy = s.tagIDStroy;
                dic_csvGroup[s.tagIDStroy] = group;
            }
            group.Add(s);
        }

    }
}