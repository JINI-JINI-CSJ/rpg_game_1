using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// 스택형태로 플레이어 인풋을 관리한다.

public class SJ_PlayerInputMng : MonoBehaviour
{
    static public SJ_PlayerInputMng G;

    [System.Serializable]
    public class _PLAYER_INPUT_OBJ
    {
        public GameObject go;
        public PlayerInput playerInput;

        public void Active(bool b)
        {
            playerInput.enabled = b;
        }
    }

    public List<_PLAYER_INPUT_OBJ> inputs = new List<_PLAYER_INPUT_OBJ>();

    void Awake()
    {
        G = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void _ActiveInput(GameObject go)
    {
        // 있으면 가장 높은 순위 및 나머지 비활성화
        PlayerInput playerInput = go.GetComponent<PlayerInput>();
        if (playerInput == null) return;
        _PLAYER_INPUT_OBJ find = inputs.Find(x => x.go == go);
        if (find != null)
        {
            //Debug.Log( "_ActiveInput : Remove : " + find.go.name ) ;
            inputs.Remove(find);
        }
        else
        {
            find = new _PLAYER_INPUT_OBJ();
            find.go = go;
            find.playerInput = playerInput;
        }

//        Debug.Log( "_ActiveInput : Add : " + find.go.name ) ;
        inputs.Add(find);
        _ActiveLast();
    }

    public void _ActiveLast()
    {
//        Debug.Log( "_ActiveLast : " + inputs.Count ) ;
        for (int i = 0; i < inputs.Count; i++)
        {
            if (i == inputs.Count - 1)
            {
                inputs[i].Active(true);

                SJ_Unity.SendMsg( inputs[i].go , "OnPlayerInputMng_ACTIVE" );
                //Debug.Log( "인풋매니저 : " + inputs[i].go.name + "/ true");

            }
            else
            {
                inputs[i].Active(false);
                SJ_Unity.SendMsg( inputs[i].go , "OnPlayerInputMng_HIDE" );
                //Debug.Log( "인풋매니저 : " + inputs[i].go.name + "/ false");
            }
        }
    }

    public void _RemoveInput(GameObject go)
    {
        PlayerInput playerInput = go.GetComponent<PlayerInput>();
        if (playerInput == null) return;
        _PLAYER_INPUT_OBJ find = inputs.Find(x => x.go == go);
        if (find != null)
        {
            //Debug.Log( "_RemoveInput : Remove : " + find.go.name ) ;
            inputs.Remove(find);
            find.Active(false);
        }
        
        _ActiveLast();
    }

    static public void ActiveInput(GameObject go)
    {
        if (G == null)
        {
            Debug.Log( "SJ_PlayerInputMng 객체 없다." );
            return;            
        }

        G._ActiveInput(go);
    }

    static public void RemoveInput(GameObject go)
    {
        if (G == null)
        {
            Debug.Log( "SJ_PlayerInputMng 객체 없다." );
            return;     
        }
        G._RemoveInput(go);
    }

    // 독점 모드 시작
    // 활성화 되있는 모든 인풋을 잠시 비활성화 , 인자 인풋만 제외 
    // 이미 독점 모드인데 또 하면 에러!!!
    // 현재 사용안함..
    PlayerInput self_input_Exclusive;
    List<PlayerInput> temp_Exclusive_list = new List<PlayerInput>();
    static public void ExclusiveMode_START( GameObject go_exc )
    {
        if( G.self_input_Exclusive != null )
        {
            Debug.LogError( "에러!!! 이미 독점 모드" );
            return;
        }

        G.self_input_Exclusive = go_exc.GetComponent<PlayerInput>();
        if( G.self_input_Exclusive == null )
        {
            Debug.LogError( "에러!!! 인풋 없음 : " + go_exc.name );
            return;
        }

        G.temp_Exclusive_list.Clear();
        PlayerInput[] inputs = GameObject.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach( var s in inputs )
        {
            if( s.enabled && s != G.self_input_Exclusive )
            {
                G.temp_Exclusive_list.Add( s );
                s.enabled = false;
            }
        }
        G.self_input_Exclusive.enabled = true;
    }

    static public void ExclusiveMode_END()
    {
        G.self_input_Exclusive.enabled = false;
        foreach( var s in G.temp_Exclusive_list )
        {
            s.enabled = true;
        }
    }
}
