using UnityEngine;
using UnityEngine.UI;


public class GTF_Global : MonoBehaviour
{
    static public GTF_Global G;

    public Image    img_FirstBlack;

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

    static public void PlayerInputAble( bool player_move , bool ui )
    {
        
    }
}
