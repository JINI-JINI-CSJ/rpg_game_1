using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AIPlayer : MonoBehaviour
{
    //
    public  SJ_AINode   ai_root;

    // json
    SimpleJSON.JSONClass     jdata = new SimpleJSON.JSONClass();
    public  void    ValueInt_Set( string name , int val ){jdata[name].AsInt = val;}
    public  int     ValueInt_Get( string name ) 
    {
        return jdata[name].AsInt;
    }
    public  void    ValueInt_Add( string name , int val ){jdata[name].AsInt += val;}

    public  void    ValueFloat_Set( string name , float val ){jdata[name].AsFloat = val;}
    public  float   ValueFloat_Get( string name ) {return jdata[name].AsFloat;}
    public  void    ValueFloat_Add( string name , float val )
    {
        float v = jdata[name].AsFloat;
        jdata[name].AsFloat = v + val;
    }

    public  void    ValueStr_Set( string name , string val ){jdata[name] = val;}
    public  string  ValueStr_Get( string name ) {return jdata[name];}

    public  SimpleJSON.JSONClass    GetJsonValue()  {return jdata;}
    
    public  void    Clear_Json(){jdata.ClearNode();}

    public  SJ_TagBaseMng   playerTag = new SJ_TagBaseMng();

    public  SJ_PropReg      propReg = new SJ_PropReg();

    //
    public  Animation   ani_root;

    public  void    PlayAni( string clip_name , float time = -1 )
    {
        
    }

    public  void    PlayAIName(string name)
    {
        bool r = ai_root.Play_Name( name );
        if(r == false)
        {
            Debug.LogError( "PlayAIName : 못찾음 : " + name );
        }
    }


    static  public  void    StopNode_PlayAI_Name( GameObject go , string name )
    {
        SJ_AINode node = go.GetComponentInParent<SJ_AINode>();
        if( node != null )
        {
            node.Stop_AI();
        }

        SJ_AIPlayer player = go.GetComponentInParent<SJ_AIPlayer>();
        if( player == null )
        {
            Debug.LogError( "PlayAI_Name : 못찾음 : player == null" );
            return;
        }
        player.PlayAIName(name);
    }

}
