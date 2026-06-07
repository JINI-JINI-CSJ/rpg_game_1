using System.Collections;
using System.Collections.Generic;

// 게임에서 버프 객체 관리를 위한 기본 설정

// 기능 1. 태그 문자열 기능
// 기능 2. 참조한 캐릭터 플레이어 
// 기능 3. 중첩 가능한지 여부
// 기능 4. 우선순위
public class SJ_BaseTagStateObj
{
    public List<SJ_BaseTagStateMng> lt_par = new List<SJ_BaseTagStateMng>();
    virtual public string[]     OnTag_Add() {return null;}
    virtual public int          OnCountAble() {return 0;} // 0 무한
    virtual public int          OnPriority(){ return 0; }

    // OnCountAble 에서 걸려서 이미 존재할때... 
    virtual public void         OnAlready_Exist( SJ_BaseTagStateObj other ){}   

    public void     Remove()
    {
        foreach( SJ_BaseTagStateMng s in lt_par )
        {
            OnRemove( s );
            s.Remove_TagStateObjObj( this );
        }
        OnClear();
        
    }
    virtual public void OnClear(){}

    virtual public void OnAdd( SJ_BaseTagStateMng mng ){}
    virtual public void OnRemove( SJ_BaseTagStateMng mng ){}

    virtual public bool OnQuery( string str , object arg )
    {
        return false;
    }

    virtual public int OnQuery_INT( string str , object arg )
    {
        return 0;
    }
}


public class SJ_BaseTagStateMng
{
    // 가지고 있는 객체 , 예) 캐릭터
    // 상속 안하고 여기로 문자열 함수 호출
    public object   obj_have;

    // 기본
    // 클래스 타입으로 정렬
    public Dictionary<System.Type,HashSet<SJ_BaseTagStateObj>>  dic_type = new Dictionary<System.Type, HashSet<SJ_BaseTagStateObj>>();

    // 태그로 정렬
    public Dictionary<string,HashSet<SJ_BaseTagStateObj>>  dic_tag = new Dictionary<string, HashSet<SJ_BaseTagStateObj>>();


    public void     ClearALL()
    {
        dic_type.Clear();
        dic_tag.Clear();
    }

    public bool     Add_TagStateObjObj( SJ_BaseTagStateObj obj )
    {
        HashSet<SJ_BaseTagStateObj> hs = null;
        if( dic_type.TryGetValue( obj.GetType() , out hs ) == false )
        {
            hs = new HashSet<SJ_BaseTagStateObj>();
            dic_type[obj.GetType()] = hs;
        }

        if( obj.OnCountAble() < 1 || hs.Count <= obj.OnCountAble() )
        {
            obj.lt_par.Add( this );
            hs.Add(obj);
            obj.OnAdd( this );
            if( obj_have != null )
            {
                SJ_CSharpUtil.CallStrFunc( obj_have , "OnAdd_SJ_BaseTagStateObj" , obj );
            }
            Add_Obj_Tag( obj );

            return true;
        }else{
            // 이미 존재함
            foreach( SJ_BaseTagStateObj s in hs )
            {
                s.OnAlready_Exist( obj );
            }
        }

        return false;
    }



    public bool     Remove_TagStateObjObj(SJ_BaseTagStateObj obj  )
    {
        HashSet<SJ_BaseTagStateObj> hs = null;
        if( dic_type.TryGetValue( obj.GetType() , out hs ) == false )
        {
            return false;
        }

        if( obj_have != null )
        {
            SJ_CSharpUtil.CallStrFunc( obj_have , "OnRemove_SJ_BaseTagStateObj" , obj );
        }

        Remove_Obj_Tag( obj );

        hs.Remove( obj );
        return true;
    }

    public HashSet<SJ_BaseTagStateObj>  Get_List( System.Type t )
    {
        HashSet<SJ_BaseTagStateObj> hs = null;
        if( dic_type.TryGetValue( t , out hs )  )
        {
            return hs;
        }
        return null;
    }

    public List<SJ_BaseTagStateObj> Get_All()
    {
        List<SJ_BaseTagStateObj> lt = new List<SJ_BaseTagStateObj>();
        foreach( HashSet<SJ_BaseTagStateObj> s in dic_type.Values )
        {
            lt.AddRange(s);
        }
        return lt;
    }

    void     Add_Obj_Tag(SJ_BaseTagStateObj obj)
    {
        string[] tags = obj.OnTag_Add();
        if( tags != null )
        {
            foreach( string tag in tags )
            {
                HashSet<SJ_BaseTagStateObj> hs = null;
                if( dic_tag.TryGetValue( tag , out hs ) == false )
                {
                    hs = new HashSet<SJ_BaseTagStateObj>();
                    dic_tag[tag] = hs;
                }
                hs.Add( obj );                
            }
        }
    }

    void     Remove_Obj_Tag(SJ_BaseTagStateObj obj)
    {
        string[] tags = obj.OnTag_Add();
        if( tags != null )
        {
            foreach( string tag in tags )
            {
                HashSet<SJ_BaseTagStateObj> hs = null;
                if( dic_tag.TryGetValue( tag , out hs ) == false )
                {
                    return;
                }
                hs.Remove( obj );
            }
        }
    }


    public HashSet<SJ_BaseTagStateObj>  Get_List_Tag( string tag )
    {
        HashSet<SJ_BaseTagStateObj> hs = null;
        if( dic_tag.TryGetValue( tag , out hs )  )return hs;
        return null;
    }

    public List<SJ_BaseTagStateObj>  Get_List_Tag_Copy( string tag )
    {
        HashSet<SJ_BaseTagStateObj> hs = null;
        if( dic_tag.TryGetValue( tag , out hs )  )
        {
            return new List<SJ_BaseTagStateObj>( hs );
        }
        return new List<SJ_BaseTagStateObj>();
    }

    public int  Get_List_Tag_Count( string tag )
    {
        HashSet<SJ_BaseTagStateObj> hs = Get_List_Tag(tag);
        if( hs != null ) return hs.Count;
        return 0;
    }

    public bool  Get_List_Tag_BOOL( string tag )
    {
        if( Get_List_Tag_Count(tag) > 0 )return true;
        return false;
    }

    public List<SJ_BaseTagStateObj> Remove_All_ByTag( string tag )
    {
        List<SJ_BaseTagStateObj> lt = Get_List_Tag_Copy(tag);
        foreach( SJ_BaseTagStateObj s in lt )
        {
            s.Remove();
        }
        return lt;
    }

    public bool     Query( string str , object arg = null )
    {
        List<SJ_BaseTagStateObj> hs = Get_List_Tag_Copy( str );

        if( hs == null ) return false;
        bool b = false;

        foreach( SJ_BaseTagStateObj s in hs )
        {
            if( s.OnQuery( str , arg ) )
            {
                b = true;
            } 
        }
        return b;
    }
}
