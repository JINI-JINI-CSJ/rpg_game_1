using UnityEngine;
using UnityEngine.UI;

public class Panel_DungeonMain : MonoBehaviour
{
    static public     Panel_DungeonMain G;
    public Text text_WorldTurn;

    public Text text_DungeonTurn;

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

    static public void Show( bool b )
    {
        G.gameObject.SetActive(b);
    }

    static public void OnEnd_PlayerMove(){G._OnEnd_PlayerMove();}
    public void _OnEnd_PlayerMove()
    {
        SJ_UnityUI_Util.TextString( text_WorldTurn , Player.TURN_WORLD.ToString() );
        SJ_UnityUI_Util.TextString( text_DungeonTurn , InGame.G.Turn_Dungeon.ToString() );
    }
}
