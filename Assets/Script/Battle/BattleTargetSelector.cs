using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 배틀 타겟 셀렉터
// 1인
// 1라인
// 전체

// 셀렉트 단위
public class BATTLE_SEL_GROUP
{
    public List<CharBase> chars = new();

    public void Add( CharBase ch )
    {
        chars.Add(ch);
    }

    public void AddRange( List<CharBase> lt )
    {
        chars.AddRange( lt );
    }

    public void ActiveTarget( bool b )
    {
        foreach( var s in chars )
        {
            s.Call_SelectTarget( b );
        }
    }
}

public class BattleTargetSelector : MonoBehaviour
{
    static public BattleTargetSelector G;

    public CursorDirectionInput cursorDirectionInput;

    public PlayerInput playerInput;

    public List<BATTLE_SEL_GROUP> sel_group = new();

    SJ_GridObjDir gridObjDir = new();

    BATTLE_SEL_GROUP bs_cur_select;

    public SJ_COMMON.Func_Arg func_SelectOK;
    public SJ_COMMON.Func_VOID func_SelectCancel;


    void Awake()
    {
        G = this;
        cursorDirectionInput.RegisterMoveX_One( InputCursor_X );
        cursorDirectionInput.RegisterMoveY_One( InputCursor_Y );
    }

    public void Active( bool b )
    {
        playerInput.enabled = b;
    }


    // 전투 바라보는 카메라 기준
    // 적군 뒷라인 부터 아군쪽으로 더하기 , ### 순서 중요!!! ###
    // 커서 위로 하면 적군 방향 , 
    // 아래로 하면 아군 방향

    // 특수 스킬 ( 부활 , 상태이상 회복 등등 )은 자체 스킬들이 하자

    static public List<BATTLE_SEL_GROUP> MakeSelectGroup( _ARMY_FORCE self_force , BATTLE_ACTION_TARGET at , bool live = true )
    {
        SJ_GridObjDir gridObjDir = G.gridObjDir;
        gridObjDir.Clear();
        List<BATTLE_SEL_GROUP> lt = G.sel_group;
        lt.Clear();

        BattleParty bp_self = BattleMain.GetBattleParty( self_force , true );
        BattleParty bp_opp = BattleMain.GetBattleParty( self_force , false );

        int x = 0,y = 0;
        switch( at )
        {
            case BATTLE_ACTION_TARGET.One_Opp_Front:
                {
                    List<CharBase> chars = bp_opp.GetBattleLive( true , false , live );
                    if( chars.Count == 0 )
                    {
                        chars = bp_opp.GetBattleLive( false , true , live );
                    }

                    foreach( var  s in chars )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.Add(s);
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );
                        x++;
                    }
                }
                break;
            case BATTLE_ACTION_TARGET.One_Opp_Back:
                {
                    foreach( var  s in bp_opp.GetBattleLive( false , true , live ) )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.Add(s);
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );
                        x++;
                    }
                }
                break;
            case BATTLE_ACTION_TARGET.One_Opp_ALL:
                {
                    foreach( var  s in bp_opp.GetBattleLive( true , true , live ) )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.Add(s);
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );
                        x++;
                    }
                }
                break;

            case BATTLE_ACTION_TARGET.Line_Opp_Front:
                {
                    // 전열 가져오기 , 전멸했다면 후열
                    List<CharBase> chars = bp_opp.GetBattleLive( true , false , live );
                    if( chars.Count == 0 )
                    {
                        chars = bp_opp.GetBattleLive( false , true , live );
                    }
                    BATTLE_SEL_GROUP bs = new();
                    bs.AddRange( chars );
                    lt.Add(bs);
                    gridObjDir.Add( x , y , bs );
                    
                }
                break;
            case BATTLE_ACTION_TARGET.Line_Opp_Back:
                {
                    // 전열 가져오기 , 전멸했다면 후열
                    List<CharBase> chars = bp_opp.GetBattleLive( false , true , live );
                    if( chars.Count == 0 )
                    {
                        chars = bp_opp.GetBattleLive( true , false , live );
                    }
                    BATTLE_SEL_GROUP bs = new();
                    bs.AddRange( chars );
                    lt.Add(bs);
                    gridObjDir.Add( x , y , bs );
                    
                }
                break;

            case BATTLE_ACTION_TARGET.Line_Opp_ALL:
                {
                    // 전후열 전부 ,  더하는 순서는 후열 먼저 
                    List<CharBase> chars_f = bp_opp.GetBattleLive( true , false , live );
                    List<CharBase> chars_b = bp_opp.GetBattleLive( false , true , live );

                    if( chars_b.Count > 0 )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.AddRange( chars_b );
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );    
                        y++;                    
                    }

                    if( chars_f.Count > 0 )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.AddRange( chars_f );
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );    
                        y++;                    
                    }
                    
                }
                break;


            // 아군
            case BATTLE_ACTION_TARGET.One_Self_Front:
                {
                    foreach( var  s in bp_self.GetBattleLive( true , false , live ) )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.Add(s);
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );
                        x++;
                    }
                }
                break;
            case BATTLE_ACTION_TARGET.One_Self_Back:
                {
                    foreach( var  s in bp_self.GetBattleLive( false , true , live ) )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.Add(s);
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );
                        x++;
                    }
                }
                break;
            case BATTLE_ACTION_TARGET.One_Self_ALL:
                {
                    foreach( var  s in bp_self.GetBattleLive( true , true , live ) )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.Add(s);
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );
                        x++;
                    }
                }
                break;

            case BATTLE_ACTION_TARGET.Line_Self_Front:
                {
                    // 전열 가져오기 , 전멸했다면 후열
                    List<CharBase> chars = bp_self.GetBattleLive( true , false , live );
                    if( chars.Count == 0 )
                    {
                        chars = bp_self.GetBattleLive( false , true , live );
                    }
                    BATTLE_SEL_GROUP bs = new();
                    bs.AddRange( chars );
                    lt.Add(bs);
                    gridObjDir.Add( x , y , bs );
                    
                }
                break;
            case BATTLE_ACTION_TARGET.Line_Self_Back:
                {
                    // 
                    List<CharBase> chars = bp_self.GetBattleLive( false , true , live );
                    if( chars.Count == 0 )
                    {
                        chars = bp_self.GetBattleLive( true , false , live );
                    }
                    BATTLE_SEL_GROUP bs = new();
                    bs.AddRange( chars );
                    lt.Add(bs);
                    gridObjDir.Add( x , y , bs );
                    
                }
                break;

            case BATTLE_ACTION_TARGET.Line_Self_ALL:
                {
                    // 전후열 전부 ,  더하는 순서는 전열 먼저 , 플레이어 입장이라서
                    List<CharBase> chars_f = bp_self.GetBattleLive( true , false , live );
                    List<CharBase> chars_b = bp_self.GetBattleLive( false , true , live );

                    if( chars_f.Count > 0 )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.AddRange( chars_f );
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );    
                        y++;                    
                    }

                    if( chars_b.Count > 0 )
                    {
                        BATTLE_SEL_GROUP bs = new();
                        bs.AddRange( chars_b );
                        lt.Add(bs);
                        gridObjDir.Add( x , y , bs );    
                        y++;                    
                    }
                    
                }
                break;

            case BATTLE_ACTION_TARGET.ALL:
                {
                    BATTLE_SEL_GROUP bs = new();
                    bs.AddRange( bp_self.GetALL() );
                    bs.AddRange( bp_opp.GetALL() );
                    gridObjDir.Add( x , y , bs );   
                }
                break;
        }
        return lt;
    }

    static public void SetCursor( BATTLE_SEL_GROUP bs )
    {
        G.gridObjDir.SetCursorByObj(bs);
        ActiveGroup();
    }

    static public void Show( bool b , SJ_COMMON.Func_Arg func_ok = null , SJ_COMMON.Func_VOID func_cancel = null )
    {
        G.enabled = b;
        G.playerInput.enabled = b;
        G.func_SelectOK = func_ok;
        G.func_SelectCancel = func_cancel;
    }

    static public void ActiveGroup()
    {
        foreach( var s in G.sel_group )
        {
            s.ActiveTarget(false);
        }

        G.bs_cur_select = G.gridObjDir.GetCursor() as BATTLE_SEL_GROUP;
        G.bs_cur_select.ActiveTarget(true);
    }

    static public BATTLE_SEL_GROUP MoveCursor( int x , int y )
    {
        G.gridObjDir.Move( x , y );
        ActiveGroup();
        return G.gridObjDir.GetCursor() as BATTLE_SEL_GROUP;
    }


    public void InputCursor_X( int off )
    {
        MoveCursor( off , 0 );
    }

    public void InputCursor_Y( int off )
    {
        MoveCursor( 0 , off );
    }

    // 상대 라인 차순 랜덤 1개체
    static public BATTLE_SEL_GROUP RandomTargetOpp_One( _ARMY_FORCE self_force )
    {
        BattleParty bp_opp = BattleMain.GetBattleParty( self_force , false );

        CharBase charBase = bp_opp.GetRandomLive();
        if( charBase == null ) return null;
        BATTLE_SEL_GROUP s = new();
        s.Add( charBase );
        return s;
    }

    //=========================================================================
    // 유니티 인풋
    public void OnNavigate(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        cursorDirectionInput.SetInput( v.x , v.y );
    }


    public void OnSubmit( InputValue value )
    {
        OnSelectOK();
    }

    public void OnCancel( InputValue value )
    {
        OnSelectCancel();
    }
    //
    //=========================================================================

    public void OnSelectOK()
    {
        playerInput.enabled = false;
        func_SelectOK?.Invoke(bs_cur_select);
    }

    public void OnSelectCancel()
    {
        playerInput.enabled = false;
        func_SelectCancel?.Invoke();
    }

}
