using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public int MAX = 10;

    // csv 아이디 , 아이템

    // 소비품
    public Dictionary<int,ItemBase> dic_item_consume = new();

    // 그외 장비 , 유니크 등등
    public Dictionary<int,ItemBase> dic_item_normal = new();

    public void Add_Consume( int csv_id , int count = 1 )
    {
        ItemBase item = null;
        //if(  )
    }


}
