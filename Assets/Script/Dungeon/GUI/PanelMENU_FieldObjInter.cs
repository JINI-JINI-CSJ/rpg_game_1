using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMENU_FieldObjInter : MonoBehaviour
{
    static public PanelMENU_FieldObjInter G;

    public class _MENU
    {
        public int ID;        
        public SJ_LANG_ID sJ_LANG_ID;
    }
    public List<_MENU> mENUs = new();

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
        
    }

    static public void Clear_MENU()
    {
        G.gameObject.SetActive(true);
        G.mENUs.Clear();
        foreach( var s in G._BUTTONs )s.go_BT.SetActive(false);
    }

    static public void ADD_MENU( SJ_TileCoordBase _fieldObj , string lang_part , string lang_word , int ID )
    {
        G.fieldObj = _fieldObj;
        int idx_cur = G.mENUs.Count;
        SJ_LANG_ID sJ_LANG_ID = new();
        sJ_LANG_ID.part = lang_part;
        sJ_LANG_ID.word = lang_word;
        _MENU menu = new();
        menu.ID = ID;
        menu.sJ_LANG_ID = sJ_LANG_ID;

        _BUTTON_OBJ bt = G._BUTTONs[idx_cur];
        bt.text.text = SJ_Language.Str( sJ_LANG_ID );
        bt.go_BT.SetActive(true);
    }


    public void OnClick_BT( int idx )
    {
        _MENU menu = mENUs[idx];
        fieldObj.OnInteract( menu.ID );
        gameObject.SetActive(false);
    }

}
