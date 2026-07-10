using UnityEngine;

// ``ID	이름	설명	리소스	태그 부서	클래스																					
public class CSV_OfficeDepartment : SJ_CSV_BaseObj
{

}

public class CSV_OfficeDepartmentPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_OfficeDepartment();
    }
}