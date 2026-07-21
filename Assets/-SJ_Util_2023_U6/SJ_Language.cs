using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class SJ_LANG_ID
{
	public string 	DESC;	

	public string 	part;
	public string 	word;
	public int 		id;

	public string DEBUG_LOG()
	{
		return "[PART : " + part + "   WORD : " + word + "    ID : " + id + "    DESC : " + DESC + "]";
	}
}

public class SJ_Language
{
	static	public	int		Total_Lang = 0;
	static	public	int		select_Lang = 0;
	static	public	int		read_CSV_Line = 0;
	static	public	string	read_Recent_Tag;

	// 파트 , 아이디 , 데이터
	static  public Dictionary<string, Dictionary< int , List<string>>>	dic_Data_int = new Dictionary<string, Dictionary<int, List<string>>>();
	// 파트 , 워드 , 데이터
	static  public Dictionary<string, Dictionary< string , List<string>>>	dic_Data_str = new Dictionary<string, Dictionary<string, List<string>>>();

	// 메타 데이터
	static public Dictionary<string , Dictionary<int,string> > dic_Data_int_meta = new Dictionary<string, Dictionary<int, string>>();
	static public Dictionary<string , Dictionary<string,string> > dic_Data_str_meta = new Dictionary<string, Dictionary<string, string>>();

	// 언어 코드 -> 인덱스
	static public List<string> CODE_TO_IDX = new List<string>();


	static public int Find_LangCodeIndex( string code )
    {
		for( int i = 0 ; i < CODE_TO_IDX.Count ; i++ )
        {
            if( CODE_TO_IDX[i] == code ) return i;
        }
		return -1;
    }

	static	public	void	Load_StringData_Text( string str_Data )
	{
																	
		SJ_CSV_Read.OpenCSV_Text( str_Data );

		read_Recent_Tag = "";
		Load_StringData_OpenedCSV();
	}

	static	public	int	Load_StringData_OpenedCSV( bool debug = false )
	{
		try
		{

			int load_count = 0;
			int base_col = 5; // 기본 데이터(아이디 , 메타 데이터 등등)
			read_Recent_Tag = "";
			while(true)
			{
				if( read_CSV_Line < 1 )
				{
					// 5 + lang 
					// 기본 데이터(아이디 , 메타 데이터 등등) + 언어 갯수
					read_CSV_Line = base_col + Total_Lang;
				}

				// `` 분류,	ID_Num,	ID_Str,	주석 ,언어들..		
				List<string> strs = SJ_CSV_Read.Read_Line(read_CSV_Line );

				if( strs == null || strs.Count < 5 ) break;


				string tag = strs[0];
				string str_id = strs[1];
				string str_word = strs[2];
				string str_meta = strs[4];

				if( string.IsNullOrEmpty(tag) )continue;

				List<string> list_word = new List<string>();
				for( int i = base_col ; i < strs.Count ; i++ )list_word.Add( strs[i] );

				// id 
				int id_n = -1;
				if( int.TryParse( str_id , out id_n ) )
				{
					Dictionary<int , List<string> > dic_sub = null;
					if( dic_Data_int.TryGetValue( tag , out dic_sub ) == false )
					{
						dic_sub = new Dictionary<int, List<string>>();
						dic_Data_int[tag] = dic_sub;
					}
					dic_sub[id_n] = list_word;


					Dictionary<int,string> dic_sub_meta = null;
					if( dic_Data_int_meta.TryGetValue( tag , out dic_sub_meta ) == false )
					{
						dic_sub_meta = new Dictionary<int,string>();
						dic_Data_int_meta[tag] = dic_sub_meta;
					}
					dic_sub_meta[id_n] = str_meta;

				}else{
					
					if( string.IsNullOrEmpty( str_word ) )
					{
						str_word = list_word[0];
					}


					Dictionary<string , List<string> > dic_sub = null;
					if( dic_Data_str.TryGetValue( tag , out dic_sub ) == false )
					{
						dic_sub = new Dictionary<string, List<string>>();
						dic_Data_str[tag] = dic_sub;
					}
					dic_sub[str_word] = list_word;				


					Dictionary<string,string> dic_sub_meta = null;
					if( dic_Data_str_meta.TryGetValue( tag , out dic_sub_meta ) == false )
					{
						dic_sub_meta = new Dictionary<string,string>();
						dic_Data_str_meta[tag] = dic_sub_meta;
					}
					dic_sub_meta[str_word] = str_meta;
				}

				load_count++;
			}
			SJ_CSV_Read.CloseCSV();
			return load_count;			
		}
		catch (System.Exception e)
		{
			Debug.LogError( e.Message );
			throw;
		}

	}

	static public List<string> Str( List<SJ_LANG_ID> sJ_LANG_IDs )
	{
		List<string> msgs = new();
		foreach( var s in sJ_LANG_IDs )
		{
			msgs.Add( Str( s ) );
		}
		return msgs;
	}

	static public string Str( SJ_LANG_ID sJ_LANG_ID )
	{
		if( string.IsNullOrEmpty( sJ_LANG_ID.word ) == false )
		{
			return Str( sJ_LANG_ID.part , sJ_LANG_ID.word );
		}
		else if( sJ_LANG_ID.id > 0 )
		{
			return Str( sJ_LANG_ID.part , sJ_LANG_ID.id );
		}
		return sJ_LANG_ID.DEBUG_LOG();
	}

	static public	string	Str( string part  , int id ,  params string[] args_rp  )
	{
		string str = "";
		Dictionary<int , List<string>> dic_sub = null;
		if( dic_Data_int.TryGetValue( part , out dic_sub ) )
		{
			List<string> lt = null;
			if( dic_sub.TryGetValue( id , out lt ) )
			{
				if( select_Lang < lt.Count )
					str = lt[select_Lang];
			}
		}

		// if( string.IsNullOrEmpty(str) )
		// {
		// 	if( string.IsNullOrEmpty(default_word) == false )
		// 	{
		// 		return default_word;
		// 	}
		// 	else
		// 	{
		// 		return str;
		// 	}
		// }

		str = STR_ReplaceParams( str , args_rp );
		return str;
	}

	static public	string	Str( string part  , string word ,  params string[] args_rp  )
	{
		//return Str(part , 0 , word);
		string str = "";
		try
		{
			Dictionary<string , List<string>> dic_sub = null;
			if( dic_Data_str.TryGetValue( part , out dic_sub ) )
			{
				List<string> lt = null;
				if( dic_sub.TryGetValue( word , out lt ) )
				{
					if( select_Lang < lt.Count )
						str = lt[select_Lang];
				}
			}			
		}
		catch (System.Exception)
		{
			//Debug.Log( "part : " + part + "        word : " + word );
			//throw;
		}

		// if( string.IsNullOrEmpty(str) )
		// {
		// 	if( string.IsNullOrEmpty(default_word) == false )
		// 	{
		// 		return default_word;
		// 	}
		// 	else
		// 	{
		// 		return str;
		// 	}
		// }

		str = STR_ReplaceParams( str , args_rp );
		return str;
	}

	// [#1] , [#2] .... 순서대로 가변 인자.
	static public string  STR_ReplaceParams( string msg , params string[] args_rp  )
	{
        for( int i = 0 ; i < args_rp.Length ; i++ )
        {
            string mark_rp = "[#" + (i+1).ToString() + "]";
            msg = msg.Replace( mark_rp , args_rp[i] );
        }
		return msg;
	}

	static public List<string> STR_RangID( string part , int start_id , int end_id )
	{
		List<string> msgs = new();
		for( int i = start_id ; i <= end_id ; i++ )
		{
			string msg = Str( part , i );
			if( string.IsNullOrEmpty(msg) == false ) msgs.Add(msg);
		}
		return msgs;
	}

	static public string MetaData( string part  , int id  )
	{
		Dictionary<int , string> dic_sub = null;
		if( dic_Data_int_meta.TryGetValue( part , out dic_sub ) )
		{
			string meta = "";
			if( dic_sub.TryGetValue( id , out meta ) )
			{
				return meta;
			}
		}
		return "";
	}	

	static public string MetaData( string part  , string word  )
	{
		Dictionary<string , string> dic_sub = null;
		if( dic_Data_str_meta.TryGetValue( part , out dic_sub ) )
		{
			string meta = "";
			if( dic_sub.TryGetValue( word , out meta ) )
			{
				return meta;
			}
		}
		return "";
	}	

	// [#n] 태그 인자 치환 , 1 부터 시작
	static public	string	Str_Arg( string part  , string word , params string[] args )
	{
		string str = Str( part  , word );
		int num = 1;	
		foreach( var s in args )
		{
			str = str.Replace( "[#"+num+"]" , s );
			num++;
		}
		return str;
	}

	// 유니티
	// 현재 언어 설정이 바꼈다.
	static public void NoticeChangeLang()
    {
        SJ_UnityUI_Text_Lang[] txt_langs = GameObject.FindObjectsByType<SJ_UnityUI_Text_Lang>(FindObjectsSortMode.None);

		Debug.Log( "NoticeChangeLang : " + txt_langs.Length );
		foreach( var s in txt_langs )
        {
            s.Update_Text_Lang();
        }
    }

}
