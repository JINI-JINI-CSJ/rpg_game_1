using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

// CSV 전역 관리자 베이스 클래스
public class SJ_CSV_Mng
{

	static public SJ_CSV_Mng g_base;

    static  public  int     load_count = 0;
    static  public  object  lock_load = new object();
    static  public  int     load_state = 0;
    static  public  bool    load_start;    
    public          string  url_Base;
    public          string  url_Base_Lang;
    
    static public bool load_Complete = false;

    static public int LOAD_FILE_OR_URL = 1; // 0 파일 , 1 온라인

	public class _SEAT_NAME
    {
        public SJ_CSV_BasePage page;
        public string name;
        public bool int_str;
        public bool lang;
    }
	public List<_SEAT_NAME>		lt_SEAT_NAME = new List<_SEAT_NAME>();


    static public object recv_object;
    static public string recv_func;


    static public async Task LoadURL(System.Type class_self, object obj = null, string func = "", bool log = false )
    {
        if( g_base == null )
        {
            g_base = SJ_CSharpUtil.NewClass_Type( class_self ) as SJ_CSV_Mng;
        }

        if(log)g_base.OnLog("로드 단계 1 ");
        recv_object = obj;
        recv_func = func;

        if (load_start)
        {
            g_base.OnLog("이미로드!! ");
            SJ_CSharpUtil.CallStrFunc_NoArg(recv_object, recv_func);
            return;
        }

        if(log)g_base.OnLog("로드 단계 2 ");

        load_start = true;
        load_state = 1;
        g_base.OnAdd_CSVUrlList();

        if(log)g_base.OnLog("로드 단계 3 ");

        await g_base.Load_GoogleSeat();

        if(log)g_base.OnLog("로드 단계 4 ");
        // g_base.OnLoadAfter();
        // SJ_CSharpUtil.CallStrFunc(recv_object, recv_func);
        // g_base.OnLog("로드 단계 5 ");
    }

    // 번역일경우 page 인자 null , lang = true
    public void Add_CSVName(SJ_CSV_BasePage _page , string seat , bool int_str , bool lang = false)
	{
		_SEAT_NAME s = new _SEAT_NAME();
		s.page = _page;
		s.name = seat;
		s.int_str = int_str;
		s.lang = lang;
		lt_SEAT_NAME.Add( s );
        OnLog("Add_CSVName : " + seat);
	}

	// 상속된 클래스에서 여기 함수에다가
	// 위 함수를 나열
    virtual	public void OnAdd_CSVUrlList(){}


    public   async Task    Load_GoogleSeat()
    {
        //Debug.Log( "csv 로딩 시작" ); 

        //=========================================================================================
        // 온라인 로딩 비동기

		foreach( _SEAT_NAME s in lt_SEAT_NAME )
		{
            load_SeatAsync(s.page , s.name  , s.int_str , s.lang); // 일부러 await 안함
		}

        //=========================================================================================
    }

    public async Task load_SeatAsync( SJ_CSV_BasePage seat , string seat_name , bool id_int_str , bool lang )
    {
        // lock(lock_load)
        // {
        //     load_count++;
        // }

        //Debug.Log( seat_name );
        OnLog("로드 : " + seat_name);

        string url = url_Base;
        //if( seat_name.Contains( "번역" ) )
		if(lang)
        {
            url = url_Base_Lang;
        }

        url+= seat_name;
        HttpClient httpClient = new HttpClient();

        using HttpResponseMessage response = await httpClient.GetAsync(url);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        OnLog( seat_name + "\n"+ jsonResponse);

        if (response.IsSuccessStatusCode)
        {   
            // Omitted for brevity...
            if( seat != null )
            {
                OnLog( "시트 : "  + seat.GetType() );
                OnLog( "데이터 : "  + jsonResponse );
                seat.LoadCSV_Text( jsonResponse , seat_name , id_int_str );
            }
            else
            {
                OnLog( "번역 데이터 : " + jsonResponse );
                SJ_Language.Load_StringData_Text( jsonResponse );
            }
        }else{

        }

        load_count++;

        OnLog( "***** 로드 완료: [" + seat_name + "] : " + load_count + "     total : " + lt_SEAT_NAME.Count + " *****" );

        if( load_count == lt_SEAT_NAME.Count )
        {
            foreach( _SEAT_NAME s in lt_SEAT_NAME )
            {
                if( s.page == null ) continue;
                OnLog( "LoadAfter ---->>> : " + s.page.ToString() );

                try
                {
                    s.page.LoadAfter();
                }
                catch (System.Exception)
                {
                    OnLog( "에러!!!! 로드 애프터 : " + s.page.ToString() );
                    //throw;
                }

                
                OnLog( "LoadAfter <<<---- : " + s.page.ToString() );
            }

            load_Complete = true;
            g_base.OnLog("로드 단계  최종 완료~~~~~~~~~~~~~~~~~~~");
            g_base.OnLoadAfter();
            SJ_CSharpUtil.CallStrFunc_NoArg(recv_object, recv_func);
            
        }
    }

    // 일반 파일 로드
    virtual	public void OnAdd_CSVFileList(){}

    static public   void    LoadFile( System.Type class_self ,  object obj = null, string func = "" )
    {
        if( g_base == null )
        {
            g_base = SJ_CSharpUtil.NewClass_Type( class_self ) as SJ_CSV_Mng;
        }

        recv_object = obj;
        recv_func = func;

        if (load_start)
        {
            g_base.OnLog("이미로드!! ");
            SJ_CSharpUtil.CallStrFunc_NoArg(recv_object, recv_func);
            return;
        }

        //g_base.OnLog("로드 단계 2 ");
        g_base.OnAdd_CSVFileList();

        load_start = true;

        foreach( _SEAT_NAME s in g_base.lt_SEAT_NAME )
		{
            string text_data = Resources.Load<TextAsset>( "CSV/" + s.name ).text;

            Debug.Log( "CSV 로드 : " + s.name + " / " + text_data );
            if( s.page != null )
            {
                
                s.page.LoadCSV_Text( text_data , s.name , s.int_str );
            }
            else
            {
                SJ_Language.Load_StringData_Text( text_data );
            }
            g_base.OnLog("파일 로드 : " + s.name);
        }
        load_Complete = true;
        g_base.OnLoadAfter();
        SJ_CSharpUtil.CallStrFunc_NoArg(recv_object, recv_func);
    }


    static public void Load( System.Type class_self, object obj = null, string func = "" , bool log = false )
    {
        if( LOAD_FILE_OR_URL == 0 )
        {
            LoadFile( class_self, obj , func );
        }
        else
        {
            LoadURL( class_self, obj , func , log );
        }
    }

    virtual public   void   OnLoadAfter(){}

    virtual public      void  OnLog( string str )
    {
        Debug.Log( str );
    }
}
