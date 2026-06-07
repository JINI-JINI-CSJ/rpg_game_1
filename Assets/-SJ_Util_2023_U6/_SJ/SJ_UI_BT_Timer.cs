using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SJ_UI_BT_Timer : MonoBehaviour
{
    public Text     text_Count;

    public int      wait_time = 3;
    int             wait_time_cur;
    public bool     active_enable = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        if( active_enable )
        {
            StartCount();
        }
    }

    public void StartCount()
    {
        wait_time_cur = wait_time;
        if( text_Count != null ) text_Count.enabled = true;

        Button bt = GetComponent<Button>();
        if( bt != null )bt.interactable = false;    

        CancelInvoke();
        UpdateUI();
        InvokeRepeating( "repeatCount" , 1 , 1 );
    }

    void repeatCount()
    {
        --wait_time_cur;
        UpdateUI();
        if( wait_time_cur == 0 )
        {
             CancelInvoke();
             if( text_Count != null ) text_Count.enabled = false;

            Button bt = GetComponent<Button>();
            if( bt != null )bt.interactable = true; 
        }
    }

    public void UpdateUI()
    {
        if( text_Count != null ) text_Count.text = wait_time_cur.ToString();
    }

}
