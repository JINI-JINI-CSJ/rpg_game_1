using UnityEngine;

/// <summary>
/// 레시피로 만들어진 아이템
/// csv 가 없다 , 레시피 아이디가 있다.
/// </summary>
/// 

public enum RecipeITEMEff_TYPE
{
    None ,
    Heal ,      // 회복 계열 , 버프 등도 가능
    Attack ,    // 공격 계열 , 디버프도 가능
}

public class Item_MaterMixture : ItemBase
{
    public uint ID_Recipe;

    // 일단 여기서 회복 , 공격 기능하기
    // 그외 기능은 클래스
    public RecipeITEMEff_TYPE eff_TYPE;

    // 레시피에선 크게 회복 , 공격 타입 및 위력 또는 등급으로만 인자
    // 여기서 세부 기능을 만들자.

    public void MakeByRecipe( Recipe recipe )
    {
        
    }

}
