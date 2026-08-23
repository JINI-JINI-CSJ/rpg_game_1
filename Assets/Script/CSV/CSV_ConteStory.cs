using UnityEngine;

// 1안
// 퀘스트 깊이 , 넓이 비율
// 일단 메인 임무 (마지막 임무)는 1개 , 이 밑으로 어떻게 깊이 및 넓이가 될지 결정.
// 0 : 완전 수평 , 모든 퀘스트 종속 관계 없음
// 1 : 완전 수직 , 동시 퀘스트 없음 , 무조건 선후 관계 , 종속관계
// 0.5 : 정 삼각형에 가까운 종속 배치
// ShapeRatio

// 2안
// 단순하게 하위임무가 몇개가 될지 확률 및 깊이
// 깊이 0 , 하위 확률 갯수 10 : 동시 임무 가능 퀘스트가 1~10 개이고 종속 임무 없음
// 깊이 n , 하위 확률 갯수 m : n 차 연퀘, 각 퀘마다 종속 갯수 1~m
// 전역 설정 : 최대 깊이 , 최대 종속 임무 갯수 확률

// 레벨 설정 : 메인 임무 , 최하단 임무 에서 비례 , 래밸 변동 폭 인자 추가

// 스토리 
// 임무
// 수행 가능 조건 : 레벨  , 우호도 , 단서 등등
// 제목 : 
// 개요 : 
// 보상 :
// 
// 


public class CSV_ConteStory : SJ_CSV_BaseObj
{

}

public class CSV_ConteStoryPage : SJ_CSV_BasePage
{
    public override SJ_CSV_BaseObj OnAlloc_Obj()
    {
        return new CSV_ConteStory();
    }
}