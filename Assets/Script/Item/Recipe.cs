using System.Collections.Generic;
using UnityEngine;

// 레시피
// 소재 개수 
// 최종 아이템 : 소모 아이템 , 장비 아이템?
// 일단 회복 , 공격 아이템으로만 하자.
public class Recipe
{

    // 재료 클래스   
    public class MatterMixture
    {
        public class PairMatter
        {
            public int num;
            public CSV_Item csv_obj; // csv 객체를 넣는다.
            public override int GetHashCode()
            {
                return num.GetHashCode() + csv_obj.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                PairMatter o = obj as PairMatter;
                return o != null && (o.num == this.num || o.csv_obj == this.csv_obj); 
            } 

            public string TagEq()
            {
                return csv_obj.ID_int.ToString() + "_" + num.ToString() + "-";
            }

            public int ScoreMatter()
            {
                return csv_obj.matter_grade * GTF_CSV.csv_Config.making_recipe_matter_grade_score * num;
            }
        }

        public List<PairMatter> pairMaters = new();
        bool hashCode_have = false;
        int hashCode = 0;

        public override int GetHashCode()
        {
            if( hashCode_have == false )
            {
                hashCode_have = true;
                hashCode = 0;
                foreach( var s in pairMaters )hashCode += s.GetHashCode();
            }
            return hashCode;
        }

    
        public string tag_eq = "";

        public string MakeTagEq()
        {
            if( string.IsNullOrEmpty(tag_eq) )
            {
                foreach( var s in pairMaters ) tag_eq += s.TagEq();
            }
            return tag_eq;
        }

        public override bool Equals(object obj)
        {
            MatterMixture o = obj as MatterMixture;
            if( MakeTagEq() == o.MakeTagEq() ) return true;
            return false;
        }


        public void AddCSV_RandomNum( Mng_X128SS rd , CSV_Item csv , int max_num )
        {
            PairMatter s = new();
            s.csv_obj = csv;
            s.num = rd.NextInt( 1 , max_num );
            pairMaters.Add(s);
        }

        // 점수계산
        // 전체 레시피 점수 범위
        // 최소 : 최소 등급재료 2개의 1씩
        // 최대 : 최대 등급재료 2개의 최대개수 (5)
        // 등급 재료 점수 : 등급당 n 점수 , 좀 크게 잡는다.

        public int TotalMatterScore()
        {
            int t = 0;
            foreach( var s in pairMaters ) t += s.ScoreMatter();
            return t;
        }

        public float TotalMatterScore_Ratio()
        {
            return (float)TotalMatterScore() / (float)GTF_CSV.csv_Config.MaxRecipeMatterScore();
        }
    }

    public uint ID;

    // 레시피 등급
    public int grade_recipe;

    // 재료들
    public MatterMixture materMixture;

    public RecipeITEMEff_TYPE recipeITEM;

    public Item_MatterMixture item_MatterMixture;

    // 효과 , 아이템을 만들자.
    // 실제효과는 Item_MaterMixture 에서 하낟.
    public void MakeItem( Mng_X128SS rd , int _grade_recipe , uint id , MatterMixture _materMixture , RecipeITEMEff_TYPE _recipeITEM )
    {
        ID = id;
        grade_recipe = _grade_recipe;
        materMixture = _materMixture;
        recipeITEM = _recipeITEM;
        item_MatterMixture = new();
        item_MatterMixture.MakeByRecipe( rd , this );
    }
    
}


// 모든 레시피 관리
public class RecipeGroupMng
{
    public const int MATTER_GRADE_TO_RECIPE_GRADE = 2;

    // 레시피 재료 태그 , 레시피
    // 주인공의 레시피는 주인공 클래스에 있다.
    static public Dictionary<string , Recipe> dic_Recipe = new();

    // 모든 레시피 만들기
    // 현재 수집아이템 희소등급 기준으로 만들기
    // 규칙 1:
    // 제일 낮은 등급부터 시작 
    // 희소등급당 n 단계 : 예) 현재 4단계 희소등급 , 2단계씩 , 총 8단 레시피 등급
    // 기준 조합법 
    // 1. 종류정하기 : 일단 2개씩만 하자. 희소등급이랑 레시피 등급이랑 같은경우 같은 희소등급 재료 2개 , 중간이라면 양쪽 1개씩 가져오기
    // 2. 개수 정하기 : 1 ~ n개 
    // 3. 점수 계산 : 각 재료의 희소 등급 * 재료 갯수 

    // 위의 규칙대로 음식 , 전투 소모 아이템 만들기
    // 효과는 Item_MaterMixture 에서 만든다.
    static public void Make()
    {
        // 음식 , 소모 아이템 2파트 만든다.
        MakePart( RecipeITEMEff_TYPE.Defense );
        MakePart( RecipeITEMEff_TYPE.Offense );
    }

    // 0   1   2   3
    // 0 1 2 3 4 5 6 
    // 마지막은 제외
    static public int Total_RecipeGrade()
    {
        return GTF_CSV.csv_Config.making_item_collect_per.Count * MATTER_GRADE_TO_RECIPE_GRADE - 1;
    }

    static public void MakePart( RecipeITEMEff_TYPE recipeITEM )
    {
        Dictionary<int,List<CSV_Item>> dic_matter = null;

        if( recipeITEM == RecipeITEMEff_TYPE.Defense )
        {
            dic_matter = Make_ItemUnique.G.dic_grade_Items_MATTER_COOKING;
        }
        else
        {
            dic_matter = Make_ItemUnique.G.dic_grade_Items_MATTER_DISPENSE;
        }

        // 총 레시피 등급 계산
        // 일단 고정으로 희소등급당 2개 , 일단 고정
        // 현재 재료 희소 등급 4개다.         
        int total_RecipeGrade = Total_RecipeGrade();

        // 정등급이면 동등급 재료 , 반 등급이면 양쪽 
        // 0   1   2   3
        // 0 1 2 3 4 5 6 
        // 마지막 등급은 1개 빼기 , 반등급 위가 없게.
        for( int i = 0 ; i < total_RecipeGrade  ; i++ )
        {
            Recipe.MatterMixture materMixture = new();

            int cur_grade_matter = i / MATTER_GRADE_TO_RECIPE_GRADE;
            int next_grade_matter = cur_grade_matter+1;

            // 일단 지정개수만큼 시도하기
            // 중복되면 그냥 실패로 진행
            for( int j = 0 ;  j < GTF_CSV.csv_Config.making_recipe_grade_num ; i++ )
            {
                // 재료 등급 인덱스 0 ~ n (현재는 3)
                // 레시피 등급으로 나눠봐서 나머지 값 생기면 반등급
                if( (i % MATTER_GRADE_TO_RECIPE_GRADE) > 0 )
                {
                    // 윗 반등급
                    // 현등급 1 , 윗등급 1
                    CSV_Item csv_1 = GTF_Random.rd_make_common.RandomList( dic_matter[cur_grade_matter] );
                    CSV_Item csv_2 = GTF_Random.rd_make_common.RandomList( dic_matter[next_grade_matter] );

                    materMixture.AddCSV_RandomNum( GTF_Random.rd_make_common , csv_1 , GTF_CSV.csv_Config.making_recipe_matter_max );
                    materMixture.AddCSV_RandomNum( GTF_Random.rd_make_common , csv_2 , GTF_CSV.csv_Config.making_recipe_matter_max );
                }
                else
                {
                    // 현 등급 2
                    CSV_Item csv_1 = GTF_Random.rd_make_common.RandomList( dic_matter[cur_grade_matter] );
                    CSV_Item csv_2 = GTF_Random.rd_make_common.RandomList( dic_matter[cur_grade_matter] );

                    if( csv_1.ID_int == csv_2.ID_int ) continue; // 그냥 넘기기

                    materMixture.AddCSV_RandomNum( GTF_Random.rd_make_common , csv_1 , GTF_CSV.csv_Config.making_recipe_matter_max );
                    materMixture.AddCSV_RandomNum( GTF_Random.rd_make_common , csv_2 , GTF_CSV.csv_Config.making_recipe_matter_max );
                }                

                if( dic_Recipe.ContainsKey( materMixture.MakeTagEq() ) )
                {
                    continue;
                }

                Recipe recipe = new();
                recipe.MakeItem( GTF_Random.rd_make_common , i , MakingMain.Make_UID( typeof(Recipe) ) , materMixture , recipeITEM );

                dic_Recipe[materMixture.MakeTagEq()] = recipe;
            }
        }
    }

}