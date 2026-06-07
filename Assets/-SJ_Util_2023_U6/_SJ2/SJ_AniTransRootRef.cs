using System.Collections.Generic;
using UnityEngine;

public class SJ_AniTransRootRef : MonoBehaviour
{
    [System.Serializable]
    public class TargetRoot
    {
        public Transform TargetParTr;
        public SJ_TransformFollower TargetFollower;
        public void TransAttach(Transform t)
        {
            if (TargetFollower != null)
            {
                SJ_Unity.SetEqTrans(t, null, TargetFollower.transform);
                TargetFollower.StartFollowing(t);
            }
            else
            {
                SJ_Unity.SetEqTrans(t, null, TargetParTr);
            }
        }
    }

    [System.Serializable]
    public class NAME_TargetRoot
    {
        public string name;
        public Transform tr_ref;
        public List<TargetRoot> targets;

        public void TransAttach(int nIdx)
        {
            if (nIdx < 0 || nIdx >= targets.Count)
            {
                Debug.LogError("에러 : nIdx 범위 벗어남 " + name + " / " + nIdx);
                return;
            }
            targets[nIdx].TransAttach(tr_ref);
        }
    }

    public List<NAME_TargetRoot> list_Roots = new List<NAME_TargetRoot>();

    // 외부에서 호출할때 실행할 NAME_TargetRoot 인자들
    // 이름 , 인덱스
    [System.Serializable]
    public class Func_SyncCall
    {
        public string name;
        public int idx;
        public void Func_SyncCall_Exec(SJ_AniTransRootRef mono)
        {
            if( string.IsNullOrEmpty(name) )
            {
                return;
            }

            NAME_TargetRoot nameTarget = mono.list_Roots.Find(x => x.name == name);
            if (nameTarget == null)
            {
                Debug.LogError("에러 : 이름 못찾음 Func_SyncCall : " + name);
                return;
            }
            nameTarget.TransAttach(idx);
        }
    }

    [System.Serializable]
    public class Func_SyncCall_Group
    {
        public string groupName;
        public List<Func_SyncCall> func_SyncCalls;
        public void Call(SJ_AniTransRootRef mono)
        {
            foreach (Func_SyncCall f in func_SyncCalls)
            {
                f.Func_SyncCall_Exec(mono);
            }
        }
    }

    public List<Func_SyncCall_Group> list_FuncSyncGroups;

    public void TRANS_ROOT(string sVal)
    {
//        Debug.Log("TRANS_ROOT : " + sVal);
        // "이름_인덱스" 형식으로 파싱한다.
        string[] arr = sVal.Split('_');
        if (arr.Length != 2)
        {

            Debug.LogError("에러 : arr.Length != 2 + " + sVal);
            return;
        }

        string sName = arr[0];
        int nIdx = 0;
        if (int.TryParse(arr[1], out nIdx) == false)
        {
            Debug.LogError("에러 : int.TryParse 실패 + " + sVal);
            return;
        }
        // list_Roots 에서 이름으로 찾기
        NAME_TargetRoot nameTarget = list_Roots.Find(x => x.name == sName);
        if (nameTarget == null)
        {
            Debug.LogError("에러 : 이름 못찾음 + " + sVal);
            return;
        }

//        Debug.Log("TRANS_ROOT : PLAY : " + sName + " / " + nIdx);
        nameTarget.TransAttach(nIdx);
    }

    public void Play_Func_SyncCall(string groupName)
    {
        Func_SyncCall_Group group = list_FuncSyncGroups.Find(x => x.groupName == groupName);
        if( group == null )
        {
            //Debug.LogError("에러 : Play_Func_SyncCall 못찾음 + " + groupName );
            return;
        }
        group.Call(this);
    }
}
