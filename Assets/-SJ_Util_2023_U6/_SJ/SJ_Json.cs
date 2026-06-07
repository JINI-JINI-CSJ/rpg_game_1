using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class SJ_Json 
{
    public	JSONClass		json = new JSONClass();

    public void Set( string str1 , string str2 , string data )
    {
        json[str1][str2] = data;
    }

    public void Set( string str1 , string str2 , int data )
    {
        json[str1][str2].AsInt = data;
    }

    
}
