using UnityEngine;

public class CSV_ConteStory : SJ_CSV_BaseObj
{

}

public class CSV_ConteStoryPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_ConteStory();
    }
}