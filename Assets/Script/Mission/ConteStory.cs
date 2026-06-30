using System.Collections.Generic;
using UnityEngine;

// 다중 퀘스트 , 연속 퀘스트 , 전설 스토리
// 제목은 어떻게 할까?
// 탬플릿 방식
// 최종 빌런 집단의 이름이나 비슷한 것 ( 아카츠키 )
// 최종 보상의 이름의 종류 ( 엑스칼리버리면 "전설의 검" , "신의 무기" 등등  )
// 
// 여력이 되면 위의 이름들이 점점 확실해지는것
// "고대 유물의 소문" -> "신의 무기"

// 제목 종류별로 템플릿이나 예제 목록 만들기 ( gpt 로 하자. )

// 임무의 종류는 어떻게 만들까?
// 미션 종류 비율 : 소문 입수 , 던전 탐색 , 보스 격파  등등의 각각 종류의 성향 값 , 마지막 미션 지정

// 특정 다중 퀘스트
// - 순서에 상관없이 보스들 격파 
// - 소문 입수로만 달성 가능 ( 술집등에서 입수 , 대신 소문 범위 넓음 )
// 성향 템플릿을 csv 로 만들자.

public class ConteStory 
{
    // 제목 
    // 템플릿 1개 , 신의 결사단 등등 
    // 템플릿 2개로 조합 , ??? 의 소문  등등

    // 템플릿 1 종류 
    public int title_Template_1;
    
    // 템플릿 2 내용
    public int title_Template_2;
    
    virtual public string GetTitle(){return "제목";}

    // 임무들 
    public List<MissionBase> missions = new();
}
