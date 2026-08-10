using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public int MAX = 10;

    // csv 아이디 , 아이템

    // 소비품
    public Dictionary<int,ItemBase> dic_item_consume = new();

    // 장비 , 유니크
    public Dictionary<int, HashSet<ItemBase> > dic_item_One = new();

    public ItemBase Add( int csv_id , int count = 1  )
    {
        CSV_Item csv = GTF_CSV.csv_ItemPage_ALL.Find_Int( csv_id ) as CSV_Item;

        if( csv.item_type == _ITEM_TYPE.Consume )
        {
            Add_Consume( csv_id , count );
        }
        else if( csv.item_type == _ITEM_TYPE.Equip )
        {
            ItemBase item = null;
            for( int i = 0 ; i < count ; i++ )
            {
                item = Add_OneItem( csv_id );
            }
            return item;
        }
        else if( csv.item_type == _ITEM_TYPE.Unique )
        {
            
        }
        
        return null;
    }


    public ItemBase Add_Consume( int csv_id , int count = 1 )
    {
        ItemBase item = null;
        if( dic_item_consume.TryGetValue( csv_id , out item ) == false )
        {
            item = ItemBase.InstItemBase( csv_id );
            item.count = 0;
            dic_item_consume[csv_id] = item;
        }
        item.count += count;
        return item;
    }

    public bool Remove_Consume( int csv_id , int count = 1 , bool only_check = false )
    {
        ItemBase item = null;
        if( dic_item_consume.TryGetValue( csv_id , out item ) == false )
        {
            return false;
        }
        if( item.count < count ) return false;
        if( only_check ) return true;
        item.count -= count;
        return true;
    }

    public ItemBase Add_OneItem( int csv_id )
    {
        HashSet<ItemBase> hs = null;
        if( dic_item_One.TryGetValue( csv_id , out hs ) == false )
        {
            hs = new();
            dic_item_One[csv_id] = hs;
        }
        ItemBase item = ItemBase.InstItemBase( csv_id );
        if( item == null ) return null;
        hs.Add( item );
        return item;
    }

    public bool Remove_OneItem( ItemBase item )
    {
        HashSet<ItemBase> hs = null;
        if( dic_item_One.TryGetValue( item.csv.ID_int , out hs ) == false ) return false;
        hs.Remove(item);
        return true;
    }
}
