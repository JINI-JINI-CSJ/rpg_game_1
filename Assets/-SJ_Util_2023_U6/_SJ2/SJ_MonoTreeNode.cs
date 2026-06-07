using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 메뉴를 위한 모노 클래스

public class SJ_MonoTreeNode : MonoBehaviour
{
    [HideInInspector]
    public SJ_MonoTreeNode root;
    [HideInInspector]
    public SJ_MonoTreeNode par_tree;
    public List<SJ_MonoTreeNode> treeNodes;
    //int cur_select = -1;    
    
    bool cur_select_self;

    // 4 방향 연결
    [System.Serializable]
    public class _DIR_OTHER
    {
        public SJ_MonoTreeNode N_MENU;
        public SJ_MonoTreeNode S_MENU;
        public SJ_MonoTreeNode W_MENU;
        public SJ_MonoTreeNode E_MENU;

        public SJ_MonoTreeNode Get( int x , int y )
        {
            if( y == -1 ) return N_MENU;
            if( y ==  1 ) return S_MENU;
            if( x == -1 ) return W_MENU;
            if( x ==  1 ) return E_MENU;
            return null;
        }
    }
    public _DIR_OTHER dir_other;

    public void InitRoot()
    {
        root = this;
        InitNode(root);
        OnFocusSelf( true );
        OnFocusSelf( true );
    }

    public void InitNode( SJ_MonoTreeNode _root )
    {
        foreach( var s in treeNodes )
        {
            s.root = _root;
            s.par_tree = this;
            s.InitNode(_root);
        }
    }

    // 커서가 이동
    public void FocusChild( int idx = -1 )
    {
        SJ_MonoTreeNode node = null;
        for( int i = 0 ; i < treeNodes.Count ; i++ )
        {
            if( idx == i )  node = treeNodes[i];
            else            treeNodes[i].OnFocusSelf(false);
        }

        if( node != null ) node.OnFocusSelf(true);
    }

    public void FocusChild( int x , int y )
    {
        SJ_MonoTreeNode node = dir_other.Get( x , y );
        foreach( var s in treeNodes )
        {
            if( s != node ) s.OnFocusSelf(false);
        }
        if( node != null ) node.OnFocusSelf(true);
    }

    virtual public void OnFocusChild( SJ_MonoTreeNode node ){}

    // 이 메뉴를 완전히 선택
    public void OpenChildFocus( bool all_hide = false )
    {
        // 본인은 포커스를 잃는다.
        OnFocusSelf( false );

        SJ_MonoTreeNode node = null;
        for( int i = 0 ; i < treeNodes.Count ; i++ )
        {
            if( treeNodes[i].cur_select_self && all_hide == false ) 
            {
                node = treeNodes[i];
            }
            else  
                treeNodes[i].OnOpenSelf(false);
        }

        if( node != null )
        {
            node.OnFocusSelf(true);
            node.OnOpenSelf(true);
        }
        OnOpenChildFocus( node );
    }

    virtual public void OnOpenChildFocus( SJ_MonoTreeNode node_child ){}

    public void OpenChildFocus( int idx )
    {
        OnFocusSelf( false );

        SJ_MonoTreeNode node = null;

        for( int i = 0 ; i < treeNodes.Count ; i++ )
        {
            if( i == idx ) 
            {
                node = treeNodes[i];
            }
            else  
                treeNodes[i].OnOpenSelf(false);
        }
        if( node != null )
        {
            node.OnFocusSelf(true);
            node.OnOpenSelf(true);
        }

        OnOpenChildFocus( node );
    }

    // 상위로 
    public void OpenParent()
    {
        if( par_tree == null ) return;

        // 자식 다 비활성
        FocusChild();
        OpenChildFocus(true);
        par_tree.OnFocusSelf(true);
        OnOpenParent( par_tree );
    }

    virtual public void OnOpenParent( SJ_MonoTreeNode par_node ){}

    virtual public void OnFocusSelf( bool b )
    {
        cur_select_self = b;
    }
    virtual public void OnOpenSelf( bool b )
    {
        FocusChild(0);
    }



}
