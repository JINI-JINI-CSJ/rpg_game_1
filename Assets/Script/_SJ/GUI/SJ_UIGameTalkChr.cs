using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 캐릭터 토크 전용 이미지 출력
/// 2 인 기본
/// 지정 위치는 자동으로 하는데 , 이미 있는 리소스가 있다면 활성화만 한다.
/// </summary>

public class SJ_UIGameTalkChr : MonoBehaviour
{
    public Color col_white = Color.white;
    public Color col_gray = Color.gray;

    public Image img_left;
    public Image img_right;

    public int default_seat = 1; // 최근 자리가 없을 경우 , 1 오른쪽 자리

    string recent_left;
    string recent_right;

    int recent_seat_num;    // 최근 갱신된 자리 , 위의 자리가 다 찾을때 이거의 반대 자리


    void Awake()
    {
        Clear();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 이미지 좌석 우선순위
    // 1. 최근 같은 이미지 
    // 2. 최근 갱신의 반대쪽
    // 3. 디폴트 갱신
    // 4. 오른쪽
    virtual public Image GetSeatImage(string filename , string tag_filename)
    {
        string img_name = filename;
        // 태그가 있다면 태그로 정의
        if( string.IsNullOrEmpty(tag_filename) == false )
        {
            img_name = tag_filename;
        }
 
        // 이전 같은 이미지 파일 이름으로 체크
        if( recent_left == img_name )
        {
            recent_seat_num = -1;
            return img_left;
        }
         
        if( recent_right == img_name)
        {
            recent_seat_num = 1;
            return img_right;
        } 

        // 없으면 가장 최신 갱신된 이미지의 반대쪽
        if( recent_seat_num == -1 )
        {
            recent_seat_num = 1;
            recent_right = img_name;
            return img_right;
        }

        if( recent_seat_num == 1 )
        {
            recent_seat_num = -1;
            recent_left = img_name;
            return img_left;
        }

        // 처음 기본 갱신 위치
        if( default_seat == 1 )
        {
            recent_seat_num = 1;
            recent_right = img_name;
            return img_right;
        }

        if( default_seat == -1 )
        {
            recent_seat_num = -1;
            recent_left = img_name;
            return img_left;
        }

        // 그것도 없으면 오른쪽
        recent_seat_num = 1;
        recent_right = img_name;
        return img_right;
    }

    public void Clear()
    {
        recent_seat_num = 0;
        recent_left = recent_right = null;
        img_left.sprite = null;
        img_right.sprite = null;

        img_left.gameObject.SetActive(false);
        img_right.gameObject.SetActive(false);
    }

    // filename : 실제 이미지 파일 이름
    // tag_filename : 이미지의 태그 , 예를들어 같은 캐릭이지만 표정이나 동작이 여러개일경우 같은 캐릭터로 인식하기 위한 태그
    public void SetImage( string filename , string tag_filename = "" )
    {
        Image image_seat = GetSeatImage( filename ,  tag_filename);

        image_seat.gameObject.SetActive(true);

        // 반대편 자리
        Image img_opp_seat = img_left;
        if( image_seat == img_left )img_opp_seat = img_right;
        
        SJ_UnityUI_Util.Image_Load( image_seat , filename );

        image_seat.color = col_white;
        img_opp_seat.color = col_gray;

        // 특수 효과가 있을 경우
        ImageActive( image_seat , img_opp_seat );
    }

    virtual public void ImageActive( Image img_main , Image img_sub ){}
}
