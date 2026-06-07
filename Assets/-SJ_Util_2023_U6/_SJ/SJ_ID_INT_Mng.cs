using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SimpleJSON;


public interface SJ_InterfaceSerialization
{
    // 인스턴스 아이디는 각자 멤버 변수
    // .. uint id

    //public string   GetSRClassName();

    public uint     GetUID();

    public void     To_Serialization( BinaryWriter bw , int version );
    public void     From_Serialization( BinaryReader br , int version );
    public void     From_Serialization_After(int version);


}

// 현재는 아이디 증가된 값만 저장
[System.Serializable]
public class SJ_ID_INT_Mng : SJ_InterfaceSerialization
{
    static public SJ_ID_INT_Mng G;

    // public struct _NAME_ID
    // {
    //     public string   name;
    //     public uint     id;
    // }

    [System.NonSerialized]
    public Dictionary<System.Type,uint>  dic_UID = new Dictionary<System.Type, uint>();
    //public List<_NAME_ID>           SZ_lt_dic_UID = new List<_NAME_ID>(); // 시리얼

    [System.NonSerialized]
    public Dictionary<System.Type,Dictionary<uint,SJ_InterfaceSerialization>> dic_inst = new Dictionary<System.Type,Dictionary<uint,SJ_InterfaceSerialization>>();

    //// 그냥 SJ_InterfaceSerialization 에서 클래스 이름까지 저장 
    //public List<SJ_InterfaceSerialization> SZ_lt_inst = new List<SJ_InterfaceSerialization>();


    //// 직렬화 저장용
    public string   GetSRClassName(){return "";}
    public uint     GetUID(){return 0;}

    static public void Create()
    {
        if( G !=null ) return;
        G = new SJ_ID_INT_Mng();
    }




     public void To_Serialization(BinaryWriter bw , int version )
    {
        bw.Write( dic_UID.Count );
        // 아이디 저장
        foreach( KeyValuePair<System.Type,uint> kv in dic_UID )
        {
            bw.Write( kv.Key.ToString() );
            bw.Write( kv.Value );
        }

    }


    public void From_Serialization(BinaryReader br , int version )
    {
        dic_UID.Clear();

        int c = br.ReadInt32();

        for( int i = 0 ; i < c ; i++ )
        {
            string key = br.ReadString();
            uint val_id = br.ReadUInt32();

            System.Type t = Type.GetType(key);

            if( t == null )
            {

                return;
            }

            dic_UID[t] = val_id;
        }

    }

    public void     From_Serialization_After( int version )
    {
        // foreach( KeyValuePair<string , Dictionary<uint,SJ_InterfaceSerialization>> kv1 in dic_inst )
        // {
        //     foreach(  KeyValuePair<uint,SJ_InterfaceSerialization> kv2 in kv1.Value )
        //     {
        //         kv2.Value.From_Serialization_After(version);
        //     }
        // }
    }


    public uint Make_UID( System.Type type )
    {
        uint val = 0;
        if( dic_UID.TryGetValue( type , out val ) == false ) // 최신 아이디
        {
            //dic[part] = val;
        }
        // 1 부터 시작 , 0 없음
        val++; 
        dic_UID[type] = val;
        return val;
    }

    public void AddInst( SJ_InterfaceSerialization obj )
    {
        Dictionary<uint,SJ_InterfaceSerialization> d_id_inst = null;
        if( dic_inst.TryGetValue( obj.GetType() , out d_id_inst ) == false)
        {
            d_id_inst = new Dictionary<uint,SJ_InterfaceSerialization>();
            dic_inst[obj.GetType() ] = d_id_inst;
        }
        d_id_inst[obj.GetUID()] = obj;
    }

    public void RemoveInst( SJ_InterfaceSerialization obj )
    {
        Dictionary<uint,SJ_InterfaceSerialization> d_id_inst = null;
        if( dic_inst.TryGetValue( obj.GetType() , out d_id_inst ) )
        {
            d_id_inst.Remove( obj.GetUID() );
        }
    }

    public SJ_InterfaceSerialization Find( System.Type type , uint id )
    {
        Dictionary<uint,SJ_InterfaceSerialization> d_id_inst = null;
        if( dic_inst.TryGetValue( type , out d_id_inst ) )
        {
            SJ_InterfaceSerialization obj = null;
            if( d_id_inst.TryGetValue( id, out obj ) )
            return obj;
        }
        return null;
    }

    //-------------------------------------------
    // 저장 유틸
    static public void  To_Serialization_LIST<T>( List<T> lt , BinaryWriter bw , int version )
    {
        if( lt == null )
        {
            bw.Write((int)0);
            return;
        }

        bw.Write( lt.Count );
        foreach ( T t in lt )
        {
            SJ_InterfaceSerialization sji = t as SJ_InterfaceSerialization;
            sji.To_Serialization( bw , version );
        }
    }

    static public List<SJ_InterfaceSerialization>  From_Serialization_LIST( System.Type type , BinaryReader br , int version , bool reg = true )
    {
        List<SJ_InterfaceSerialization> list = new List<SJ_InterfaceSerialization>();
        int c = br.ReadInt32();
        for( int i = 0; i < c; i++ )
        {
            SJ_InterfaceSerialization inst = SJ_CSharpUtil.NewClass_Type( type ) as SJ_InterfaceSerialization;       
            inst.From_Serialization( br , version );
            list.Add( inst );

            if( reg )
            {
                G.AddInst( inst );
            }
        }
        return list;
    }

    static public void     From_Serialization_After_LIST<T>( List<T> lt , int version)
    {
        foreach ( T t in lt )
        {
            SJ_InterfaceSerialization sji = t as SJ_InterfaceSerialization;
            sji.From_Serialization_After(  version );
        }
    }

    static public void To_Serialization_UID<T>( List<T> lt , BinaryWriter bw )
    {
        bw.Write( lt.Count );
        foreach ( T t in lt )
        {
            SJ_InterfaceSerialization sji = t as SJ_InterfaceSerialization;
            bw.Write( sji.GetUID() );
        }
    }

    static public List<uint>  From_Serialization_UID( BinaryReader br  )
    {
        List<uint> lt = new List<uint>();
        int c = br.ReadInt32();
        for( int i = 0; i < c; i++ )
        {
            lt.Add( br.ReadUInt32() );
        }
        return lt;
    }

}
