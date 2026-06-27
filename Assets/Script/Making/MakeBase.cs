using UnityEngine;

// 메이크 작업 단위 기본 클래스
public class MakeBase : MonoBehaviour
{
    // 월드 최초 생성 , 게임 시나리오 처음 시작할때만.
    virtual public void OnMake(){}

    virtual public void OnSave(){}

    virtual public void OnLoad(){}

    virtual public void OnAfterWork(){}
}
