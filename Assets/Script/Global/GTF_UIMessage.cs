using UnityEngine;

public class GTF_UIMessage : MonoBehaviour
{
    static public GTF_UIMessage G;

    void Awake()
    {
        G = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void MsgShort( string msg )
    {
        SJ_UIDefaultShortMsg.SetMsg( msg );
    }
    
}
