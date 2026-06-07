
using System.Collections;
using System.Collections.Generic;

// 구독
public interface SJ_SubscribeObj 
{
    public void OnNotice_SubscribeObj( SJ_SubscribeMng par , object arg_obj = null , int arg_int = 0 , string arg_str = "" );
}
public class SJ_SubscribeMng
{
    public HashSet<SJ_SubscribeObj>     hs_obj = new HashSet<SJ_SubscribeObj>();

    public void Clear(){hs_obj.Clear();}
    public void RegSubscribeMng( SJ_SubscribeObj obj )
    { 
        hs_obj.Add( obj );
    }

    public void RemoveSubscribeMng( SJ_SubscribeObj obj ){ if( hs_obj.Contains( obj ) )hs_obj.Remove( obj );}
    public void Notice( object arg_obj = null , int arg_int = 0 , string arg_str = "" )
    {
        // 삭제 할수도 있으니 복사해서
        List<SJ_SubscribeObj> lt_copy = new List<SJ_SubscribeObj>( hs_obj );
        foreach( var item in lt_copy )
        {
            item.OnNotice_SubscribeObj( this , arg_obj , arg_int , arg_str  );
        }
    }
}


// 계산용 값
// 추가 값 , 보정값
public class SJ_PrcValue
{
    public int ID;
    public string ID_STR;
    public float src;
    public float last;
    public int last_int;
    public float add_int;
    public float fix_float;
    virtual public void LastCal()
    {
        last = (src + add_int) * (1.0f + fix_float);
        last_int = (int)last;
    }

    public void Clear()
    {
        fix_float = 0;
        add_int = 0;
        last_int = (int)src;
        last = src;
    }

    public void Clear_FIX_OBJ()
    {
        dic_fix.Clear();
        Clear();
    }


    public class SJ_VAL_FIX_OBJ
    {
        public object ref_obj_id;
        public float add;
        public float fix;
    }

    public Dictionary<object, SJ_VAL_FIX_OBJ> dic_fix = new Dictionary<object, SJ_VAL_FIX_OBJ>();

    public SJ_VAL_FIX_OBJ Find_Obj( object _ref_obj_id )
    {
        SJ_VAL_FIX_OBJ s = null;
        dic_fix.TryGetValue(_ref_obj_id, out s);
        return s;
    }

    public object Add_FIX_OBJ(float _add, float _fix, object _ref_obj_id = null)
    {
        if (_ref_obj_id == null)
        {
            // 아이디 구분용
            _ref_obj_id = new object();
        }
        SJ_VAL_FIX_OBJ s = new SJ_VAL_FIX_OBJ();

        s.ref_obj_id = _ref_obj_id;
        s.add = _add;
        s.fix = _fix;

        add_int += s.add;
        fix_float += s.fix;
        LastCal();

        dic_fix[_ref_obj_id] = s;

        return _ref_obj_id;
    }

    public bool Remove_FIX_OBJ(object _ref_obj_id)
    {
        SJ_VAL_FIX_OBJ s = null;
        if (dic_fix.TryGetValue(_ref_obj_id, out s))
        {
            //Debug.Log( " Remove_FIX_OBJ  A --> : " + fix_float + " : " + s.fix );
            add_int -= s.add;
            fix_float -= s.fix;
            LastCal();
            //Debug.Log( " Remove_FIX_OBJ  B <-- : " + fix_float + " : " + s.fix );
            dic_fix.Remove(_ref_obj_id);
            return true;
        }
        return false;
    }
    
    // 값 재설정
    public bool ReCalc_FIX_OBJ(object _ref_obj_id, float _add, float _fix)
    {
        SJ_VAL_FIX_OBJ s = null;
        if (dic_fix.TryGetValue(_ref_obj_id, out s))
        {
            // 기존 값 빼기
            add_int -= s.add;
            fix_float -= s.fix;

            // 새로 설정
            s.add = _add;
            s.fix = _fix;

            // 새로 추가
            add_int += s.add;
            fix_float += s.fix;

            LastCal();
            return true;
        }
        return false;
    }   

    // 객체아이디가 없으면 한번만 더하기
    public bool First_Add_FIX_OBJ(float _add, float _fix, object _ref_obj_id )
    {
        if( Find_Obj( _ref_obj_id ) == null )
        {
            Add_FIX_OBJ( _add , _fix , _ref_obj_id );
        }
        else
        {
            ReCalc_FIX_OBJ( _ref_obj_id , _add, _fix );
        }
        return true;
    }
}


public class SJ_PrcValueMng
{
    // 계산 값들
    // 아이디 , 값
    public Dictionary<int,SJ_PrcValue>      dic_id_SJ_PrcValue = new Dictionary<int, SJ_PrcValue>();
    public Dictionary<string,SJ_PrcValue>   dic_str_SJ_PrcValue = new Dictionary<string, SJ_PrcValue>();
    // 구독
    public SJ_SubscribeMng sj_SubscribeMng = new SJ_SubscribeMng();

    public void     DestroyValue()
    {
        dic_id_SJ_PrcValue.Clear();
        dic_str_SJ_PrcValue.Clear();
        sj_SubscribeMng.Clear();
    }

    public SJ_PrcValue     FindAlloc_SJ_PrcValue( int id )
    {
        SJ_PrcValue find = null;
        if( dic_id_SJ_PrcValue.TryGetValue( id , out find ) == false )
        {
            find = new SJ_PrcValue();
            dic_id_SJ_PrcValue[id] = find;
            find.ID = id;
        }
        return find;
    }

    public SJ_PrcValue     FindAlloc_SJ_PrcValue( string str )
    {
        SJ_PrcValue find = null;
        if( dic_str_SJ_PrcValue.TryGetValue( str , out find ) == false )
        {
            find = new SJ_PrcValue();
            dic_str_SJ_PrcValue[str] = find;
            find.ID_STR= str;
        }
        return find;
    }

    public void     SetValue( int id , float src  )
    {
        SJ_PrcValue find = FindAlloc_SJ_PrcValue(id);
        find.src = src;
        find.LastCal();

        sj_SubscribeMng.Notice( find );
    }

    public void     SetValue( string str , float src  )
    {
        SJ_PrcValue find = FindAlloc_SJ_PrcValue(str);
        find.src = src;
        find.LastCal();

        sj_SubscribeMng.Notice( find );
    }

    virtual public float    Value( int id )
    {
        SJ_PrcValue find = FindAlloc_SJ_PrcValue(id);
        return find.last;
    }

    virtual public float    Value( string str )
    {
        SJ_PrcValue find = FindAlloc_SJ_PrcValue(str);
        return find.last;
    }

    virtual public int    Value_Int( int id )
    {
        SJ_PrcValue find = FindAlloc_SJ_PrcValue(id);
        return find.last_int;
    }

    virtual public int    Value_Int( string str )
    {
        SJ_PrcValue find = FindAlloc_SJ_PrcValue(str);
        return find.last_int;
    }

    public Dictionary<object, SJ_PrcValue.SJ_VAL_FIX_OBJ> Find_ID_Dic(int id)
    {
        SJ_PrcValue prcValue = FindAlloc_SJ_PrcValue(id);
        return prcValue.dic_fix;
    }

    public SJ_PrcValue.SJ_VAL_FIX_OBJ Find_FIX_OBJ(int id, object obj)
    {
        SJ_PrcValue find = null;
        if (dic_id_SJ_PrcValue.TryGetValue(id, out find))
        {
            SJ_PrcValue.SJ_VAL_FIX_OBJ fo = null;
            if (find.dic_fix.TryGetValue(obj, out fo))
            {
                return fo;
            }
        }
        return null;
    }

    public SJ_PrcValue.SJ_VAL_FIX_OBJ Find_FIX_OBJ( string str , object obj )
    {
        SJ_PrcValue find = null;
        if( dic_str_SJ_PrcValue.TryGetValue( str , out find ) )
        {
            SJ_PrcValue.SJ_VAL_FIX_OBJ fo = null;
            if( find.dic_fix.TryGetValue( obj , out fo ) )
            {
                return fo;
            }
        }
        return null;
    }

    public SJ_PrcValue.SJ_VAL_FIX_OBJ Find_FIX_OBJ_RefClass(object obj)
    {
        foreach (SJ_PrcValue v in dic_id_SJ_PrcValue.Values)
        {
            SJ_PrcValue.SJ_VAL_FIX_OBJ fo = null;
            if (v.dic_fix.TryGetValue(obj, out fo))
            {
                return fo;
            }
        }

        foreach (SJ_PrcValue v in dic_str_SJ_PrcValue.Values)
        {
            SJ_PrcValue.SJ_VAL_FIX_OBJ fo = null;
            if (v.dic_fix.TryGetValue(obj, out fo))
            {
                return fo;
            }
        }
        return null;
    }

    public bool ADD_VAL_INF(int id, object obj, float val_add = 0, float val_fix = 0)
    {
        SJ_PrcValue find_val = FindAlloc_SJ_PrcValue(id);
        find_val.Add_FIX_OBJ(val_add, val_fix, obj);
        sj_SubscribeMng.Notice(find_val);
        return true;
    }

    public bool     ADD_VAL_INF( string str , object obj , float val_add = 0 , float val_fix = 0 )
    {
        SJ_PrcValue find_val = FindAlloc_SJ_PrcValue(str);
        find_val.Add_FIX_OBJ( val_add , val_fix , obj );
        sj_SubscribeMng.Notice( find_val );
        return true;
    }

    public void    REMOVE_VAL_INF_RefID( int id , object _ref_obj_id )
    {
        SJ_PrcValue find = null;
        if (dic_id_SJ_PrcValue.TryGetValue(id, out find))
        {
            if( find.Remove_FIX_OBJ(_ref_obj_id) )
                sj_SubscribeMng.Notice(find);
        }
    }
    
    public void    REMOVE_VAL_INF_RefID(  string str , object _ref_obj_id )
    {
        SJ_PrcValue find = null;
        if (dic_str_SJ_PrcValue.TryGetValue(str, out find))
        {
            if( find.Remove_FIX_OBJ(_ref_obj_id) )
                sj_SubscribeMng.Notice(find);
        }
    }

    public void REMOVE_VAL_INF_RefClass(object _ref_obj_id)
    {
        foreach (SJ_PrcValue v in dic_id_SJ_PrcValue.Values)
        {
            if (v.Remove_FIX_OBJ(_ref_obj_id))
            {
                sj_SubscribeMng.Notice(v);
            }
        }

        foreach (SJ_PrcValue v in dic_str_SJ_PrcValue.Values)
        {
            if (v.Remove_FIX_OBJ(_ref_obj_id))
            {
                sj_SubscribeMng.Notice(v);
            }
        }
    }
    
    public void ReCalc_FIX_OBJ_RefID(int id , object _ref_obj_id ,  float val_add, float val_fix)
    {
        SJ_PrcValue v = null;
        if (dic_id_SJ_PrcValue.TryGetValue(id, out v))
        {
            if (v.ReCalc_FIX_OBJ(_ref_obj_id, val_add, val_fix))
            {
                sj_SubscribeMng.Notice(v);
            }
        }
    }

    public void ReCalc_FIX_OBJ_RefID(string str , object _ref_obj_id, float val_add, float val_fix)
    {
        SJ_PrcValue v = null;
        if (dic_str_SJ_PrcValue.TryGetValue(str, out v))
        {
            if (v.ReCalc_FIX_OBJ(_ref_obj_id, val_add, val_fix))
            {
                sj_SubscribeMng.Notice(v);
            }
        }
    }
    
    public void ReCalc_FIX_OBJ_RefClass(object _ref_obj_id, float val_add, float val_fix)
    {
        foreach (SJ_PrcValue v in dic_id_SJ_PrcValue.Values)
        {
            if (v.ReCalc_FIX_OBJ(_ref_obj_id, val_add, val_fix))
            {
                sj_SubscribeMng.Notice(v);
            }
        }

        foreach (SJ_PrcValue v in dic_str_SJ_PrcValue.Values)
        {
            if (v.ReCalc_FIX_OBJ(_ref_obj_id, val_add, val_fix))
            {
                sj_SubscribeMng.Notice(v);
            }
        }
    }

    public bool First_Add_FIX_OBJ( int id , float _add, float _fix, object _ref_obj_id  )
    {
        SJ_PrcValue prcValue = FindAlloc_SJ_PrcValue( id );
        return prcValue.First_Add_FIX_OBJ( _add, _fix, _ref_obj_id );
    }

    public List<SJ_PrcValue.SJ_VAL_FIX_OBJ> GetAll_FixObj()
    {
        List<SJ_PrcValue.SJ_VAL_FIX_OBJ> res = new List<SJ_PrcValue.SJ_VAL_FIX_OBJ>();
        foreach (SJ_PrcValue v in dic_id_SJ_PrcValue.Values)
        {
            foreach (var fo in v.dic_fix.Values)
            {
                res.Add(fo);
            }
        }

        foreach (SJ_PrcValue v in dic_str_SJ_PrcValue.Values)
        {
            foreach (var fo in v.dic_fix.Values)
            {
                res.Add(fo);
            }
        }
        return res;
    }

    public void ClearALL_FIX_OBJ()
    {
        foreach (SJ_PrcValue v in dic_id_SJ_PrcValue.Values)
        {
            v.Clear_FIX_OBJ();
        }

        foreach (SJ_PrcValue v in dic_str_SJ_PrcValue.Values)
        {
            v.Clear_FIX_OBJ();
        }
    }

    public void All_ReUpdate()
    {
        sj_SubscribeMng.Notice();
    }

    public void Copy( SJ_PrcValueMng src )
    {
        dic_id_SJ_PrcValue = new Dictionary<int, SJ_PrcValue>( src.dic_id_SJ_PrcValue );
        dic_str_SJ_PrcValue = new Dictionary<string, SJ_PrcValue>( src.dic_str_SJ_PrcValue );
    }
}