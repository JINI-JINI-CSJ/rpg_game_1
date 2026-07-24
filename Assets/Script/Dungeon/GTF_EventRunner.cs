using UnityEngine;

public class GTF_EventRunner : MonoBehaviour
{
    static public GTF_EventRunner G;

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

    static public bool EventRun( string event_name )
    {
        

        return true;
    }
}
