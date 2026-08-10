using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 필드 상호 작용 
public class PanelMENU_FieldObjInter : MonoBehaviour
{
    static public PanelMENU_FieldObjInter G;

    public class _MENU
    {
        public int ID;        
        public SJ_LANG_ID sJ_LANG_ID;
    }
    public List<_MENU> menus_add = new();

    [System.Serializable]
    public class _BUTTON_OBJ
    {
        public Text text;
        public GameObject go_BT;
    }
    public List<_BUTTON_OBJ> _BUTTONs;

    SJ_TileCoordBase fieldObj;
     

    void Awake()
    {
        G = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    static public void Clear_MENU()
    {
        G.gameObject.SetActive(true);
        G.menus_add.Clear();
        foreach( var s in G._BUTTONs )s.go_BT.SetActive(false);
    }

    static public void ADD_MENU( SJ_TileCoordBase _fieldObj , string lang_part , string lang_word , int CommonID  )
    {
        G.fieldObj = _fieldObj;
        int idx_cur = G.menus_add.Count;
        SJ_LANG_ID sJ_LANG_ID = new();
        sJ_LANG_ID.part = lang_part;
        sJ_LANG_ID.word = lang_word;
        _MENU menu = new();
        menu.ID = CommonID;
        menu.sJ_LANG_ID = sJ_LANG_ID;
        G.menus_add.Add(menu);

        _BUTTON_OBJ bt = G._BUTTONs[idx_cur];
        bt.text.text = SJ_Language.Str( sJ_LANG_ID );
        bt.go_BT.SetActive(true);
    }


    public void OnClick_BT( int idx )
    {
        Debug.Log( "OnClick_BT : " + idx );

        _MENU menu = menus_add[idx];
        fieldObj.OnInteract( menu.ID );
        gameObject.SetActive(false);
    }

}
