using System.Text.RegularExpressions;
using System.Net.Mime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


//SJ_CSV_Read.OpenCSV( "TH_Ruins_Lite" );

//while( true )
//{
//	// Code	Name	Desc	Open_stage	Explore_Time	Explore_Gold	Explore_Crystal
//	string tag = "";
//	List<string>			csv_str = SJ_CSV_Read.Read_Line( ref tag );
//	if( csv_str == null )	break;
//	if( tag == "Code" )		continue;

//	_TH_RUINS_INF	s	=	new _TH_RUINS_INF();
//	s.pid				=	Obj_misc.TryParseInt( csv_str[0] , true );
//	s.time_min			=	Obj_misc.TryParseInt( csv_str[4] );
//	s.gold_per_time		=	Obj_misc.TryParseFloat( csv_str[5] );
//	s.crystal_per_time	=	Obj_misc.TryParseFloat( csv_str[6] );

//	list_Ruins_Inf.Add( s );
//}

//SJ_CSV_Read.CloseCSV();

/// <summary>
/// 
/// // 구글 스프레드 시트 csv 다운로드 예제
/// https://docs.google.com/spreadsheets/d/1vDdMtqOIus2KN-HeGqdSRYMhR1OfdtHBfvr1ucTFg2o/gviz/tq?tqx=out:csv&sheet=%EC%9C%A0%EB%8B%9B
/// 
/// 다운로드
/// https://kutar37.tistory.com/entry/%EA%B5%AC%EA%B8%80-%EC%8A%A4%ED%94%84%EB%A0%88%EB%93%9C%EC%8B%9C%ED%8A%B8-API-%ED%99%9C%EC%9A%A9%ED%95%98%EA%B8%B0-SELECT
/// 
/// csv 다운로드
/// https://twitter.com/bada_im/status/744479199734292480
/// 
/// https://docs.google.com/spreadsheets/d/메인아이디/gviz/tq?tqx=out:json 하시면 됩니다. csv로 내려받을 땐 out:csv고 웹으로 볼 땐 out:html입니다
/// tq= 인자로는 sql 이고요 당연히 gid=도 필요합니
/// 
/// </summary>

public	class SJ_CSV_Common
{
	
	static public float TryParseFloat( string str )
	{
		float val = 0;
		float.TryParse( str , out val );
		return val;
	}

	static public int TryParseInt( string str )
	{
		if( str.Length < 1 ) return 0;

		int val = 0;
		int.TryParse( str , out val );
		//try
		//{
		//	int m = int.Parse(str);
		//}
		//catch (FormatException e)
		//{
		//	Debug.Log(" TryParseInt " + "[" + str + "] " + e.Message);
		//}

		return val;
	}

	static	public	bool	Check_TryParseInt(string str)
	{
		int val = -1;
		if(	int.TryParse( str , out val ) ) return true;
		return false;
	}

	static	public	List<int>	TryParseInt_List( List<string> strs )
	{
		List<int> lt_int = new List<int>();
		foreach( string s in  strs )
		{
			lt_int.Add( TryParseInt(s) );
		}
		return lt_int;
	}

	static public List<int> Split_Int( string str , string sp_word )
	{
		string[] str_ar = str.Split( sp_word );
		List<int> list_int = new List<int>();
		foreach( string s in str_ar )
		{
			list_int.Add( TryParseInt(s) );
		}

		return list_int;
	}

	static public List<string> Read_1Dim_String( string str )
	{
		List<string> list_str = new List<string>();
		if( string.IsNullOrEmpty( str ) ) return list_str;
		str =	str.Replace( "(" , "" );
		str =	str.Replace( ")" , "" );
		string[]	 str_ar = str.Split(',');
		list_str.AddRange( str_ar );
		return list_str;
	}

	static public List<string> Read_1Dim_String_1( string str )
	{
		List<string> list_str = new List<string>();
		if( string.IsNullOrEmpty( str ) ) return list_str;
		str =	str.Replace( "[" , "" );
		str =	str.Replace( "]" , "" );
		string[]	 str_ar = str.Split(',');
		list_str.AddRange( str_ar );
		return list_str;
	}

	static public List<int> Read_1Dim_String_1_INT( string str )
	{
		List<int> list = new List<int>();
		List<string> list_str = Read_1Dim_String_1(str);
		foreach( string  s in list_str )
		{
			list.Add( TryParseInt(s) );
		}
		return list;
	}

	static public List<float> Read_1Dim_String_1_Float( string str )
	{
		List<float> list = new List<float>();
		List<string> list_str = Read_1Dim_String_1(str);
		foreach( string  s in list_str )
		{
			list.Add( TryParseFloat(s) );
		}
		return list;
	}
}


public class SJ_CSV_Read
{
	static	StringReader 	sr_csv;

	static public string 	RemoveString_ForNum( string str , bool end_point_remove = true )
	{
		string str_new = str.Trim();

		str_new =	str_new.Replace( " " ,"");
		str_new =	str_new.Replace( "," ,"");
		str_new =	str_new.Replace( "\"" ,"");
		str_new =	str_new.Replace( "%" ,"");

		if( end_point_remove )
			str_new =	str_new.Replace( "." ,"");

		return str_new;
	}

	
	static public	bool 		ReadStringLine_CSV( string str_line , ref List<string> list_str )
	{
		list_str.Clear();
		string 	str = "";
		bool 	spl_dub_ap_token = false;
		char[] 	chr_list = str_line.ToCharArray();
		foreach( char c in chr_list )
		{
			if( c == '"' )
			{
				if(	spl_dub_ap_token )
				{
					spl_dub_ap_token = false;
				}else{
					spl_dub_ap_token = true;
				}
			}
			else if( c == ',' )
			{
				if( spl_dub_ap_token )
				{
					str += c;
				}else{
					list_str.Add( str );
					str = "";
				}
			}else{
				str += c;
			}
		}

		list_str.Add( str );
		
		return !spl_dub_ap_token;
	}

	static	public	string	OpenCSV_ReadText( string res_file_name )
	{
		TextAsset 	text_as = (TextAsset)Resources.Load( res_file_name , typeof(TextAsset) );
		if( text_as == null ) 
		{
			Debug.LogError("OpenCSV Error : text_as == null : " + res_file_name );
			return "";
		}
		return text_as.text;
	}

	static	public	int	OpenCSV( string res_file_name )
	{
		TextAsset 	text_as = (TextAsset)Resources.Load( res_file_name , typeof(TextAsset) );
		if( text_as == null ) 
		{
			Debug.LogError("OpenCSV Error : text_as == null : " + res_file_name );
			return -1;
		}
		sr_csv 	= 	new StringReader( text_as.text );
		if( sr_csv == null )
		{
			Debug.LogError("OpenCSV Error :  sr == null : " + res_file_name );
			return -1;
		}

		// 첫번째 줄의 갯수 얻어오고 닫았다가 다시 연다.
		string 			txt;
		List<string> 	list_strline_csv = new List<string>();
		txt = sr_csv.ReadLine();
		if( txt == null ) return -1;
		list_strline_csv.Clear();
		ReadStringLine_CSV(txt, ref list_strline_csv);
		int cell_count = list_strline_csv.Count;
		sr_csv.Close();

		sr_csv 	= 	new StringReader( text_as.text );


		return cell_count;
	}


	static	public	bool	OpenCSV_Text( string str_Data )
	{
		sr_csv 	= 	new StringReader( str_Data );
		if( sr_csv == null )
		{
			Debug.LogError("OpenCSV Error :  sr == null : " + str_Data );
			return false;
		}
		return true;
	}

	static	public	int		CheckStringInChar( string str , char c )
	{
		string[] s = str.Split(c);
		if( s.Length == 0 ) return 0;
		return s.Length - 1;
	}


	static	public	List<string>	Read_Line( int fix_array_count = -1 , bool debug_log = false )
	{
		string 			txt;
		List<string> 	list_strline_csv = new List<string>();

		while (true)
		{
			txt = "";

			int c_upper = 0;
			while(true)
			{
				string read_line = sr_csv.ReadLine();

				if ( read_line == null )
				{
					return list_strline_csv;
				}

				txt += read_line;

				MatchCollection matches = 	Regex.Matches( txt , "\"" );
				c_upper = matches.Count;
				if( c_upper % 2 == 0 )
				{
					break;
				}

				txt += "\n";
				if( read_line.Length == 0 )
				{
					txt += "\n";
				}
			}



			if( debug_log ) 
				Debug.Log( txt );
			
			list_strline_csv.Clear();
			ReadStringLine_CSV(txt, ref list_strline_csv);

			string[] spl = list_strline_csv.ToArray();
			if (spl.Length < 1) continue;

			if (txt.Length > 1 )
			{
				string tag_fs = txt.Substring(0, 2);

				// 주석이면 다음줄~~ 
				if( tag_fs.Contains("`") )
				{
					list_strline_csv.Clear();
					continue;
				}
			}

			// 없는거는 뒤에서부터 빼기
			int last_str_idx = 0;
			for( int i = list_strline_csv.Count - 1 ; i > -1 ; i-- )
			{
				if( string.IsNullOrEmpty( list_strline_csv[i] ) == false )
				{
					last_str_idx = i;
					break;
				}
			}
			if( list_strline_csv.Count-1 > last_str_idx )
			{
				int remove_c = list_strline_csv.Count-1 - last_str_idx;

				list_strline_csv.RemoveRange(list_strline_csv.Count-remove_c ,remove_c );
			}

			// 강제 자리수 맞추기면.. 그만큼 더함
			if( fix_array_count > -1 )
			{
				if( fix_array_count > list_strline_csv.Count )
				{
					int c = fix_array_count - list_strline_csv.Count;
					for( int i = 0 ; i < c ; i++ ) 
					{
						list_strline_csv.Add("");
					}
				}
			}

			return list_strline_csv;
		}

		return list_strline_csv;
	}

	static	public	void	CloseCSV()
	{
		if( sr_csv != null ) sr_csv.Close();
	}

	//

	static	public	void	Exist_SetInt( ref int ref_int , string str_csv )
	{
		if( string.IsNullOrEmpty(str_csv) )return;
		ref_int = SJ_CSV_Common.TryParseInt(str_csv);
	}

	static	public	void	Exist_SetFloat( ref float ref_float , string str_csv )
	{
		if( string.IsNullOrEmpty(str_csv) )return;
		ref_float = SJ_CSV_Common.TryParseFloat(str_csv);
	}

	static	public	void	Exist_SetStr( ref string ref_str , string str_csv )
	{
		if( string.IsNullOrEmpty(str_csv) )return;
		ref_str = str_csv;
	}

}


public class SJ_CSV_Write
{
	static	StreamWriter	sw;

	static	string			strLine = "";

	static	public	bool	CreateCSV( string file_name )
	{
		sw = SJ_Unity.FileCreateWriteTxt( file_name );
		if( sw == null )return false;
		return true;
	}

	static	public	void	CloseCSV()
	{
		if( sw != null ) sw.Close();
	}

	static	public	void	Add_Column( int val )
	{
		strLine += val.ToString() + ",";
	}

	static	public	void	Add_Column( float val )
	{
		strLine += val.ToString( ".3f" ) + ",";
	}

	static	public	void	Add_Column( string val )
	{
		strLine += "\"" + val + "\",";
	}

	static	public	void	WriteLine()
	{
		sw.WriteLine( strLine );
		strLine = "";
	}
}


// 2020-05-11
// ID 로 시작하는 형태의 CSV 
// dic_ 계열로 찾는다.
public	class SJ_CSV_BaseObj
{
	public	int			ID_int;
	public	string		ID_str;

	SJ_CSV_BasePage		par;
	string[]			strs;
	int					cur_strs_idx;

	virtual	public	void	OnRead( SJ_CSV_BasePage _par , string[] _strs )
	{
		par = _par;
		strs = _strs;
		if( par.id_int_str == false )
		{
			ID_int = Next_Int();
		}
		else
		{
			ID_str = Next();
		}
	}

	public	bool 			Check_Next()
	{
		if( cur_strs_idx >= strs.Length )return false;
		return true;
	}

	public bool 			Check_AbleNext()
	{
		if( cur_strs_idx >= strs.Length )
		{
			return false;
		}
		return true;
	}

	public	string 			Next()
	{
		if( cur_strs_idx >= strs.Length )
		{
			//Debug.Log( "주의!!! CSV 컬럼갯수 : " + par.sheet_name + " : " + par.sheet_cur_line  );
			return "";
		}
		string str = strs[cur_strs_idx];
		cur_strs_idx++;
		return str;
	}

	public	int				Next_Int()
	{
		return SJ_CSV_Common.TryParseInt(Next());
	}

	public	float			Next_Float()
	{
		return SJ_CSV_Common.TryParseFloat(Next());
	}

	public	List<string> 			Next_Split(string Split)
	{
		string 		str 	=	Next();
		string[] 	strs 	= 	str.Split(Split);
		return new List<string>(strs);
	}

	public	List<int> 			Next_Split_Int(string Split)
	{
		string 		str 	=	Next();
		string[] 	strs 	= 	str.Split(Split);
		List<int> 	lt  	= new List<int>();
		foreach(string str2 in strs)
		{
			lt.Add(SJ_CSV_Common.TryParseInt(str2));
		}
		return lt;
	}

	public	List<float> 			Next_Split_Float(string Split)
	{
		string 		str 	=	Next();
		string[] 	strs 	= 	str.Split(Split);
		List<float> lt  	= new List<float>();
		foreach(string str2 in strs)
		{
			lt.Add(SJ_CSV_Common.TryParseFloat(str2));
		}
		return lt;
	}

	public	List<string>	ArrData( bool inc_id = false )
	{
		List<string> lt = new List<string>( strs );
		if( inc_id == false && lt.Count > 0 ) lt.RemoveAt(0);
		return lt;
	}

	public	List<int>	ArrData_Int( bool inc_id = false )
	{
		List<int> lt = new List<int>();
		foreach( string s in strs )lt.Add( SJ_CSV_Common.TryParseInt(s) );
		if( inc_id == false && lt.Count > 0 ) lt.RemoveAt(0);
		return lt;
	}	

	public	List<float>	ArrData_Float( bool inc_id = false )
	{
		List<float> lt = new List<float>();
		foreach( string s in strs )lt.Add( SJ_CSV_Common.TryParseFloat(s) );
		if( inc_id == false && lt.Count > 0  ) lt.RemoveAt(0);
		return lt;
	}	

	public	int 	GetValCount(int idx , bool inc_id = false)
	{
		int c = strs.Length;
		if( inc_id == false && c > 0 )c--;
		return c;
	}

	public	string 	GetVal(int idx , bool inc_id = false )
	{
		if( inc_id )
		{
			if( strs.Length > idx ) return strs[idx];
		}else{
			if( strs.Length > idx+1 ) return strs[idx+1];
		}
		
		return "";
	}

	public	int 	GetValInt(int idx, bool inc_id = false)
	{

		return SJ_CSV_Common.TryParseInt( GetVal(idx , inc_id) );

	}

	public	float 	GetValFloat(int idx, bool inc_id = false)
	{
		return SJ_CSV_Common.TryParseFloat( GetVal(idx,inc_id) );
	}

	public void   	Remain_Data( List<string> lt )
	{
		if( lt == null ) lt = new();
		for( int i = cur_strs_idx ; i < strs.Length ; i++ )
		{
			lt.Add( strs[i] );
		}
	}

	public List<int>   	Remain_Data_Int()
	{
		List<string> lt = new();
		Remain_Data( lt );
		List<int> lt_int = new();
		foreach( var s in lt )
		{
			lt_int.Add( SJ_CSV_Common.TryParseInt(s) );
		}
		return lt_int;
	}

	public List<float>   	Remain_Data_Float()
	{
		List<string> lt = new();
		Remain_Data( lt );
		List<float> lt_float = new();
		foreach( var s in lt )
		{
			lt_float.Add( SJ_CSV_Common.TryParseFloat(s) );
		}
		return lt_float;
	}

    static public void  To_Serialization_CSV_INT_ID<T>( List<T> lt , BinaryWriter bw )
    {
        bw.Write( lt.Count );
        foreach ( T t in lt )
        {
			SJ_CSV_BaseObj csv = t as SJ_CSV_BaseObj;
			bw.Write( csv.ID_int );
        }
    }

    static public List<int>  From_Serialization_INT_ID( BinaryReader br  )
    {
        List<int> lt = new List<int>();
        int c = br.ReadInt32();
        for( int i = 0; i < c; i++ )
        {
            lt.Add( br.ReadInt32() );
        }
        return lt;
    }

	static public string ArrData_Arg( List<string> lt , int idx )
	{
		if( idx < 0 || idx >= lt.Count )return "";
		return lt[idx];
	}

	static public int ArrData_Arg_INT( List<string> lt , int idx )
	{
		int val = 0;
		int.TryParse( ArrData_Arg(lt , idx) , out val );
		return val;
	}

	static public float ArrData_Arg_FLOAT( List<string> lt , int idx )
	{
		float val = 0;
		float.TryParse( ArrData_Arg(lt , idx) , out val );
		return val;
	}
}

public	class SJ_CSV_BasePage
{
	public	Dictionary<int ,SJ_CSV_BaseObj >	dic_int = new Dictionary<int, SJ_CSV_BaseObj>();
	public	Dictionary<string ,SJ_CSV_BaseObj >	dic_str = new Dictionary<string, SJ_CSV_BaseObj>();
	
	//public	List<SJ_CSV_BaseObj>		lt_obj = new List<SJ_CSV_BaseObj>();

	public	bool	id_int_str;
	public	string	sheet_name;
	public	int		sheet_cur_line;
	int fix_array_count = -1;

	virtual	public	SJ_CSV_BaseObj	OnAlloc_Obj(){return null;}
	
	virtual	public	void	Read()
	{
		sheet_cur_line = 0;
		while(true)
		{
			List<string> strs =	ReadLine();
			if( strs.Count < 1 ) break;

			SJ_CSV_BaseObj obj = OnAlloc_Obj();

			if( obj == null )
			{
				SJ_CSV_Mng.g_base.OnLog( "에러=====>>>> OnAlloc_Obj : " + this.ToString() );
				return;
			}

			obj.OnRead(this , strs.ToArray() );
			if( id_int_str == false )
			{
				if( obj.ID_int == 0 ) continue;
				dic_int[obj.ID_int] = obj;
			}
			else
			{
				dic_str[obj.ID_str] = obj;
			}
			//lt_obj.Add( obj );
			sheet_cur_line++;
		}
		SJ_CSV_Read.CloseCSV();
	}	

	public	List<string>	ReadLine()
	{
		return SJ_CSV_Read.Read_Line(fix_array_count);
	}

	public	string	ReadLine_Str( int idx )
	{
		List<string> strs =	ReadLine();
		if( strs.Count <= idx ) return "";
		return strs[idx];
	}

	public	int	ReadLine_Int( int idx )
	{
		List<string> strs =	ReadLine();
		if( strs.Count <= idx ) return 0;
		return SJ_CSV_Common.TryParseInt( strs[idx] );
	}

	public	float	ReadLine_Float( int idx )
	{
		List<string> strs =	ReadLine();
		if( strs.Count <= idx ) return 0;
		return SJ_CSV_Common.TryParseFloat( strs[idx] );
	}

	public List<int> ReadLine_IntList(int idx)
	{
		List<string> strs = ReadLine();
		if (strs.Count <= idx) return new List<int>();
		return Array_Read_Int(strs.ToArray(), idx);
	}

	public List<float> ReadLine_FloatList(int idx)
	{
		List<string> strs = ReadLine();
		if (strs.Count <= idx) return new List<float>();
		return Array_Read_Float(strs.ToArray(), idx);
	}

	public bool LoadCSV_Res(string _sheet_path, bool _id_int_str, int _fix_array_count = -1)
	{
		sheet_name = _sheet_path;
		id_int_str = _id_int_str;
		fix_array_count = _fix_array_count;
		int r = SJ_CSV_Read.OpenCSV(sheet_name);
		if (r < 0) return false;
		Read();
		return true;
	}

	public	bool	LoadCSV_Text( string str_data , string _sheet_path , bool	_id_int_str , int _fix_array_count = -1 )
	{
		sheet_name = _sheet_path;
		id_int_str = _id_int_str;
		fix_array_count = _fix_array_count;
		bool r = SJ_CSV_Read.OpenCSV_Text( str_data );
		if( r == false ) return false;
		Read();
		return true;
	}


	public	SJ_CSV_BaseObj	Find_Int( int ID , bool noDebug = false )
	{ 
		SJ_CSV_BaseObj f = null;
		if( dic_int.TryGetValue( ID , out f ) )return f;

		if( noDebug == false )
			Debug.LogError( "주의!!!! 못찾음 : " + sheet_name + " : " + ID );
		return null;
	}

	public	SJ_CSV_BaseObj	Find_Str( string ID , bool noDebug = false )
	{ 
		if( string.IsNullOrEmpty(ID) )
		{
			if( noDebug == false )
				Debug.LogError( "주의!!!! string.IsNullOrEmpty(ID) : " + sheet_name + " : " );
		 	return null;
		}

		SJ_CSV_BaseObj f = null;
		if( dic_str.TryGetValue( ID , out f ) )return f;
		if( noDebug == false )
			Debug.LogError( "주의!!!! 못찾음 : " + sheet_name + " : " + ID );
		return null;
	}


	static	public	List<string>		Array_Read( List<string> arr , int start_idx )
	{
		List<string>	list_str = new List<string>();
		for( int i = start_idx ; i < arr.Count ; i++ )
		{
			list_str.Add( arr[i] );
		}
		return list_str;
	}

	static	public	List<string>		Array_Read( string[] arr , int start_idx )
	{
		List<string> lt = new List<string>(arr);
		return Array_Read( lt , start_idx );
	}

	static	public	List<int>		Array_Read_Int( string[] arr , int start_idx )
	{
		List<int>	list_int = new List<int>();
		for( int i = start_idx ; i < arr.Length ; i++ )
		{
			list_int.Add( SJ_CSV_Common.TryParseInt(arr[i]) );
		}
		return list_int;
	}

	static	public	List<float>		Array_Read_Float( string[] arr , int start_idx )
	{
		List<float>	list_float = new List<float>();
		for( int i = start_idx ; i < arr.Length ; i++ )
		{
			list_float.Add( SJ_CSV_Common.TryParseFloat(arr[i]) );
		}
		return list_float;
	}

	// 다른 것 카피
	// 주의 같은것만 해야 한다.
	public	void 	Add( SJ_CSV_BasePage page_other )
	{
		if( id_int_str )
		{
			foreach( KeyValuePair<string,SJ_CSV_BaseObj> kv in page_other.dic_str )
			{
				dic_str[kv.Key] = kv.Value;
			}
		}else{

			foreach( KeyValuePair<int,SJ_CSV_BaseObj> kv in page_other.dic_int )
			{
				dic_int[kv.Key] = kv.Value;
			}
		}
	}

	public List<T> CopyData_INT<T>()
	{
		List<T> values = new( dic_int.Values.Cast<T>() );
		return values;
	}

	public List<T> CopyData_STR<T>()
	{
		List<T> values = new( dic_str.Values.Cast<T>() );
		return values;
	}

	virtual public void LoadAfter(){}
}