using UnityEngine;

public class SJ_SimpleBillboard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        transform.LookAt( Camera.main.transform );
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt( Camera.main.transform );
    }
}
