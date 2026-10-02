using UnityEngine;

/// <summary>
/// 레시피로 만들어진 아이템
/// csv 가 없다 , 레시피 아이디가 있다.
/// </summary>
/// 

public enum RecipeITEMEff_TYPE
{
    None ,
    Defense ,      // 회복 계열 , 버프 등도 가능
    Offense ,    // 공격 계열 , 디버프도 가능
}

public class Item_MatterMixture : ItemBase
{
    public uint ID_Recipe;

    // 일단 여기서 회복 , 공격 기능하기
    // 그외 기능은 클래스
    public RecipeITEMEff_TYPE eff_TYPE;

    // 레시피에선 크게 회복 , 공격 타입 및 위력 또는 등급으로만 인자
    // 여기서 세부 기능을 만들자.
    // 기타 기능은 상속

    // 회복 , 공격  수치값
    public int val_pow;
    public BATTLE_ACTION_TARGET bat;

    public enum Defense_TYPE
    {
        None , 
        HP_Heal ,
        MP_Heal ,
        Bad_State_Heal ,
        Buff , 
        MAX ,
    }

    public Defense_TYPE defense_TYPE;

    public enum Offense_TYPE
    {
        None ,
        HP_Atk , 
        MP_Atk , 
        Bad_State_Atk ,
        Debuff ,
        MAX 
    }

    public Offense_TYPE offense_TYPE;

    virtual public void MakeByRecipe( Mng_X128SS rd , Recipe recipe )
    {
        if( recipe.recipeITEM == RecipeITEMEff_TYPE.Defense )
        {
            Make_Defense( rd );
        }
        else
        {
            Make_Offense( rd );
        }
    }


    // 타겟 타입 : 1체 , 1라인 , 전체  
    // 위력 : 1 , 1/3 , 1/6 
    void Make_Defense( Mng_X128SS rd )
    {
        // 세부 추가 기능은 여기서 클래스 만들자. 공격도 마찬가지
        // 일단 위에 정의한것들은 여기서 해자

        
    }

    //
    void Make_Offense( Mng_X128SS rd )
    {
        
    }

    override public BATTLE_ACTION_TARGET GetTargetType()
    {
        return bat;
    }

    override public void Action( BATTLE_SEL_GROUP sel_group )
    {
        
    }
}
