using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AINode : SJ_StepMode_Mono
{
    public  SJ_AIPlayer     player;

    public  SJ_AINode       par_ai;

    public  List<SJ_AINode> chiles_ai;

    public  string  AI_Name;

    // 랜덤으로 활성화 할때 확률값
    public  int     AI_Percent;

    public  bool    useChildPercent;

    public  SJ_TagBaseMng   eventTag = new SJ_TagBaseMng();

    public  List<SJ_TagBaseObj_Mono>    lt_event = new List<SJ_TagBaseObj_Mono>();

    public  void    Start_AI()
    {
        eventTag.ClearAll();
        foreach( SJ_TagBaseObj_Mono s in lt_event )s.Add( eventTag );
        Start_ListMode();
    }

    public  void    Stop_AI( bool child = true )
    {
        eventTag.ClearAll();
        StopPlay();
        foreach(SJ_AINode s in chiles_ai )
        {
            s.Start_AI(); 
        }
    }

    override public  void    OnStop()
    {
        eventTag.ClearAll();
        if( par_ai != null ) par_ai.Notice_ChildEnd(this);
    }

    public  void    FuncCall( int tag , int evt_int = 0 , string evt_str = "" , Dictionary<string,object> args = null )
    {
        if(play) 
            eventTag.FuncCall( tag , evt_int , evt_str , args );

        foreach( SJ_AINode s in chiles_ai )
        {
            s.FuncCall(tag , evt_int , evt_str , args);
        }
    }

    override public  void    Update_Func()
    {
        base.Update_Func();
        if( play == false ) return;
        foreach(SJ_AINode s in chiles_ai )
        {
            s.Update_Func(); 
        }
    }

    public  bool    Play_Name( string name , bool child = true  )
    {
        if( AI_Name == name )
        {
            Start_AI();
            return true;
        }
        if( child == false ) return false;
        foreach( SJ_AINode s in chiles_ai )
        {
            if(s.Play_Name(name , child) ) return true;
        }
        return false;
    }

    public  void    PlayChild_Percent()
    {
        List<int>   lt = new List<int>();
        foreach( SJ_AINode s in chiles_ai )
        {
            lt.Add( s.AI_Percent );
        }

        int idx =    SJ_Unity.Random_RangeStepList( lt.ToArray() );
        chiles_ai[idx].Start_AI();
    }


    public  void    Notice_ChildEnd( SJ_AINode node )
    {
        if( useChildPercent )
        {
            PlayChild_Percent();
        }
    }


}
