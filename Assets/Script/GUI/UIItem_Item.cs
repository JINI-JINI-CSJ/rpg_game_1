using UnityEngine;
using UnityEngine.UI;

public class UIItem_Item : MonoBehaviour
{
    public Image image;
    public Text text_Name;
    public Text text_Desc;
    public Text text_Count;

    ItemBase itemBase;
    CSV_Item csv;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInit( object arg )
    {
        itemBase = arg as ItemBase;
        csv = arg as CSV_Item;
        if( itemBase != null )
        {
            csv = itemBase.csv;
            SetCSV();
            SetItemBase();
        }
        else if( csv != null )
        {
            SetCSV();
        }
    }

    public void SetCSV()
    {
        SJ_UnityUI_Util.Image_Load( image , "2D/ITEM/" + csv.res );
        SJ_UnityUI_Util.TextString( text_Name , csv.GetName() );
        SJ_UnityUI_Util.TextString( text_Name , csv.GetDesc() );
        if( text_Count != null )text_Count.gameObject.SetActive(false);
    }

    public void SetItemBase()
    {
        if( text_Count != null )text_Count.gameObject.SetActive(true);
        SJ_UnityUI_Util.TextString( text_Count , itemBase.count.ToString() );
    }
}
