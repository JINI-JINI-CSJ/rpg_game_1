using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SJ_MonoMenu_SimpleUnit : MonoBehaviour
{
    public Color color_Active = Color.white;
    public Color color_DeActive = Color.gray;

    public Image image;
    public Text text;
    public SpriteRenderer spriteRenderer;
    public Toggle toggle;

    public UnityEvent unityEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetActive(bool b)
    {
        Color col = color_Active;
        if (b == false) col = color_DeActive;

        if (image != null) image.color = col;
        if (text != null) text.color = col;
        if (spriteRenderer != null) spriteRenderer.color = col;

        if (toggle != null) toggle.SetIsOnWithoutNotify(b);

    }

    public void CallFunc()
    {
        unityEvent.Invoke();
    }

    // GUI 이벤트 트리거로 직접 연결
    // 들어오면 상위에 내가 활성화 됬다고 알림 , 나가는 건 따로 처리 안해도 될듯
    // 상위에선 그냥 이 메뉴 액티브 함수 하면 된다.
    public void OnMouseEnter_Menu()
    {
        SJ_MonoMenu_Simple menu_par = GetComponentInParent<SJ_MonoMenu_Simple>();
        menu_par.SetCurMenu( this );
    }
}
