using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_SimpleLinkObj : MonoBehaviour
{
    static SJ_SimpleLinkObj G = null;

    [System.Serializable]
    public class _INF
    {
        public string name;
        public List<Component> lt_comp;
    }
    public List<_INF> list = new List<_INF>();
    public Dictionary<string, _INF> dic = new Dictionary<string, _INF>();

    void Awake()
    {
        G = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public _INF _Find(  string name)
    {
        if( dic.Count == 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                dic.Add(list[i].name, list[i]);
            }
        }

        if (dic.ContainsKey(name))
        {
            return dic[name];
        }
        else
        {
            Debug.LogError("Not Found " + name);
            return null;
        }
    }

    public static _INF Find(string name)
    {
        if (G == null) return null;
        return G._Find(name);
    }

}
