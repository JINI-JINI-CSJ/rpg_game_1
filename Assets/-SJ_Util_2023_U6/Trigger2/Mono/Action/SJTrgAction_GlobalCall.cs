using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJTrgAction_GlobalCall : SJTrgAction_Default_Mono
{

    public  string name_Class;
    public  string name_Func;
    public  List<string>    args;

    public override void OnAction()
    {
        SJ_GlobalCall.Call_Func( name_Class , name_Func , args.ToArray()  );
    }

}
