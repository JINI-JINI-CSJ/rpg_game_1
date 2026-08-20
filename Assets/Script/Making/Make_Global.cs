using System.Linq;
using UnityEngine;



public class Make_Global : MakeBase
{
    static public Make_Global G;

    //===========================================
    // 총 도시 , 총 국가    
    [System.Serializable]
    public class _CITY_MAKE_NUM
    {
        public int total_city = 200;
        public int total_nation = 6;
        public float per_Major = 0.1f;
        public float per_Minor = 0.3f;      // 중도시
        public float per_Village = 0.6f;    // 소도시

        public int Num_Major(){return (int)( (float)total_city * per_Major);}
        public int Num_Minor(){return (int)( (float)total_city * per_Minor);}
        public int Num_Village(){return (int)( (float)total_city * per_Village);}
    }
    public _CITY_MAKE_NUM city_make_num = new();

    // 도시 특수 태그
    public StockMakeInf stock_CitySpcTag = new();



    // 일반 , 특수 도시 비율  : 일반 도시 비율 기준으로 
    public _BIAS_TWO_VAL bias_city_normal_spc = new();

    // 



    //===========================================
    // 유니크 캐릭터
    // 등급별 갯수


    //===========================================
    // 유니크 아이템
    // 등급별 갯수


    void Awake()
    {
        G = this;
    }

    override public void OnMake()
    {
        stock_CitySpcTag.AddRange( GTF_CSV.csv_TagDefinePage.GetTagPart_Str( "TAG_TRIBE" ) );
    }

    public float BiasCityNormalSpc()
    {
        return bias_city_normal_spc.Random( GTF_Random.rd_make_world );
    }
}
