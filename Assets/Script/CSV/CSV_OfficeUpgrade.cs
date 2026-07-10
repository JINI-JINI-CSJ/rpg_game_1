using UnityEngine;

// ``ID	이름	설명	주 사무실 0 , 부서 1	레벨	가격	총 부서 슬롯갯수(주사무실)
public class CSV_OfficeUpgrade : SJ_CSV_BaseObj
{

}

public class CSV_OfficeUpgradePage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_OfficeUpgrade();
    }
}